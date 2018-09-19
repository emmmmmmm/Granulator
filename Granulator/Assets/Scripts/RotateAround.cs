using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour {

    public Transform child;
    public float distToCenter = 1;
    public float speed = 1f;
    public bool active = false;
    public float angle = 0;
    
    // Use this for initialization
	void Start () {
        if (child != null) active = true;
        else Debug.LogError("No rotator found");
        child.position = transform.position + transform.right * distToCenter;
   
	}
	
	// Update is called once per frame
	void Update () {
        child.position = transform.position + transform.right * distToCenter;
        if (!active) { 
        //angle += speed * Time.deltaTime;
        if (angle > 360) angle = 0;

        Vector3 angles = transform.eulerAngles;

        angles.y = angle; 

        transform.eulerAngles = (angles);
        }
        else {
            transform.Rotate(transform.up, speed * Time.deltaTime);
        }


    }
    private void OnDrawGizmos()
    {
    //    if (!active) return;
        Gizmos.DrawLine(transform.position, child.position);
        Gizmos.DrawWireSphere(transform.position, .1f);
        Gizmos.DrawWireSphere(child.position, .1f);
    }
}
