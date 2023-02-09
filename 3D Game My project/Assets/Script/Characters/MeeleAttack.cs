using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class MeeleAttack : MonoBehaviour
{
    private int i = 0; //testing...

    [SerializeField] private CharacterStatus live_charStats;        //tylko do podgl¹du
    [SerializeField] private ScrObj_charStats scrObj_CharStats;

    
    [SerializeField] private string[] enemiesArray;
    [Tooltip("Na którym combo colider dzia³a"), SerializeField] private float onCombo;
    //[SerializeField] private ScrObj_charStats TESTINGscrObj_CharStats;  //testing....
    


    private AudioSource audioSource;
     
    private void OnEnable()
    {
        live_charStats = GetComponentInParent<CharacterStatus>();        //automatyczny serialize dla wygody
        scrObj_CharStats = live_charStats.scrObj_CharStats;     //automatyczny serialize dla wygody    
        enemiesArray = live_charStats.currentEnemiesArray;      //przypisanie enemieArraya z characterStats
        
    }
    private void Start()
    {
        
       
    }
    private void Update()
    {
        if(live_charStats.isAttacking && live_charStats.currentComboMeele == onCombo) //W³¹czanie colliderów tylko dla aktualnego combo
        {
            gameObject.GetComponent<Collider>().enabled = true;
        }
        else { gameObject.GetComponent<Collider>().enabled = false; }
    }




    private void OnTriggerEnter(Collider other)
    {        
        audioSource = other.GetComponent<AudioSource>(); //audiosource z objectu other

        if (enemiesArray.Contains(other.tag))    // Sprawdzanie czy enemiesArray pobrane z CharacterStats zawiera other.tag !!!
                                                                                                                           //dodatkowo sprawdza parametr OnCombo na ka¿dym Collider i porównuje go do aktualnego combo, ¿eby nie atakowaæ ogonem przy scratch 
        {
            
            audioSource.PlayOneShot(other.GetComponent<CharacterStatus>().scrObj_CharStats.damagedEnemy, 1);    //audio Play

            other.GetComponent<Animator>().SetTrigger("IsHit");  //Trigger Hit Animatora

            other.GetComponent<CharacterStatus>().currentHP -= GetComponentInParent<CharacterStatus>().currentDamageCombo;

            if (other.GetComponent<CharacterStatus>().currentHP <= 0f && !other.GetComponent<CharacterStatus>().isDead)

            {
                other.GetComponent<CharacterStatus>().currentCharLevel = UnityEngine.Random.Range(live_charStats.currentCharLevel - 3, live_charStats.currentCharLevel + 3);  //po œmierci ustawia level targetu na zbli¿ony do atakuj¹cego
                live_charStats.currentXP += other.GetComponent<CharacterStatus>().currentXP_GainedFromKill;

                /* // --> co sie dzieje przy spadku hp do 0 // (animacja znikania jak czacha)
                 other.GetComponent<CharacterStatus>().currentHP = 0f;  //trik ¿eby nie zmniejszyæ hp poni¿ej 0*/ // zbêdne
            }

            i++;  //zmienna testing only

        }
        else if (other.CompareTag("Destructibles") && live_charStats.isAttacking && live_charStats.currentComboMeele == onCombo) 
        {
            audioSource.PlayOneShot(other.GetComponent<CharacterStatus>().scrObj_CharStats.damagedDestructibles, 1);

            //other.GetComponent<Animator>().SetTrigger("IsHit");   animacja hit dla environment -> in da progress

            other.GetComponent<CharacterStatus>().currentHP -= GetComponentInParent<CharacterStatus>().currentDamageCombo;

            if (other.GetComponent<CharacterStatus>().currentHP <= 0f && !other.GetComponent<CharacterStatus>().isDead)
            {
                live_charStats.currentXP += other.GetComponent<CharacterStatus>().currentXP_GainedFromKill;                
            } 

        }
    }


}
