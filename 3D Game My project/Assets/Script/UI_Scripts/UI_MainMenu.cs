using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void PlayGame()
    {
        Debug.Log("PLAY!");
        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

}
