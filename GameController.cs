using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject player;
    public GameObject ui;
    public Camera cam;
    public bool startCutScene;
    private bool cutSceneStarted;
    private bool locationReached = false;
    private int destination;
    private float lerp = 0;
    private float speed = .05f;
    Quaternion lookRotation;
    public List<Transform> cutSceneLocations1;
    public List<Transform> cutSceneLookat;
    // Use this for initialization
    void Start () {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        if(startCutScene == true)
        {
            startCutscene();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(cutSceneStarted)
        {
            if(lerp < 1.5)
            {
                lerp += Time.deltaTime * speed;
            }
            if (cutSceneLocations1.Count > 0)
            {
                /*Vector3 direction = cutSceneLocations1[destination].position - cam.transform.position;
                lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                        //Quaternion.FromToRotation(transform.forward, direction);
                cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, lookRotation, speed);

                */
                cam.transform.LookAt(cutSceneLookat[destination]);
                cam.transform.position = Vector3.Lerp(cam.transform.position, cutSceneLocations1[destination].position, lerp);
                if(cam.transform.position == cutSceneLocations1[destination].position)
                {
                    Debug.Log("Reached!");
                    if(destination+1 >= (cutSceneLocations1.Count))
                    {
                        stopCutscene();
                    }
                    else
                    {
                        destination += 1;
                        lerp = 0;
                    }
                }
            }
            else
            {
                stopCutscene();
            }
        }
	}

    void startCutscene()
    {
        player.GetComponent<AetherPlayerController>().AMS1.enableEmission = false;
        player.GetComponent<AetherPlayerController>().AMS2.enableEmission = false;
        player.GetComponent<AetherPlayerController>().AMSVU.enableEmission = false;
        player.GetComponent<AetherPlayerController>().AMSVD.enableEmission = false;
        player.GetComponent<AetherPlayerController>().enabled = false;
        cam.GetComponent<ThirdPersonCamera>().enabled = false;
        ui.gameObject.SetActive(false);
        cam.transform.position = cutSceneLocations1[0].transform.position;
        cam.transform.rotation = cutSceneLocations1[0].transform.rotation;
        cutSceneStarted = true;
    }

    void stopCutscene()
    {
        cutSceneStarted = false;
        cam.GetComponent<ThirdPersonCamera>().enabled = true;
        ui.gameObject.SetActive(true);
        player.GetComponent<AetherPlayerController>().enabled = true;
    }
}
