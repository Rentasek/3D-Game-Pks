using System;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Diagnostics;

public static class SkillForge
{
    #region CastingType
    public static class CastingType
    {
        ////////////////////////////////////////////////////
        /////////////////// Casting Type ///////////////////
        ////////////////////////////////////////////////////

        #region CastingInstant
        /// <summary>
        /// Metoda odpowiedzialna za VFX, animacje Animatora, Audio i odpalenie TargetType[i] skilla        
        /// <br><i>Rodzaj castowania -> <b>Instant</b> działa w momencie użycia (ograniczone cooldownem)</i></br>        
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param>
        public static void CastingInstant(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            if (Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) > 0)
            {
                if (Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) > skill._resourceCost)   //Jeśli nie castuje i ma właśnie zacząć //Start Casting
                {                    
                    if (skill._currentCooldownRemaining <= 0.05f)  //Jeśli zostało 0.05f lub mniej cooldownu może użyć instanta
                    {
                        if (skill._castingVisualEffect != null) skill._castingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown

                        Utils.Skill_StopAllAnimatorMovement(scrObj_Skill, live_charStats);

                        for (int targetTypeIndex = 0; targetTypeIndex < scrObj_Skill._targetTypes.Length; targetTypeIndex++)
                        {
                            switch (scrObj_Skill._targetTypes[targetTypeIndex]._targetType)
                            {
                                case ScrObj_skill.TargetType.None:
                                    break;
                                case ScrObj_skill.TargetType.ConeOnCombo:
                                    TargetType.Skill_Melee_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                    break;
                                case ScrObj_skill.TargetType.ConeOnTime:
                                    TargetType.Skill_Cone_Target(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                    break;
                                case ScrObj_skill.TargetType.Projectile:
                                    break;
                                case ScrObj_skill.TargetType.AreaOfEffectMouse:
                                    break;
                                case ScrObj_skill.TargetType.Self:
                                    TargetType.Skill_Self_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                    break;
                                case ScrObj_skill.TargetType.Pierce:
                                    break;
                                case ScrObj_skill.TargetType.Chain:
                                    break;
                                case ScrObj_skill.TargetType.Boom:
                                    break;                                    
                            }                            
                        }

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill._animatorBoolName, true);
                        if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorTriggerName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill._animatorTriggerName);

                        if (scrObj_Skill._casterAudioClip != null)
                        {
                            skill._audioSourceCaster.volume = scrObj_Skill._casterAudioVolume;
                            skill._audioSourceCaster.PlayOneShot(scrObj_Skill._casterAudioClip, scrObj_Skill._casterAudioVolume);
                        }

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill._animatorFloatName, skill._currentComboProgress); //przed updatem comboProgress
                        skill._currentComboProgress += 0.5f;
                        if (skill._currentComboProgress >= 1) skill._currentComboProgress = 0f;

