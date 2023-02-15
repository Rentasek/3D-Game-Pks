using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PauseMenu : MonoBehaviour, IPlayerUpdate
{
    public static bool isPaused = false;
    public GameObject pauseMenu;
    public CharacterStatus live_charStats;
    public Player_Input player_Input;
    private bool mouseLocked;
    [SerializeField]private GameObject pauseOptionsPanel, pauseMainPanel;

    private void OnEnable()
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

    public void PlayerUpdate()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        player_Input = Camera.main.GetComponent<CameraController>().player.GetComponent<Player_Input>();
    }

    public void Resume()
    {
        live_charStats.charInput._enableMouseRotate = mouseLocked;  //private var do przetrzymania mouse inputa, przywraca stan      

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    private void Pause()
    {
        mouseLocked = live_charStats.charInput._enableMouseRotate; //private var do przetrzymania mouse inputa, zapamiętuje aktualny stan
        live_charStats.charInput._enableMouseRotate = false;    //unlock mouse
        Cursor.lockState = CursorLockMode.None;         //unlock mouse
        pauseMenu.SetActive(true);

        pauseOptionsPanel.SetActive(false); //resetowanie aktywnego panelu menu
        pauseMainPanel.SetActive(true);    //resetowanie aktywnego panelu menu


        Time.timeScale = 0f; //zatrzymuje czas gry, możnaużywać do SlowMo
        isPaused = true;
    }
    public void MainMenu()
    {
        Debug.Log("Main Menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);        
    }

}
