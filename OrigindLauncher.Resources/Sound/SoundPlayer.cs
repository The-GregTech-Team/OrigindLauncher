using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NAudio.Vorbis;
using NAudio.Wave;

namespace OrigindLauncher.Resources.Sound
{
    public static class SoundPlayer
    {
        private static readonly IWavePlayer SoundOut = new DirectSoundOut();

        public static void PlaySound(Stream soundStream)
        {
            var vorbisWaveReader = new VorbisWaveReader(soundStream);
            SoundOut.Init(vorbisWaveReader);
            SoundOut.Play();
        }

    }
}
