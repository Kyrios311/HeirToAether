using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ThirdPersonCamera : MonoBehaviour {
    private const float Y_AngleMin = -75.0f;
    private const float Y_AngleMax = 75.0f;

    public GameObject player;
    public Transform lookAt;
    public Transform camTransform;

    private Camera cam;
    public int invert = 1;
    private float distance = 7.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float sensitivityX = 2.0f;
    private float sensitivityY = 2.0f;

    private bool fadeInBlack = false, fadeOutBlack = false;
    private bool fadeInWhite = false, fadeOutWhite = false;
    public Image overlay;
    private Color blackColor = Color.black;
    private Color whiteColor = new Color(1f, 1f, 1f, 1f);
    private float timer = 2f;


    bool dead = false;
    bool nextLevel = false;
    // Use this for initialization
    void Start()
    {
        overlay.color = whiteColor;
        timer = 0;
        fadeInWhite = true;
        Cursor.lockState = CursorLockMode.Locked;
        camTransform = transform;
        currentX = lookAt.transform.rotation.x;
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.L))
        {
            SceneManager.LoadScene(2);
        }
        if (Input.GetKey(KeyCode.R))
        {
            PlayerDeath();
        }
        if (Input.GetKey(KeyCode.C))
        {
            if(Cursor.lockState== CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            
        }

        if (!dead)
        {
            currentX += Input.GetAxis("Mouse X");
            currentY += Input.GetAxis("Mouse Y") * invert;
            currentY = Mathf.Clamp(currentY, Y_AngleMin, Y_AngleMax);
        }

        //Fade in and fade out for death
        if (fadeInBlack)
        {
            overlay.color = Color.Lerp(overlay.color, Color.clear, Time.deltaTime * 3);
            if (overlay.color == Color.clear)
            {
                fadeInBlack = false;
            }
        }
        else if (fadeOutBlack)
        {
            timer = timer + Time.deltaTime;
            overlay.color = Color.Lerp(overlay.color, blackColor, Time.deltaTime * 3);
            if (timer >= 4)
            {
                fadeOutBlack = false;
                timer = 0;
                dead = false;
                player.SendMessage("FallRespawn");
                fadeInBlack = true;
            }
        }
        if (fadeInWhite)
        {
            overlay.color = Color.Lerp(overlay.color, Color.clear, Time.deltaTime * 3);
            if (overlay.color == Color.clear)
            {
                fadeInWhite = false;
            }
        }
        else if (fadeOutWhite)
        {
            timer = timer + Time.deltaTime;
            overlay.color = Color.Lerp(overlay.color, blackColor, Time.deltaTime * 3);
            if (timer >= 4)
            {
                fadeOutWhite = false;
                LoadNextLevel();
                timer = 0;
            }
        }
    }

    private void LateUpdate()
    {
        TrackPlayer();
    }

    public void PlayerDeath()
    {
        dead = true;
        fadeOutBlack = true;
    }

    public void NextLevel()
    {
        fadeOutWhite = true;
    }

    public void LoadNextLevel()
    {
        if((SceneManager.GetActiveScene().buildIndex + 1) == 3)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void TrackPlayer()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        if(!dead)
        {
            camTransform.position = lookAt.position + rotation * dir;
        }
        camTransform.LookAt(lookAt.position);
        /*RaycastHit hit;
        if(Physics.Raycast(transform.position, player.transform.position, out hit))
        {
            if(!hit.collider.CompareTag("Player"))
            {
                dir = new Vector3(0, 0, -distance/2);
                camTransform.position = lookAt.position + rotation * dir;
                if (Physics.Raycast(transform.position, player.transform.position, out hit))
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        dir = new Vector3(0, 0, -distance / 2);
                        camTransform.position = lookAt.position + rotation * dir;
                    }
                }
            }
        }*/
    }
}
