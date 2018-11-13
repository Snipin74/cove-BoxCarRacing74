using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {


    public string playGameLevel;

    public void playGame()
    {
        Application.LoadLevel(playGameLevel);
        Time.timeScale = 1f;
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
