namespace Guitar_Tuner
{
    class Autocorrelator
    {
        float[] prevBuffer;
        int minOffset;
        int maxOffset;
        float sampleRate;

        public Autocorrelator(int sampleRate)
        {
            this.sampleRate = (float)sampleRate;
            int minFreq = 75;
            int maxFreq = 335;

            this.maxOffset = sampleRate / minFreq;
            this.minOffset = sampleRate / maxFreq;
        }

        public float DetectPitch(float[] buffer, int frames)
        {
            if (prevBuffer == null)
            {
                prevBuffer = new float[frames];
            }
            float secCor = 0;
            int secLag = 0;

            float maxCorr = 0;
            int maxLag = 0;

            // starting with low frequencies, working to higher
            for (int lag = maxOffset; lag >= minOffset; lag--)
            {
                float corr = 0; // this is calculated as the sum of squares
                for (int i = 0; i < frames; i++)
                {
                    int oldIndex = i - lag;
                    float sample = ((oldIndex < 0) ? prevBuffer[frames + oldIndex] : buffer[oldIndex]);
                    corr += (sample * buffer[i]);
                }
                if (corr > maxCorr)
                {
                    maxCorr = corr;
                    maxLag = lag;
                }
                if (corr >= 0.9 * maxCorr)
                {
                    secCor = corr;
                    secLag = lag;
                }
            }
            for (int n = 0; n < frames; n++)
            {
                prevBuffer[n] = buffer[n];
            }
            float noiseThreshold = frames / 1000f;
            //Debug.WriteLine(String.Format("Max Corr: {0} ({1}), Sec Corr: {2} ({3})", this.sampleRate / maxLag, maxCorr, this.sampleRate / secLag, secCor));
            if (maxCorr < noiseThreshold || maxLag == 0) return 0.0f;
            //return 44100.0f / secLag;   //--works better for singing
            return this.sampleRate / maxLag;
        }
    }
}
