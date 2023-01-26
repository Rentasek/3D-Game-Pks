using System;
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
        StartCoroutine(LoadGameScene()); //Load Gry asynchronicznie w tle
        //SceneManager.LoadScene(1);
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoadGameScene = SceneManager.LoadSceneAsync(1);

        while(!asyncLoadGameScene.isDone) //dopóki nie za³aduje zwraca null
        {
            yield return null;
        }
    }

    public void ExitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

}
