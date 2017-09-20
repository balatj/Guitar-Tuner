using NAudio.Wave;
using System;

namespace Guitar_Tuner
{
    public class Pitch
    {
        IWaveProvider source;
        WaveBuffer waveBuffer;
        Autocorrelator pitchDetector;

        public Pitch(IWaveProvider source)
        {
            if (source.WaveFormat.SampleRate != 44100)
            {
                throw new ArgumentException("Source must be at 44.1kHz");
            }

            if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Source must be IEEE floating point audio data");
            }

            if (source.WaveFormat.Channels != 1)
            {
                throw new ArgumentException("Source must be a mono input source");
            }

            this.source = source;
            this.pitchDetector = new Autocorrelator(source.WaveFormat.SampleRate);
            this.waveBuffer = new WaveBuffer(8192);
        }

        public float Get(byte[] buffer)
        {
            if (waveBuffer == null || waveBuffer.MaxSize < buffer.Length)
            {
                waveBuffer = new WaveBuffer(buffer.Length);
            }

            int bytesRead = source.Read(waveBuffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                bytesRead = buffer.Length;
            }

            int frames = bytesRead / sizeof(float);

            return pitchDetector.DetectPitch(waveBuffer.FloatBuffer, frames);
        }
    }
}
