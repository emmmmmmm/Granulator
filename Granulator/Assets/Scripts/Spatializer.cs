using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spatializer : MonoBehaviour
{
    public AnimationCurve distanceRollOff;
    public AnimationCurve rotationRollOff; 
    public float maxDist = 100;
    private AudioListener listener;
    public bool attenuateDistance = true;
    public bool attenuateDirection = true;
    public bool panning = true;
    public bool doppler = true;
    public float dopplerAmount = 1f;
    private Vector3 toListener;
    private Vector3 prevToListener;
    // private Quaternion rotationBetween;
    private float distToListener = 1;

    private float sideAttenuation;
    private float distAttenuation;
    private float LR;

  
    //-------------------------------------------------
    void Start()
    {
        toListener = Vector3.one;
        prevToListener = Vector3.one;
        listener = FindObjectOfType<AudioListener>();
    }
    //-------------------------------------------------
    private void Update() // fixedUpdate?? -> would have to change in grain as well!
    {
        UpdateValues();
    }
    //-------------------------------------------------
    private void UpdateValues()
    {
        // from main thread only :/
        toListener = transform.position - listener.transform.position;
        // rotationBetween = Quaternion.FromToRotation(transform.forward, listener.transform.forward);
        distToListener = Vector3.Magnitude(toListener);
        distToListener = distToListener < maxDist ? distToListener : maxDist;

        distAttenuation = Mathf.Clamp01(distanceRollOff.Evaluate(distToListener / maxDist)); // using the Animation Curve for distance attenuation
        sideAttenuation = Mathf.Clamp01(rotationRollOff.Evaluate(
            (Vector3.Dot(transform.forward, listener.transform.forward) * -1f + 1f) / 2f
            )); // 0->1 // 


        LR = (Vector3.Dot(toListener.normalized, listener.transform.right)); 
        LR = (LR + 1f) / 2f;

        // TODO: 
        // how can I do this faster?
        // might be able to do that in OAFR, bc I'm only reading transforms? -> test! -> nope. :/

        // TODO: is there a way to do this without having to set pitch on the noise-gen every frame?? 
        if (doppler) GetComponent<AudioSource>().pitch += Mathf.Clamp((dopplerAmount - (toListener.sqrMagnitude / prevToListener.sqrMagnitude) * dopplerAmount), -3f, 3f);
        prevToListener = toListener;
    }

    //-------------------------------------------------
    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            for (int c = 0; c < channels; c++)
            {
                // distance attenuation
                if (attenuateDistance) data[i + c] *= distAttenuation;
                // direction attenuation
                if (attenuateDirection) data[i + c] *= sideAttenuation;


                // TODO: 
                // pan
                // depending on the number of channels there's a specific angle between channels. (2chans: 180 degs, 4 chans 90 degs, 5 chans 60 degs etc. 
                // BUT if it's 6 channel, then the 6th channel could be the .1?? or not?
                // OOOR is what I'll get here ALWAYS mono or stereo??

                if (panning && channels == 2)
                {
                    // is channels always 2 here? I DONT KNOW
                    if (c == 0) data[i + c] *= (Mathf.Sqrt(1f - LR));
                    if (c == 1) data[i + c] *= (Mathf.Sqrt(LR));

                }
            }
        }
    }
    //-------------------------------------------------
}
