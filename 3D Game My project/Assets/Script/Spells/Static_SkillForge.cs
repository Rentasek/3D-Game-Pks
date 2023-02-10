﻿using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public static class Static_SkillForge
{
    /// <summary>
    /// Pobiera EnemiesArray i dokłada Destructibles jako cel skilla
    /// <br><i>Może być wykorzystany currentEnemiesArray z live_CharStats</i></br>
    /// </summary>
    /// <param name="enemiesArray">Pobierany EnemiesArray z FieldOfView</param>
    /// <returns></returns>
    public static string[] EnemyArraySelector(string[] enemiesArray)
    {
        string []skill_EnemiesArray = new string[enemiesArray.Length + 1];              //tworzy array +1 od current enemies arraya
        enemiesArray.CopyTo(skill_EnemiesArray, 0);                                     //kopiuje current enemies arraya od indexu 0
        skill_EnemiesArray[skill_EnemiesArray.Length - 1] = "Destructibles";            //wstawia jako ostatni index Destructibles żeby zawsze można było go zniszczyć
        return (skill_EnemiesArray);
    }

    /// <summary>
    /// Metoda odpowiedzialna za VFX, animacje Animatora i Audio skilla
    /// </summary>
    /// <param name="inputCasting">Czy inputuje cast?</param>
    /// <param name="isCasting">Czy castuje?</param>
    /// <param name="spell_CanCast">Czy może castować?</param>
    /// <param name="currentMP">Aktualne MP postaci</param>
    /// <param name="skill_CastingVisualEffect">VFX przy castowaniu skilla</param>
    /// <param name="skill_CastingAudioSource">AudioSource przy castowaniu skilla(skill gameObject)</param>
    /// <param name="skill_CastingAudioClip">Clip Audio przy castowaniu skilla</param>
    /// <param name="skill_CastingAudioVolume">Audio Volume przy castowaniu skilla</param>
    /// <param name="skill_Animator">Animator przy castowaniu skilla</param>
    /// <param name="skill_AnimatorBool">Boolean Animatora przy castowaniu skilla(IsCasting)</param>    
    public static void Skill_VFX_Audio( bool inputCasting, bool isCasting, bool spell_CanCast, float currentMP, VisualEffect skill_CastingVisualEffect, AudioSource skill_CastingAudioSource,
        AudioClip skill_CastingAudioClip, float skill_CastingAudioVolume, Animator skill_Animator, string skill_AnimatorBool)
    {

        if (inputCasting && spell_CanCast && currentMP >= 1f)
        {
            if (!isCasting)
            {
                skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                skill_CastingAudioSource.PlayOneShot(skill_CastingAudioClip, skill_CastingAudioVolume);//clip audio     
            }
            isCasting = true;
            skill_Animator.SetBool(skill_AnimatorBool, isCasting);
        }
        else
        {
            skill_CastingVisualEffect.Stop();
            isCasting = false;
            skill_Animator.SetBool(skill_AnimatorBool, isCasting);
        }
    }


    /// <summary>
    /// Mechanika dynamic Cone Radius
    /// <br><i>Dynamicznie skaluje zasięg i kąt Skilla</i></br>
    /// </summary>
    /// <param name="isCasting">Czy castuje?</param>
    /// <param name="skill_currentRadius">Aktualny Radius skilla</param>
    /// <param name="skill_currentAngle">Aktualny Kąt skilla</param>
    /// <param name="skill_MinRadius">Bazowy Minimalny Radius skilla</param>
    /// <param name="skill_MaxRadius">Bazowy Maxymalny Radius skilla</param>
    /// <param name="skill_MinAngle">Bazowy Minimalny Kąt skilla</param>
    /// <param name="skill_MaxAngle">Bazowy Maxymalny Kąt skilla</param>
    /// <param name="skill_currentVectorRadius">(ref/refrence) Aktualny wektor(kierunek) w którum porusza się currentRadius skilla</param>
    /// <param name="skill_currentVectorAngle">(ref/refrence) Aktualny wektor(kierunek) w którum porusza się currentAngle skilla</param>
    /// <param name="skill_TimeMaxRadius">Czas zmiany MinRadius -> MaxRadius i vice-versa</param>
    /// <param name="skill_TimeMaxAngle">Czas zmiany MinAngle -> MaxAngle i vice-versa</param>
    public static void Skill_DynamicCone(bool isCasting, float skill_currentRadius, float skill_currentAngle, float skill_MinRadius, float skill_MaxRadius,
        float skill_MinAngle, float skill_MaxAngle, float skill_currentVectorRadius, float skill_currentVectorAngle, float skill_TimeMaxRadius, float skill_TimeMaxAngle)
    {
        if (isCasting)
        {
            skill_currentRadius = Mathf.SmoothDamp(skill_currentRadius, skill_MaxRadius, ref skill_currentVectorRadius, skill_TimeMaxRadius);
            //dynamiczny BreathCone radius -> ++ on input

            skill_currentAngle = Mathf.SmoothDamp(skill_currentAngle, skill_MaxAngle, ref skill_currentVectorAngle, skill_TimeMaxAngle);
            //dynamiczny BreathCone Angle -> ++ on input
        }
        else
        {
            skill_currentRadius = Mathf.SmoothDamp(skill_currentRadius, skill_MinRadius, ref skill_currentVectorRadius, skill_TimeMaxRadius);
            //dynamiczny BreathCone radius -> -- off input

            skill_currentAngle = Mathf.SmoothDamp(skill_currentAngle, skill_MinAngle, ref skill_currentVectorAngle, skill_TimeMaxAngle);
            //dynamiczny BreathCone Angle -> -- off 
        }
    }
    /// <summary>
    /// Szuka targetów w dynamic Cone Radius
    /// <br><i>Zwraca do Skill Objectu listę colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle) </i></br> 
    /// </summary>
    /// <param name="isCasting">Czy castuje?</param>
    /// <param name="skill_currentRadius">Aktualny Radius skilla</param>
    /// <param name="skill_currentAngle">Aktualny Kąt skilla</param>
    /// <param name="skill_MinRadius">Bazowy Minimalny Radius skilla</param>
    /// <param name="skill_MaxRadius">Bazowy Maxymalny Radius skilla</param>
    /// <param name="skill_MinAngle">Bazowy Minimalny Kąt skilla</param>
    /// <param name="skill_MaxAngle">Bazowy Maxymalny Kąt skilla</param>
    /// <param name="skill_currentVectorRadius">(ref/refrence) Aktualny wektor(kierunek) w którum porusza się currentRadius skilla</param>
    /// <param name="skill_currentVectorAngle">(ref/refrence) Aktualny wektor(kierunek) w którum porusza się currentAngle skilla</param>
    /// <param name="skill_TimeMaxRadius">Czas zmiany MinRadius -> MaxRadius i vice-versa</param>
    /// <param name="skill_TimeMaxAngle">Czas zmiany MinAngle -> MaxAngle i vice-versa</param>
    /// <param name="skill_gameObject">GameObject skilla - potrzebny do transform</param>
    /// <param name="skill_EnemiesArray">Enemies Array z klasy skill(do bazowego enemies array dopisane Destructibles, "Metoda EnemyArraySelector")</param>
    /// <param name="skill_targetInRange">Bool zwracający czy w Range jest przeciwnik</param>
    /// <param name="skill_targetInAngle">Bool zwracający czy w Angle(i Range) jest przeciwnik</param>
    /// <param name="skill_ObstaclesMask">Layer Mask z przeszkodami przez które nie da się atakować(z klasy skill)</param>
    /// <param name="skill_targetColliders">Zwracana lista colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle)</param>
    public static void Skill_AttackConeCheck(bool isCasting, float skill_currentRadius, float skill_currentAngle, float skill_MinRadius, float skill_MaxRadius,
       float skill_MinAngle, float skill_MaxAngle, float skill_currentVectorRadius, float skill_currentVectorAngle, float skill_TimeMaxRadius, float skill_TimeMaxAngle, GameObject skill_gameObject, string[] skill_EnemiesArray,
       bool skill_targetInRange, bool skill_targetInAngle, LayerMask skill_ObstaclesMask, List<Collider> skill_targetColliders)  //dynamiczna lista colliderów z OverlapSphere
    {
        Skill_DynamicCone(isCasting, skill_TimeMaxAngle, skill_currentRadius, skill_currentAngle, skill_MinRadius, skill_MaxRadius,
        skill_MinAngle, skill_MaxAngle, skill_currentVectorRadius, skill_currentVectorAngle, skill_TimeMaxRadius);

        if (isCasting)
        {
            for (int i = 0; i < Physics.OverlapSphere(skill_gameObject.transform.position, skill_currentRadius).Length; i++)
            {
                for (int j = 0; j < skill_EnemiesArray.Length; j++)
                {
                    if (Physics.OverlapSphere(skill_gameObject.transform.position, skill_currentRadius)[i].CompareTag(skill_EnemiesArray[j]))
                    {
                        skill_targetInRange = true;                     //target jest w breath range

                        Vector3 directionToTarget = (Physics.OverlapSphere(skill_gameObject.transform.position, skill_currentRadius)[i].transform.position - skill_gameObject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector wyrażony w radianach
                                                                                                                                                                                                              //sprawdzanie aktualnie ostatniego elementu z listy
                        if (Vector3.Angle(skill_gameObject.transform.forward, directionToTarget) < skill_currentAngle / 2)
                        //sprawdzanie angle wektora forward charactera i direction to target
                        //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                        {
                            skill_targetInAngle = true;
                            if (!Physics.Raycast(skill_gameObject.transform.position, directionToTarget, skill_currentRadius, skill_ObstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                            {
                                if (skill_targetColliders.IndexOf(Physics.OverlapSphere(skill_gameObject.transform.position, skill_currentRadius)[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                {
                                    skill_targetColliders.Add(Physics.OverlapSphere(skill_gameObject.transform.position, skill_currentRadius)[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies
                                }
                                else
                                {
                                    skill_targetColliders.Remove(Physics.OverlapSphere(skill_gameObject.transform.position, skill_currentRadius)[i]);
                                    if (skill_targetColliders.Count <= 0) skill_targetInAngle = false; //jeśli nie ma żadnych targetów w Cone
                                }
                            }

                        }
                    }

                }
            }
        }
        else
        {
            skill_targetInAngle = false;
            skill_targetInRange = false;
            skill_targetColliders.Clear();  //czyszczenie listy colliderów
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isCasting">Czy castuje?</param>
    /// <param name="skill_targetColliders">Zwracana lista colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle)</param>
    /// <param name="currentXP">Aktualny XP postaci</param>
    /// <param name="currentMP">Aktualne MP postaci</param>
    /// <param name="skill_currentDamage">Aktualny Damage skilla</param>
    /// <param name="currentCharLevel">Aktualny Level postaci(do zmiany lvla przeciwnika po zabiciu)</param>
    /// <param name="skill_currentMPCost">Aktualny MP cost skilla</param>
    public static void Skill_Damage(bool isCasting, List<Collider> skill_targetColliders, float currentXP, float currentMP, float skill_currentDamage, int currentCharLevel, float skill_currentMPCost)
    {
        if (isCasting)
        {
            for (int i = 0; i < skill_targetColliders.Count; i++)
            {
                skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP = Mathf.MoveTowards(skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP, - skill_currentDamage, skill_currentDamage * Time.deltaTime);  // DMG / Sek

                if (skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP <= 0f && !skill_targetColliders[i].GetComponent<CharacterStatus>().isDead)

                {
                    currentXP += skill_targetColliders[i].GetComponent<CharacterStatus>().currentXP_GainedFromKill;
                    if (skill_targetColliders[i].CompareTag("Monster")) { skill_targetColliders[i].GetComponent<CharacterStatus>().currentCharLevel = Random.Range(currentCharLevel - 3, currentCharLevel + 3); }  //po śmierci ustawia level targetu na zbliżony do atakującego
                    //podbija lvl tylko Monsterów, Playera i Environment nie
                }

            }
            //live_charStats.currentMP = Mathf.SmoothStep(live_charStats.currentMP, -live_charStats.currentSpell_MPCost, Time.deltaTime);  // MPCost / Sek
            currentMP = Mathf.MoveTowards(currentMP, -skill_currentMPCost, skill_currentMPCost * Time.deltaTime);    // MPCost / Sek
        }
    }

    /// <summary>
    /// Update wartości Damage / Cost / Cooldown przed użyciem skilla
    /// </summary>
    /// <param name="skill_currentDamage">Aktualny Damage skilla</param>
    /// <param name="skill_currentMPCost">Aktualny MP cost skilla</param>
    /// <param name="skill_currentCooldown">Aktualny Cooldown skilla</param>
    /// <param name="skill_BaseDamage">Bazowy Damage skilla</param>
    /// <param name="skill_BaseMPCost">Bazowy MP cost skilla</param>
    /// <param name="skill_BaseCooldown">Bazowy Cooldown skilla</param>
    /// <param name="skill_Multiplier">Multiplier do Levela do obliczeń</param>
    /// <param name="currentCharLevel">aktualny Level do obliczeń</param>
    /// <param name="currentBonusSkillDamage">Aktualby bonus damage do skilla</param>
    public static void Skill_ValuesUpdate(float skill_currentDamage, float skill_currentMPCost, float skill_currentCooldown, float skill_BaseDamage, float skill_BaseMPCost, float skill_BaseCooldown,
        float skill_Multiplier, int currentCharLevel, float currentBonusSkillDamage)
    {
        skill_currentDamage = skill_BaseDamage + (skill_BaseDamage * (currentCharLevel * skill_Multiplier)) + (skill_BaseDamage * (currentBonusSkillDamage * skill_Multiplier)); //+bonus
        skill_currentMPCost = skill_BaseMPCost + (skill_BaseMPCost * (currentCharLevel * skill_Multiplier));
        skill_currentCooldown = skill_BaseCooldown;
    }
}


