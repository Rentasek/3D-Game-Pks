using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public static class Static_SkillForge
{
    #region CastingType

    public static class CastingType
    {
        ////////////////////////////////////////////////////
        /////////////////// Casting Type ///////////////////
        ////////////////////////////////////////////////////

        #region Skill_CastingUniversal_VFX_Audio

        /// <summary>
        /// Metoda odpowiedzialna za VFX, animacje Animatora i Audio skilla
        /// <br><i>Rodzaj castowania -> Castable (przytrzymaj i poczekaj aż wystrzeli)</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_CastingUniversal_VFX_Audio(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            //InputSelector(scrObj_Skill, skill, live_charStats);

            if (skill.skill_input && skill.skill_CanCast && Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) >= skill.skill_currentResourceCost)
            {
                if (!live_charStats.charStatus._isCasting) //Jeśli nie castuje i ma właśnie zacząć //Start Casting
                {
                    #region IsCasting -> Instant
                    if (skill.skill_currentCooldownRemaining <= 0.05f)  //Jeśli zostało 0.05f lub mniej cooldownu może użyć instanta
                    {
                        if (skill.skill_CastingVisualEffect != null) skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                        
                        Utils.Skill_StopAllAnimatorMovement(scrObj_Skill, live_charStats);

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, true);
                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill.skill_AnimatorTriggerName);

                        skill.skill_IsCastingInstant = true;
                        skill.skill_currentCooldownRemaining = 1f; //Ustawia cooldown czyli IsCastingInstant wychodzi z true (raz) ale przy następnej klatce już nie wejdzie do if bo ma cooldown

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill.skill_AnimatorFloatName, skill.skill_currentComboProgress); //przed updatem comboProgress
                        skill.skill_currentComboProgress += /*scrObj_Skill.skill_BaseCooldown +*/ 0.5f;
                        if (skill.skill_currentComboProgress >= 1) skill.skill_currentComboProgress = 0f; // trzeci Atak maxymalnie może podbić do [1.83f] więc żeby się wykonał trzeba dać powyżej [1.83f] (1.50f(1.51f dla bezpieczeństwa) + cooldown(0.33))
                        
                        #region AudioClipy //Specjalnie puszczane przy instant ale po animatorze
                        if (scrObj_Skill.skill_InstantAudioClip != null)
                        {
                            skill.skill_AudioSourceInstant.volume = scrObj_Skill.skill_CastingAudioVolume;
                            skill.skill_AudioSourceInstant.PlayOneShot(scrObj_Skill.skill_InstantAudioClip, scrObj_Skill.skill_CastingAudioVolume);
                        }

                        if (scrObj_Skill.skill_CastableAudioClip != null && !skill.skill_AudioSourceCastable.isPlaying)
                        {
                            skill.skill_AudioSourceCastable.volume = scrObj_Skill.skill_CastingAudioVolume;
                            skill.skill_AudioSourceCastable.clip = scrObj_Skill.skill_CastableAudioClip;
                            skill.skill_AudioSourceCastable.PlayScheduled(scrObj_Skill.skill_TimeCast);
                        }

                        if (scrObj_Skill.skill_HoldAudioClip != null && !skill.skill_AudioSourceHold.isPlaying) //jak złapie dobry Audoclip na hold to wrzuce
                        {
                            skill.skill_AudioSourceHold.volume = scrObj_Skill.skill_CastingAudioVolume;
                            skill.skill_AudioSourceHold.PlayOneShot(scrObj_Skill.skill_HoldAudioClip, scrObj_Skill.skill_CastingAudioVolume);
                        }
                        #endregion
                    }
                    #endregion
                }
                live_charStats.charStatus._isCasting = true;

                #region IsCasting -> Castable
                skill.skill_currentCastingProgress = Mathf.MoveTowards(skill.skill_currentCastingProgress, 1f, Time.deltaTime / scrObj_Skill.skill_TimeCast); //casting progress rośnie do 1 w (1sek * 1/TimeCast ) czyli (sek * "n" [np.0.5f] = "n" część sekundy)
                skill.skill_IsCastingFinishedCastable = (skill.skill_currentCastingProgress >= 0.95f) ? true : false;
                if (skill.skill_IsCastingFinishedCastable)
                {
                    Utils.Skill_ResetAnyAudioSourceInstantly(skill);

                    skill.skill_currentCastingProgress = 0f;    //reset progressu po wycastowaniu Skilla

                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) //reset AnimatorIsCastingBooleana i CastingAudioClipa przy IsCastingFinished <- najlepiej żeby był taki sam jak długość AudioClipa przy HOLD
                    {                                                
                        live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, false);
                    }

                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerOnFinishedCastingName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill.skill_AnimatorTriggerOnFinishedCastingName);  //Animacja po wykonaniu casta                                           
                    skill.skill_currentCooldownRemaining = 1f;  //Ustawia cooldown po wycastowaniu żeby nie castował znów zbyt szybko, przydatnie jeśli mamy animację po wystrzeleniu
                }
                #endregion
            }
            else
            {
                Utils.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats);
            }

            #region IsCasting -> Hold

            skill.skill_IsCastingHold = live_charStats.charStatus._isCasting;

            #endregion
        }
        #endregion

        #region Skill_Castable_VFX_Audio
        /*/// <summary> OLD CODE
      /// Metoda odpowiedzialna za VFX, animacje Animatora i Audio skilla
      /// <br><i>Rodzaj castowania -> Castable (przytrzymaj i poczekaj aż wystrzeli)</i></br>
      /// </summary>
      /// <param name="skill_input">Czy inputuje cast?</param>
      /// <param name="isCasting">Czy castuje?</param>
      /// <param name="skill_CanCast">Czy może castować?</param>
      /// <param name="skill_currentResource">Aktualne Resource postaci</param>
      /// <param name="skill_currentResourceCost">Aktualny koszt Resource skilla</param>
      /// <param name="skill_currentCastingProgress">Aktualny progress Castowania</param>
      /// <param name="skill_currentCastingFinished">Bool zwracany true jeśli progress castowania dojdzie do 100%</param>
      /// <param name="skill_TimeCast">Czas potrzebny do wycastowania(bool CastingFinished)</param>
      /// <param name="skill_CastingVisualEffect">VFX przy castowaniu skilla</param>
      /// <param name="skill_AudioSource">AudioSource przy castowaniu skilla(skill gameObject)</param>
      /// <param name="skill_CastingAudioClip">Clip Audio przy castowaniu skilla</param>
      /// <param name="skill_CastingAudioVolume">Audio Volume przy castowaniu skilla</param>
      /// <param name="currentAnimator">Animator przy castowaniu skilla</param>
      /// <param name="skill_AnimatorBool">Boolean Animatora przy castowaniu skilla(IsCasting)</param>    
      public static void Skill_Castable_VFX_Audio(bool skill_input, bool isCasting, bool skill_CanCast, float skill_currentResource, float skill_currentResourceCost, float skill_currentCastingProgress, bool skill_currentCastingFinished,
          float skill_TimeCast, VisualEffect skill_CastingVisualEffect, AudioSource skill_AudioSource, AudioClip skill_CastingAudioClip, float skill_CastingAudioVolume, Animator currentAnimator, string skill_AnimatorBool)
      {
          if (skill_input && skill_CanCast && skill_currentResource >= skill_currentResourceCost)
          {
              if (!isCasting)
              {
                  skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                  skill_AudioSource.PlayOneShot(skill_CastingAudioClip, skill_CastingAudioVolume);//clip audio  
              }
              isCasting = true;
              currentAnimator.SetBool(skill_AnimatorBool, isCasting);
              skill_currentCastingProgress = Mathf.Lerp(skill_currentCastingProgress, 1f, Time.deltaTime / skill_TimeCast);
              skill_currentCastingFinished = (skill_currentCastingProgress >= 1f) ? true : false;
              if (skill_currentCastingFinished) skill_currentCastingProgress = 0f; //reset progressu po wycastowaniu Skilla
          }
          else
          {
              skill_CastingVisualEffect.Stop();
              isCasting = false;
              skill_currentCastingProgress = 0f; //reset progressu przy przerwaniu casta / niespełnieniu warunków
              currentAnimator.SetBool(skill_AnimatorBool, isCasting);
          }
          Debug.Log(nameof(Skill_Castable_VFX_Audio));
      }*/


        /// <summary>
        /// Metoda odpowiedzialna za VFX, animacje Animatora i Audio skilla
        /// <br><i>Rodzaj castowania -> Castable (przytrzymaj i poczekaj aż wystrzeli)</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_Castable_VFX_Audio(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            Utils.Skill_InputSelector(scrObj_Skill, skill, live_charStats);

            if (skill.skill_input && skill.skill_CanCast && Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) >= skill.skill_currentResourceCost)
            {
                if (!live_charStats.charStatus._isCasting)
                {
                    skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                    skill.skill_AudioSourceCastable.PlayOneShot(scrObj_Skill.skill_CastableAudioClip, scrObj_Skill.skill_CastingAudioVolume);//clip audio    
                }
                live_charStats.charStatus._isCasting = true;
                if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, live_charStats.charStatus._isCasting);
                skill.skill_currentCastingProgress = Mathf.Lerp(skill.skill_currentCastingProgress, 1f, Time.deltaTime / scrObj_Skill.skill_TimeCast);
                skill.skill_IsCastingFinishedCastable = (skill.skill_currentCastingProgress >= 1f) ? true : false;
                if (skill.skill_IsCastingFinishedCastable) skill.skill_currentCastingProgress = 0f; //reset progressu po wycastowaniu Skilla
            }
            else
            {
                Utils.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats);

                Utils.Skill_Target_ConeReset(skill); //Reset TargetList
            }
        }
        #endregion

        #region Skill_Instant_VFX_Audio
        /*/// <summary> OLD CODE
    /// Metoda odpowiedzialna za VFX, animacje Animatora i Audio skilla
    /// <br><i>Rodzaj castowania -> Instant (klik i działa)</i></br>
    /// </summary>
    /// <param name="skill_input">Czy inputuje cast?</param>
    /// <param name="isCasting">Czy castuje?</param>
    /// <param name="skill_CanCast">Czy może castować?</param>
    /// <param name="skill_currentResource">Aktualne Resource postaci</param>
    /// <param name="skill_currentResourceCost">Aktualny koszt Resource skilla</param>
    /// <param name="skill_currentCastingInstant">Bool zwracany true jeśli jest instantCast</param>
    /// <param name="skill_CastingVisualEffect">VFX przy castowaniu skilla</param>
    /// <param name="skill_AudioSource">AudioSource przy castowaniu skilla(skill gameObject)</param>
    /// <param name="skill_CastingAudioClip">Clip Audio przy castowaniu skilla</param>
    /// <param name="skill_CastingAudioVolume">Audio Volume przy castowaniu skilla</param>
    /// <param name="currentAnimator">Animator przy castowaniu skilla</param>
    /// <param name="skill_AnimatorTrigger">Trigger Animatora przy castowaniu skilla(IsCasting)</param>
    public static void Skill_Instant_VFX_Audio(bool skill_input, bool isCasting, bool skill_CanCast, float skill_currentResource, float skill_currentResourceCost, bool skill_currentCastingInstant,
        VisualEffect skill_CastingVisualEffect, AudioSource skill_AudioSource, AudioClip skill_CastingAudioClip, float skill_CastingAudioVolume, Animator currentAnimator, string skill_AnimatorTrigger)
    {
        if (skill_input && skill_CanCast && skill_currentResource >= skill_currentResourceCost)
        {
            if (!isCasting)
            {
                skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                skill_AudioSource.PlayOneShot(skill_CastingAudioClip, skill_CastingAudioVolume);//clip audio                     
                currentAnimator.SetTrigger(skill_AnimatorTrigger);
            }
            isCasting = true;            
        }
        else
        {
            skill_CastingVisualEffect.Stop();
            isCasting = false;                    
        }
        skill_currentCastingInstant = isCasting;
        Debug.Log(nameof(Skill_Instant_VFX_Audio));
    }*/

        /// <summary>
        /// Metoda odpowiedzialna za VFX, animacje Animatora i Audio skilla
        /// <br><i>Rodzaj castowania -> Instant (klik i działa)</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_Instant_VFX_Audio(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            Utils.Skill_InputSelector(scrObj_Skill, skill, live_charStats);

            if (skill.skill_input && skill.skill_CanCast && Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) >= skill.skill_currentResourceCost)
            {
                if (!live_charStats.charStatus._isCasting)
                {
                    skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                    skill.skill_AudioSourceInstant.PlayOneShot(scrObj_Skill.skill_InstantAudioClip, scrObj_Skill.skill_CastingAudioVolume);//clip audio 
                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill.skill_AnimatorTriggerName);
                }
                live_charStats.charStatus._isCasting = true;
            }
            else
            {
                Utils.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats);

                Utils.Skill_Target_ConeReset(skill); //Reset TargetList
            }
            skill.skill_IsCastingInstant = live_charStats.charStatus._isCasting;
        }
        #endregion

        #region Skill_Hold_VFX_Audio

        /* /// <summary> OLD CODE
         /// Metoda odpowiedzialna za VFX, animacje Animatora i Audio skilla
         /// <br><i>Rodzaj castowania -> HOLD (przytrzymaj przycisk)</i></br>
         /// </summary>
         /// <param name="skill_input">Czy inputuje cast?</param>
         /// <param name="isCasting">Czy castuje?</param>
         /// <param name="skill_CanCast">Czy może castować?</param>
         /// <param name="skill_currentResource">Aktualne Resource postaci</param>
         /// <param name="skill_currentResourceCost">Aktualny koszt Resource skilla</param>
         /// <param name="skill_CastingVisualEffect">VFX przy castowaniu skilla</param>
         /// <param name="skill_AudioSource">AudioSource przy castowaniu skilla(skill gameObject)</param>
         /// <param name="skill_CastingAudioClip">Clip Audio przy castowaniu skilla</param>
         /// <param name="skill_CastingAudioVolume">Audio Volume przy castowaniu skilla</param>
         /// <param name="currentAnimator">Animator przy castowaniu skilla</param>
         /// <param name="skill_AnimatorBool">Boolean Animatora przy castowaniu skilla(IsCasting)</param>    
         public static void Skill_Hold_VFX_Audio( bool skill_input, bool isCasting, bool skill_CanCast, float skill_currentResource, float skill_currentResourceCost, VisualEffect skill_CastingVisualEffect, AudioSource skill_AudioSource,
             AudioClip skill_CastingAudioClip, float skill_CastingAudioVolume, Animator currentAnimator, string skill_AnimatorBool)
         {

             if (skill_input && skill_CanCast && skill_currentResource >= skill_currentResourceCost)
             {
                 if (!isCasting)
                 {
                     skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                     skill_AudioSource.PlayOneShot(skill_CastingAudioClip, skill_CastingAudioVolume);//clip audio     
                 }
                 isCasting = true;
                 currentAnimator.SetBool(skill_AnimatorBool, isCasting);
                 Debug.Log(nameof(Skill_Hold_VFX_Audio));
             }
             else
             {
                 skill_CastingVisualEffect.Stop();
                 isCasting = false;
                 currentAnimator.SetBool(skill_AnimatorBool, isCasting);
             }
             Debug.Log("InputCasting : "+ inputCasting);
             Debug.Log("skill_CanCast : " + skill_CanCast);
             Debug.Log("skill_currentResource : " + skill_currentResource);
             Debug.Log("skill_currentResourceCost : " + skill_currentResourceCost);
         }*/

        /// <summary>
        /// Metoda odpowiedzialna za VFX, animacje Animatora i Audio skilla
        /// <br><i>Rodzaj castowania -> HOLD (przytrzymaj przycisk)</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param> 
        public static void Skill_Hold_VFX_Audio(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            Utils.Skill_InputSelector(scrObj_Skill, skill, live_charStats);

            if (skill.skill_input && skill.skill_CanCast && Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) >= skill.skill_currentResourceCost)
            {
                if (!live_charStats.charStatus._isCasting)
                {
                    skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                    skill.skill_AudioSourceHold.PlayOneShot(scrObj_Skill.skill_HoldAudioClip, scrObj_Skill.skill_CastingAudioVolume);//clip audio     
                }
                live_charStats.charStatus._isCasting = true;
                if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, live_charStats.charStatus._isCasting);
            }
            else
            {
                Utils.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats);

                Utils.Skill_Target_ConeReset(skill); //Reset TargetList
            }
        }
        #endregion
    }



    #endregion CastingType

    #region TargetType
    ///////////////////////////////////////////////////
    /////////////////// Target Type ///////////////////
    ///////////////////////////////////////////////////

    #region Skill_Cone_Target
    //
    /////

    /*/// <summary> OLD CODE
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
/// <param name="skill_casterGameobject">GameObject skilla (castera) -> potrzebny do transform</param>
/// <param name="skill_EnemiesArray">Enemies Array z klasy skill(do bazowego enemies array dopisane Destructibles, "Metoda EnemyArraySelector")</param>
/// <param name="skill_targetInRange">Bool zwracający czy w Range jest przeciwnik</param>
/// <param name="skill_targetInAngle">Bool zwracający czy w Angle(i Range) jest przeciwnik</param>
/// <param name="skill_ObstaclesMask">Layer Mask z przeszkodami przez które nie da się atakować(z klasy skill)</param>
/// <param name="skill_targetColliders">Zwracana lista colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle)</param>
public static void Skill_Cone_AttackConeCheck(bool isCasting, float skill_currentRadius, float skill_currentAngle, float skill_MinRadius, float skill_MaxRadius,
   float skill_MinAngle, float skill_MaxAngle, float skill_currentVectorRadius, float skill_currentVectorAngle, float skill_TimeMaxRadius, float skill_TimeMaxAngle, GameObject skill_casterGameobject, string[] skill_EnemiesArray,
   bool skill_targetInRange, bool skill_targetInAngle, LayerMask skill_ObstaclesMask, List<Collider> skill_targetColliders)  //dynamiczna lista colliderów z OverlapSphere
{
    Skill_Cone_DynamicCone(isCasting, skill_TimeMaxAngle, skill_currentRadius, skill_currentAngle, skill_MinRadius, skill_MaxRadius,
    skill_MinAngle, skill_MaxAngle, skill_currentVectorRadius, skill_currentVectorAngle, skill_TimeMaxRadius);

    if (isCasting)
    {
        for (int i = 0; i < Physics.OverlapSphere(skill_casterGameobject.transform.position, skill_currentRadius).Length; i++)
        {
            for (int j = 0; j < skill_EnemiesArray.Length; j++)
            {
                if (Physics.OverlapSphere(skill_casterGameobject.transform.position, skill_currentRadius)[i].CompareTag(skill_EnemiesArray[j]))
                {
                    skill_targetInRange = true;                     //target jest w breath range

                    Vector3 directionToTarget = (Physics.OverlapSphere(skill_casterGameobject.transform.position, skill_currentRadius)[i].transform.position - skill_casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector wyrażony w radianach
                                                                                                                                                                                                          //sprawdzanie aktualnie ostatniego elementu z listy
                    if (Vector3.Angle(skill_casterGameobject.transform.forward, directionToTarget) < skill_currentAngle / 2)
                    //sprawdzanie angle wektora forward charactera i direction to target
                    //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                    {
                        skill_targetInAngle = true;
                        if (!Physics.Raycast(skill_casterGameobject.transform.position, directionToTarget, skill_currentRadius, skill_ObstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                        {
                            if (skill_targetColliders.IndexOf(Physics.OverlapSphere(skill_casterGameobject.transform.position, skill_currentRadius)[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                            {
                                skill_targetColliders.Add(Physics.OverlapSphere(skill_casterGameobject.transform.position, skill_currentRadius)[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies
                            }
                            else
                            {
                                skill_targetColliders.Remove(Physics.OverlapSphere(skill_casterGameobject.transform.position, skill_currentRadius)[i]);
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
    Debug.Log(nameof(Skill_Cone_AttackConeCheck));
}*/

    #region OverlapSphereNonAloc
    /*/// <summary>nowy OverlapSphereNonAloc ma swoje plusy bo nie tworzy nowego array przy kazdym Cast ale ogólnie niewiele zmienia a trzeba robić dodatkową tablicę niedynamiczną
    /// Szuka targetów w dynamic Cone Radius
    /// <br><i>Zwraca do Skill Objectu listę colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle) </i></br> 
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_Cone_Target(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
    {
        Utils.Skill_Cone_DynamicCone(scrObj_Skill, skill, live_charStats);

        if (live_charStats.currentCharStatus.isCasting)
        {
            for (int i = 0; i < Physics.OverlapSphereNonAlloc(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius, skill.skill_localColliders); i++)
            {
                for (int j = 0; j < skill.skill_EnemiesArray.Length; j++)
                {
                    if (skill.skill_localColliders[i].CompareTag(skill.skill_EnemiesArray[j]))
                    {
                        skill.skill_targetInRange = true; //target jest w breath range

                        Vector3 directionToTarget = (skill.skill_localColliders[i].transform.position - skill.skill_casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector (kierunek w którym od niego znajduje się target)
                                                                                                                                                                     //sprawdzanie aktualnie ostatniego elementu z listy
                        if (Vector3.Angle(skill.skill_casterGameobject.transform.forward, directionToTarget) < skill.skill_currentAngle / 2)
                        //sprawdzanie angle wektora forward charactera i direction to target
                        //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                        {
                            if (!Physics.Raycast(skill.skill_casterGameobject.transform.position, directionToTarget, Vector3.Distance(skill.skill_casterGameobject.transform.position, skill.skill_localColliders[i].transform.position), scrObj_Skill.skill_ObstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                            {
                                skill.skill_targetInAngle = true;

                                if (skill.skill_targetColliders.IndexOf(skill.skill_localColliders[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                {
                                    skill.skill_targetColliders.Add(skill.skill_localColliders[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies
                                }
                                else
                                {
                                    skill.skill_targetColliders.Remove(skill.skill_localColliders[i]);
                                    if (skill.skill_targetColliders.Count <= 0) skill.skill_targetInAngle = false; //jeśli nie ma żadnych targetów w Cone
                                }
                            }

                        }
                    }
                }
            }
        }
        else
        {
            Utils.Skill_Target_ConeReset(skill);
        }
    }*/
    #endregion

    #region Działający OverlapSphere
    /// <summary>Działający Stary OverlapSphere
    /// Szuka targetów w dynamic Cone Radius
    /// <br><i>Zwraca do Skill Objectu listę colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle) </i></br> 
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_Cone_Target(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
    {
        Utils.Skill_Cone_DynamicCone(scrObj_Skill, skill, live_charStats);

        if (live_charStats.charStatus._isCasting)
        {
            for (int i = 0; i < Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius).Length; i++)
            {
                for (int j = 0; j < skill.skill_EnemiesArray.Length; j++)
                {
                    if (Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i].CompareTag(skill.skill_EnemiesArray[j]))
                    {
                        skill.skill_targetInRange = true; //target jest w breath range

                        Vector3 directionToTarget = (Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i].transform.position - skill.skill_casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector (kierunek w którym od niego znajduje się target)
                                                                                                                                                                                                                                            //sprawdzanie aktualnie ostatniego elementu z listy
                        if (Vector3.Angle(skill.skill_casterGameobject.transform.forward, directionToTarget) < skill.skill_currentAngle / 2)
                        //sprawdzanie angle wektora forward charactera i direction to target
                        //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                        {
                            if (!Physics.Raycast(skill.skill_casterGameobject.transform.position, directionToTarget, Vector3.Distance(skill.skill_casterGameobject.transform.position, Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i].transform.position), scrObj_Skill.skill_ObstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                            {
                                skill.skill_targetInAngle = true;

                                if (skill.skill_targetColliders.IndexOf(Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                {
                                    skill.skill_targetColliders.Add(Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies
                                }
                                else
                                {
                                    skill.skill_targetColliders.Remove(Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i]);
                                    if (skill.skill_targetColliders.Count <= 0) skill.skill_targetInAngle = false; //jeśli nie ma żadnych targetów w Cone
                                }
                            }

                        }
                    }
                }
            }
        }
        else
        {
            Utils.Skill_Target_ConeReset(skill);
        }
    }
    #endregion
        
    #endregion

    #region Skill_Melee_Target

    /// <summary>
    /// Szuka targetów w Melee Radius
    /// <br><i>Zwraca do Skill Objectu listę colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle) </i></br> 
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_Melee_Target(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
    {
        Utils.Skill_Melee_DynamicCone(scrObj_Skill, skill, live_charStats);

        if (live_charStats.charStatus._isCasting)
        {
            for (int i = 0; i < Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius).Length; i++)
            {
                for (int j = 0; j < skill.skill_EnemiesArray.Length; j++)
                {
                    if (Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i].CompareTag(skill.skill_EnemiesArray[j]))
                    {
                        skill.skill_targetInRange = true; //target jest w Radius range

                        Vector3 directionToTarget = (Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i].transform.position - skill.skill_casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector (kierunek w którym od niego znajduje się target)
                                                                                                                                                                                                                                            //sprawdzanie aktualnie ostatniego elementu z listy
                        if (Vector3.Angle(skill.skill_casterGameobject.transform.forward, directionToTarget) < skill.skill_currentAngle / 2)
                        //sprawdzanie angle wektora forward charactera i direction to target
                        //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                        {
                            if (!Physics.Raycast(skill.skill_casterGameobject.transform.position, directionToTarget, Vector3.Distance(skill.skill_casterGameobject.transform.position, Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i].transform.position), scrObj_Skill.skill_ObstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                            {
                                skill.skill_targetInAngle = true;

                                if (skill.skill_targetColliders.IndexOf(Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                {
                                    skill.skill_targetColliders.Add(Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies
                                }
                                else
                                {
                                    skill.skill_targetColliders.Remove(Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i]);
                                    if (skill.skill_targetColliders.Count <= 0) skill.skill_targetInAngle = false; //jeśli nie ma żadnych targetów w Cone
                                }
                            }

                        }
                    }
                }
            }
        }
        else
        {
            Utils.Skill_Target_ConeReset(skill);
        }
    }
    #endregion

    #region Skill_Self_Target

    /// <summary>
    /// Self target
    /// <br><i>Zwraca do Skill Objectu listę colliderów [1] element zgodny z parametrami(Self) </i></br> 
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_Self_Target(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
    {
        if (live_charStats.charStatus._isCasting)
        {
            if (skill.skill_targetColliders.IndexOf(live_charStats.gameObject.GetComponent<Collider>()) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
            {
                skill.skill_targetColliders.Add(live_charStats.gameObject.GetComponent<Collider>());
                skill.skill_targetInAngle = true;
                skill.skill_targetInRange = true;
            }
            else
            {
                skill.skill_targetColliders.Remove(live_charStats.gameObject.GetComponent<Collider>());
                if (skill.skill_targetColliders.Count <= 0) { skill.skill_targetInAngle = false; skill.skill_targetInRange = false; } //jeśli nie ma żadnych targetów w Cone
            }
        }
        else
        {
            Utils.Skill_Target_ConeReset(skill);
        }
    }
    #endregion

    #endregion TargetType

    #region EffectType

    ///////////////////////////////////////////////////
    /////////////////// Effect Type ///////////////////
    ///////////////////////////////////////////////////

    #region Skill_DamageOverTime

    /*/// <summary> OLD CODE
    /// 
    /// </summary>
    /// <param name="isCasting">Czy castuje?</param>
    /// <param name="skill_targetColliders">Zwracana lista colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle)</param>
    /// <param name="currentXP">Aktualny XP postaci</param>
    /// <param name="skill_currentResource">Aktualne MP postaci</param>
    /// <param name="skill_currentDamage">Aktualny Damage skilla</param>
    /// <param name="currentCharLevel">Aktualny Level postaci(do zmiany lvla przeciwnika po zabiciu)</param>
    /// <param name="skill_currentResourceCost">Aktualny Resource cost skilla</param>
    public static void Skill_TargetList_DamageOverTime(bool isCasting, List<Collider> skill_targetColliders, float currentXP, float skill_currentResource, float skill_currentDamage, int currentCharLevel, float skill_currentResourceCost)
    {
        if (isCasting)
        {
            for (int i = 0; i < skill_targetColliders.Count; i++)
            {
                skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP = Mathf.MoveTowards(skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP, - skill_currentDamage, skill_currentDamage * Time.deltaTime);  // DMG / Sek

                if (skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP <= 0f && !skill_targetColliders[i].GetComponent<CharacterStatus>().isDead)

                {
                    currentXP += skill_targetColliders[i].GetComponent<CharacterStatus>().currentXP_GainedFromKill;
                    if (skill_targetColliders[i].CompareTag("Monster")) { skill_targetColliders[i].GetComponent<CharacterStatus>().currentCharLevel = UnityEngine.Random.Range(currentCharLevel - 3, currentCharLevel + 3); }  //po śmierci ustawia level targetu na zbliżony do atakującego
                    //podbija lvl tylko Monsterów, Playera i Environment nie
                }

            }
            //live_charStats.currentMP = Mathf.SmoothStep(live_charStats.currentMP, -live_charStats.currentSpell_MPCost, Time.deltaTime);  // MPCost / Sek
            skill_currentResource = Mathf.MoveTowards(skill_currentResource, -skill_currentResourceCost, skill_currentResourceCost * Time.deltaTime);    // MPCost / Sek
        }
    }
    
    #region Skill_DamageOverTime - działający bez arraya
    /// <summary>
    /// DamageOverTime (MoveTowards)- Target Colliderów na Collider List
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_DamageOverTime(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
    {
        Utils.Skill_EffectValuesUpdate(scrObj_Skill, skill, live_charStats, live_charStats.currentCharacterBonusStats); //nie chce mi się dopisywać linka do CharacterBonusStats :P

        if (live_charStats.isCasting)
        {
            for (int i = 0; i < skill.skill_targetColliders.Count; i++)
            {
                skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP = Mathf.MoveTowards(skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP, -skill.skill_currentDamage, skill.skill_currentDamage * Time.deltaTime);  // DMG / Sek

                if (skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP <= 0f && !skill.skill_targetColliders[i].GetComponent<CharacterStatus>().isDead)
                {
                    live_charStats.currentXP += skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentXP_GainedFromKill;
                    if (skill.skill_targetColliders[i].CompareTag("Monster")) { skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentCharLevel = UnityEngine.Random.Range(live_charStats.currentCharLevel - 3, live_charStats.currentCharLevel + 3); }  //po śmierci ustawia level targetu na zbliżony do atakującego
                                                                                                                                                                                                                                                             //podbija lvl tylko Monsterów, Playera i Environment nie
                }
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill.skill_ResourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    live_charStats.currentHP = Mathf.MoveTowards(live_charStats.currentHP, -skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    live_charStats.currentMP = Mathf.MoveTowards(live_charStats.currentMP, -skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, -skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);     // ResourceCost / Sek 
                    break;
            }
        }
    }
    #endregion 

     
    */

    #region Skill_DamageOverTime - działający z arrayem
    /// <summary>
    /// DamageOverTime (MoveTowards)- Target Colliderów na Collider List
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_DamageOverTime(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, bool isCastingType)
    {
        Utils.Skill_EffectValuesUpdate(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats); //nie chce mi się dopisywać linka do CharacterBonusStats :P

        if (isCastingType)
        {
            for (int i = 0; i < skill.skill_targetColliders.Count; i++)
            {                
                skill.skill_targetColliders[i].GetComponent<CharacterStatus>().TakeDamageOverTime(skill.skill_currentDamage, live_charStats);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill.skill_ResourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    live_charStats.charStats._hp = Mathf.MoveTowards(live_charStats.charStats._hp, -skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    live_charStats.charStats._mp = Mathf.MoveTowards(live_charStats.charStats._mp, -skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, -skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);     // ResourceCost / Sek 
                    break;
            }
        }
    }
    #endregion

    #endregion

    #region Skill_Hit

    /// <summary>
    /// Hit Instant - Target Colliderów na Collider List
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_Hit(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, bool isCastingType)
    {
        Utils.Skill_EffectValuesUpdate(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats); //nie chce mi się dopisywać linka do CharacterBonusStats :P

        if (isCastingType)
        {
            for (int i = 0; i < skill.skill_targetColliders.Count; i++)
            {
                skill.skill_targetColliders[i].GetComponent<CharacterStatus>().TakeDamgeInstant(skill.skill_currentDamage, live_charStats);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill.skill_ResourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    live_charStats.charStats._hp -= skill.skill_currentResourceCost; // ResourceCost Instant
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    live_charStats.charStats._mp -= skill.skill_currentResourceCost;  // ResourceCost Instant
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    live_charStats.charStats._stam -= skill.skill_currentResourceCost; // ResourceCost Instant
                    break;
            }
        }
    }

    #endregion

    #region Skill_HealOverTime

    /// <summary>
    /// HealOverTime (MoveTowards)- Target Colliderów na Collider List
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_HealOverTime(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, bool isCastingType)
    {
        Utils.Skill_EffectValuesUpdate(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats); //nie chce mi się dopisywać linka do CharacterBonusStats :P

        if (isCastingType)
        {
            for (int i = 0; i < skill.skill_targetColliders.Count; i++)
            {
                skill.skill_targetColliders[i].GetComponent<CharacterStatus>().TakeHealOverTime(skill.skill_currentDamage);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill.skill_ResourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    live_charStats.charStats._hp = Mathf.MoveTowards(live_charStats.charStats._hp, -skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    live_charStats.charStats._mp = Mathf.MoveTowards(live_charStats.charStats._mp, -skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, -skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);     // ResourceCost / Sek 
                    break;
            }
        }
    }

    #endregion

    #region Skill_Heal

    /// <summary>
    /// Heal Instant - Target Colliderów na Collider List
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_Heal(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, bool isCastingType)
    {
        Utils.Skill_EffectValuesUpdate(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats); //nie chce mi się dopisywać linka do CharacterBonusStats :P

        if (isCastingType)
        {
            for (int i = 0; i < skill.skill_targetColliders.Count; i++)
            {
                skill.skill_targetColliders[i].GetComponent<CharacterStatus>().TakeHealInstant(skill.skill_currentDamage);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill.skill_ResourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    live_charStats.charStats._hp -= skill.skill_currentResourceCost; // ResourceCost Instant
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    live_charStats.charStats._mp -= skill.skill_currentResourceCost;  // ResourceCost Instant
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    live_charStats.charStats._stam -= skill.skill_currentResourceCost; // ResourceCost Instant
                    break;
            }
        }
    }

    #endregion

    #endregion

    #region Utils

    public static class Utils
    {
        /////////////////////////////////////////////
        /////////////////// UTILS ///////////////////
        /////////////////////////////////////////////

        #region Skill_EnemyArraySelector
        /*/// <summary>OLD CODE
        /// Pobiera EnemiesArray, dokłada Destructibles jako cel skilla
        /// <br>Zwraca skill_EnemiesArray (return (skill_EnemiesArray))
        /// <br><i>Może być wykorzystany currentEnemiesArray z live_CharStats</i></br>
        /// </summary>
        /// <param name="enemiesArray">Pobierany EnemiesArray z live_CharStats</param>
        /// <returns></returns>
        public static string[] Skill_EnemyArraySelector(string[] enemiesArray)
        {
            string[] skill_EnemiesArray = new string[enemiesArray.Length + 1];              //tworzy array +1 od current enemies arraya
            enemiesArray.CopyTo(skill_EnemiesArray, 0);                                     //kopiuje current enemies arraya od indexu 0
            skill_EnemiesArray[skill_EnemiesArray.Length - 1] = "Destructibles";            //wstawia jako ostatni index Destructibles żeby zawsze można było go zniszczyć
            return (skill_EnemiesArray);
        }*/

        /// <summary>
        /// <br>Pobiera EnemiesArray z live_charStats, dokłada Destructibles jako cel skilla</br>
        /// <br>Wypełnia skill_EnemiesArray jako ref objectu, czyli zwraca bezpośredio do SkillObjectu nie trzeba przypisywać</br>
        /// </summary>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_EnemyArraySelector(Skill skill, CharacterStatus live_charStats)
        {
            skill.skill_EnemiesArray = new string[live_charStats.charInfo._enemiesArray.Length + 1];              //tworzy array +1 od current enemies arraya
            live_charStats.charInfo._enemiesArray.CopyTo(skill.skill_EnemiesArray, 0);                                     //kopiuje current enemies arraya od indexu 0
            skill.skill_EnemiesArray[skill.skill_EnemiesArray.Length - 1] = "Destructibles";                                  //wstawia jako ostatni index Destructibles żeby zawsze można było go zniszczyć           
        }

        #endregion

        #region Skill_InputSelector

        /*/// <summary> OLD CODE
        /// Update skill_input i skill_otherInput w zależności od ustawień w scriptableObjecie
        /// </summary>
        /// <param name="skill_InputCastingType">Enumerator ze scriptable objectu Skill</param>
        /// <param name="skill_input">Skill input lokalny dla klasy skill</param>
        /// <param name="skill_otherInput">Skill input Other lokalny dla klasy skill</param>
        /// <param name="inputPrimary">Skill inputPrimary z klasy Input/Live_charStats dla klasy skill (LPM)</param>
        /// <param name="inputSecondary">Skill inputSecondary z klasy Input/Live_charStats dla klasy skill (RPM)</param>
        public static void InputSelector(ScrObj_skill.Skill_InputCastingType skill_InputCastingType, bool skill_input, bool skill_otherInput, bool inputPrimary, bool inputSecondary)
        {
            switch(skill_InputCastingType) 
            {
                case ScrObj_skill.Skill_InputCastingType.primary:
                    skill_input = inputPrimary;
                    skill_otherInput = inputSecondary;                
                    break;
                case ScrObj_skill.Skill_InputCastingType.secondary:
                    skill_input = inputSecondary;
                    skill_otherInput= inputPrimary;
                    break;
            }
        }

        /// <summary> OLD Returning InputSelector
        /// Update skill_input i skill_otherInput w zależności od ustawień w scriptableObjecie
        /// Użyty w skill_Castings
        /// <br>Zwraca Arraya booleanów ponieważ można returnować tylko 1 zmienną a potrzeba input i other input</br>
        /// <br>inputs[1] = input tego skilla, inputs[2] = input_other </br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static bool[] InputSelectorReadOnly(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            bool[] inputs = new bool[2];

            switch (scrObj_Skill.skill_InputType)
            {
                case ScrObj_skill.Skill_InputType.primary:
                    inputs[1] = live_charStats.inputPrimary;
                    inputs[2] = live_charStats.inputSecondary;
                    break;
                case ScrObj_skill.Skill_InputType.secondary:
                    inputs[1] = live_charStats.inputSecondary;
                    inputs[2] = live_charStats.inputPrimary;
                    break;
            }
            return inputs;
        }

         */

        /// <summary>
        /// Update skill_input i skill_otherInput w zależności od ustawień w scriptableObjecie
        /// Użyty w skill_Castings
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_InputSelector(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            switch (scrObj_Skill.skill_InputType)
            {
                case ScrObj_skill.Skill_InputType.primary:
                    skill.skill_input = live_charStats.charInput._primary;
                    skill.skill_otherInput = live_charStats.charInput._secondary;
                    break;
                case ScrObj_skill.Skill_InputType.secondary:
                    skill.skill_input = live_charStats.charInput._secondary;
                    skill.skill_otherInput = live_charStats.charInput._primary;
                    break;
            }
        }

        #endregion

        #region Skill_ResourceTypeCurrentFloatReadOnly

        /*/// <summary> OLD CODE
        /// Update Resource Type w zależności od ustawień w scriptableObjecie
        /// </summary>
        /// <param name="skill_ResourceType">Enumerator ze scriptable objectu Skill</param>
        /// <param name="currentHP">Statystyka HP z live_charStats</param>
        /// <param name="currentMP">Statystyka MP z live_charStats</param>
        /// <param name="currentStam">Statystyka Stamina z live_charStats</param>
        /// <param name="skill_currentResourceType">Resource type Lokalny dla klasy Skill</param>
        public static void ResourceTypeSelector(ScrObj_skill.Skill_ResourceType skill_ResourceType, float currentHP, float currentMP, float currentStam, float skill_currentResourceType)
        {
            switch (skill_ResourceType)
            {
                case ScrObj_skill.Skill_ResourceType.hp:
                    skill_currentResourceType = currentHP;
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    skill_currentResourceType = currentMP;
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    skill_currentResourceType = currentStam;
                    break;
            }
        }*/

        /* /// <summary>OLD Metody Resource In && Out
         /// Update Resource Type w zależności od ustawień w scriptableObjecie
         /// </summary>
         /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
         /// <param name="skill">Ten GameObject skill</param>
         /// <param name="live_charStats">Live_charStats Castera</param>
         public static void ResourceTypeSelector_In(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
         {
             switch (scrObj_Skill.skill_ResourceType)
             {
                 case ScrObj_skill.Skill_ResourceType.hp:
                     skill.skill_currentResource = live_charStats.currentHP;
                     break;

                 case ScrObj_skill.Skill_ResourceType.mana:
                     skill.skill_currentResource = live_charStats.currentMP;
                     break;

                 case ScrObj_skill.Skill_ResourceType.stamina:
                     skill.skill_currentResource = live_charStats.currentStam;
                     break;
             }
         }

         /// <summary>
         /// Update Resource Type w zależności od ustawień w scriptableObjecie
         /// </summary>
         /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
         /// <param name="skill">Ten GameObject skill</param>
         /// <param name="live_charStats">Live_charStats Castera</param>
         public static void ResourceTypeSelector_Out(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
         {
             switch (scrObj_Skill.skill_ResourceType)
             {
                 case ScrObj_skill.Skill_ResourceType.hp:
                     live_charStats.currentHP = skill.skill_currentResource; 
                     break;

                 case ScrObj_skill.Skill_ResourceType.mana:
                     live_charStats.currentMP = skill.skill_currentResource;
                     break;

                 case ScrObj_skill.Skill_ResourceType.stamina:
                     live_charStats.currentStam = skill.skill_currentResource;
                     break;
             }
         }*/

        /// <summary>
        /// Update Resource Type w zależności od ustawień w scriptableObjecie
        /// Zwraca (return) lokalny float danego Resource (ReadOnly), może być użyty do metody jako argument curentResource
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static float Skill_ResourceTypeCurrentFloatReadOnly(ScrObj_skill scrObj_Skill, CharacterStatus live_charStats)
        {
            float currentResource = new();

            switch (scrObj_Skill.skill_ResourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    currentResource = live_charStats.charStats._hp;
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    currentResource = live_charStats.charStats._mp;
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    currentResource = live_charStats.charStats._stam;
                    break;
            }

            return currentResource;
        }
        #endregion

        #region Skill_EffectValuesUpdate

        /*/// <summary> OLD CODE
        /// Update wartości Damage / Cost / Cooldown przed użyciem skilla (obliczanie z currentCharLevel i bonusCharStats )
        /// </summary>
        /// <param name="skill_currentDamage">Aktualny Damage skilla</param>
        /// <param name="skill_currentMPCost">Aktualny MP cost skilla</param>
        /// <param name="skill_currentCooldown">Aktualny Cooldown skilla</param>
        /// <param name="skill_BaseDamage">Bazowy Damage skilla</param>
        /// <param name="skill_BaseResourceCost">Bazowy Resource cost skilla</param>
        /// <param name="skill_BaseCooldown">Bazowy Cooldown skilla</param>
        /// <param name="skill_Multiplier">Multiplier do Levela i BonusCharStats do obliczeń [0.1f] -> na każdy lev i na każdy BonusDMG dodaje 0.1f BAZOWYCH DMG</param>
        /// <param name="currentCharLevel">aktualny Level do obliczeń</param>
        /// <param name="currentBonusSkillDamage">Aktualby bonus damage do skilla</param>
        public static void Skill_ValuesUpdate(float skill_currentDamage, float skill_currentMPCost, float skill_currentCooldown, float skill_BaseDamage, float skill_BaseResourceCost, float skill_BaseCooldown,
            float skill_Multiplier, int currentCharLevel, float currentBonusSkillDamage)
        {
            skill_currentDamage = skill_BaseDamage + (skill_BaseDamage * (currentCharLevel * skill_Multiplier)) + (skill_BaseDamage * (currentBonusSkillDamage * skill_Multiplier)); //+bonus
            skill_currentMPCost = skill_BaseResourceCost + (skill_BaseResourceCost * (currentCharLevel * skill_Multiplier));
            skill_currentCooldown = skill_BaseCooldown;
        }*/

        /// <summary>
        /// Update wartości Damage / Cost  przed użyciem skilla (obliczanie z currentCharLevel i bonusCharStats )
        /// Wykorzystany w skill_Effects
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_EffectValuesUpdate(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, CharacterBonusStats currentCharacterBonusStats)
        {
            skill.skill_currentDamage = scrObj_Skill.skill_BaseDamage + (scrObj_Skill.skill_BaseDamage * (live_charStats.charInfo._charLevel * scrObj_Skill.skill_Multiplier)) + (scrObj_Skill.skill_BaseDamage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_Skill.skill_Multiplier)); //+bonus
            skill.skill_currentResourceCost = scrObj_Skill.skill_BaseResourceCost + (scrObj_Skill.skill_BaseResourceCost * (live_charStats.charInfo._charLevel * scrObj_Skill.skill_Multiplier));

        }
        #endregion

        #region Skill_EveryFrameValuesUpdate

        /// <summary>
        /// Update wartości skilla używany poza iSCasting )
        /// Wykorzystany w FixedUpdate, musi być na samej górze FixedUpdate ponieważ resetuje IsCastingInstant
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_EveryFrameValuesUpdate(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, CharacterBonusStats currentCharacterBonusStats)
        {
            Skill_InputSelector(scrObj_Skill, skill, live_charStats);

            skill.skill_currentCooldownRemaining = Mathf.MoveTowards(skill.skill_currentCooldownRemaining, 0f, Time.deltaTime / scrObj_Skill.skill_BaseCooldown); //cooldown remaining spada do zera w czasie (1sek * 1/basecooldown ) czyli 30f -> 0f w 10sek
            if (skill.skill_currentCooldownRemaining <= 0.05f) skill.skill_currentComboProgress = Mathf.MoveTowards(skill.skill_currentComboProgress, 0f, Time.deltaTime / scrObj_Skill.skill_BaseCooldown );   //aktualny combo progress spada do zera w czasie (1sek * (1/basecooldown)) czyli 30f -> 0f w 30sek spada tylko jeśli nie jest na cooldownie

            if (skill.skill_currentCooldownRemaining >= 0.06f) { skill.skill_IsCastingInstant = false; } //Reset Instanta na początku każdej klatki jeśli jest na cooldown

        }
        #endregion

        #region Skill_Cone_DynamicCone

        /*/// <summary> OLD CODE
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
        public static void Skill_Cone_DynamicCone(bool isCasting, float skill_currentRadius, float skill_currentAngle, float skill_MinRadius, float skill_MaxRadius,
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
            Debug.Log(nameof(Skill_Cone_DynamicCone));
        }*/

        /// <summary>
        /// Mechanika Dynamicznego Range/Angle Cone
        /// <br><i>Dynamicznie skaluje zasięg i kąt Skilla</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_Cone_DynamicCone(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            if (live_charStats.charStatus._isCasting)
            {
                skill.skill_currentRadius = Mathf.SmoothDamp(skill.skill_currentRadius, scrObj_Skill.skill_MaxRadius, ref skill.skill_currentVectorRadius, scrObj_Skill.skill_TimeMaxRadius);
                //dynamiczny BreathCone radius -> ++ on input

                skill.skill_currentAngle = Mathf.SmoothDamp(skill.skill_currentAngle, scrObj_Skill.skill_MaxAngle, ref skill.skill_currentVectorAngle, scrObj_Skill.skill_TimeMaxAngle);
                //dynamiczny BreathCone Angle -> ++ on input
            }
            else
            {
                skill.skill_currentRadius = Mathf.SmoothDamp(skill.skill_currentRadius, scrObj_Skill.skill_MinRadius, ref skill.skill_currentVectorRadius, scrObj_Skill.skill_TimeMaxRadius);
                //dynamiczny BreathCone radius -> -- off input

                skill.skill_currentAngle = Mathf.SmoothDamp(skill.skill_currentAngle, scrObj_Skill.skill_MinAngle, ref skill.skill_currentVectorAngle, scrObj_Skill.skill_TimeMaxAngle);
                //dynamiczny BreathCone Angle -> -- off 
            }
        }
        #endregion

        #region Skill_Melee_DynamicCone

        /// <summary>
        /// Mechanika Dynamicznego Range/Angle Cone dla TargetType Melee
        /// <br><i>Dynamicznie skaluje zasięg i kąt Skilla</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_Melee_DynamicCone(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            skill.skill_currentRadius = Mathf.Lerp(scrObj_Skill.skill_MinRadius, scrObj_Skill.skill_MaxRadius, Mathf.InverseLerp(0f, 1.5f, skill.skill_currentComboProgress));

            skill.skill_currentAngle = Mathf.Lerp(scrObj_Skill.skill_MinAngle, scrObj_Skill.skill_MaxAngle, Mathf.InverseLerp(0f, 1.5f, skill.skill_currentComboProgress));

            /*if (live_charStats.isCasting)
            {
                skill.skill_currentRadius = Mathf.SmoothDamp(skill.skill_currentRadius, scrObj_Skill.skill_MaxRadius, ref skill.skill_currentVectorRadius, scrObj_Skill.skill_TimeMaxRadius);
                //dynamiczny BreathCone radius -> ++ on input

                skill.skill_currentAngle = Mathf.SmoothDamp(skill.skill_currentAngle, scrObj_Skill.skill_MaxAngle, ref skill.skill_currentVectorAngle, scrObj_Skill.skill_TimeMaxAngle);
                //dynamiczny BreathCone Angle -> ++ on input
            }
            else
            {
                skill.skill_currentRadius = Mathf.SmoothDamp(skill.skill_currentRadius, scrObj_Skill.skill_MinRadius, ref skill.skill_currentVectorRadius, scrObj_Skill.skill_TimeMaxRadius);
                //dynamiczny BreathCone radius -> -- off input

                skill.skill_currentAngle = Mathf.SmoothDamp(skill.skill_currentAngle, scrObj_Skill.skill_MinAngle, ref skill.skill_currentVectorAngle, scrObj_Skill.skill_TimeMaxAngle);
                //dynamiczny BreathCone Angle -> -- off 
            }*/
        }
        #endregion

        #region Skill_Target_ConeReset
        /// <summary>
        /// Resetuje Boole targetInRange/Angle oraz listę Colliderów
        /// </summary>
        /// <param name="skill">Ten GameObject skill</param>
        public static void Skill_Target_ConeReset(Skill skill)
        {
            skill.skill_targetInAngle = false;
            skill.skill_targetInRange = false;
            skill.skill_targetColliders.Clear();  //czyszczenie listy colliderów
        }
        #endregion

        #region Skill_ResetAnyCasting

        /// <summary>
        /// Reset wszystkich Static metod Casting
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param> 
        public static void Skill_ResetAnyCasting(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            if (skill.skill_CastingVisualEffect != null) skill.skill_CastingVisualEffect.Stop();
            if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, false);
            if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerName)) live_charStats.charComponents._Animator.ResetTrigger(scrObj_Skill.skill_AnimatorTriggerName);

            if (skill.skill_AudioSourceInstant != null)
            {
                skill.skill_AudioSourceInstant.volume = Mathf.MoveTowards(skill.skill_AudioSourceInstant.volume, 0, Time.deltaTime / 4); //obniza volume 1-> 0 w 4sek
                if (skill.skill_AudioSourceInstant.volume <= 0.05f) { skill.skill_AudioSourceInstant.Stop(); }
            }

            if (skill.skill_AudioSourceCastable != null)
            {
                skill.skill_AudioSourceCastable.volume = Mathf.MoveTowards(skill.skill_AudioSourceCastable.volume, 0, Time.deltaTime / 4); //obniza volume 1-> 0 w 4sek
                if (skill.skill_AudioSourceCastable.volume <= 0.05f) { skill.skill_AudioSourceCastable.Stop(); }
            }

            if (skill.skill_AudioSourceHold != null)
            {
                skill.skill_AudioSourceHold.volume = Mathf.MoveTowards(skill.skill_AudioSourceHold.volume, 0, Time.deltaTime / 4); //obniza volume 1-> 0 w 4sek
                if (skill.skill_AudioSourceHold.volume <= 0.05f) { skill.skill_AudioSourceHold.Stop(); }
            }

            live_charStats.charStatus._isCasting = false;

            skill.skill_IsCastingInstant = false;

            skill.skill_IsCastingHold = false;

            skill.skill_currentCastingProgress = 0f; //reset progressu przy przerwaniu casta / niespełnieniu warunków
            skill.skill_IsCastingFinishedCastable = false;

            Skill_Target_ConeReset(skill); //Reset TargetList
        }
        #endregion
        
        /// <summary>
        /// Natychmiastowo przerywa wszystkie Skill_AudioSource
        /// </summary>
        /// <param name="scrObj_Skill"></param>
        /// <param name="skill"></param>
        /// <param name="live_charStats"></param>
        public static void Skill_ResetAnyAudioSourceInstantly(Skill skill)
        {
            if (skill.skill_AudioSourceInstant != null)
            {
                skill.skill_AudioSourceInstant.Stop();
            }

            if (skill.skill_AudioSourceCastable != null)
            {
                skill.skill_AudioSourceCastable.Stop();
            }

            if (skill.skill_AudioSourceHold != null)
            {
                skill.skill_AudioSourceHold.Stop();
            }
        } 

        #region Skill_CastingTypeCurrentFloatReadOnly
        /// <summary>
        /// <br>Przyjmuje currentCastingIndex z arraya EffetType [i] oraz tego Skilla</br>
        /// <br>Zwraca Boola IsCastingType [FinishedCastable / Instant / Hold]</br>
        /// <br>Bool IsCastingType [FinishedCastable / Instant / Hold] przekazywany jest dalej do wybranej przez switcha Static metody z EffectType</br>
        /// <br>Metoda wykorzystana w "for" CastingTypeArray Skill script</br> 
        /// </summary>
        /// <param name="currentCastingIndex">currentCastingIndex z arraya EffetType [i]</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <returns></returns>
        public static bool Skill_CastingTypeCurrentFloatReadOnly(int currentCastingIndex, Skill skill)
        {
            bool currentCastingType = new();

            switch (currentCastingIndex)
            {
                case 0:
                    currentCastingType = skill.skill_IsCastingFinishedCastable;
                    break;

                case 1:
                    currentCastingType = skill.skill_IsCastingInstant;
                    break;

                case 2:
                    currentCastingType = skill.skill_IsCastingHold;
                    break;
            }
            return currentCastingType;
        }
        #endregion

        #region Skill_StopAllAnimatorMovementOnCast
        /// <summary>
        /// <br>>Klasa zatrzymująca cały movement postaci (Movement -> [yAnim = 0], ResetRrigger -> [Jump], Animator bool -> [false])</br
        /// <br>>Nazwy Zmiennych animatora pobierane ze scrObj_Skilla</br
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="live_charStats"></param>
        public static void Skill_StopAllAnimatorMovement(ScrObj_skill scrObj_Skill, CharacterStatus live_charStats)
        {
            if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_StopOnCastFloatNameAnimator)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill.skill_StopOnCastFloatNameAnimator, 0.2f);
            if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_StopOnCastTriggerNameAnimator)) live_charStats.charComponents._Animator.ResetTrigger(scrObj_Skill.skill_StopOnCastTriggerNameAnimator);
            if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_StopOnCastBoolNameAnimator)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_StopOnCastBoolNameAnimator, false);

        }
        #endregion
    }

    #endregion Utils

}


