using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mic : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip audioClip;
    public string deviceName;
    public static bool useMic;

    //float timestamp;

    const float DBWistle = 40;
    const float minPeakWistle = 1100;
    const float concentrationWistle = 0.95f;

    const float minDBClap = 30;
    const float maxDBClap = 60;
    const float minPeakClap = 1000;
    const float maxPeakClap = 2000;
    const float minConcentrationClap = 0.2f;
    const float maxConcentrationClap = 0.5f;

    public static int treeHeight;
    int oldTreeHeight;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        useMic = false;
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();

        treeHeight = 0;
        oldTreeHeight = 0;

        //timestamp = 0;

        StartCoroutine(DetectSound());
    }

    void UseMicrophone()
    {
        if (BlockInteraction.MicInteraction())
        {
            useMic = !useMic;

            if (useMic)
            {
                if (Microphone.devices.Length > 0)
                {
                    deviceName = Microphone.devices[0];
                    // buffer Size in Seconds = 10;
                    // AudioSettings.outputSampleRate -> 48000 Hz
                    audioSource.clip = Microphone.Start(deviceName, true, 10, AudioSettings.outputSampleRate);

                    while (Microphone.GetPosition(deviceName) < (AudioSettings.outputSampleRate / 1000) * 30);
                }
                else
                {
                    TextCanvas.AddMessage("There are no microphone devices.", Color.red);
                    useMic = false;
                }
            }
            if (!useMic)
            {
                audioSource.clip = audioClip;
            }

            audioSource.loop = true;
            audioSource.Play();

            string micText = useMic ? "Select E: microphone is on" : "Select E: microphone is off";
            Color micColor = useMic ? Color.red : Color.black;
            TextCanvas.AddMessage(micText, micColor);
        }
        
    }

    IEnumerator DetectSound()
    {
        while (true)
        {
            yield return null;

            if (useMic)
            {
                float energy = AudioAnalysis.MeanEnergy(audioSource);
                float DB = AudioAnalysis.ConvertToDB(energy);
                float peakFrequency = AudioAnalysis.CumputeSpectrumPeak(audioSource, true);
                float concentration = AudioAnalysis.ConcentrationAroundPeak(peakFrequency);
                float RMS = AudioAnalysis.ComputeRMS(audioSource);

                // wistle
                if (DB > DBWistle && peakFrequency > minPeakWistle && concentration > concentrationWistle)
                {
                    treeHeight = (int)Mathf.Clamp(((peakFrequency - minPeakWistle) * 2) / 100, 2, 20);

                    if (oldTreeHeight != treeHeight)
                    {
                        oldTreeHeight = treeHeight;

                        //TextCanvas.AddMessage("peak: " + peakFrequency, Color.black);

                        BlockInteraction.SetTreeHeight(treeHeight);
                        BlockInteraction.SetInteractionType(BlockInteraction.InterationType.WISTLE);
                        BlockInteraction.StartSoundInteraction();
                    }
                    
                    yield return new WaitForSeconds(0.2f);
                }
                // clap
                else if (minDBClap < DB && DB < maxDBClap &&
                    minPeakClap < peakFrequency && peakFrequency < maxPeakClap && 
                    minConcentrationClap < concentration && concentration < maxConcentrationClap)
                {
                    BlockInteraction.SetInteractionType(BlockInteraction.InterationType.CLAP);
                    BlockInteraction.StartSoundInteraction();

                    //TextCanvas.AddMessage("DB: " + DB + ", PK:" + peakFrequency + ", CCT: " + concentration + ", RMS: " + RMS, Color.black);
                    yield return new WaitForSeconds(0.2f);
                }
                //print("DB: " + DB + ", PK:" + peakFrequency + ", CCT: " + concentration + ", RMS: " + RMS);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        UseMicrophone();    
    }

}




// Test Wistle an Clap

//if (Time.realtimeSinceStartup - timestamp > 0.2f)
//{
//    if (IsUsingMic())
//    {
//        float energy = AudioAnalysis.MeanEnergy(audioSource);
//        float DB = AudioAnalysis.ConvertToDB(energy);
//        float peakFrequency = AudioAnalysis.CumputeSpectrumPeak(audioSource, true);
//        float concentration = AudioAnalysis.ConcentrationAroundPeak(peakFrequency);

//        float RMS = AudioAnalysis.ComputeRMS(audioSource);

//        print("DB: " + DB + " ----------- PEAK: " + peakFrequency + " ----------- concentration: " + concentration);
//        if (DB < 1.5f && peakFrequency > 200 && concentration < 0.92f)
//        {
//            print("DB: " + DB + " ----------- PEAK: " + peakFrequency + " ----------- concentration: " + concentration);
//        }
//        if (DB > 10 && peakFrequency > 1000 && concentration < 0.7f)
//        {
//            print("CLAP - DB: " + DB + " ----------- PEAK: " + peakFrequency + " ----------- concentration: " + concentration + " --------- RMS: " + RMS);
//        }
//        if (DB > 20 && peakFrequency > 1000 && concentration < 0.7f)
//        {
//            print("DB: " + DB + " ----------- PEAK: " + peakFrequency + " ----------- concentration: " + concentration);
//        }

//        timestamp = Time.realtimeSinceStartup;
//    }

//}
