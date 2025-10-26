using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace CTR_MonoGame
{
    public class IOBoard
    {
        public bool Coin1
        {
            get;
            protected set;
        }
        public bool Coin2
        {
            get;
            protected set;
        }
        public bool DBV
        {
            get;
            protected set;
        }
        public bool Test
        {
            get;
            protected set;
        }
        public bool Diag
        {
            get;
            protected set;
        }
        public bool Up
        {
            get;
            protected set;
        }
        public bool Down
        {
            get;
            protected set;
        }

        public bool TicketError
        {
            get;
            protected set;
        }

        SerialPort port;
        ushort ticketsDispensed;

        public IOBoard()
        {
            if (Util.OnDevice)
            {
                while (port == null)
                {
                    try
                    {
                        port = new SerialPort(@"/dev/ttyACM0", 9600, Parity.None, 8, StopBits.One);
                        port.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Serial Port Error:");
                        Console.WriteLine(e.ToString());
                        Thread.Sleep(1000);
                        port = null;
                    }
                }
            }
        }

        ~IOBoard()
        {
            if (Util.OnDevice)
            {
                port.Close();
            }
        }

        public void Update()
        {
            if (Util.OnDevice)
            {
                SendCommand(0x64, 1, 0, 0, 0);
                ProcessButtons(GetResponse());
                SendCommand(0x65, 42, 0xFFFF, 0xFFFF, 0xFFFF);
                SendCommand(0x71, 2, 0, 0, 0);
                ProcessTickets(GetResponse());
            }
        }

        public void AddCoinCounter(int num)
        {
            if (Util.OnDevice)
            {
                SendCommand(0x23, 4, (ushort)num, 0, 0);
            }
        }

        public void SetBonusDisplay(int num)
        {
            if (Util.OnDevice)
            {
                SendCommand(0x18, 0, (ushort)(Math.Min(num, 9999)), 0, 0);
            }
        }

        private void ProcessTickets(byte[] p)
        {
            if (p[1] != 0xB0)
            {
                return;
            }
            FCOptions.TicketsOwed = p[5] * 256 + p[6];
            ushort ticketCounter = (ushort)(p[3] * 256 + p[4]);
            if (ticketCounter > ticketsDispensed)
            {
                SendCommand(0x23, 2, (ushort)(ticketCounter - ticketsDispensed), 0, 0);
                ticketsDispensed = ticketCounter;
            }

            if (FCOptions.TicketsOwed <= 0)
            {
                SendCommand(0x66, 2, 0, 0, 0);
                ticketsDispensed = 0;
            }
            TicketError = p[8] > 0;
        }

        public void AddTicketCounter(int count)
        {
            if (Util.OnDevice)
            {
                SendCommand(0x23, 2, (ushort)(count), 0, 0);
            }
        }

        public void GiveTickets(int tickets)
        {
            if (Util.OnDevice && tickets > 0)
            {
                SendCommand(0x22, 2, (ushort)tickets, 0, 0);
                //SendCommand(0x23, 2, (ushort)tickets, 0, 0);
            }
        }

        public void ClearTicketsToGive()
        {
            if (Util.OnDevice)
            {
                SendCommand(0x15, 2, 0, 0, 0);
            }
        }

        public void SetLockout(bool energize)
        {
            if (Util.OnDevice)
            {
                SendCommand(0x16, 2, (ushort)(energize ? 1 : 0), 0, 0);
            }
        }

        public void SetStarLights(bool star1, bool star2, bool star3)
        {
            if (Util.OnDevice)
            {
                SendCommand(0x13, 4, (ushort)(star1 ? 10 : 0), (ushort)(star1 ? 0 : 10), 0);
                SendCommand(0x13, 5, (ushort)(star2 ? 10 : 0), (ushort)(star2 ? 0 : 10), 0);
                SendCommand(0x13, 1, (ushort)(star3 ? 10 : 0), (ushort)(star3 ? 0 : 10), 0);
            }
        }

        public void SetColorBar(bool red, bool green, bool blue)
        {
            if (Util.OnDevice)
            {
                SendCommand(0x13, 8, (ushort)(red ? 10 : 0), (ushort)(red ? 0 : 10), 0);
                SendCommand(0x13, 9, (ushort)(green ? 10 : 0), (ushort)(green ? 0 : 10), 0);
                SendCommand(0x13, 10, (ushort)(blue ? 10 : 0), (ushort)(blue ? 0 : 10), 0);
            }
        }

        private void ProcessButtons(byte[] data)
        {
            Down = ((data[7] & 2) > 0);
            Up = ((data[7] & 1) > 0);
            Test = ((data[8] & 128) > 0);
            Diag = ((data[8] & 64) > 0);
            DBV = ((data[8] & 32) > 0);
            Coin2 = ((data[8] & 8) > 0);
            Coin1 = ((data[8] & 4) > 0);
        }

        void SendCommand(byte command, byte id, ushort message1, ushort message2, ushort message3)
        {
            byte[] data = new byte[] { 0x55, command, id, MSB(message1), LSB(message1), MSB(message2), LSB(message2),
                MSB(message3), LSB(message3), 0xAA};
            port.Write(data, 0, data.Length);
        }

        byte MSB(ushort message)
        {
            return (byte)(message >> 8);
        }

        byte LSB(ushort message)
        {
            return (byte)message;
        }

        byte[] GetResponse()
        {
            List<byte> buffer = new List<byte>();
            while (port.ReadByte() != 0x55)
            { }
            buffer.Add(0x55);
            for (int i = 0; i < 9; i++)
            {
                buffer.Add((byte)port.ReadByte());
            }

            return buffer.ToArray();
        }
    }
}
