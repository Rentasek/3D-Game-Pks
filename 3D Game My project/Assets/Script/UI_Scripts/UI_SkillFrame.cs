using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillFrame : MonoBehaviour, IPlayerUpdate
{
    [SerializeField] private CharacterStatus live_charStats;
    [SerializeField] private CharacterBonusStats currentCharacterBonusStats;
    [SerializeField] private GameObject skill_statText, skill_buttonUP, skill_buttonDown, skill_Image;



    [SerializeField] private float skill_BonusFloat;
    [SerializeField] private float skill_OutputFloat;
    [SerializeField] private string skill_selectedName;

    [SerializeField] private bool updateSkills;





    //implementacje z list¹ pozycji enumeratora
    public enum CharStat
    {
        HealthMaxStat, ManaMaxStat, StaminaMaxStat, WalkSpeed, CloseRangeAttDamage, SpellRangeAttDamage, SkillPTS /*,
        XPCurrentStat, XPNeededStat, CharLevel, RunSpeed, AttCooldown, AttStaminaCost, HealthRegenStat, HealthMultiplierStat, ManaRegenStat,ManaMultiplierStat, StaminaRegenStat, StaminaMultiplierStat*/
    };
    [SerializeField] CharStat charStat = new();          //enumerator, tworzy nowy obiekt CharStat dla ka¿dego elementu z listy 
    [SerializeField] Sprite[] skillImagesList;

    private void OnEnable()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        currentCharacterBonusStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterBonusStats>();

        if (updateSkills) //po zupdatowaniu w Edytorze trzeba ustawiæ Interractable na Buttonach
        {
            SkillFrameSetUp();
            updateSkills = false;
        }
    }

    /*void OnEnable()
    {
        SkillFrameSetUp();
    }*/

    public void PlayerUpdate()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        currentCharacterBonusStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterBonusStats>();
    }        

    private void Update()
    {
        if(gameObject.activeSelf)
        {
            live_charStats.UpdateBonusStats(); //  Update Klasy CharStats 
            SkillFrameUpdate();            
        }
    }


    private void SkillFrameSetUp() //Update tylko OnValidate
    {
        //skill_statText = GetComponentInChildren<TextMeshProUGUI>().gameObject; //OLD
        //skill_buttonUP = transform.FindChild("Skill_ButtonUP").gameObject; //OLD
        skill_statText = transform.Find("Skill_Text (TMP)").gameObject;
        skill_buttonUP = transform.Find("Skill_ButtonUP").gameObject;  //Naj³twiej znaleŸæ Child by name poprzez transform
        skill_buttonDown = transform.Find("Skill_ButtonDown").gameObject;  //Naj³twiej znaleŸæ Child by name poprzez transform
        skill_Image = transform.Find("Skill_Image").gameObject;

        skill_buttonUP.GetComponent<Button>().onClick.AddListener(SkillUp);
        skill_buttonDown.GetComponent<Button>().onClick.AddListener(SkillDown);

        skill_buttonUP.GetComponent<Button>().interactable = true;
        skill_buttonDown.GetComponent<Button>().interactable = true;
        

        switch (charStat)
        {
            case CharStat.HealthMaxStat:
                skill_BonusFloat = currentCharacterBonusStats.bonus_currentMaxHP;
                skill_OutputFloat = live_charStats.charStats._maxHP;
                skill_selectedName = "Health Max";
                skill_Image.GetComponent<Image>().sprite = skillImagesList[0];
                break;

            case CharStat.ManaMaxStat:
                skill_BonusFloat = currentCharacterBonusStats.bonus_currentMaxMP;
                skill_OutputFloat = live_charStats.charStats._maxMP;
                skill_selectedName = "Mana Max";
                skill_Image.GetComponent<Image>().sprite = skillImagesList[1];
                break;
         
            case CharStat.StaminaMaxStat:
                skill_BonusFloat = currentCharacterBonusStats.bonus_currentMaxStam;
                skill_OutputFloat = live_charStats.charStats._maxStam;
                skill_selectedName = "Stamina Max";
                skill_Image.GetComponent<Image>().sprite = skillImagesList[2];
                break;

            case CharStat.WalkSpeed:
                skill_BonusFloat = currentCharacterBonusStats.bonus_currentWalkSpeed;
                skill_OutputFloat = live_charStats.charMove._walkSpeed;
                skill_selectedName = "Walk Speed";
                skill_Image.GetComponent<Image>().sprite = skillImagesList[3];                
                break;

            case CharStat.CloseRangeAttDamage:
                if (live_charStats.fov._closeRangeSkill != null)
                {
                    skill_BonusFloat = currentCharacterBonusStats.bonus_currentDamageCombo;
                    skill_OutputFloat = live_charStats.fov._closeRangeSkill.skill_currentDamage;
                }
                skill_selectedName = "Melee Damage";
                skill_Image.GetComponent<Image>().sprite = skillImagesList[4];
                break;

            case CharStat.SpellRangeAttDamage:
                if (live_charStats.fov._spellRangeSkill != null)
                {
                    skill_BonusFloat = currentCharacterBonusStats.bonus_Skill_Damage;
                    skill_OutputFloat = live_charStats.fov._spellRangeSkill.skill_currentDamage;
                }
                skill_selectedName = "Spell Damage";
                skill_Image.GetComponent<Image>().sprite = skillImagesList[5];
                break;
            
            case CharStat.SkillPTS:                
                skill_OutputFloat = currentCharacterBonusStats.bonus_SkillPoints;
                skill_selectedName = "Skill Points";
                skill_Image.GetComponent<Image>().sprite = skillImagesList[6];
                break;

        }
        //Debug.Log(charStat  + " : " + selectedStatCurrentFloat );   
    }
    
    
    private void SkillFrameUpdate()
    {
       
        SkillPointCheck();        
        skill_statText.GetComponent<TextMeshProUGUI>().SetText(skill_selectedName + "\n" + Mathf.Round(skill_OutputFloat*10f)*0.1f); // 0,1 po przecinku

        switch (charStat)
        {
            case CharStat.HealthMaxStat:
                skill_BonusFloat = currentCharacterBonusStats.bonus_currentMaxHP;
                skill_OutputFloat = live_charStats.charStats._maxHP;                
                break;

            case CharStat.ManaMaxStat:
                skill_BonusFloat = currentCharacterBonusStats.bonus_currentMaxMP;
                skill_OutputFloat = live_charStats.charStats._maxMP;
                break;

            case CharStat.StaminaMaxStat:
                skill_BonusFloat = currentCharacterBonusStats.bonus_currentMaxStam;
                skill_OutputFloat = live_charStats.charStats._maxStam;
                break;

            case CharStat.WalkSpeed:
                skill_BonusFloat = currentCharacterBonusStats.bonus_currentWalkSpeed;
                skill_OutputFloat = live_charStats.charMove._walkSpeed;
                break;

            case CharStat.CloseRangeAttDamage:
                if(live_charStats.fov._closeRangeSkill != null)
                {
                    skill_BonusFloat = currentCharacterBonusStats.bonus_currentDamageCombo;
                    skill_OutputFloat = live_charStats.fov._closeRangeSkill.skill_currentDamage;
                }                               
                break;

            case CharStat.SpellRangeAttDamage:
                if(live_charStats.fov._spellRangeSkill != null)
                {
                    skill_BonusFloat = currentCharacterBonusStats.bonus_Skill_Damage;
                    skill_OutputFloat = live_charStats.fov._spellRangeSkill.skill_currentDamage;
                }                
                break;

            case CharStat.SkillPTS:
                skill_OutputFloat = currentCharacterBonusStats.bonus_SkillPoints;
                break;

        }
    }

    void SkillPointCheck()
    {
        if (currentCharacterBonusStats.bonus_SkillPoints <= 0) skill_buttonUP.GetComponent<Button>().interactable = false;
        else skill_buttonUP.GetComponent<Button>().interactable = true;

        if (skill_BonusFloat <= 0) skill_buttonDown.GetComponent<Button>().interactable = false;
        else skill_buttonDown.GetComponent<Button>().interactable = true;
    }

    public void SkillUp()
    {
        if (live_charStats.charInput._running && currentCharacterBonusStats.bonus_SkillPoints >= 10)
        {
            skill_BonusFloat += 10;
            currentCharacterBonusStats.bonus_SkillPoints -= 10;
        }
        else
        {
            skill_BonusFloat++;
            currentCharacterBonusStats.bonus_SkillPoints--;
        } 
        SkillUpdate(); //Update klasy Bonus char
    }
    public void SkillDown()
    {
        if (live_charStats.charInput._running && skill_BonusFloat >= 10)
        {
            skill_BonusFloat -= 10;
            currentCharacterBonusStats.bonus_SkillPoints += 10;
        }
        else
        {
            skill_BonusFloat--;
            currentCharacterBonusStats.bonus_SkillPoints++;
        }
        SkillUpdate(); //Update klasy Bonus char
    }

    public void SkillUpdate()
    {
        switch (charStat)
        {
            case CharStat.HealthMaxStat:
                currentCharacterBonusStats.bonus_currentMaxHP = skill_BonusFloat;
                
                break;

            case CharStat.ManaMaxStat:
                currentCharacterBonusStats.bonus_currentMaxMP = skill_BonusFloat;
                break;

            case CharStat.StaminaMaxStat:
                currentCharacterBonusStats.bonus_currentMaxStam = skill_BonusFloat;
                break;

            case CharStat.WalkSpeed:
                currentCharacterBonusStats.bonus_currentWalkSpeed = skill_BonusFloat;
                break;

            case CharStat.CloseRangeAttDamage:
                currentCharacterBonusStats.bonus_currentDamageCombo = skill_BonusFloat;
                break;

            case CharStat.SpellRangeAttDamage:
                currentCharacterBonusStats.bonus_Skill_Damage = skill_BonusFloat;
                break;
/*
            case CharStat.SkillPTS:                
                skill_OutputFloat = currentCharacterBonusStats.bonus_SkillPoints;                
                break;*/
        }
               
    }    

}
