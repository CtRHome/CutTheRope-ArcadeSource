using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CTR_MonoGame
{
    public enum Currency { Dollar, Euro, Pound, Yen, Won, Ruble, Real, Peso, Rand, Krona, Guilder, Franc, Coin, Token };
    public enum TicketName { Ticket, Coupon };
    public enum AttractMusicFrequency { Always, Off, Two, Five, Ten, Fifteen };

    [Serializable]
    public struct OptionsStruct
    {
        public uint fileVersion;
        // Game Adjustments
        public int pointsPerStar;
        public decimal parTimeOffset;
        public int timeLimit;
        public int maxTimeBonusPoints;
        public int pointsLostPerSecond;
        public int pointsPerTicket;
        public bool parBonusEveryStage;
        public float[] parTimePerLevel;
        public bool showObjectives;
        public bool showWarnings;
        public int stage2Med;
        public int stage2EasyStage3Med;
        public int stage2EasyStage3Hard;
        public int stage2MedStage3Hard;
        public int stage2MedStage3Hardest;
        public bool enableBonusLevel;
        public int bigBonusValue;
        public int mediumBonusValue;
        public int smallBonusValue;
        public int maxCreditsAllowed;
        // Coin Adjustments
        public bool freePlay;
        public decimal gameCost;
        public Currency currency;
        public bool swipeCard;
        public decimal currencyUnitValue;
        public decimal coin1;
        public decimal coin2;
        public decimal dbv;
        // Payout Adjustments
        public bool useTickets;
        public TicketName ticketName;
        public bool fixedTickets;
        public int fixedTicketsPerGame;
        public int ticketMultiplier;
        public decimal payoutPCT;
        public decimal ticketValue;
        public int mercyTickets;
        public ulong totalTicketsOut;
        public bool instantPayout;
        // Audits
        public int totalGamesPlayed;
        public int[] playsPerLevel;
        public double[] timePerLevel;
        public double[] fastestPerfectTimePerLevel;
        public int[] starsPerLevel;
        public int[] deathsPerLevel;
        public long[] pointsPerLevel;
        public int[] perfectPlaysPerLevel;
        public int ticketsOwed;
        public decimal credits;
        public int repeatGames;
        public int threeFedOmNoms;
        public int smallBonusCount;
        public int mediumBonusCount;
        public int bigBonusCount;
        public ulong pulsesIn;
        // Volume
        public int gameVolume;
        public int attractVolume;
        public AttractMusicFrequency attractMusicFrequency;
    }

    public static class FCOptions
    {
        static OptionsStruct _options;

        const int FILE_VERSION = 27;

        private static bool suspendWrites, dirty;

        static FCOptions()
        {
            suspendWrites = false;
            dirty = false;

            if (File.Exists(FCOptionsLocation.FILE_LOCATION + "options.dat"))
            {
                TextReader tr = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(OptionsStruct));
                    tr = new StreamReader(FCOptionsLocation.FILE_LOCATION + "options.dat");
                    _options = (OptionsStruct)serializer.Deserialize(tr);
                    tr.Close();
                }
                catch
                {
                    if (tr != null)
                    {
                        tr.Close();
                    }
                    FCOptions.RebuildOptions();
                }
                try
                {
                    while (FCOptions.FileVersion != FILE_VERSION)
                    {
                        if (FCOptions.FileVersion == 17)
                        {
                            PerfectPlaysPerLevel = new int[CTRGame.MAX_LOADABLE_LEVEL];
                            Stage2Med = 5840;
                            Stage2EasyStage3Med = 3100;
                            Stage2EasyStage3Hard = 3800;
                            Stage2MedStage3Hard = 2900;
                            Stage2MedStage3Hardest = 3900;

                            FCOptions.FileVersion++;
                        }
                        else if (FCOptions.FileVersion == 16)
                        {
                            FCOptions.ShowObjectives = true;
                            FCOptions.ShowWarnings = true;
                            FCOptions.FileVersion++;
                        }
                        else if (FCOptions.FileVersion == 0)
                        {
                            FCOptions.FileVersion++;
                        }
                        else
                        {
                            FCOptions.RebuildOptions();
                        }
                    }
                }
                catch (Exception)
                {
                    FCOptions.RebuildOptions();
                }
            }
            else
            {
                RebuildOptions();
            }
        }

        public static void RebuildOptions()
        {
            _options = new OptionsStruct();
            _options.fileVersion = FILE_VERSION;
            _options.pointsPerStar = 1000;
            _options.parTimeOffset = 0.2M;
            _options.maxTimeBonusPoints = 3000;
            _options.pointsLostPerSecond = 400;
            _options.pointsPerTicket = 500;
            _options.timeLimit = 25;
            _options.freePlay = false;
            _options.gameCost = 1.0M;
            _options.currency = Currency.Dollar;
            _options.currencyUnitValue = 1.0M;
            _options.coin1 = _options.coin2 = 1.0M;
            _options.dbv = 1.0M;
            _options.ticketName = TicketName.Coupon;
            _options.fixedTickets = false;
            _options.fixedTicketsPerGame = 40;
            _options.ticketMultiplier = 2;
            _options.payoutPCT = 0.3M;
            _options.ticketValue = 0.00225M;
            _options.mercyTickets = 0;
            _options.totalGamesPlayed = 0;
            _options.playsPerLevel = new int[CTRGame.MAX_LOADABLE_LEVEL];
            _options.timePerLevel = new double[CTRGame.MAX_LOADABLE_LEVEL];
            _options.fastestPerfectTimePerLevel = new double[CTRGame.MAX_LOADABLE_LEVEL];
            _options.starsPerLevel = new int[CTRGame.MAX_LOADABLE_LEVEL];
            _options.deathsPerLevel = new int[CTRGame.MAX_LOADABLE_LEVEL];
            _options.pointsPerLevel = new long[CTRGame.MAX_LOADABLE_LEVEL];
            _options.parTimePerLevel = new float[CTRGame.MAX_LOADABLE_LEVEL];
            _options.perfectPlaysPerLevel = new int[CTRGame.MAX_LOADABLE_LEVEL];
            for (int i = 0; i < CTRGame.MAX_LOADABLE_LEVEL; i++)
            {
                _options.parTimePerLevel[i] = 20;
            }
            SetParTimes();
            _options.gameVolume = 20;
            _options.attractVolume = 20;
            _options.ticketsOwed = 0;
            _options.credits = 0;
            _options.stage2Med = 5840;
            _options.stage2EasyStage3Med = 3100;
            _options.stage2EasyStage3Hard = 3800;
            _options.stage2MedStage3Hard = 2900;
            _options.stage2MedStage3Hardest = 3900;
            _options.enableBonusLevel = true;
            _options.parBonusEveryStage = false;
            _options.showObjectives = true;
            _options.showWarnings = false;
            _options.repeatGames = 0;
            _options.threeFedOmNoms = 0;
            _options.useTickets = true;
            _options.pulsesIn = 0;
            _options.attractMusicFrequency = AttractMusicFrequency.Always;
            _options.swipeCard = true;
            _options.instantPayout = true;
            _options.bigBonusValue = 100;
            _options.mediumBonusValue = 20;
            _options.smallBonusValue = 10;
            _options.maxCreditsAllowed = -1;
            WriteOptionsFile();
        }

        private static void SetParTimes()
        {
            Dictionary<int, float> times = new Dictionary<int, float>();
            times.Add(0, 0.969000459f);
            times.Add(1, 5.45701027f);
            times.Add(2, 2.906996f);
            times.Add(3, 2.80499625f);
            times.Add(4, 6.205019f);
            times.Add(5, 4.18199539f);
            times.Add(6, 11.0840759f);
            times.Add(7, 7.072029f);
            times.Add(8, 5.71201324f);
            times.Add(9, 3.28099513f);
            times.Add(10, 20f);
            times.Add(11, 5.42301f);
            times.Add(12, 11.0670757f);
            times.Add(13, 6.545023f);
            times.Add(15, 4.385998f);
            times.Add(16, 7.327032f);
            times.Add(17, 6.120018f);
            times.Add(18, 5.967016f);
            times.Add(20, 20f);
            times.Add(25, 11.6280823f);
            times.Add(26, 4.147995f);
            times.Add(27, 4.232996f);
            times.Add(29, 20f);
            times.Add(31, 7.95603943f);
            times.Add(32, 11.3390789f);
            times.Add(33, 3.26399517f);
            times.Add(34, 20f);
            times.Add(37, 13.1071f);
            times.Add(38, 9.07805252f);
            times.Add(39, 7.59903526f);
            times.Add(40, 9.027052f);
            times.Add(41, 6.66402435f);
            times.Add(43, 10.54007f);
            times.Add(44, 5.71201324f);
            times.Add(45, 20f);
            times.Add(46, 20f);
            times.Add(48, 20f);
            times.Add(49, 20f);
            times.Add(50, 5.66101265f);
            times.Add(51, 20f);
            times.Add(52, 13.1411f);
            times.Add(53, 9.061052f);
            times.Add(54, 20f);
            times.Add(55, 11.8150845f);
            times.Add(57, 5.47401047f);
            times.Add(58, 20f);
            times.Add(62, 7.36103249f);
            times.Add(63, 7.10602951f);
            times.Add(64, 20f);
            times.Add(65, 20f);
            times.Add(67, 20f);
            times.Add(73, 20f);
            times.Add(75, 1.24099994f);
            times.Add(76, 5.593012f);
            times.Add(77, 3.07699561f);
            times.Add(81, 13.8041077f);
            times.Add(86, 20f);
            times.Add(87, 4.419998f);
            times.Add(89, 20f);
            times.Add(90, 10.3870678f);
            times.Add(99, 20f);
            times.Add(125, 10.2170658f);
            times.Add(126, 8.092041f);
            times.Add(128, 20f);
            times.Add(132, 20f);
            times.Add(136, 20f);
            times.Add(150, 9.537058f);
            times.Add(152, 20f);
            times.Add(153, 20f);
            times.Add(155, 20f);
            times.Add(161, 20f);
            times.Add(163, 20f);
            times.Add(165, 20f);
            times.Add(168, 23.6812229f);
            times.Add(175, 2.24399757f);
            times.Add(176, 4.87900352f);
            times.Add(177, 8.636047f);
            times.Add(178, 9.962063f);
            times.Add(179, 8.058041f);
            times.Add(180, 4.828003f);
            times.Add(181, 8.02404f);
            times.Add(188, 2.940996f);
            times.Add(191, 20f);
            times.Add(193, 20f);
            times.Add(200, 3.297995f);
            times.Add(201, 3.331995f);
            times.Add(203, 4.930004f);
            times.Add(206, 20f);
            times.Add(210, 6.1710186f);
            times.Add(214, 20f);
            times.Add(218, 20f);
            times.Add(219, 20f);
            times.Add(220, 20f);
            times.Add(225, 17.44215f);
            times.Add(226, 20f);
            times.Add(227, 4.675001f);
            times.Add(228, 9.69006f);
            times.Add(232, 20f);
            times.Add(233, 20f);
            times.Add(239, 20f);
            times.Add(240, 20f);
            foreach (int id in times.Keys)
            {
                _options.parTimePerLevel[id] = times[id];
            }
        }

        public static uint FileVersion
        {
            get { return _options.fileVersion; }
            set
            {
                _options.fileVersion = value;
                WriteOptionsFile();
            }
        }

        public static int PointsPerStar
        {
            get { return _options.pointsPerStar; }
            set
            {
                _options.pointsPerStar = value;
                WriteOptionsFile();
            }
        }

        public static decimal ParTimeOffset
        {
            get { return _options.parTimeOffset; }
            set
            {
                _options.parTimeOffset = value;
                WriteOptionsFile();
            }
        }

        public static int TimeLimit
        {
            get { return _options.timeLimit; }
            set
            {
                _options.timeLimit = value;
                WriteOptionsFile();
            }
        }

        public static int MaxTimeBonusPoints
        {
            get { return _options.maxTimeBonusPoints; }
            set
            {
                _options.maxTimeBonusPoints = value;
                WriteOptionsFile();
            }
        }

        public static int PointsLostPerSecond
        {
            get { return _options.pointsLostPerSecond; }
            set
            {
                _options.pointsLostPerSecond = value;
                WriteOptionsFile();
            }
        }

        public static int PointsPerTicket
        {
            get { return _options.pointsPerTicket; }
            set
            {
                _options.pointsPerTicket = value;
                WriteOptionsFile();
            }
        }

        public static int Stage2Med
        {
            get { return _options.stage2Med; }
            set
            {
                _options.stage2Med = value;
                WriteOptionsFile();
            }
        }

        public static int Stage2EasyStage3Med
        {
            get { return _options.stage2EasyStage3Med; }
            set
            {
                _options.stage2EasyStage3Med = value;
                WriteOptionsFile();
            }
        }

        public static int Stage2EasyStage3Hard
        {
            get { return _options.stage2EasyStage3Hard; }
            set
            {
                _options.stage2EasyStage3Hard = value;
                WriteOptionsFile();
            }
        }

        public static int Stage2MedStage3Hard
        {
            get { return _options.stage2MedStage3Hard; }
            set
            {
                _options.stage2MedStage3Hard = value;
                WriteOptionsFile();
            }
        }

        public static int Stage2MedStage3Hardest
        {
            get { return _options.stage2MedStage3Hardest; }
            set
            {
                _options.stage2MedStage3Hardest = value;
                WriteOptionsFile();
            }
        }

        public static bool EnableBonusLevel
        {
            get { return _options.enableBonusLevel; }
            set
            {
                _options.enableBonusLevel = value;
                WriteOptionsFile();
            }
        }

        public static bool ShowObjectives
        {
            get { return _options.showObjectives; }
            set
            {
                _options.showObjectives = value;
                WriteOptionsFile();
            }
        }

        public static bool SwipeCard
        {
            get { return _options.swipeCard; }
            set
            {
                _options.swipeCard = value;
                WriteOptionsFile();
            }
        }



        public static bool ShowWarnings
        {
            get { return _options.showWarnings; }
            set
            {
                _options.showWarnings = value;
                WriteOptionsFile();
            }
        }

        public static bool ParBonusEveryStage
        {
            get { return _options.parBonusEveryStage; }
            set
            {
                _options.parBonusEveryStage = value;
                WriteOptionsFile();
            }
        }

        public static bool FreePlay
        {
            get { return _options.freePlay; }
            set
            {
                _options.freePlay = value;
                WriteOptionsFile();
            }
        }

        public static decimal GameCost
        {
            get { return _options.gameCost; }
            set
            {
                _options.gameCost = value;
                WriteOptionsFile();
            }
        }

        public static Currency Currency
        {
            get { return _options.currency; }
            set
            {
                _options.currency = value;
                WriteOptionsFile();
            }
        }

        public static decimal CurrencyUnitValue
        {
            get { return _options.currencyUnitValue; }
            set
            {
                _options.currencyUnitValue = value;
                WriteOptionsFile();
            }
        }

        public static decimal Coin1
        {
            get { return _options.coin1; }
            set
            {
                _options.coin1 = value;
                WriteOptionsFile();
            }
        }

        public static decimal Coin2
        {
            get { return _options.coin2; }
            set
            {
                _options.coin2 = value;
                WriteOptionsFile();
            }
        }

        public static decimal DBV
        {
            get { return _options.dbv; }
            set
            {
                _options.dbv = value;
                WriteOptionsFile();
            }
        }

        public static TicketName TicketName
        {
            get { return _options.ticketName; }
            set
            {
                _options.ticketName = value;
                WriteOptionsFile();
            }
        }

        public static bool FixedTickets
        {
            get { return _options.fixedTickets; }
            set
            {
                _options.fixedTickets = value;
                WriteOptionsFile();
            }
        }

        public static int FixedTicketsPerGame
        {
            get { return _options.fixedTicketsPerGame; }
            set
            {
                _options.fixedTicketsPerGame = value;
                WriteOptionsFile();
            }
        }

        public static int TicketMultiplier
        {
            get { return _options.ticketMultiplier; }
            set
            {
                _options.ticketMultiplier = value;
                WriteOptionsFile();
            }
        }

        public static int SmallBonusCount
        {
            get { return _options.smallBonusCount; }
            set
            {
                _options.smallBonusCount = value;
                WriteOptionsFile();
            }
        }

        public static int MediumBonusCount
        {
            get { return _options.mediumBonusCount; }
            set
            {
                _options.mediumBonusCount = value;
                WriteOptionsFile();
            }
        }

        public static int BigBonusCount
        {
            get { return _options.bigBonusCount; }
            set
            {
                _options.bigBonusCount = value;
                WriteOptionsFile();
            }
        }

        public static decimal PayoutPCT
        {
            get { return _options.payoutPCT; }
            set
            {
                _options.payoutPCT = value;
                WriteOptionsFile();
            }
        }

        public static decimal TicketValue
        {
            get { return _options.ticketValue; }
            set
            {
                _options.ticketValue = value;
                WriteOptionsFile();
            }
        }

        public static int MercyTickets
        {
            get { return _options.mercyTickets; }
            set
            {
                _options.mercyTickets = value;
                WriteOptionsFile();
            }
        }

        public static int TotalGamesPlayed
        {
            get { return _options.totalGamesPlayed; }
            set
            {
                _options.totalGamesPlayed = value;
                WriteOptionsFile();
            }
        }

        public static int[] PlaysPerLevel
        {
            get { return (int[])_options.playsPerLevel.Clone(); }
            set
            {
                _options.playsPerLevel = (int[])value.Clone();
                WriteOptionsFile();
            }
        }

        public static double[] TimePerLevel
        {
            get { return (double[])_options.timePerLevel.Clone(); }
            set
            {
                _options.timePerLevel = (double[])value.Clone();
                WriteOptionsFile();
            }
        }

        public static double[] FastestPerfectTimePerLevel
        {
            get { return (double[])_options.fastestPerfectTimePerLevel.Clone(); }
            set
            {
                _options.fastestPerfectTimePerLevel = (double[])value.Clone();
                WriteOptionsFile();
            }
        }

        public static int[] StarsPerLevel
        {
            get { return (int[])_options.starsPerLevel.Clone(); }
            set
            {
                _options.starsPerLevel = (int[])value.Clone();
                WriteOptionsFile();
            }
        }

        public static int[] DeathsPerLevel
        {
            get { return (int[])_options.deathsPerLevel.Clone(); }
            set
            {
                _options.deathsPerLevel = (int[])value.Clone();
                WriteOptionsFile();
            }
        }

        public static int[] PerfectPlaysPerLevel
        {
            get { return (int[])_options.perfectPlaysPerLevel.Clone(); }
            set
            {
                _options.perfectPlaysPerLevel = (int[])value.Clone();
                WriteOptionsFile();
            }
        }

        public static long[] PointsPerLevel
        {
            get { return (long[])_options.pointsPerLevel.Clone(); }
            set
            {
                _options.pointsPerLevel = (long[])value.Clone();
                WriteOptionsFile();
            }
        }

        public static float[] ParTimePerLevel
        {
            get { return (float[])_options.parTimePerLevel.Clone(); }
            set
            {
                _options.parTimePerLevel = (float[])value.Clone();
                WriteOptionsFile();
            }
        }

        public static int GameVolume
        {
            get { return _options.gameVolume; }
            set
            {
                _options.gameVolume = Math.Max(0, Math.Min(100, value));
                WriteOptionsFile();
            }
        }

        public static int AttractVolume
        {
            get { return _options.attractVolume; }
            set
            {
                _options.attractVolume = Math.Max(0, Math.Min(100, value));
                WriteOptionsFile();
            }
        }

        public static int TicketsOwed
        {
            get { return _options.ticketsOwed; }
            set
            {
                if (_options.ticketsOwed != value)
                {
                    _options.ticketsOwed = value;
                    WriteOptionsFile();
                }
            }
        }

        public static decimal Credits
        {
            get { return _options.credits; }
            set
            {
                _options.credits = value;
                WriteOptionsFile();
            }
        }

        public static int RepeatGames
        {
            get { return _options.repeatGames; }
            set
            {
                _options.repeatGames = value;
                WriteOptionsFile();
            }
        }

        public static int ThreeFedOmNoms
        {
            get { return _options.threeFedOmNoms; }
            set
            {
                _options.threeFedOmNoms = value;
                WriteOptionsFile();
            }
        }

        public static ulong TotalTicketsOut
        {
            get { return _options.totalTicketsOut; }
            set
            {
                _options.totalTicketsOut = value;
                WriteOptionsFile();
            }
        }

        public static ulong PulsesIn
        {
            get { return _options.pulsesIn; }
            set
            {
                _options.pulsesIn = value;
                WriteOptionsFile();
            }
        }

        public static bool UseTickets
        {
            get { return _options.useTickets; }
            set
            {
                _options.useTickets = value;
                WriteOptionsFile();
            }
        }

        public static bool InstantPayout
        {
            get { return _options.instantPayout; }
            set
            {
                _options.instantPayout = value;
                WriteOptionsFile();
            }
        }

        public static int BigBonusValue
        {
            get { return _options.bigBonusValue; }
            set
            {
                _options.bigBonusValue = value;
                WriteOptionsFile();
            }
        }

        public static int MediumBonusValue
        {
            get { return _options.mediumBonusValue; }
            set
            {
                _options.mediumBonusValue = value;
                WriteOptionsFile();
            }
        }

        public static int SmallBonusValue
        {
            get { return _options.smallBonusValue; }
            set
            {
                _options.smallBonusValue = value;
                WriteOptionsFile();
            }
        }

        public static int MaxCreditsAllowed
        {
            get { return _options.maxCreditsAllowed; }
            set
            {
                _options.maxCreditsAllowed = value;
                WriteOptionsFile();
            }
        }



        public static AttractMusicFrequency AttractMusicFrequency
        {
            get { return _options.attractMusicFrequency; }
            set
            {
                _options.attractMusicFrequency = value;
                WriteOptionsFile();
            }
        }

        public static bool SuspendWrites
        {
            get { return suspendWrites; }
            set
            {
                if (value)
                {
                    suspendWrites = true;
                }
                else
                {
                    suspendWrites = false;
                    if (dirty)
                    {
                        WriteOptionsFile();
                        dirty = false;
                    }
                }
            }
        }

        private static void WriteOptionsFile()
        {
            if (suspendWrites)
            {
                dirty = true;
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OptionsStruct));
                if (FCOptionsLocation.FILE_LOCATION != "" && !Directory.Exists(FCOptionsLocation.FILE_LOCATION))
                {
                    Directory.CreateDirectory(FCOptionsLocation.FILE_LOCATION);
                }
                using (FileStream tw = new FileStream(FCOptionsLocation.FILE_LOCATION + "options.dat", FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(tw, _options);
                    tw.Flush(true);
                    tw.Close();
                }
            }
        }

    }

    public static class FCOptionsLocation
    {
        public static string FILE_LOCATION = "";
    }
}
