using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PauseMenu : MonoBehaviour, IPlayerUpdate
{
    //public bool isPaused = false;
    public GameObject pauseMenu;
    public CharacterStatus live_charStats;
    public Player_Input player_Input;
    [SerializeField]private bool mouseLocked;
    [SerializeField]private GameObject pauseCharSelection, pauseOptionsMenu, pauseMainPanel;

    private void OnEnable() 
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        player_Input = Camera.main.GetComponent<CameraController>().player.GetComponent<Player_Input>();
    }
    

    private void LateUpdate()
    {
        if(player_Input.pauseKeyPressed)
            if (player_Input.isPaused)
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
        if (pauseMenu.activeSelf) { Pause(); } else Time.timeScale = 1f; //dziêki temu po zmianie chara dalej mamy CursorLockMode.None, ale musi byæ w if poniewa¿ pauzuje gre OnValidate!!

    }

    public void Resume()
    {
        //live_charStats.charInput._enableMouseRotate = mouseLocked;  //private var do przetrzymania mouse inputa, przywraca stan 

        if (live_charStats.charInput._enableMouseRotate) Cursor.lockState = CursorLockMode.Locked; //tutaj live_charStats.charInput._enableMouseRotate dzia³a jak przechowalnia lock.state
        else Cursor.lockState = CursorLockMode.None;

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        //isPaused = false;
        player_Input.isPaused = false;
    }
    private void Pause()
    {        
        pauseMenu.SetActive(true);
        
        if(pauseMenu.activeSelf)
        {
            //mouseLocked = live_charStats.charInput._enableMouseRotate; //private var do przetrzymania mouse inputa, zapamiêtuje aktualny stan
            //live_charStats.charInput._enableMouseRotate = false;    //unlock mouse
            
            pauseCharSelection.SetActive(false); //resetowanie aktywnego panelu menu
            pauseOptionsMenu.SetActive(false); //resetowanie aktywnego panelu menu
            pauseMainPanel.SetActive(true);    //resetowanie aktywnego panelu menu


            Time.timeScale = 0f; //zatrzymuje czas gry, mo¿nau¿ywaæ do SlowMo
            //isPaused = true;
            player_Input.isPaused= true;

            Cursor.lockState = CursorLockMode.None;         //unlock mouse musi byæ po player_Input.isPaused= true poniewa¿ nadpisa³oby go
        }

        
    }
    public void MainMenu()
    {
        Debug.Log("Main Menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);        
    }

}
