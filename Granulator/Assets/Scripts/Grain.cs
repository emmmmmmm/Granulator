using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Grain : MonoBehaviour
{
    public bool isPlaying = false;
    private int grainPos;
    private int grainLength;
    public float grainPitch;
    private float grainPitchRand;
    private float grainVol;
    private float grainAttack = 0.1f;
    private float grainRelease = 0.1f;
    public AudioClip audioClip;
    private AudioSource audioSource;
    private float[] samples;
    private float[] grainSamples;
    private int channels;
    private int currentIndex = -1;

    //---------------------------------------------------------------------
    void Start()
    {
        UpdateGrain();
        this.gameObject.AddComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialize = false;
        audioSource.spatialBlend = 0;
        audioSource.clip = null;
    }
    //---------------------------------------------------------------------
    public void UpdateGrain()
    {
        samples = new float[audioClip.samples * audioClip.channels];
        channels = audioClip.channels;
        audioClip.GetData(samples, 0);
    }
    //---------------------------------------------------------------------
    void Update()
    {
        audioSource.pitch = grainPitch + grainPitchRand;
    }
    //---------------------------------------------------------------------
    public void NewGrain(int newGrainPos, int newGrainLength, float newGrainPitch, float newGrainPitchRand, float newGrainVol, float newGrainAttack, float newGrainRelease, Vector3 pos)
    {
        grainPos = (int)((newGrainPos / channels)) * channels; // rounding to make sure pos always starts at first channel!
        grainLength = newGrainLength;
        grainPitch = newGrainPitch;
        grainPitchRand = newGrainPitchRand;
        grainVol = newGrainVol;
        grainAttack = newGrainAttack;
        grainRelease = newGrainRelease;
        isPlaying = true;
        BuildSamplesAR();
    }
    //---------------------------------------------------------------------
    private void BuildSamplesAR()
    {
        grainSamples = new float[grainLength];

        int sourceIndex = grainPos;

        // build ar of samples for this grain:
        for (int i = 0; i < grainSamples.Length - channels; i += channels)
        {
            sourceIndex = grainPos + i;

            // loop the file if the grain is longer than the source-audio! (or the grain starts at the very end of the source-audio)
            while (sourceIndex + channels > samples.Length)
                sourceIndex -= samples.Length;

            for (int j = 0; j < channels; j++)
            {
                grainSamples[i + j] = samples[sourceIndex + j];
            }
        }

        // fades for the grain, so it doesn't create clicks on start/stop!
        // control with grain attack and grain release inputs
        for (int i = 0; i < grainSamples.Length; i += channels)
        {
            for (int j = 0; j < channels; j++)
            {
                if (i < grainSamples.Length * grainAttack) grainSamples[i + j] *= Map(i, 0, grainSamples.Length * grainAttack, 0f, 1f);
                else if (i > grainSamples.Length * (1.0f - grainRelease)) grainSamples[i + j] *= Map(i, grainSamples.Length * (1.0f - grainRelease), grainSamples.Length, 1f, 0f);
            }

        }
        currentIndex = -1;
    }
    //---------------------------------------------------------------------
    private float GetNextSample(int index, int sample)
    {
        if (currentIndex + 1 >= grainSamples.Length)
        {
            currentIndex = -1;
            isPlaying = false;

        }
        currentIndex++;
        return grainSamples[currentIndex];
    }
    //---------------------------------------------------------------------
    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            for (int c = 0; c < channels; c++)
            {
                if (isPlaying) data[i + c] = GetNextSample(i, c) * grainVol;
                else data[i + c] = 0;
            }
        }
    }
    //---------------------------------------------------------------------
    // utility func to map a value from one range to another range
    private float Map(float val, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + ((outMax - outMin) / (inMax - inMin)) * (val - inMin);
    }
}
