using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {
    public GameObject menu;
    public GameObject credits;
	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void PlayGame()
    {
        SceneManager.LoadScene("Scene00");
    }

    void Credits()
    {
        menu.SetActive(false);
        credits.SetActive(true);
    }

    void Menu()
    {
        credits.SetActive(false);
        menu.SetActive(true);
    }

    void Quit()
    {
        Application.Quit();
    }
}
