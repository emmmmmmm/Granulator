using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Granulator))]
public class GrainEngineScript : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float RPM = 0;
    [Range(0.0f, 1.0f)]
    public float minGrainPos = 0;
    [Range(0.0f, 1.0f)]
    public float maxGrainPos = 1;

    public int minGrainLength = 10;
    public int maxGrainLength = 1000;

    public int minGrainDist = 50;
    public int maxGrainDist = 10;

    public float minGrainPitch = .5f;
    public float maxGrainPitch = 2f;

    [Range(0.0f, 1.0f)]
    public float minGrainVol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float maxGrainVol = .8f;

    private Granulator granulator;

    //---------------------------------------------------------------------
    void Start() // change to awake?
    {
        granulator = GetComponent<Granulator>();
    }

    //---------------------------------------------------------------------
    void Update()
    {
        if (minGrainPitch < 0) minGrainPitch = 0;
        granulator.grainLength = (int)map(RPM, 0.0f, 1f, minGrainLength, maxGrainLength);
        granulator.grainPos = map(RPM, 0.0f, 1f, minGrainPos, maxGrainPos);
        granulator.grainDist = (int)map(RPM, 0.0f, 1f, minGrainDist, maxGrainDist);
        granulator.grainPitch = map(RPM, 0.0f, 1f, minGrainPitch, maxGrainPitch);
        granulator.grainVol = map(RPM, 0.0f, 1f, minGrainVol,maxGrainVol);

    }

    //---------------------------------------------------------------------
    // utility func to map a value from one range to another range
    private float map(float val, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + ((outMax - outMin) / (inMax - inMin)) * (val - inMin);
    }
}
