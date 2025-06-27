
using NAudio.Wave;

namespace AK47_CyberSecurity_POE
{
    internal class WaveOutEvent
    {
        public WaveOutEvent()
        {
        }

        public Action<object, object> PlaybackStopped { get; internal set; }
        public PlaybackState PlaybackState { get; internal set; }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }

        internal void Init(AudioFileReader audioFile)
        {
            throw new NotImplementedException();
        }

        internal void Play()
        {
            throw new NotImplementedException();
        }
    }
}