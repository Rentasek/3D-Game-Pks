using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenu;
    public CharacterStatus live_charStats;
    public Player_Input player_Input;
    private bool mouseLocked;
    [SerializeField]private GameObject pauseOptionsPanel, pauseMainPanel;

    private void OnValidate()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        player_Input = Camera.main.GetComponent<CameraController>().player.GetComponent<Player_Input>();
    }

    private void Update()
    {
        if(player_Input.pauseKeyPressed)
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        
    }

    public void Resume()
    {
        live_charStats.inputEnableMouseRotate = mouseLocked;  //private var do przetrzymania mouse inputa, przywraca stan      

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    private void Pause()
    {
        mouseLocked = live_charStats.inputEnableMouseRotate; //private var do przetrzymania mouse inputa, zapamiêtuje aktualny stan
        live_charStats.inputEnableMouseRotate = false;    //unlock mouse
        Cursor.lockState = CursorLockMode.None;         //unlock mouse
        pauseMenu.SetActive(true);

        pauseOptionsPanel.SetActive(false); //resetowanie aktywnego panelu menu
        pauseMainPanel.SetActive(true);    //resetowanie aktywnego panelu menu


        Time.timeScale = 0f; //zatrzymuje czas gry, mo¿nau¿ywaæ do SlowMo
        isPaused = true;
    }
    public void MainMenu()
    {
        Debug.Log("Main Menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);        
    }

}
