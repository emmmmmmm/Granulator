using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePosition : MonoBehaviour
{
    [Range(0, 1)]
    public float pos = 0;
    AudioSource audioSource;
    public bool jump = true;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Log(audioSource.clip.length);
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = audioSource.time;
        float totalTime = audioSource.clip.length;

        //pos =  currentTime/totalTime;
        if (jump) { 
        audioSource.time = pos*totalTime;
            jump = false;
        }
    }
}
