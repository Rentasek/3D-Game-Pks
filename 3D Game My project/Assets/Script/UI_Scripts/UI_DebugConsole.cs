using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_DebugConsole : MonoBehaviour, IPlayerUpdate
{
    [SerializeField] private CharacterStatus live_charStats;
    [SerializeField] private Player_Input player_Input;
    [SerializeField] private Spell_FireBreath currentSpell;


    private void OnEnable()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        player_Input = Camera.main.GetComponent<CameraController>().player.GetComponent<Player_Input>();
        currentSpell = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>().spell;
        
    }

    public void PlayerUpdate()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        player_Input = Camera.main.GetComponent<CameraController>().player.GetComponent<Player_Input>();
        currentSpell = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>().spell;
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (live_charStats == null) live_charStats = GetComponentInParent<CharacterStatus>();
        //if (player_Input == null) player_Input = GetComponentInParent<Player_Input>();
        //if (TryGetComponent(out Canvas canvas)) GetComponent<Canvas>().worldCamera = Camera.main;  //sprawdzanie czy bar ma component canvas, jeœli tak pzydziela main camera do Event Camera, wygoda :P 

    }
    private void LateUpdate()
    {

        GetComponent<TextMeshProUGUI>().SetText(
            "Player Input Enable [P] " + live_charStats.playerInputEnable + ".\n" +
            "Enable Mouse Rotate [X] " + live_charStats.inputEnableMouseRotate + ".\n" +
            "Switch Jump Mode [J] " + live_charStats.currentJumpMode_J_ + " .\n" +
            "Options Panel [0] " + player_Input.optionsActivated + " .\n" +
            "Save Game [5] .\n" +
            "Load Game [6] .\n" +
            "Reset Position [R] .\n" +
            "Set Position [B] .\n" +
            "Mouse movement [Middle mouse]" + !live_charStats.inputEnableMouseRotate + ".\n" +
            "Normal Attack [Left mouse]" + ".\n" +
            "Special Attack [Right mouse]" + currentSpell + ".\n" +
            "MoveSpeed " + live_charStats.currentMoveSpeed + ".\n" +
            "IsGrounded " + live_charStats.isGrounded + ".\n" +
            "input_Testing " + live_charStats.inputJumping + ".\n" +
            "Char Level: " + live_charStats.currentCharLevel + "  Reset Level [L]" + ".\n"

            ); 
        


    }
}
