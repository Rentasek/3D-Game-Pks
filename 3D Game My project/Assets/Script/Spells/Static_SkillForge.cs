using UnityEngine;

public static class Static_SkillForge
{
    #region CastingType
    ////////////////////////////////////////////////////
    /////////////////// Casting Type ///////////////////
    ////////////////////////////////////////////////////

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
        InputSelector(scrObj_Skill, skill, live_charStats);

        if (skill.skill_input && live_charStats.skill_CanCast && skill.skill_currentResource >= skill.skill_currentResourceCost)
        {
            if (!live_charStats.isCasting)
            {
                skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                skill.skill_AudioSource.PlayOneShot(scrObj_Skill.skill_CastingAudioClip, scrObj_Skill.skill_CastingAudioVolume);//clip audio    
            }
            live_charStats.isCasting = true;
            live_charStats.currentAnimator.SetBool(scrObj_Skill.skill_AnimatorBool, live_charStats.isCasting);
            skill.skill_currentCastingProgress = Mathf.Lerp(skill.skill_currentCastingProgress, 1f, Time.deltaTime / scrObj_Skill.skill_TimeCast);
            skill.skill_currentCastingFinished = (skill.skill_currentCastingProgress >= 1f) ? true : false;
            if (skill.skill_currentCastingFinished) skill.skill_currentCastingProgress = 0f; //reset progressu po wycastowaniu Skilla
        }
        else
        {
            skill.skill_CastingVisualEffect.Stop();
            live_charStats.isCasting = false;
            skill.skill_currentCastingProgress = 0f; //reset progressu przy przerwaniu casta / niespełnieniu warunków
            live_charStats.currentAnimator.SetBool(scrObj_Skill.skill_AnimatorBool, live_charStats.isCasting);
        }
    }

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
        InputSelector(scrObj_Skill, skill, live_charStats);

        if (skill.skill_input && live_charStats.skill_CanCast && skill.skill_currentResource >= skill.skill_currentResourceCost)
        {
            if (!live_charStats.isCasting)
            {
                skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                skill.skill_AudioSource.PlayOneShot(scrObj_Skill.skill_CastingAudioClip, scrObj_Skill.skill_CastingAudioVolume);//clip audio 
                live_charStats.currentAnimator.SetTrigger(scrObj_Skill.skill_AnimatorTrigger);
            }
            live_charStats.isCasting = true;
        }
        else
        {
            skill.skill_CastingVisualEffect.Stop();
            live_charStats.isCasting = false;
        }
        skill.skill_currentCastingInstant = live_charStats.isCasting;
    }

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
        InputSelector(scrObj_Skill, skill, live_charStats);

        if (skill.skill_input && live_charStats.skill_CanCast && skill.skill_currentResource >= skill.skill_currentResourceCost)
        {
            if (!live_charStats.isCasting)
            {
                skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                skill.skill_AudioSource.PlayOneShot(scrObj_Skill.skill_CastingAudioClip, scrObj_Skill.skill_CastingAudioVolume);//clip audio     
            }
            live_charStats.isCasting = true;
            live_charStats.currentAnimator.SetBool(scrObj_Skill.skill_AnimatorBool, live_charStats.isCasting);
        }
        else
        {
            skill.skill_CastingVisualEffect.Stop();
            live_charStats.isCasting = false;
            live_charStats.currentAnimator.SetBool(scrObj_Skill.skill_AnimatorBool, live_charStats.isCasting);
        }
        /*Debug.Log("InputCasting : "+ inputCasting);
        Debug.Log("skill_CanCast : " + skill_CanCast);
        Debug.Log("skill_currentResource : " + skill_currentResource);
        Debug.Log("skill_currentResourceCost : " + skill_currentResourceCost);*/
    }
    #endregion CastingType

    #region TargetType
    ///////////////////////////////////////////////////
    /////////////////// Target Type ///////////////////
    ///////////////////////////////////////////////////

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

    /// <summary>
    /// Szuka targetów w dynamic Cone Radius
    /// <br><i>Zwraca do Skill Objectu listę colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle) </i></br> 
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_Cone_AttackConeCheck(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
    {
        Skill_Cone_DynamicCone(scrObj_Skill, skill, live_charStats);

        if (live_charStats.isCasting)
        {
            for (int i = 0; i < Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius).Length; i++)
            {
                for (int j = 0; j < skill.skill_EnemiesArray.Length; j++)
                {
                    if (Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i].CompareTag(skill.skill_EnemiesArray[j]))
                    {
                        skill.skill_targetInRange = true; //target jest w breath range

                        Vector3 directionToTarget = (Physics.OverlapSphere(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius)[i].transform.position - skill.skill_casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector wyrażony w radianach
                                                                                                                                                                                                              //sprawdzanie aktualnie ostatniego elementu z listy
                        if (Vector3.Angle(skill.skill_casterGameobject.transform.forward, directionToTarget) < skill.skill_currentAngle / 2)
                        //sprawdzanie angle wektora forward charactera i direction to target
                        //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                        {
                            skill.skill_targetInAngle = true;
                            if (!Physics.Raycast(skill.skill_casterGameobject.transform.position, directionToTarget, skill.skill_currentRadius, scrObj_Skill.skill_ObstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                            {
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
            skill.skill_targetInAngle = false;
            skill.skill_targetInRange = false;
            skill.skill_targetColliders.Clear();  //czyszczenie listy colliderów
        }
    }

    #endregion TargetType

    #region EffectType

    ///////////////////////////////////////////////////
    /////////////////// Effect Type ///////////////////
    ///////////////////////////////////////////////////

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
    }*/


    /// <summary>
    /// DamageOverTime (MoveTowards)- Target Colliderów na Collider List
    /// </summary>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_TargetList_DamageOverTime(Skill skill, CharacterStatus live_charStats)
    {
        if (live_charStats.isCasting)
        {
            for (int i = 0; i < skill.skill_targetColliders.Count; i++)
            {
                skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP = Mathf.MoveTowards(skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP, -skill.skill_currentDamage, skill.skill_currentDamage * Time.deltaTime);  // DMG / Sek

                if (skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentHP <= 0f && !skill.skill_targetColliders[i].GetComponent<CharacterStatus>().isDead)

                {
                    live_charStats.currentXP += skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentXP_GainedFromKill;
                    if (skill.skill_targetColliders[i].CompareTag("Monster")) { skill.skill_targetColliders[i].GetComponent<CharacterStatus>().currentCharLevel = Random.Range(live_charStats.currentCharLevel - 3, live_charStats.currentCharLevel + 3); }  //po śmierci ustawia level targetu na zbliżony do atakującego
                    //podbija lvl tylko Monsterów, Playera i Environment nie
                }
            }
            skill.skill_currentResource = Mathf.MoveTowards(skill.skill_currentResource, - skill.skill_currentResourceCost, skill.skill_currentResourceCost * Time.deltaTime);    // MPCost / Sek
        }
    }

    #endregion EffectType

    #region Utils
    /////////////////////////////////////////////
    /////////////////// UTILS ///////////////////
    /////////////////////////////////////////////

    /// <summary>
    /// Pobiera EnemiesArray i dokłada Destructibles jako cel skilla
    /// <br><i>Może być wykorzystany currentEnemiesArray z live_CharStats</i></br>
    /// </summary>
    /// <param name="enemiesArray">Pobierany EnemiesArray z FieldOfView</param>
    /// <returns></returns>
    public static string[] EnemyArraySelector(string[] enemiesArray)
    {
        string[] skill_EnemiesArray = new string[enemiesArray.Length + 1];              //tworzy array +1 od current enemies arraya
        enemiesArray.CopyTo(skill_EnemiesArray, 0);                                     //kopiuje current enemies arraya od indexu 0
        skill_EnemiesArray[skill_EnemiesArray.Length - 1] = "Destructibles";            //wstawia jako ostatni index Destructibles żeby zawsze można było go zniszczyć
        return (skill_EnemiesArray);
    }

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
    }*/

    /// <summary>
    /// Update skill_input i skill_otherInput w zależności od ustawień w scriptableObjecie
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void InputSelector(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
    {
        switch (scrObj_Skill.skill_InputCastingType)
        {
            case ScrObj_skill.Skill_InputCastingType.primary:
                skill.skill_input = live_charStats.inputPrimary;
                skill.skill_otherInput = live_charStats.inputSecondary;
                break;
            case ScrObj_skill.Skill_InputCastingType.secondary:
                skill.skill_input = live_charStats.inputSecondary;
                skill.skill_otherInput = live_charStats.inputPrimary;
                break;
        }
    }

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

    /// <summary>
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
    }

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
    /// Update wartości Damage / Cost / Cooldown przed użyciem skilla (obliczanie z currentCharLevel i bonusCharStats )
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    public static void Skill_ValuesUpdate(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, CharacterBonusStats currentCharacterBonusStats)
    {
        skill.skill_currentDamage = scrObj_Skill.skill_BaseDamage + (scrObj_Skill.skill_BaseDamage * (live_charStats.currentCharLevel * scrObj_Skill.skill_Multiplier)) + (scrObj_Skill.skill_BaseDamage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_Skill.skill_Multiplier)); //+bonus
        skill.skill_currentResourceCost = scrObj_Skill.skill_BaseResourceCost + (scrObj_Skill.skill_BaseResourceCost * (live_charStats.currentCharLevel * scrObj_Skill.skill_Multiplier));
        skill.skill_currentCooldown = scrObj_Skill.skill_BaseCooldown;
    }

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
        if (live_charStats.isCasting)
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


    #endregion Utils
}


