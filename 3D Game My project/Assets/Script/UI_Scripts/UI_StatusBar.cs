using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatusBar : MonoBehaviour
{
    [SerializeField] private float selectedBarCurrentFloat;
    [SerializeField] private float selectedBarMaxFloat;
    [SerializeField] private bool LookAtCamera = false;
    [SerializeField] private Quaternion backupRotation;

    public CharacterStatus live_charStats;

    public enum StatusBar { HealthStat, ManaStat, StaminaStat, XPStat }; //implementacje z list¹ pozycji enumeratora
    [SerializeField] StatusBar statusBar = new StatusBar();         //enumerator, tworzy nowy obiekt status bar dla ka¿dego elementu z listy 



    /// IMPLEMENTACJA
    private void OnEnable()
    {
        if (live_charStats == null) live_charStats = GetComponentInParent<CharacterStatus>();
    }
       

    private void Start()
    {
       
        backupRotation = transform.rotation;        //backup Rotation
        if (TryGetComponent(out Canvas canvas))GetComponent<Canvas>().worldCamera = Camera.main;  //sprawdzanie czy bar ma component canvas, jeœli tak pzydziela main camera do Event Camera, wygoda :P 
    }
    
    void FixedUpdate()
    {
            switch (statusBar)
            {
                case StatusBar.HealthStat:
                    selectedBarCurrentFloat = Mathf.Clamp(live_charStats.charStats._hp,0f, live_charStats.charStats._maxHP);             //Enum na switchu odnosi siê tylko do jednej zmiennej w charStats zamiast wszystkich na raz, wiêc nie obci¹¿a procka
                    selectedBarMaxFloat = live_charStats.charStats._maxHP;                    
                    break;


                case StatusBar.ManaStat:

                selectedBarCurrentFloat = Mathf.Clamp(live_charStats.charStats._mp, 0f, live_charStats.charStats._maxMP);
                    selectedBarMaxFloat = live_charStats.charStats._maxMP;
                    break;


                case StatusBar.StaminaStat:

                selectedBarCurrentFloat = Mathf.Clamp(live_charStats.charStats._stam, 0f, live_charStats.charStats._maxStam);
                    selectedBarMaxFloat = live_charStats.charStats._maxStam;
                    break;


                case StatusBar.XPStat:

                    selectedBarCurrentFloat = Mathf.Clamp(live_charStats.charStats._xp, 0f, live_charStats.charStats._neededXP);
                    selectedBarMaxFloat = live_charStats.charStats._neededXP;
                    break;

            }        
        

        GetComponent<Image>().fillAmount = Mathf.InverseLerp(0f, selectedBarMaxFloat, selectedBarCurrentFloat); //lerp wype³niaj¹cy fill amount w spricie paska
        GetComponentInChildren<TextMeshProUGUI>().SetText(((int)selectedBarCurrentFloat) + " / " + ((int)selectedBarMaxFloat)); //ustawianie textu w child
      
        if (LookAtCamera)
        {
            transform.LookAt(Camera.main.transform);    //obraca grafikê w kierunku main camera, 
            transform.Rotate(0,180,0);                 // obrót w osi y bo grafika jest obrócona bokiem
        }
        else transform.rotation = backupRotation;

        
    }
}
