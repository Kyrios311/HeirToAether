using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {
    float speed = 100;
    public Transform cloudStart;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(-Time.deltaTime * speed, 0, 0, Space.World);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CloudEnd")
        {
            
            transform.position = cloudStart.position + new Vector3(0, transform.position.y-cloudStart.position.y, transform.position.z - cloudStart.position.z);
        }
    }

}
