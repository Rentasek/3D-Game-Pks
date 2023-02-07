using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] public int sceneNumber = 1;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        sceneNumber = 1;
    }

    public void PlayGame()
    {
        Debug.Log("PLAY!");        
        StartCoroutine(LoadGameScene()); //Load Gry asynchronicznie w tle
        //SceneManager.LoadScene(1);
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoadGameScene = SceneManager.LoadSceneAsync(sceneNumber);

        while(!asyncLoadGameScene.isDone) //dopóki nie za³aduje zwraca null
        {
            yield return null;
        }
    }

    public void Level0Select()
    {
        sceneNumber = 1;
    }

    public void Level1Select()
    {
        sceneNumber = 2;
    }


    public void ExitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

}
