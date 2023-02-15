using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Unity.Mathematics;

[ExecuteInEditMode]
public class UI_CharStatsPanel : MonoBehaviour, IPlayerUpdate
{
    [SerializeField] private int statsCount;
    [SerializeField] private CharacterStatus live_charStats;
    [SerializeField] private GameObject statText, parentObj;
    [SerializeField] private int toDestroy;

    [SerializeField] private float[] selectedStatCurrentFloat;
    [SerializeField] private string[] selectedStatCurrentName;


    //implementacje z list¹ pozycji enumeratora
    public enum CharStat
    {
        CharLevel, WalkSpeed, RunSpeed, CloseRangeAttCooldown, CloseRangeAttDamage, CloseRangeAttResourceCost, HealthMaxStat, HealthRegenStat, HealthMultiplierStat, ManaMaxStat, ManaRegenStat,
        ManaMultiplierStat, StaminaMaxStat, StaminaRegenStat, StaminaMultiplierStat, XPCurrentStat, XPNeededStat
    };
    [SerializeField] CharStat[] charStat;          //enumerator, tworzy nowy obiekt CharStat dla ka¿dego elementu z listy 



    //Enum bêdzie zawiera³ wszystkie mo¿liwe staty, na podstawie statsCounta okreœlimy d³ugoœæ arraya zawieraj¹cego statu do wyboru,
    //dodatkowo ka¿dy statsCount stworzy Instantie nowego TextObjectu z mo¿liwoœci¹ wyboru statu z enuma

    private void LateUpdate()
    {
        StatsFillIn();
    }

