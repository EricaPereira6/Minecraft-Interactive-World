using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioAnalysis
{
    static float[] samples = new float[1024];

    static float samplingFrequency = AudioSettings.outputSampleRate;


    public static float[] GetSamples(AudioSource audioSource)
    {
        audioSource.GetOutputData(samples, 0);
        return samples;
    }

    //nao sao amostras no dominio do tempo, sao amosntras no dominio da frequencia
    public static float[] GetSpectrum(AudioSource audioSource)
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
        return samples;
    }


    // medir a concentracao de energia em torno do pico de freq
    public static float ConcentrationAroundPeak(float peakFrequency)
    {
        int index = (int)(peakFrequency * samples.Length / (0.5f * samplingFrequency));
        int centredBand = samples.Length / 200;  // so considera 10 riscas a volta do pico - 5 esq, 5 dir
        float total = 0;
        float insideBand = 0;

        // medir a energia total
        for (int i = 0; i < samples.Length - 5; i++)
        {
            total += samples[i] * samples[i];
            if (Mathf.Abs(i - index) < centredBand)  // dentro da banda, so as 10 riscas
            {
                insideBand += samples[i] * samples[i];
            }
        }
        // retorna-se a percentagem de energia naquela banda
        return insideBand / total;   
    }


    public static float CumputeSpectrumPeak(AudioSource audioSource, bool interpolate)
    {
        GetSpectrum(audioSource);
        float max = 0;
        float index = 0;

        // n vale a pena precorrer as riscas espetrais todas
        // o maximo da freq nunca chega aos 20 000 e poucos -> 3/4 das samples
        for (int i = 0; i < 3 * samples.Length / 4; i++) 
        {
            if (samples[i] > max)
            {
                max = samples[i];
                index = i;
            }
        }
        // melhorar precisao - interpolacao quadratica
        // so posso interpolar se o pico não estiver nas extremidades, senao da erro
        if (interpolate && index > 0 && index < samples.Length - 1) 
        {
            float dL = samples[(int)index] - samples[(int)index - 1]; // diferanca entre o peak e o indice anterior
            float dR = samples[(int)index] - samples[(int)index + 1]; // diferanca entre o peak e o indice seguinte

            index += (dL - dR) / (2 * (dL + dR)); // correcao
        }

        // converter indice na frequencia do pico
        // de o index for igual a samples, o pico é 24 000 -> sampling * 0.5f
        float peak = (index * 0.5f * samplingFrequency) / samples.Length;
        return peak;
    }


    //energia media de cada amostra - valor quadratico medio
    public static float MeanEnergy(AudioSource audioSource)
    {
        audioSource.GetOutputData(samples, 0);
        float sum = 0;
        for(int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        return sum / samples.Length;
    }


    // valor eficaz - raiz do valor quadratico medio (RMS)
    // medida estatistica da magnitude de uma quantidade variavel
    public static float ComputeRMS(AudioSource audioSource)
    {
        float meanEnergy = MeanEnergy(audioSource);
        return Mathf.Sqrt(meanEnergy);
    }


    public static float ConvertToDB(float val)
    {
        float reference = 1e-7f;
        if (val < reference) val = reference;
        return 10f * Mathf.Log10(val / reference);
    }


    public static float[] FreqBands(AudioSource audioSource)
    {
        GetSpectrum(audioSource);

        int nBands = 7;
        float[] bands = new float[nBands];
        int numSamples = (8 * samples.Length) / 1024;

        int iniSample = 0;
        for (int i = 0; i < nBands; i++)
        {
            float sum = 0;
            for (int j = iniSample; j < iniSample + numSamples; j++)
            {
                sum += samples[j] * samples[j];
            }

            bands[i] = ConvertToDB(sum / numSamples);
            iniSample += numSamples;
            numSamples += 2;
        }
        return bands;

    }
}
