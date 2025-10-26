using System;
using System.Collections.Generic;
using Tao.Sdl;

namespace CTR_MonoGame
{
    class SoundFX
    {
        static Dictionary<string, IntPtr> cache = new Dictionary<string, IntPtr>();

        IntPtr ptr;

        public SoundFX(string filename)
        {
            string canonicalName = filename;
            if (!canonicalName.EndsWith(".ogg"))
            {
                canonicalName += ".ogg";
            }
            if (!canonicalName.StartsWith("Content/"))
            {
                canonicalName = "Content/" + canonicalName;
            }
            if (!cache.ContainsKey(canonicalName))
            {
                cache.Add(canonicalName, SdlMixer.Mix_LoadWAV(canonicalName));
            }
            ptr = cache[canonicalName];
        }

        public void Play()
        {
            if (SdlMixer.Mix_PlayChannel(-1, ptr, 0) < 0)
            {
                Console.WriteLine(SdlMixer.Mix_GetError());
            }
        }

        public void PlayFor(int milliseconds)
        {
            if (SdlMixer.Mix_PlayChannelTimed(-1, ptr, -1, milliseconds) < 0)
            {
                Console.WriteLine(SdlMixer.Mix_GetError());
            }
        }
    }
}