    /*private void OnValidate()
    {
        StatsFillIn();

    }*/
    public void PlayerUpdate()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();        
    }
    void OnEnable()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        StatsFillIn();

        StatsTextBoxCreate();

        PanelSize();
    }
    private void OnDisable()
    {
        live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
        StatsTextBoxDestroy();
    }

    /*private void Start()
    {
        //if(Input.GetKeyUp(KeyCode.G)) { StatsTextBox(); }

    }*/


    public void StatsFillIn()
    {
        selectedStatCurrentFloat = new float[statsCount];
        selectedStatCurrentName = new string[statsCount];

        for (int i = 0; i < statsCount; i++)
        {
            switch (charStat[i])
            {
                case CharStat.CharLevel:
                    selectedStatCurrentFloat[i] = live_charStats.charInfo._charLevel;
                    selectedStatCurrentName[i] = "Character Level";
                    break;

                case CharStat.WalkSpeed:
                    selectedStatCurrentFloat[i] = live_charStats.charMove._walkSpeed;
                    selectedStatCurrentName[i] = "Walk Speed";
                    break;

                case CharStat.RunSpeed:
                    selectedStatCurrentFloat[i] = live_charStats.charMove._runSpeed;
                    selectedStatCurrentName[i] = "Run Speed";
                    break;

                case CharStat.CloseRangeAttCooldown:
                    selectedStatCurrentFloat[i] = live_charStats.fov._closeRangeSkill.scrObj_Skill.skill_BaseCooldown;
                    selectedStatCurrentName[i] = "Melee Attack Cooldown";
                    break;

                case CharStat.CloseRangeAttDamage:
                    selectedStatCurrentFloat[i] = live_charStats.fov._closeRangeSkill.skill_currentDamage;
                    selectedStatCurrentName[i] = "Melee Attack Damage";
                    break;

                case CharStat.CloseRangeAttResourceCost:
                    selectedStatCurrentFloat[i] = live_charStats.fov._closeRangeSkill.skill_currentResourceCost;
                    selectedStatCurrentName[i] = "Melee Attack Stamina Cost";
                    break;

                case CharStat.HealthMaxStat:
                    selectedStatCurrentFloat[i] = live_charStats.charStats._maxHP;
                    selectedStatCurrentName[i] = "Health Max";
                    break;

                case CharStat.HealthRegenStat:
                    selectedStatCurrentFloat[i] = live_charStats.charComponents._scrObj_CharStats.regenHP;
                    selectedStatCurrentName[i] = "Health Regeneration";
                    break;

                case CharStat.HealthMultiplierStat:
                    selectedStatCurrentFloat[i] = live_charStats.charComponents._scrObj_CharStats.HP_Multiplier * live_charStats.charInfo._charLevel;
                    selectedStatCurrentName[i] = "Health Multiplier ";
                    break;

                case CharStat.ManaMaxStat:
                    selectedStatCurrentFloat[i] = live_charStats.charStats._maxMP;
                    selectedStatCurrentName[i] = "Mana Max";
                    break;

                case CharStat.ManaRegenStat:
                    selectedStatCurrentFloat[i] = live_charStats.charComponents._scrObj_CharStats.regenMP;
                    selectedStatCurrentName[i] = "Mana Regeneration";
                    break;

                case CharStat.ManaMultiplierStat:
                    selectedStatCurrentFloat[i] = live_charStats.charComponents._scrObj_CharStats.MP_Multiplier * live_charStats.charInfo._charLevel;
                    selectedStatCurrentName[i] = "Mana Multiplier";
                    break;

                case CharStat.StaminaMaxStat:
                    selectedStatCurrentFloat[i] = live_charStats.charStats._maxStam;
                    selectedStatCurrentName[i] = "Stamina Max";
                    break;

                case CharStat.StaminaRegenStat:
                    selectedStatCurrentFloat[i] = live_charStats.charComponents._scrObj_CharStats.regenStam;
                    selectedStatCurrentName[i] = "Stamina Regeneration";

                    break;

                case CharStat.StaminaMultiplierStat:
                    selectedStatCurrentFloat[i] = live_charStats.charComponents._scrObj_CharStats.Stam_Multiplier * live_charStats.charInfo._charLevel;
                    selectedStatCurrentName[i] = "Stamina Multiplier";

                    break;

                case CharStat.XPCurrentStat:
                    selectedStatCurrentFloat[i] = live_charStats.charStats._xp;
                    selectedStatCurrentName[i] = "XP Current";
                    break;

                case CharStat.XPNeededStat:
                    selectedStatCurrentFloat[i] = live_charStats.charStats._neededXP;
                    selectedStatCurrentName[i] = "XP Needed to next Level";

                    break;
            }            
            //Debug.Log(charStat[i] + " : " + selectedStatCurrentFloat[i]);
        }

    }

    public void StatsTextBoxCreate() //tworzenie text boxów przy pomocy Instanntiate
    {
        StatsTextBoxDestroy();

        for (int i = 0; i < statsCount; i++)
        {
            GameObject clonedText = Instantiate(statText, new Vector3(statText.transform.position.x, statText.transform.position.y - i * 50f * statText.GetComponent<RectTransform>().rect.height, statText.transform.position.z), statText.transform.rotation, parentObj.transform);
            clonedText.SetActive(true);
            clonedText.tag = "UI_TextBox";
            clonedText.GetComponent<TextMeshProUGUI>().SetText(selectedStatCurrentName[i] + " : " + selectedStatCurrentFloat[i]);
        }


    }

    public void StatsTextBoxDestroy()
    { //niszczenie text boxów 
        //int gameObjectsToDestroy = GameObject.FindGameObjectsWithTag("UI_TextBox").Length;

        toDestroy = GameObject.FindGameObjectsWithTag("UI_TextBox").Length;

        for (int i = 0; i < toDestroy; i++)
        {
            GameObject.DestroyImmediate(GameObject.FindGameObjectWithTag("UI_TextBox")); //rozwalanie starych objectów przy odswie¿aniu
        }
    }

    public void PanelSize()
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,(statsCount + 1) /** 50f*/ * statText.GetComponent<RectTransform>().rect.height);
        //trik - bierze wysokoœæ z text objectu        
    }




}