                        skill._currentCooldownRemaining = 1f; //Ustawia cooldown czyli IsCastingInstant wychodzi z true (raz) ale przy następnej klatce już nie wejdzie do if bo ma cooldown
                          
                        
                    } 
                    live_charStats.charStatus._isCasting = true;
                }
                else
                {
                    Utils.Skill_ResetCastingAndVFXAnimsAudio(scrObj_Skill, skill, live_charStats);
                    //Utils.Skill_ResetTargetList(scrObj_Skill, skill, live_charStats, targetTypeIndex); //Reset TargetList w targetTypeIndexie
                }
            }
            else
            {
                Utils.Skill_ResetCastingAndVFXAnimsAudio(scrObj_Skill, skill, live_charStats);
                //Utils.Skill_ResetTargetList(scrObj_Skill, skill, live_charStats, targetTypeIndex); //Reset TargetList w targetTypeIndexie
            }
        }
        #endregion

        #region CastingHold
        /// <summary>
        /// Metoda odpowiedzialna za VFX, animacje Animatora, Audio i odpalenie TargetType[i] skilla       
        /// <br><i>Rodzaj castowania -> <b>Hold</b> dziala od razu (przytrzymaj)</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param>
        public static void CastingHold(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            if (Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) > 0)
            {
                if (Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) > skill._resourceCost)   //Jeśli nie castuje i ma właśnie zacząć //Start Casting
                {
                    if (skill._currentCooldownRemaining <= 0.05f)  //Jeśli zostało 0.05f lub mniej cooldownu może użyć instanta
                    {
                        if (skill._castingVisualEffect != null) skill._castingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown
                        
                        Utils.Skill_StopAllAnimatorMovement(scrObj_Skill, live_charStats);

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill._animatorBoolName, true);
                        if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorTriggerName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill._animatorTriggerName);
                        
                        skill._currentCooldownRemaining = 1f; //Ustawia cooldown czyli IsCastingInstant wychodzi z true (raz) ale przy następnej klatce już nie wejdzie do if bo ma cooldown

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill._animatorFloatName, skill._currentComboProgress); //przed updatem comboProgress
                        skill._currentComboProgress += 0.5f;
                        if (skill._currentComboProgress >= 1) skill._currentComboProgress = 0f;

                        #region AudioClipy //Specjalnie puszczane przy instant ale po animatorze
                        if (scrObj_Skill._casterAudioClip != null && !skill._audioSourceCaster.isPlaying)
                        {
                            skill._audioSourceCaster.volume = scrObj_Skill._casterAudioVolume;
                            skill._audioSourceCaster.clip = scrObj_Skill._casterAudioClip;
                            skill._audioSourceCaster.PlayScheduled(scrObj_Skill._timeCast);                            
                        }
                        #endregion
                    }
                    live_charStats.charStatus._isCasting = true;
                }                

                for (int targetTypeIndex = 0; targetTypeIndex < scrObj_Skill._targetTypes.Length; targetTypeIndex++)
                {
                    switch (scrObj_Skill._targetTypes[targetTypeIndex]._targetType)
                    {
                        case ScrObj_skill.TargetType.None:
                            break;
                        case ScrObj_skill.TargetType.ConeOnCombo:
                            TargetType.Skill_Melee_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                            break;
                        case ScrObj_skill.TargetType.ConeOnTime:
                            TargetType.Skill_Cone_Target(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                            break;
                        case ScrObj_skill.TargetType.Projectile:
                            break;
                        case ScrObj_skill.TargetType.AreaOfEffectMouse:
                            break;
                        case ScrObj_skill.TargetType.Self:
                            TargetType.Skill_Self_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                            break;
                        case ScrObj_skill.TargetType.Pierce:
                            break;
                        case ScrObj_skill.TargetType.Chain:
                            break;
                        case ScrObj_skill.TargetType.Boom:
                            break;
                    }
                }
            }
            else
            {
                Utils.Skill_ResetCastingAndVFXAnimsAudio(scrObj_Skill, skill, live_charStats);                           
            }
            
        }
        #endregion

        #region CastingCastable
        /// <summary>
        /// Metoda odpowiedzialna za VFX, animacje Animatora, Audio i odpalenie TargetType[i] skilla       
        /// <br><i>Rodzaj castowania -> <b>Castable</b> (przytrzymaj i poczekaj aż wystrzeli)</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param>
        public static void CastingCastable(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            if (Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) > 0)
            {
                if (Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) > skill._resourceCost)   //Jeśli nie castuje i ma właśnie zacząć //Start Casting
                {
                    if (skill._currentCooldownRemaining <= 0.05f)  //Jeśli zostało 0.05f lub mniej cooldownu może użyć instanta
                    {
                        if (skill._castingVisualEffect != null) skill._castingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown

                        Utils.Skill_StopAllAnimatorMovement(scrObj_Skill, live_charStats);

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill._animatorBoolName, true);
                        if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorTriggerName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill._animatorTriggerName);

                        skill._currentCooldownRemaining = 1f; //Ustawia cooldown czyli IsCastingInstant wychodzi z true (raz) ale przy następnej klatce już nie wejdzie do if bo ma cooldown

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill._animatorFloatName, skill._currentComboProgress); //przed updatem comboProgress
                        skill._currentComboProgress += 0.5f;
                        if (skill._currentComboProgress >= 1) skill._currentComboProgress = 0f;

                        #region AudioClipy //Specjalnie puszczane przy instant ale po animatorze
                        if (scrObj_Skill._casterAudioClip != null)
                        {
                            skill._audioSourceCaster.volume = scrObj_Skill._casterAudioVolume;
                            skill._audioSourceCaster.PlayOneShot(scrObj_Skill._casterAudioClip, scrObj_Skill._casterAudioVolume);
                        }
                        #endregion
                    }
                    live_charStats.charStatus._isCasting = true;
                }               
               
                skill._currentCastingProgress = Mathf.MoveTowards(skill._currentCastingProgress, 1f, Time.deltaTime / scrObj_Skill._timeCast); //casting progress rośnie do 1 w (1sek * 1/TimeCast ) czyli (sek * "n" [np.0.5f] = "n" część sekundy)
                //skill.skill_IsCastingFinishedCastable = (skill.skill_currentCastingProgress >= 0.95f);
                if (skill._currentCastingProgress >= 0.95f)
                {
                    Utils.Skill_ResetVFXAnimsAudio(scrObj_Skill, skill, live_charStats); //Reset Audio / VFX / Animator po wycastowaniu ale przed Triggerem OnFinished

                    //Odpalenie skilla po skończeniu castowania
                    for (int targetTypeIndex = 0; targetTypeIndex < scrObj_Skill._targetTypes.Length; targetTypeIndex++)
                    {
                        switch (scrObj_Skill._targetTypes[targetTypeIndex]._targetType)
                        {
                            case ScrObj_skill.TargetType.None:
                                break;
                            case ScrObj_skill.TargetType.ConeOnCombo:
                                TargetType.Skill_Melee_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                break;
                            case ScrObj_skill.TargetType.ConeOnTime:
                                TargetType.Skill_Cone_Target(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                break;
                            case ScrObj_skill.TargetType.Projectile:
                                break;
                            case ScrObj_skill.TargetType.AreaOfEffectMouse:
                                break;
                            case ScrObj_skill.TargetType.Self:
                                TargetType.Skill_Self_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                break;
                            case ScrObj_skill.TargetType.Pierce:
                                break;
                            case ScrObj_skill.TargetType.Chain:
                                break;
                            case ScrObj_skill.TargetType.Boom:
                                break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorTriggerOnFinishedCastingName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill._animatorTriggerOnFinishedCastingName);  //Animacja po wykonaniu casta                                           
                    
                    if (scrObj_Skill._casterAudioClip != null)
                    {
                        skill._audioSourceCaster.volume = scrObj_Skill._casterAudioVolume;
                        skill._audioSourceCaster.PlayOneShot(scrObj_Skill._onFinishCastingAudioClip, scrObj_Skill._casterAudioVolume);
                    }

                    skill._currentCastingProgress = 0f;    //reset progressu po wycastowaniu Skilla             
                    skill._currentCooldownRemaining = 1f;  //Ustawia cooldown po wycastowaniu żeby nie castował znów zbyt szybko, przydatnie jeśli mamy animację po wystrzeleniu
                }
               
            }
            else
            {
                Utils.Skill_ResetCastingAndVFXAnimsAudio(scrObj_Skill, skill, live_charStats);
                //Utils.Skill_ResetTargetList(scrObj_Skill, skill, live_charStats, targetTypeIndex); //Reset TargetList w targetTypeIndexie
            }

        }
        #endregion
    }
    #endregion CastingType

    #region TargetType
    public static class TargetType
    {
        ///////////////////////////////////////////////////
        /////////////////// Target Type ///////////////////
        ///////////////////////////////////////////////////

        #region Skill_Cone_Target         
        /// <summary>nowy OverlapSphereNonAloc ma swoje plusy bo nie tworzy nowego array przy kazdym Cast ale ogólnie niewiele zmienia a trzeba robić dodatkową tablicę niedynamiczną
        /// Szuka targetów w dynamic Cone Radius
        /// <br><i>Zwraca do Skill Objectu listę colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle) </i></br> 
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param> 
        public static void Skill_Cone_Target(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex)
        {
            Utils.Skill_DynamicCone_Update(scrObj_Skill, skill, live_charStats, targetTypeIndex);           

            for (int i = 0; i < Physics.OverlapSphereNonAlloc(skill._casterGameobject.transform.position, scrObj_Skill._skillMaxRadius, skill._allLocalColliders); i++)
            { // trzeba zrobić tak żeby physics.overlapa używał tylko 1raz dla wszystkich casting types, a później z listy wyselekcjonować według kryteriów w targetTypach      
                for (int j = 0; j < skill._enemiesArray.Length; j++)
                {   
                    if (skill._allLocalColliders[i].CompareTag(skill._enemiesArray[j]))
                    {
                        float distanceToTarget = Vector3.Distance(skill._allLocalColliders[i].transform.position, skill._casterGameobject.transform.position);

                        if (distanceToTarget <= skill.targetDynamicValues[targetTypeIndex]._currentRadius)
                        {
                            skill.targetDynamicValues[targetTypeIndex]._targetInRange = true; //target jest w breath range

                            Vector3 directionToTarget = (skill._allLocalColliders[i].transform.position - skill._casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector (kierunek w którym od niego znajduje się target)
                                                                                                                                                                       //sprawdzanie aktualnie ostatniego elementu z listy
                            if (Vector3.Angle(skill._casterGameobject.transform.forward, directionToTarget) < skill.targetDynamicValues[targetTypeIndex]._currentAngle / 2)
                            //sprawdzanie angle wektora forward charactera i direction to target
                            //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                            {
                                if (!Physics.Raycast(skill._casterGameobject.transform.position, directionToTarget, distanceToTarget, scrObj_Skill._targetTypes[targetTypeIndex]._obstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                                {
                                    skill.targetDynamicValues[targetTypeIndex]._targetInAngle = true;

                                    if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.IndexOf(skill._allLocalColliders[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                    {
                                        skill.targetDynamicValues[targetTypeIndex]._targetColliders.Add(skill._allLocalColliders[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies                                    
                                    }
                                    else
                                    {
                                        skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill._allLocalColliders[i]);
                                        if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0) skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;  //jeśli nie ma żadnych targetów w Cone Angle
                                    }
                                }
                                else
                                {
                                    skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill._allLocalColliders[i]);
                                    if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0) skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;  //jeśli nie ma żadnych targetów w Cone Angle
                                }
                            }
                            else
                            {
                                skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill._allLocalColliders[i]);
                                if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0) skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;  //jeśli nie ma żadnych targetów w Cone Angle 
                            }
                        }
                        else
                        {
                            skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill._allLocalColliders[i]);
                            if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0) 
                            {
                                skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;  //jeśli nie ma żadnych targetów w Cone Angle
                                skill.targetDynamicValues[targetTypeIndex]._targetInRange = false;  //jeśli nie ma żadnych targetów w Cone Radius
                            } 
                        }
                    }
                }
            }
            
            if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count > 0)
            {
                for (int effectTypeIndex = 0; effectTypeIndex < scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes.Length; effectTypeIndex++)
                {
                    switch (scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._effectType)
                    {
                        case ScrObj_skill.EffectType.None:
                            break;

                        case ScrObj_skill.EffectType.Hit:
                            EffectType.Skill_Hit(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.DamageOverTime:
                            EffectType.Skill_DamageOverTime(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.Heal:
                            EffectType.Skill_Heal(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.HealOverTime:
                            EffectType.Skill_HealOverTime(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.Summon:
                            break;
                    }
                }
            }
        }    
        #endregion

        #region Skill_Melee_Target
        /// <summary>
        /// Szuka targetów w Melee Radius
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// <br><i>Zwraca do Skill Objectu listę colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle) </i></br> 
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param> 
        public static void Skill_Melee_Target_new(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex)
        {
            Utils.Skill_OnCombo_DynamicCone(scrObj_Skill, skill, live_charStats, targetTypeIndex);

            for (int i = 0; i < Physics.OverlapSphereNonAlloc(skill._casterGameobject.transform.position, scrObj_Skill._skillMaxRadius, skill._allLocalColliders); i++)
            {    
                for (int j = 0; j < skill._enemiesArray.Length; j++)
                {
                    if (skill._allLocalColliders[i].CompareTag(skill._enemiesArray[j]))
                    {
                        float distanceToTarget = Vector3.Distance(skill._allLocalColliders[i].transform.position, skill._casterGameobject.transform.position);

                        if (distanceToTarget <= skill.targetDynamicValues[targetTypeIndex]._currentRadius)
                        {
                            skill.targetDynamicValues[targetTypeIndex]._targetInRange = true; //target jest w breath range

                            Vector3 directionToTarget = (skill._allLocalColliders[i].transform.position - skill._casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector (kierunek w którym od niego znajduje się target)
                                                                                                                                                                       //sprawdzanie aktualnie ostatniego elementu z listy
                            if (Vector3.Angle(skill._casterGameobject.transform.forward, directionToTarget) < skill.targetDynamicValues[targetTypeIndex]._currentAngle / 2)
                            //sprawdzanie angle wektora forward charactera i direction to target
                            //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                            {
                                if (!Physics.Raycast(skill._casterGameobject.transform.position, directionToTarget, distanceToTarget, scrObj_Skill._targetTypes[targetTypeIndex]._obstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                                {
                                    skill.targetDynamicValues[targetTypeIndex]._targetInAngle = true;

                                    if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.IndexOf(skill._allLocalColliders[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                    {
                                        skill.targetDynamicValues[targetTypeIndex]._targetColliders.Add(skill._allLocalColliders[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies                                    
                                    }
                                    else
                                    {
                                        skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill._allLocalColliders[i]);
                                        if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0) skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;  //jeśli nie ma żadnych targetów w Cone Angle
                                    }
                                }
                                else
                                {
                                    skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill._allLocalColliders[i]);
                                    if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0) skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;  //jeśli nie ma żadnych targetów w Cone Angle
                                }
                            }
                            else
                            {
                                skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill._allLocalColliders[i]);
                                if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0) skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;  //jeśli nie ma żadnych targetów w Cone Angle 
                            }
                        }
                        else
                        {
                            skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill._allLocalColliders[i]);
                            if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0)
                            {
                                skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;  //jeśli nie ma żadnych targetów w Cone Angle
                                skill.targetDynamicValues[targetTypeIndex]._targetInRange = false;  //jeśli nie ma żadnych targetów w Cone Radius
                            }
                        }
                    }
                }
            }

            if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count > 0)
            {
                for (int effectTypeIndex = 0; effectTypeIndex < scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes.Length; effectTypeIndex++)
                {
                    switch (scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._effectType)
                    {
                        case ScrObj_skill.EffectType.None:
                            break;

                        case ScrObj_skill.EffectType.Hit:
                            EffectType.Skill_Hit(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.DamageOverTime:
                            EffectType.Skill_DamageOverTime(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.Heal:
                            EffectType.Skill_Heal(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.HealOverTime:
                            EffectType.Skill_HealOverTime(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.Summon:
                            break;
                    }
                }
            }
        }
        #endregion

        #region Skill_Collider_Target
        /// <summary>
        /// Szuka targetów w ColliderRange -> OnTriggerEnter  
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// <br><i>Zwraca do Skill Objectu listę colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle) </i></br> 
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param> 
        public static void Skill_Collider_Target(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex)
        {
            /// trzeba zrobić prefaba z colliderem i skryptem OnTriggerEnter
            /// na OntriggerEnter powinien wrzucać targety do target list i odpalać isCastingType
            /// wszystko powinno działać na on triggerEnter żeby nie spamowało przy przelatywaniu przez target
            /// trzeba zrobić tak żeby po uderzeniu target znikał z listy targetów czyli:
            /// OnTriggerEnter -> wrzuca colissionTarget na targetList, odpala isCastingType i po 1 klatce wywala z targetList 
            /// trzeba jeszce ustalić limit uderzeń jakie prefab może przyjąć aż sie nie zneutralizuje
            /// Można OnTrigerEnter zrobić: OnTriggerEnter -> WYRZUCA wszystkie targety z colliderList!! targetList.Clear(); -> potem wrzuca collisiontarget na targetList, odpala isCastingType i po 1 klatce wywala z targetList 



            /// dla hold -> będzie wysyłał prefaba co chwilę (nie może być non stop bo bezie za dużo) trzeba jakiś delay ustawić dla hold też bedzie odpalał Hold (takie przedłużenie po tym jak postać przestanie inputować)
            /// dla instant -> poleci 1 partical prefab i odpali instantCasting jak trafi
            /// dla castable -> prefab odpali na CastingFinished
            /// można też zrobić alternatywne booleany tylko do wypuszczania prefaba ale po co?
            /// 
        }
        #endregion

        #region Skill_Self_Target
        /// <summary>
        /// Self target
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// <br><i>Zwraca do Skill Objectu listę colliderów [1] element zgodny z parametrami(Self) </i></br> 
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param> 
        public static void Skill_Self_Target_new(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex)
        {
            if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.IndexOf(live_charStats.gameObject.GetComponent<Collider>()) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders.Add(live_charStats.gameObject.GetComponent<Collider>());
                skill.targetDynamicValues[targetTypeIndex]._targetInAngle = true;
                skill.targetDynamicValues[targetTypeIndex]._targetInRange = true;   
            }
            else
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(live_charStats.gameObject.GetComponent<Collider>());
                if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0)
                {
                    skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;
                    skill.targetDynamicValues[targetTypeIndex]._targetInRange = false;
                } //jeśli nie ma żadnych targetów w Cone
            }

            if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count > 0)
            {
                for (int effectTypeIndex = 0; effectTypeIndex < scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes.Length; effectTypeIndex++)
                {
                    switch (scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._effectType)
                    {
                        case ScrObj_skill.EffectType.None:
                            break;

                        case ScrObj_skill.EffectType.Hit:
                            EffectType.Skill_Hit(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.DamageOverTime:
                            EffectType.Skill_DamageOverTime(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.Heal:
                            EffectType.Skill_Heal(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.HealOverTime:
                            EffectType.Skill_HealOverTime(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.EffectType.Summon:
                            break;
                    }
                }
            }
        }
        #endregion

        
    }
    #endregion TargetType

    #region EffectType
    public static class EffectType
    {
        ///////////////////////////////////////////////////
        /////////////////// Effect Type ///////////////////
        ///////////////////////////////////////////////////

        #region Skill_DamageOverTime
        /// <summary>
        /// DamageOverTime (MoveTowards)- Target Colliderów na Collider List
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param> 
        public static void Skill_DamageOverTime(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex, int effectTypeIndex)
        {
            Utils.Skill_EffectValuesUpdate(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats, targetTypeIndex, effectTypeIndex); //nie chce mi się dopisywać linka do CharacterBonusStats :P

            for (int i = 0; i < skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count; i++)
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders[i].GetComponent<CharacterStatus>().TakeDamageOverTime(skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage, live_charStats);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill._resourceType)
            {
                case ScrObj_skill.ResourceType.health:
                    live_charStats.charStats._hp = Mathf.MoveTowards(live_charStats.charStats._hp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.ResourceType.mana:
                    live_charStats.charStats._mp = Mathf.MoveTowards(live_charStats.charStats._mp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.ResourceType.stamina:
                    live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, -skill._resourceCost, skill._resourceCost * Time.deltaTime);     // ResourceCost / Sek 
                    break;
            }
        }
        #endregion        

        #region Skill_Hit
        /// <summary>
        /// Hit Instant - Target Colliderów na Collider List
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param> 
        public static void Skill_Hit(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex, int effectTypeIndex)
        {
            Utils.Skill_EffectValuesUpdate(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats, targetTypeIndex, effectTypeIndex); //nie chce mi się dopisywać linka do CharacterBonusStats :P

            for (int i = 0; i < skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count; i++)
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders[i].GetComponent<CharacterStatus>().TakeDamgeInstant(skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage, live_charStats);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill._resourceType)
            {
                case ScrObj_skill.ResourceType.health:
                    live_charStats.charStats._hp -= skill._resourceCost; // ResourceCost Instant
                    break;

                case ScrObj_skill.ResourceType.mana:
                    live_charStats.charStats._mp -= skill._resourceCost;  // ResourceCost Instant
                    break;

                case ScrObj_skill.ResourceType.stamina:
                    live_charStats.charStats._stam -= skill._resourceCost; // ResourceCost Instant
                    break;
            }
        }
        #endregion
        
        #region Skill_HealOverTime
        /// <summary>
        /// HealOverTime (MoveTowards)- Target Colliderów na Collider List
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param>  
        public static void Skill_HealOverTime(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex, int effectTypeIndex)
        {
            Utils.Skill_EffectValuesUpdate(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats, targetTypeIndex, effectTypeIndex); //nie chce mi się dopisywać linka do CharacterBonusStats :P

            for (int i = 0; i < skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count; i++)
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders[i].GetComponent<CharacterStatus>().TakeHealOverTime(skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill._resourceType)
            {
                case ScrObj_skill.ResourceType.health:
                    live_charStats.charStats._hp = Mathf.MoveTowards(live_charStats.charStats._hp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.ResourceType.mana:
                    live_charStats.charStats._mp = Mathf.MoveTowards(live_charStats.charStats._mp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.ResourceType.stamina:
                    live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, -skill._resourceCost, skill._resourceCost * Time.deltaTime);     // ResourceCost / Sek 
                    break;
            }
        }
        #endregion              
              
        #region Skill_Heal
        /// <summary>
        /// Heal Instant - Target Colliderów na Collider List
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param>          
        public static void Skill_Heal(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex, int effectTypeIndex)
        {
            Utils.Skill_EffectValuesUpdate(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats, targetTypeIndex, effectTypeIndex); //nie chce mi się dopisywać linka do CharacterBonusStats :P

            for (int i = 0; i < skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count; i++)
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders[i].GetComponent<CharacterStatus>().TakeHealInstant(skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill._resourceType)
            {
                case ScrObj_skill.ResourceType.health:
                    live_charStats.charStats._hp -= skill._resourceCost; // ResourceCost Instant
                    break;

                case ScrObj_skill.ResourceType.mana:
                    live_charStats.charStats._mp -= skill._resourceCost;  // ResourceCost Instant
                    break;

                case ScrObj_skill.ResourceType.stamina:
                    live_charStats.charStats._stam -= skill._resourceCost; // ResourceCost Instant
                    break;
            }
        }
        #endregion

    }
    #endregion

    #region Utils
    public static class Utils
    {
        /////////////////////////////////////////////
        /////////////////// UTILS ///////////////////
        /////////////////////////////////////////////

        #region Skill_EnemyArraySelector        
        /// <summary>
        /// <br>Pobiera EnemiesArray z live_charStats, dokłada Destructibles jako cel skilla</br>
        /// <br>Wypełnia skill_EnemiesArray jako ref objectu, czyli zwraca bezpośredio do SkillObjectu nie trzeba przypisywać</br>
        /// </summary>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_EnemyArraySelector(Skill skill, CharacterStatus live_charStats)
        {
            skill._enemiesArray = new string[live_charStats.charInfo._enemiesArray.Length + 1];              //tworzy array +1 od current enemies arraya
            live_charStats.charInfo._enemiesArray.CopyTo(skill._enemiesArray, 0);                                     //kopiuje current enemies arraya od indexu 0
            skill._enemiesArray[skill._enemiesArray.Length - 1] = "Destructibles";                                  //wstawia jako ostatni index Destructibles żeby zawsze można było go zniszczyć           
        }
        #endregion

        #region Skill_InputSelector
        /// <summary>
        /// Update skill_input i skill_otherInput w zależności od ustawień w scriptableObjecie
        /// Użyty w skill_Castings
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_InputSelector(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            switch (scrObj_Skill._inputType)
            {
                case ScrObj_skill.InputType.primary:
                    skill._skillInput = live_charStats.charInput._primary;
                    skill._skillOtherInput = live_charStats.charInput._secondary;
                    break;
                case ScrObj_skill.InputType.secondary:
                    skill._skillInput = live_charStats.charInput._secondary;
                    skill._skillOtherInput = live_charStats.charInput._primary;
                    break;
            }
        }
        #endregion

        #region Skill_ResourceTypeCurrentFloatReadOnly
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

            switch (scrObj_Skill._resourceType)
            {
                case ScrObj_skill.ResourceType.health:
                    currentResource = live_charStats.charStats._hp;
                    break;

                case ScrObj_skill.ResourceType.mana:
                    currentResource = live_charStats.charStats._mp;
                    break;

                case ScrObj_skill.ResourceType.stamina:
                    currentResource = live_charStats.charStats._stam;
                    break;
            }

            return currentResource;
        }
        #endregion       

        #region Skill_EffectValuesUpdate
        /// <summary>
        /// Update wartości Damage / Cost  przed użyciem skilla (obliczanie z currentCharLevel i bonusCharStats )
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// Wykorzystany w skill_Effects
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_EffectValuesUpdate(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, CharacterBonusStats currentCharacterBonusStats, int targetTypeIndex, int effectTypeIndex)
        {
            if (skill == live_charStats.fov._spellRangeSkill)
            {
                skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage = scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._baseDamage + (scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._baseDamage * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier)) + (scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._baseDamage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_Skill._multiplier)); //+bonus
                skill._resourceCost = scrObj_Skill._baseResourceCost + (scrObj_Skill._baseResourceCost * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier));
            }
            else if (skill == live_charStats.fov._closeRangeSkill)
            {
                skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage = scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._baseDamage + (scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._baseDamage * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier)) + (scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._baseDamage * (currentCharacterBonusStats.bonus_currentDamageCombo * scrObj_Skill._multiplier)); //+bonus
                skill._resourceCost = scrObj_Skill._baseResourceCost + (scrObj_Skill._baseResourceCost * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier));
            }
        }
        #endregion

        #region Skill_EveryFrameValuesUpdate    
        /// <summary>
        /// Update wartości skilla używany poza iSCasting 
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// Wykorzystany w FixedUpdate, musi być na samej górze FixedUpdate ponieważ resetuje IsCasting
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_EveryFrameValuesUpdate_new(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, CharacterBonusStats currentCharacterBonusStats)
        {
            Skill_InputSelector(scrObj_Skill, skill, live_charStats);

            skill._currentCooldownRemaining = Mathf.MoveTowards(skill._currentCooldownRemaining, 0f, Time.deltaTime / scrObj_Skill._baseCooldown); //cooldown remaining spada do zera w czasie (1sek * 1/basecooldown ) czyli 30f -> 0f w 10sek
            if (skill._currentCooldownRemaining <= 0.05f) skill._currentComboProgress = Mathf.MoveTowards(skill._currentComboProgress, 0f, Time.deltaTime / scrObj_Skill._baseCooldown);   //aktualny combo progress spada do zera w czasie (1sek * (1/basecooldown)) czyli 30f -> 0f w 30sek spada tylko jeśli nie jest na cooldownie

            if (skill._currentCooldownRemaining >= 0.06f) { live_charStats.charStatus._isCasting = false; } //Reset Instanta na początku każdej klatki jeśli jest na cooldown
        }
        #endregion

        #region Dynamic Cone All
        #region Skill_DynamicCone_Update
        /// <summary>
        /// <br>Updatuje wszystkie rodzaje DynamicCone w zależności od tego czy isCasting == true/false</br>
        /// <br>Do metody trzeba podać aktualny [targetTypeIndex] z targetType[]</br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>
        public static void Skill_DynamicCone_Update(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex)
        {
            switch (scrObj_Skill._targetTypes[targetTypeIndex]._targetType) // Switch wybiera jaki jest oznaczony EnumTargetType w TargetType[targetTypeIndex]
            {
                case ScrObj_skill.TargetType.None:
                    break;

                case ScrObj_skill.TargetType.ConeOnCombo:
                    Skill_OnCombo_DynamicCone(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                    break;

                case ScrObj_skill.TargetType.ConeOnTime:
                    Skill_OnTime_DynamicCone(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                    break;

                case ScrObj_skill.TargetType.Projectile:
                    break;

                case ScrObj_skill.TargetType.AreaOfEffectMouse:
                    break;

                case ScrObj_skill.TargetType.Self:
                    break;

                case ScrObj_skill.TargetType.Pierce:
                    break;

                case ScrObj_skill.TargetType.Chain:
                    break;

                case ScrObj_skill.TargetType.Boom:
                    break;
            }
        }
        #endregion

        #region Skill_Cone_DynamicCone 
        /// <summary>
        /// Mechanika Dynamicznego Range/Angle Cone
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// <br><i>Dynamicznie skaluje zasięg i kąt Skilla</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_OnTime_DynamicCone(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex)
        {
            if (live_charStats.charStatus._isCasting)
            {
                skill.targetDynamicValues[targetTypeIndex]._currentRadius = Mathf.SmoothDamp(skill.targetDynamicValues[targetTypeIndex]._currentRadius, scrObj_Skill._targetTypes[targetTypeIndex]._maxRadius, ref skill.targetDynamicValues[targetTypeIndex]._currentVectorRadius, scrObj_Skill._targetTypes[targetTypeIndex]._timeMaxRadius);
                //dynamiczny BreathCone radius -> ++ on input

                skill.targetDynamicValues[targetTypeIndex]._currentAngle = Mathf.SmoothDamp(skill.targetDynamicValues[targetTypeIndex]._currentAngle, scrObj_Skill._targetTypes[targetTypeIndex]._maxAngle, ref skill.targetDynamicValues[targetTypeIndex]._currentVectorAngle, scrObj_Skill._targetTypes[targetTypeIndex]._timeMaxAngle);
                //dynamiczny BreathCone Angle -> ++ on input
            }
            else
            {
                skill.targetDynamicValues[targetTypeIndex]._currentRadius = Mathf.SmoothDamp(skill.targetDynamicValues[targetTypeIndex]._currentRadius, scrObj_Skill._targetTypes[targetTypeIndex]._minRadius, ref skill.targetDynamicValues[targetTypeIndex]._currentVectorRadius, scrObj_Skill._targetTypes[targetTypeIndex]._timeMaxRadius);
                //dynamiczny BreathCone radius -> -- off input

                skill.targetDynamicValues[targetTypeIndex]._currentAngle = Mathf.SmoothDamp(skill.targetDynamicValues[targetTypeIndex]._currentAngle, scrObj_Skill._targetTypes[targetTypeIndex]._minAngle, ref skill.targetDynamicValues[targetTypeIndex]._currentVectorAngle, scrObj_Skill._targetTypes[targetTypeIndex]._timeMaxAngle);
                //dynamiczny BreathCone Angle -> -- off 
            }
        }
        #endregion

        #region Skill_OnCombo_DynamicCone
        /// <summary>
        /// Mechanika Dynamicznego Range/Angle Cone dla TargetType Melee / wykorzystujących OnCombo_DynamicCone 
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// <br><i>Dynamicznie skaluje zasięg i kąt Skilla</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_OnCombo_DynamicCone(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex)
        {
            if (live_charStats.charStatus._isCasting) { skill.targetDynamicValues[targetTypeIndex]._currentRadius = scrObj_Skill._targetTypes[targetTypeIndex]._maxRadius; }
            else { skill.targetDynamicValues[targetTypeIndex]._currentRadius = scrObj_Skill._targetTypes[targetTypeIndex]._minRadius; }

            skill.targetDynamicValues[targetTypeIndex]._currentAngle = Mathf.Lerp(scrObj_Skill._targetTypes[targetTypeIndex]._minAngle, scrObj_Skill._targetTypes[targetTypeIndex]._maxAngle, Mathf.InverseLerp(0f, 1.5f, skill._currentComboProgress));
        }
        #endregion
        #endregion

        #region Resets_Audio / VFX / Casting / Animator
        #region Skill_ResetTargetList 
        /// <summary>
        /// Resetuje Boole targetInRange/Angle, DynamicCone oraz listę Colliderów -> New
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// </summary>
        /// <param name="skill">Ten GameObject skill</param>
        public static void Skill_ResetTargetList(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex)
        {
            skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false;
            skill.targetDynamicValues[targetTypeIndex]._targetInRange = false;
            skill.targetDynamicValues[targetTypeIndex]._targetColliders.Clear(); //czyszczenie listy colliderów

            Skill_DynamicCone_Update(scrObj_Skill, skill, live_charStats, targetTypeIndex);
        }
        #endregion        

        #region Skill_ResetCastingAndVFXAnimsAudio
        /// <summary>
        /// Reset wszystkich Static metod Casting oraz VFX i Animatora do Skilla
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param> 
        public static void Skill_ResetCastingAndVFXAnimsAudio(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            Skill_ResetVFXAnimsAudio(scrObj_Skill, skill, live_charStats);

            live_charStats.charStatus._isCasting = false;
        }
        #endregion   

        #region Skill_ResetCastingAudioSourceInstantly
        /// <summary>
        /// Natychmiastowo przerywa Skill_AudioSourceCaster
        /// </summary>
        /// <param name="scrObj_Skill"></param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats"></param>
        public static void Skill_ResetCastingAudioSourceInstantly(Skill skill)
        {
            if (skill._audioSourceCaster != null)
            {
                skill._audioSourceCaster.Stop();
            }
        }
        #endregion       

        #region Skill_ResetVFXAnimsAudio
        /// <summary>
        /// Reset wszystkich Static metod Casting oraz VFX i Animatora do Skilla
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param> 
        public static void Skill_ResetVFXAnimsAudio(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            if (skill._castingVisualEffect != null) skill._castingVisualEffect.Stop();
            if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill._animatorBoolName, false);
            if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorTriggerName)) live_charStats.charComponents._Animator.ResetTrigger(scrObj_Skill._animatorTriggerName);

            if (skill._audioSourceCaster != null)
            {
                skill._audioSourceCaster.volume = Mathf.MoveTowards(skill._audioSourceCaster.volume, 0, Time.deltaTime / 2); //obniza volume 1-> 0 w 2sek
                if (skill._audioSourceCaster.volume <= 0.05f) { skill._audioSourceCaster.Stop(); }
                if (!string.IsNullOrWhiteSpace(scrObj_Skill._animatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill._animatorFloatName, 0f); // razem z Volume żeby nie resetował instantowo
            }

            live_charStats.charStatus._isCasting = false;

            //skill.skill_currentCastingProgress = 0f; //reset progressu przy przerwaniu casta / niespełnieniu warunków    
        }
        #endregion        

        #region Skill_StopAllAnimatorMovementOnCast
        /// <summary>
        /// <br>Klasa zatrzymująca cały movement postaci (Movement -> [yAnim = 0], ResetRrigger -> [Jump], Animator bool -> [false])</br>
        /// <br>Nazwy Zmiennych animatora pobierane ze scrObj_Skilla</br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="live_charStats"></param>
        public static void Skill_StopAllAnimatorMovement(ScrObj_skill scrObj_Skill, CharacterStatus live_charStats)
        {
            if (!string.IsNullOrWhiteSpace(scrObj_Skill._stopOnCastFloatNameAnimator)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill._stopOnCastFloatNameAnimator, 0.2f);
            if (!string.IsNullOrWhiteSpace(scrObj_Skill._stopOnCastTriggerNameAnimator)) live_charStats.charComponents._Animator.ResetTrigger(scrObj_Skill._stopOnCastTriggerNameAnimator);
            if (!string.IsNullOrWhiteSpace(scrObj_Skill._stopOnCastBoolNameAnimator)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill._stopOnCastBoolNameAnimator, false);

        }
        #endregion 
        #endregion
    }
    #endregion Utils
}

