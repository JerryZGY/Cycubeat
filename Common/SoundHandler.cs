using System;
using System.Media;
using System.Windows;

namespace Cycubeat
{
    public class SoundHandler
    {
        private SoundPlayer snd;

        private Uri cheerUri = new Uri("/Cycubeat;component/Sounds/Cheer.wav", UriKind.Relative);

        private Uri booUri = new Uri("/Cycubeat;component/Sounds/Boo.wav", UriKind.Relative);

        public SoundHandler()
        {
            snd = new SoundPlayer();
        }

        public void PlayCheerSound()
        {
            snd.Stream = Application.GetResourceStream(cheerUri).Stream;
            snd.Play();
        }

        public void PlayBooSound()
        {
            snd.Stream = Application.GetResourceStream(booUri).Stream;
            snd.Play();
        }
    }
}