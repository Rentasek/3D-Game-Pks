using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonActive : MonoBehaviour, IPlayerUpdate
{
    public CharacterStatus live_charStats;
    public Button currentButton;

    public enum ui_Button { AttackButton, SpecialAttackButton, MouseMovementButton, MouseRotateButton, IsometricButton, ThirdPersonButton, SprintButton }; //implementacje z list¹ pozycji enumeratora
    [SerializeField] ui_Button ui_button = new ui_Button();         //enumerator, tworzy nowy obiekt status bar dla ka¿dego elementu z listy 



    private void OnEnable()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        currentButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        UI_buttonUpdate();
    }

    public void PlayerUpdate()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
    }


    public void UI_buttonUpdate()
    {   
        switch (ui_button)
        {
            case ui_Button.AttackButton:
                if (live_charStats.charSkillCombat._skillArray[0]._skillInput) currentButton.interactable = true;
                else currentButton.interactable = false;
                break;

            case ui_Button.SpecialAttackButton:
                if (live_charStats.charSkillCombat._skillArray[1]._skillInput) currentButton.interactable = true;
                else currentButton.interactable = false;
                break;

            case ui_Button.MouseMovementButton:
                if (live_charStats.charInput._mouseCurrentMoving) currentButton.interactable = true;
                else currentButton.interactable = false;
                break;
            case ui_Button.MouseRotateButton:
                if (!live_charStats.charInput._enableMouseRotate || !live_charStats.charInfo._playerInputEnable) currentButton.interactable = false;
                else currentButton.interactable = true;
                break;
            case ui_Button.IsometricButton:
                if (live_charStats.charInfo._playerInputEnable) currentButton.interactable = false;
                else currentButton.interactable = true;
                break;
            case ui_Button.ThirdPersonButton:
                if (live_charStats.charInfo._playerInputEnable) currentButton.interactable = true;
                else currentButton.interactable = false;
                break;
            case ui_Button.SprintButton:
                if (live_charStats.charInput._running) currentButton.interactable = true;
                else currentButton.interactable = false;
                break;
        }

    }


}
