using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassScript : MonoBehaviour {
    public GameObject player;
    public List<GameObject> grassList;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject g in grassList)
        {
            if(Vector3.Distance(g.transform.position, player.transform.position) < 150)
            {
                g.SetActive(true);
            }
            else
            {
                g.SetActive(false);
            }
        }
	}
}
