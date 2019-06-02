using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spectrogram : MonoBehaviour
{
    MicrophoneInput analyser;
    public int numBands = 16;
    GameObject[] bands;
    public int scale = 1;
    public float slowing = 1;
    public Material material;
    public Gradient colorGradient;
    // Start is called before the first frame update
    void Start()
    {
        analyser = FindObjectOfType<MicrophoneInput>();
        bands = new GameObject[numBands];
        for (int i = 0; i < numBands; i++) {
            bands[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bands[i].transform.parent = this.transform;
            //bands[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
            bands[i].transform.localPosition = new Vector3(Mathf.Log(i+1)*1f, 0, 0); // new Vector3(i, 0, 0);
            bands[i].transform.localScale = Vector3.one * (Mathf.Log(i + 2) - Mathf.Log(i + 1)); // WHY DOESN'T THIS WORK AS EXPECTED??
            bands[i].GetComponent<MeshRenderer>().material = material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        material.color = colorGradient.Evaluate(100f*analyser.GetVolume());
        float[] spectrum = analyser.GetSpectrumData(numBands);  
        for(int i = 0; i < bands.Length; i++)
        {
            bands[i].transform.localScale =
                new Vector3(bands[i].transform.localScale.x,
                Mathf.Lerp(bands[i].transform.localScale.y, spectrum[i] * scale, Time.deltaTime / slowing),
               Mathf.Lerp(bands[i].transform.localScale.z, spectrum[i] * scale, Time.deltaTime / slowing));
            //bands[i].transform.localScale.z);
                
        }
    }
}
