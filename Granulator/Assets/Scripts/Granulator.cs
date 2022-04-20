using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    TODO:
    . pause audio-sources of graines when inactive! (that should help a lot performance-wise! )
    . adjust grain-length with pitch! ... somehow
    . add velocity to grains, so they can move around in space! -> make them movers
    . add Octaver option (to randomly generate grains with x-octaves offset! !! <- that'd be fun i guess?
    . add the possibility to spawn grains inside a volume, best case: use a 2d (3d?) probability-map.
 */



public class Granulator : MonoBehaviour
{
    [Header("Granular Attributes")]
    public int maxGrains = 10;
    [Range(0.0f, 1000f)]
    public int grainLength = 100;       // ms
    [Range(0.0f, 1000f)]
    public int grainLengthRand = 0;     // ms
    [Range(0.0f, 1.0f)]
    public float grainPos = 0;          // from 0 > 1
    [Range(0.0f, 1.0f)]
    public float grainPosRand = 0;      // from 0 > 1
    [Range(0.0f, 1000f)]
    public int grainDist = 10;          // ms
    [Range(0.0f, 1000f)]
    public int grainDistRand = 0;       // ms
    [Range(0.0f, 5f)]
    public float grainPitch = 1;
    [Range(0.0f, 1f)]
    public float grainPitchRand = 0;
    [Range(0.0f, 2.0f)]
    public float grainVol = 1;          // from 0 > 1
    [Range(0.0f, 1.0f)]
    public float grainVolRand = 0;      // from 0 > 1
    [Range(0.0f, 0.5f)]

    public float grainAttack = .3f;     // from 0 > 1
    [Range(0.0f, 0.5f)]
    public float grainRelease = .3f;    // from 0 > 1


    [Header("Player")]
    public bool isPlaying = true;       // the on/off button
                                        //public bool updateGrainPos = true;


    public AudioClip audioClip;
    public GameObject grainPrefab;
    private AudioClip lastClip;
    // tmp vars
    private int newGrainPos = 0;
    private float newGrainPitch = 0;
    private float newGrainPitchRand = 0;
    private int newGrainLength = 0;
    private int newGrainDist = 0;
    private float newGrainVol = 0;
    private Vector3 newGrainTransform = Vector3.zero;

    private int channels;
    private Grain[] grains;
    private int grainTimer = 0;
    private Vector3 pos;

    public bool moveGrains = true;



    [Header("GrainPositionProperties")]
    public float pos3DJit = 3;

    [Header("Density Field - Experimental")]
    public bool useDensityField = false;
    public float densityfieldX = 10;
    public float densityFieldZ = 10;
    public Texture2D densityFieldTexture;

    //---------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        if (useDensityField)
        {
            Gizmos.color = Color.red;
            Vector3 A = Vector3.zero;
            Vector3 B = Vector3.right * densityfieldX;
            Vector3 C = new Vector3(densityfieldX, 0, densityFieldZ);
            Vector3 D = Vector3.forward * densityFieldZ;

            Gizmos.DrawLine(A, B);
            Gizmos.DrawLine(B, C);
            Gizmos.DrawLine(C, D);
            Gizmos.DrawLine(D, A);
        }
    }

    //---------------------------------------------------------------------
    public Vector3 GetGrainPosition()
    {

        // SOMEHOW find a point inbetween A-B-C-D

        /*
         https://stackoverflow.com/questions/8327776/set-pixel-on-randomized-position-depending-on-brightness-in-the-underlying-image
         */


        if (useDensityField)
        {
            return GetPosFromHeightMap();
        }
        else
        {
            // alternatively I could spawn them randomly around a "spawnpoint" ?
            Vector3 jit = new Vector3(
                Random.Range(-pos3DJit, pos3DJit),
                Random.Range(-pos3DJit, pos3DJit),
                Random.Range(-pos3DJit, pos3DJit)
                );


            // for now just return parent position
            return transform.position + jit;
        }



    }

    //---------------------------------------------------------------------
    Vector3 GetPosFromHeightMap()
    {
        if (densityFieldTexture)
        {
            Vector3 ret = Vector3.zero;
            int n = 0;
            int x, z;
            float p;
            while (n < (densityFieldTexture.width * densityFieldTexture.height))
            {
                x = (Random.Range(0, densityFieldTexture.width));
                z = (Random.Range(0, densityFieldTexture.height));
                p = Random.value;

                if (p < densityFieldTexture.GetPixel(x, z).grayscale)
                {
                    ret.x = densityfieldX - (float)x / densityFieldTexture.width * densityfieldX;
                    ret.z = densityFieldZ - (float)z / densityFieldTexture.height * densityFieldZ;
                    break;
                }
                n++;
            }
            return ret;
        }
        else return Vector3.zero;
    }


    //---------------------------------------------------------------------
    private void Start()
    {
        this.gameObject.AddComponent<AudioSource>();
    }

    void Awake()
    {
        grains = new Grain[maxGrains];
        for (int i = 0; i < grains.Length; i++)
        {
            GameObject tmp = Instantiate(grainPrefab); //, this.transform);
            grains[i] = tmp.GetComponent<Grain>();
            grains[i].audioClip = audioClip;
            grains[i].transform.position = transform.position;
            // grains[i].updatePos = updateGrainPos;
        }
    }

    // would be good if actually every grain will get freshly randomized values, but idk if that's possible... 
    // updating the random-vals every frame has to suffice for now... :/
    //---------------------------------------------------------------------
    void Update()
    {
        if (lastClip != audioClip)
        {
            lastClip = audioClip;
            for (int i = 0; i < grains.Length; i++)
            {
                grains[i].audioClip = audioClip;
                grains[i].UpdateGrain();
            }
        }


        // clamp values to "reasonable" ranges:
        grainPos = Clamp(grainPos, 0, 1);
        grainPosRand = Clamp(grainPosRand, 0, 1);
        grainDist = (int)Clamp(grainDist, 1, 10000);
        grainDistRand = (int)Clamp(grainDistRand, 0, 10000);
        grainLengthRand = (int)Clamp(grainLengthRand, 0, 10000);


        grainPitch = Clamp(grainPitch, 0, 1000);
        grainPitchRand = Clamp(grainPitchRand, 0, 1000);
        grainVol = Clamp(grainVol, 0, 2);
        grainVolRand = Clamp(grainVolRand, 0, 1);


        // calculate randomized values for new grains:
        newGrainPos = (int)((grainPos + Random.Range(0, grainPosRand)) * audioClip.samples);
        newGrainTransform = GetGrainPosition();
        newGrainPitch = grainPitch;
        newGrainPitchRand = Random.Range(-grainPitchRand, grainPitchRand);
        newGrainLength = (int)(audioClip.frequency / 1000 * (grainLength + Random.Range(0, grainLengthRand)));
        newGrainDist = audioClip.frequency / 1000 * (grainDist + Random.Range(0, grainDistRand));

        newGrainVol = Clamp(grainVol + Random.Range(-grainVolRand, grainVolRand), 0, 3);

        // update Pitch for ALL grains (?)
        // so we get instant pitch updates, and not just for newly spawned grains
        for (int i = 0; i < grains.Length; i++)
        {
            // updating pitch like this introduces crackle
            // grains[i].grainPitch = newGrainPitch;

            // update Grain position:
            if (moveGrains) grains[i].UpdatePosition(GetGrainPosition(), true);


        }

        pos = transform.position;





    }
    //---------------------------------------------------------------------
    // utility func to clamp a value between min and max
    float Clamp(float val, float min, float max)
    {
        val = val > min ? val : min;
        val = val < max ? val : max;
        return val;
    }
    //---------------------------------------------------------------------
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isPlaying) return;
        for (int i = 0; i < data.Length; i++)
        {
            grainTimer--;
            int index = 0;
            if (grainTimer <= 0)
            {
                while (grains[index].isPlaying && index < grains.Length - 1) index++;
                // if there's a free grain, restart it:
                if (index < grains.Length)
                {
                    grains[index].NewGrain(newGrainTransform, newGrainPos, newGrainLength, newGrainPitch, newGrainPitchRand, newGrainVol, grainAttack, grainRelease, pos);
                    grainTimer = newGrainDist; // reset timer
                }
            }
        }
    }
}