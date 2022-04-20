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
    private Vector3 grainTransform;
    private float grainAttack = 0.1f;
    private float grainRelease = 0.1f;
    public AudioClip audioClip;
    private AudioSource audioSource;
    private float[] samples;
    private float[] grainSamples;
    private int channels;
    private int currentIndex = -1;
    private Granulator granulator;


    private MeshRenderer body;

    public float posUpdateSpeed = 10;

    //---------------------------------------------------------------------
    void Start()
    {
        UpdateGrain();
        gameObject.AddComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialize = false;
        audioSource.spatialBlend = 0;
        audioSource.clip = null;
        granulator = GetComponentInParent<Granulator>();
        body = GetComponentInChildren<MeshRenderer>();
    }
    //---------------------------------------------------------------------
    // called if audio file changes
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
        transform.position = grainTransform;
        if (isPlaying)  { body.enabled = true; body.gameObject.SetActive(true); }
        else            { body.enabled = false; body.gameObject.SetActive(false); }
    }
    //---------------------------------------------------------------------
    public void NewGrain(Vector3 newGrainTransform, int newGrainPos, int newGrainLength, float newGrainPitch, float newGrainPitchRand, float newGrainVol, float newGrainAttack, float newGrainRelease, Vector3 pos)
    {
        grainPos = (int)((newGrainPos / channels)) * channels; // rounding to make sure pos always starts at first channel!
        grainTransform = newGrainTransform;
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
    public void UpdatePosition(Vector3 pos, bool lerpPos = false)
    {
        if (lerpPos)
            grainTransform = Vector3.Lerp(transform.position, pos, posUpdateSpeed * Time.deltaTime);
        else
            grainTransform = pos;
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
