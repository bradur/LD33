using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    int currentLevel = 0;
    public GameObject continueButton;
    // Use this for initialization
    void Start () {
        currentLevel = PlayerPrefs.GetInt("Level");
        if (currentLevel > 0)
        {
            PlayerPrefs.SetInt("Continue", 1);
            continueButton.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Continue()
    {
        Application.LoadLevel(1);
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("Continue", 0);
        Application.LoadLevel(1);
    }

    // Update is called once per frame
    void Update () {
    
    }
}
