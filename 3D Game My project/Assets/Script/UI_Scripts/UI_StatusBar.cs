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

    public enum StatusBar { HealthStat, ManaStat, StaminaStat, XPStat }; //implementacje z list� pozycji enumeratora
    [SerializeField] StatusBar statusBar = new StatusBar();         //enumerator, tworzy nowy obiekt status bar dla ka�dego elementu z listy 



    /// IMPLEMENTACJA
    private void OnEnable()
    {
        if (live_charStats == null) live_charStats = GetComponentInParent<CharacterStatus>();
    }
       

    private void Start()
    {
       
        backupRotation = transform.rotation;        //backup Rotation
        if (TryGetComponent(out Canvas canvas))GetComponent<Canvas>().worldCamera = Camera.main;  //sprawdzanie czy bar ma component canvas, je�li tak pzydziela main camera do Event Camera, wygoda :P 
    }
    
    void FixedUpdate()
    {
            switch (statusBar)
            {
                case StatusBar.HealthStat:
                    selectedBarCurrentFloat = Mathf.Clamp(live_charStats.currentHP,0f, live_charStats.currentMaxHP);             //Enum na switchu odnosi si� tylko do jednej zmiennej w charStats zamiast wszystkich na raz, wi�c nie obci��a procka
                    selectedBarMaxFloat = live_charStats.currentMaxHP;                    
                    break;


                case StatusBar.ManaStat:

                selectedBarCurrentFloat = Mathf.Clamp(live_charStats.currentMP, 0f, live_charStats.currentMaxMP);
                    selectedBarMaxFloat = live_charStats.currentMaxMP;
                    break;


                case StatusBar.StaminaStat:

                selectedBarCurrentFloat = Mathf.Clamp(live_charStats.currentStam, 0f, live_charStats.currentMaxStam);
                    selectedBarMaxFloat = live_charStats.currentMaxStam;
                    break;


                case StatusBar.XPStat:

                    selectedBarCurrentFloat = Mathf.Clamp(live_charStats.currentXP, 0f, live_charStats.currentNeededXP);
                    selectedBarMaxFloat = live_charStats.currentNeededXP;
                    break;

            }        
        

        GetComponent<Image>().fillAmount = Mathf.InverseLerp(0f, selectedBarMaxFloat, selectedBarCurrentFloat); //lerp wype�niaj�cy fill amount w spricie paska
        GetComponentInChildren<TextMeshProUGUI>().SetText(((int)selectedBarCurrentFloat) + " / " + ((int)selectedBarMaxFloat)); //ustawianie textu w child
      
        if (LookAtCamera)
        {
            transform.LookAt(Camera.main.transform);    //obraca grafik� w kierunku main camera, 
            transform.Rotate(0,180,0);                 // obr�t w osi y bo grafika jest obr�cona bokiem
        }
        else transform.rotation = backupRotation;

        
    }
}
