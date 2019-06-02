using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // required for dealing with audiomixers

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
    AudioSource audioSource;
    public AudioMixer audioMixer;
    public string deviceName = "null";// "Analog (1+2) (RME Fireface UC)";
    public string[] devices;
    float[] spectrum;
    public float[] simpleSpectrum;
    public float minVal = 0.000001f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        devices = Microphone.devices;
        Microphone.GetDeviceCaps(deviceName, out int minFreq, out int maxFreq);
       // Debug.Log(minFreq + " / " + maxFreq);

       // audioSource.clip = Microphone.Start(deviceName, true, 1, maxFreq);
        audioSource.loop = true;
        //while (!(Microphone.GetPosition(deviceName) > 0)) { }
        Debug.Log("start playing... position is " + Microphone.GetPosition(deviceName));
        audioSource.Play();

        spectrum = new float[256];

       // audioMixer.SetFloat("AnalysisVolume", -100); // mute audio output (via exposed parameter in audio mixer!)

    }

    // Update is called once per frame
    void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
    }

    public float GetVolume()
    {
        float ret = 0;
        for (int i = 0; i < spectrum.Length; i++) ret += spectrum[i];
        return ret / spectrum.Length;
    }

    float[] SimplifySpectrum(float[] spectrum,int numbands)
    {

        
        float[] ret = new float[numbands];
        int ratio = spectrum.Length / ret.Length;
        for (int i = 0, j = 0; i < ret.Length; i++)
        {
            for (j = 0; j < ratio; j++)
            {
                ret[i] += spectrum[i + j];
            }
            ret[i] /= ratio;
            ret[i] = Mathf.Log(1f+ret[i]);
        }
        return ret;
    }

    public float[] GetSpectrumData(int numbands) {
        return SimplifySpectrum(spectrum, numbands);
    }


    public float[] GetSpectrumDataLinear(int numbands)
    {
        float[] ret = new float[numbands];

        for (int i = 0; i < numbands; i++) {
            

        }


        return ret;
    }
}
