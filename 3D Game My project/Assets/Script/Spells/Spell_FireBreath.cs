using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class Spell_FireBreath : MonoBehaviour
{
    public CharacterStatus live_charStats;

    [Header("Testing/Debug")]
    [SerializeField] public bool breath_input;
    /*[SerializeField] public bool breath_CanBreath;*/
    [SerializeField] private float signedAngleLeft;
    [SerializeField] private float signedAngleRight;
    [SerializeField] private Vector3 signedAngleCollider;
    [SerializeField] private Collider targetCollider;
    [SerializeField] private List<Collider> breath_targetColliders;  

    [Space]
    [Header("FireBreath Cone Settings")]
    [SerializeField] private bool breath_targetInBreathRange ;
    [SerializeField] private bool breath_targetInBreathAngle ;

    [SerializeField] private float breath_currentFireRadius;
    [SerializeField] private float breath_currentFireAngle;
    [Space]
    [SerializeField] private float breath_currentVectorFireRadius;
    [SerializeField] private float breath_currentVectorFireAngle;
    [Space]
    [SerializeField] private float breath_MinFireRadius;
    [SerializeField] private float breath_MinFireAngle;
    [SerializeField] private float breath_MaxFireRadius;
    [SerializeField] private float breath_MaxFireAngle;
    [Space]
    [SerializeField] private float breath_TimeFireRadius;
    [SerializeField] private float breath_TimeFireAngle;
    [Space]
    [SerializeField] private float breath_MPCost;
    [SerializeField] private float breath_Damage;
    [Space]
    [SerializeField,TagField] private string[] breath_EnemiesArray; //Pozwala na wybór Enemies przy pomocy Tag 
    [SerializeField] private LayerMask breath_ObstaclesMask; //Obstacles dla spella
    [Space]
    [Header("Audio")]
    [SerializeField] private AudioSource breath_AudioSource;
    [SerializeField] private AudioClip fireBreathAudioClip;
    [SerializeField] private VisualEffect breath_VisualEffect;
    [SerializeField] private float audioVolume;
    [SerializeField] private float audioLerpDecrease;
    [Space]
    [Header("Variable VFX")]
    [SerializeField] private float breath_ParticlesPerSec;    
    [SerializeField] private float breath_LifetimeMin;
    [SerializeField] private float breath_LifetimeMax;
    [SerializeField] private float breath_MovingTowardsFactor;








    private void OnEnable()
    {
        live_charStats = GetComponentInParent<CharacterStatus>();
        breath_AudioSource = GetComponentInParent<AudioSource>();//debugg jeœli nie ustawione w inspectorze
        breath_VisualEffect = GetComponentInParent<VisualEffect>();
        //live_charStats.skill_secondarySkill = GetComponent<Spell_FireBreath>();
        live_charStats.charSkillCombat.spell_MaxRadius = breath_MaxFireRadius;
        EnemyArraySelector();
    }

    private void EnemyArraySelector()
    {
        breath_EnemiesArray = new string[live_charStats.charInfo.currentEnemiesArray.Length + 1];    //tworzy array +1 od current enemies arraya
        live_charStats.charInfo.currentEnemiesArray.CopyTo(breath_EnemiesArray, 0);                  //kopiuje current enemies arraya od indexu 0
        breath_EnemiesArray[breath_EnemiesArray.Length - 1] = "Destructibles";              //wstawia jako ostatni index Destructibles ¿eby zawsze mo¿na by³o go zniszczyæ
    }


    // Update is called once per frame
    void Update()
    {
        breath_input = live_charStats.characterInput.inputSecondary; //testing inspector
        live_charStats.charSkillCombat.skill_CanCast = !live_charStats.characterInput.inputPrimary && !live_charStats.currentCharStatus.isRunning && live_charStats.currentCharMove.currentMoveSpeed != live_charStats.currentCharMove.currentRunSpeed;
    }
    private void FixedUpdate()
    {
        audioVolume = GetComponentInParent<AudioSource>(live_charStats.characterInput.inputSecondary).volume;    //testing inspector

        if (!live_charStats.currentCharStatus.isDead) //tylko dla ¿ywych :P
        {
            FireBreathVFX_Audio();


            if (live_charStats.currentMP >= 1f)
            {                
                BreathAttackConeCheck();
            }

        }

        //IMPLEMENTACJA zadawania dmg//
        FireBreathDamage();
    }
    private void FireBreathVFX_Audio()
    {

        if (live_charStats.characterInput.inputSecondary && live_charStats.charSkillCombat.skill_CanCast && live_charStats.currentMP >= 1f) 
        {
            if (!live_charStats.currentCharStatus.isCasting)
            {
                breath_VisualEffect.Play(); //mo¿e siê odpaliæ tylko raz przy ka¿dym inpucie, nie mo¿e siê nadpisaæ -> taki sam efekt jak przy GetKeyDown
                breath_AudioSource.PlayOneShot(fireBreathAudioClip, 0.5f);//clip audio
                //GetComponentInParent<AudioSource>(live_charStats.inputCasting).volume = 1;//play audio volume
                //GetComponentInParent<AudioSource>(live_charStats.inputCasting).Play();//play audio
                


            }
            live_charStats.currentCharStatus.isCasting = true;
            live_charStats.currentAnimator.SetBool("IsCasting", live_charStats.currentCharStatus.isCasting);
        }
        else
        {
            breath_VisualEffect.Stop();
            //GetComponentInParent<AudioSource>(live_charStats.inputCasting).Stop();//stop audio jeœli nie ma inputa
            //GetComponentInParent<AudioSource>(live_charStats.inputCasting).volume -= audioLerpDecrease * Time.deltaTime;//volume down audio jeœli nie ma inputa

            live_charStats.currentCharStatus.isCasting = false;
            live_charStats.currentAnimator.SetBool("IsCasting", live_charStats.currentCharStatus.isCasting);
        }



    }


    //mechanika BREATH Cone


    private void FireBreathDynamicCone()
    {

        if (live_charStats.currentCharStatus.isCasting)
        {
            breath_currentFireRadius = Mathf.SmoothDamp(breath_currentFireRadius, breath_MaxFireRadius, ref breath_currentVectorFireRadius, breath_TimeFireRadius);
            //dynamiczny BreathCone radius -> ++ on input

            breath_currentFireAngle = Mathf.SmoothDamp(breath_currentFireAngle, breath_MaxFireAngle, ref breath_currentVectorFireAngle, breath_TimeFireAngle);
            //dynamiczny BreathCone Angle -> ++ on input
        }
        else
        {
            breath_currentFireRadius = Mathf.SmoothDamp(breath_currentFireRadius, breath_MinFireRadius, ref breath_currentVectorFireRadius, breath_TimeFireRadius);
            //dynamiczny BreathCone radius -> -- off input

            breath_currentFireAngle = Mathf.SmoothDamp(breath_currentFireAngle, breath_MinFireAngle, ref breath_currentVectorFireAngle, breath_TimeFireAngle);
            //dynamiczny BreathCone Angle -> -- off input
        }

    }
    private void BreathAttackConeCheck()  //dynamiczna lista colliderów z OverlapSphere
    {
        FireBreathDynamicCone();

        if (live_charStats.currentCharStatus.isCasting)
        {            
            for (int i = 0; i < Physics.OverlapSphere(transform.position, breath_currentFireRadius).Length; i++)
            {
                for (int j = 0; j < breath_EnemiesArray.Length; j++)
                {
                    if (Physics.OverlapSphere(transform.position, breath_currentFireRadius)[i].CompareTag(breath_EnemiesArray[j]))
                    {
                        breath_targetInBreathRange = true;                     //target jest w breath range

                        Vector3 directionToTarget = (Physics.OverlapSphere(transform.position, breath_currentFireRadius)[i].transform.position - transform.position).normalized; //0-1(normalized) ró¿nica pomiêdzy targetem a characterem Vector3.normalized ==> vector wyra¿ony w radianach
                                                                                                                                                                                 //sprawdzanie aktualnie ostatniego elementu z listy
                        if (Vector3.Angle(transform.forward, directionToTarget) < breath_currentFireAngle / 2)
                        //sprawdzanie angle wektora forward charactera i direction to target
                        //target mo¿e byæ na + albo - od charactera dlatego w ka¿d¹ stronê angle / 2
                        {                            
                            breath_targetInBreathAngle = true;                            
                            if (!Physics.Raycast(transform.position, directionToTarget, breath_currentFireRadius, breath_ObstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiêdzy playerem a targetem //  
                            {
                                if (breath_targetColliders.IndexOf(Physics.OverlapSphere(transform.position, breath_currentFireRadius)[i]) < 0) //sprawdza czy nie ma na liœcie. Je¿eli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                {
                                    breath_targetColliders.Add(Physics.OverlapSphere(transform.position, breath_currentFireRadius)[i]); //przypisuje do listy colliders jeœli ma taga z listy enemies
                                }
                                else
                                {
                                    breath_targetColliders.Remove(Physics.OverlapSphere(transform.position, breath_currentFireRadius)[i]);
                                    if (breath_targetColliders.Count() <= 0) breath_targetInBreathAngle = false; //jeœli nie ma ¿adnych targetów w Cone
                                }
                            }
                                
                        }
                    }
                    
                }
            }
        }
        else
        {
            breath_targetInBreathAngle = false;
            breath_targetInBreathRange = false;
            breath_targetColliders.Clear();  //czyszczenie listy colliderów
        }
    }

    private void FireBreathDamage()
    {
        if (live_charStats.currentCharStatus.isCasting)
        {
            for (int i = 0; i < breath_targetColliders.Count; i++)
            {                
                breath_targetColliders[i].GetComponent<CharacterStatus>().currentHP = Mathf.MoveTowards(breath_targetColliders[i].GetComponent<CharacterStatus>().currentHP, -live_charStats.charSkillCombat.currentSpell_Damage, live_charStats.charSkillCombat.currentSpell_Damage * Time.deltaTime);  // DMG / Sek

                if (breath_targetColliders[i].GetComponent<CharacterStatus>().currentHP <= 0f && !breath_targetColliders[i].GetComponent<CharacterStatus>().currentCharStatus.isDead)

                {
                    live_charStats.currentXP += breath_targetColliders[i].GetComponent<CharacterStatus>().currentXP_GainedFromKill;
                    if (breath_targetColliders[i].CompareTag("Monster")) { breath_targetColliders[i].GetComponent<CharacterStatus>().charInfo.currentCharLevel = Random.Range(live_charStats.charInfo.currentCharLevel - 3, live_charStats.charInfo.currentCharLevel + 3); }  //po œmierci ustawia level targetu na zbli¿ony do atakuj¹cego
                    //podbija lvl tylko Monsterów, Playera i Environment nie
                }

            }
            //live_charStats.currentMP = Mathf.SmoothStep(live_charStats.currentMP, -live_charStats.currentSpell_MPCost, Time.deltaTime);  // MPCost / Sek
            live_charStats.currentMP = Mathf.MoveTowards(live_charStats.currentMP, -live_charStats.charSkillCombat.currentSpell_MPCost, live_charStats.charSkillCombat.currentSpell_MPCost * Time.deltaTime);    // MPCost / Sek
        }        
    }







#if UNITY_EDITOR //zamiast skryptu w Editor

    private void OnDrawGizmos() //rusyje wszystkie
    {
        GizmosDrawer();
    }
    /*private void OnDrawGizmosSelected() //rysuje tylko zaznaczone
    {
        GizmosDrawer();
    }*/

    private void GizmosDrawer()
    {

        Handles.color = live_charStats.charSkillCombat.spell_breathAngleColor;
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.AngleAxis(-(breath_currentFireAngle / 2), Vector3.up) * transform.forward, breath_currentFireAngle, breath_currentFireRadius); //rysuje coneAngle view               


        if (breath_targetInBreathRange && breath_targetInBreathAngle)
        {
            Handles.color = live_charStats.charSkillCombat.spell_breathRaycastColor;
            for (int i = 0; i < breath_targetColliders.Count; i++)
            {
                Handles.DrawLine(transform.position, breath_targetColliders[i].transform.position, live_charStats.fov.fov_editorLineThickness); //rysowanie lini w kierunku targetów breatha jeœli nie zas³ania go obstacle Layer
            }

        }
    }

#endif

}




