using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class WhiteNoiseGenerator : MonoBehaviour {
    System.Random rand = new System.Random();
  public  int chans;
    private void Start()
    {
        GetComponent<AudioSource>().spatialize = false;
        GetComponent<AudioSource>().spatialBlend = 0;
    }
    private void Update()
    {
        GetComponent<AudioSource>().pitch = 1f;
    }
    void OnAudioFilterRead(float[] data, int channels)
    {
        chans = channels;
        for (int i = 0; i < data.Length; i += channels)
        {
            for (int c = 0; c < channels; c++)
            {
                data[i+c] = (float)(rand.NextDouble() * 2.0 - 1.0);
            }
        }
    }
}
