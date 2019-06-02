using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camRotator : MonoBehaviour
{
    MicrophoneInput analyser;
    public int scale = 360;
    public float slowing = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        analyser = FindObjectOfType<MicrophoneInput>();
    }

    // Update is called once per frame
    void Update()
    {
        float amount = analyser.GetVolume();

        this.transform.localRotation = Quaternion.Lerp(
            this.transform.localRotation, 
            Quaternion.Euler(new Vector3(0, amount * scale, 0)),
            Time.deltaTime/slowing);
    }
}
