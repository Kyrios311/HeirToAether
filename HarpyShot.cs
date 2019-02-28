using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpyShot : MonoBehaviour {
    private GameObject player;
	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, 4f);
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(player.transform.position);
        float step = 6 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
    }
}
