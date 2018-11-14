using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Lerp : MonoBehaviour {


    public Transform endTr;
    public float speed = 10f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, endTr.position, Time.deltaTime * speed);
        
	}
}
