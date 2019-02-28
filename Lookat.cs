using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookat : MonoBehaviour {
    public GameObject cam;
    public GameObject player;
	// Use this for initialization
	void Start () {
        GetComponent<MeshRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if(Vector3.Distance(transform.position, player.transform.position) < 20f)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
	}
}
