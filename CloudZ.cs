using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudZ : MonoBehaviour {
    float speed = 100;
    public Transform cloudStart;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(0, 0, Time.deltaTime * speed, Space.World);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CloudEnd")
        {
            
            transform.position = cloudStart.position + new Vector3(transform.position.x - cloudStart.position.x, transform.position.y-cloudStart.position.y, 0);
        }
    }

}
