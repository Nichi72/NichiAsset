using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ObjCut : MonoBehaviour {
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag=="MoveObj")
        {
            GameObject Temp = other.transform.gameObject;
            Temp.GetComponent<MeshRenderer>().enabled = false;
            Temp.transform.GetChild(0).gameObject.SetActive(true);
            Temp.transform.GetChild(1).gameObject.SetActive(true);
            Destroy(Temp, 2f);
        }
    }

}
