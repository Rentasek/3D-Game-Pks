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

        #region Skill_CastingUniversal_VFX_Audio

        /// <summary>
        /// Metoda odpowiedzialna za VFX, animacje Animatora i Audio skilla
        /// <br><i>Rodzaj castowania -> <b>Castable</b> (przytrzymaj i poczekaj aż wystrzeli)</i></br>
        /// <br><i>Rodzaj castowania -> <b>Instant</b> działa w momencie użycia (ograniczone cooldownem)</i></br>
        /// <br><i>Rodzaj castowania -> <b>Hold</b> dziala od razu (przytrzymaj)</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_CastingUniversal_VFX_Audio(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            //InputSelector(scrObj_Skill, skill, live_charStats);

            if (skill.skill_input && skill.skill_CanCast && Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) > 0)
            {
                if (Utils.Skill_ResourceTypeCurrentFloatReadOnly(scrObj_Skill, live_charStats) > skill._resourceCost)   //Jeśli nie castuje i ma właśnie zacząć //Start Casting
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

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill.skill_AnimatorFloatName, skill._currentComboProgress); //przed updatem comboProgress
                        skill._currentComboProgress += /*scrObj_Skill.skill_BaseCooldown +*/ 0.5f;
                        if (skill._currentComboProgress >= 1) skill._currentComboProgress = 0f; // trzeci Atak maxymalnie może podbić do [1.83f] więc żeby się wykonał trzeba dać powyżej [1.83f] (1.50f(1.51f dla bezpieczeństwa) + cooldown(0.33))

                        #region AudioClipy //Specjalnie puszczane przy instant ale po animatorze
                        if (scrObj_Skill.skill_OneShotOverlapAudioClip != null)
                        {
                            skill.skill_AudioSourceInstant.volume = scrObj_Skill.skill_CasterAudioVolume;
                            skill.skill_AudioSourceInstant.PlayOneShot(scrObj_Skill.skill_OneShotOverlapAudioClip, scrObj_Skill.skill_CasterAudioVolume);
                        }

                        if (scrObj_Skill.skill_OneShotNonOverlapAudioClip != null && !skill.skill_AudioSourceHold.isPlaying)
                        {
                            skill.skill_AudioSourceHold.volume = scrObj_Skill.skill_CasterAudioVolume;
                            skill.skill_AudioSourceHold.PlayOneShot(scrObj_Skill.skill_OneShotNonOverlapAudioClip, scrObj_Skill.skill_CasterAudioVolume);
                        }

                        if (scrObj_Skill.skill_TimeCastOverlapAudioClip != null)
                        {
                            skill.skill_AudioSourceCastable.volume = scrObj_Skill.skill_CasterAudioVolume;
                            skill.skill_AudioSourceCastable.clip = scrObj_Skill.skill_TimeCastOverlapAudioClip;
                            skill.skill_AudioSourceCastable.PlayScheduled(scrObj_Skill.skill_TimeCast);
                        }

                        if (scrObj_Skill.skill_TimeCastNonOverlapAudioClip != null && !skill.skill_AudioSourceCastable.isPlaying)
                        {
                            skill.skill_AudioSourceCastable.volume = scrObj_Skill.skill_CasterAudioVolume;
                            skill.skill_AudioSourceCastable.clip = scrObj_Skill.skill_TimeCastNonOverlapAudioClip;
                            skill.skill_AudioSourceCastable.PlayScheduled(scrObj_Skill.skill_TimeCast);
                        }
                        #endregion
                    }                    
                    #endregion  
                    
                    live_charStats.charStatus._isCasting = true;                   

                } else { Utils.Skill_ResetCastingAudioSourceInFixedTime(skill); }

                #region IsCasting -> Castable
                skill.skill_currentCastingProgress = Mathf.MoveTowards(skill.skill_currentCastingProgress, 1f, Time.deltaTime / scrObj_Skill.skill_TimeCast); //casting progress rośnie do 1 w (1sek * 1/TimeCast ) czyli (sek * "n" [np.0.5f] = "n" część sekundy)
                skill.skill_IsCastingFinishedCastable = (skill.skill_currentCastingProgress >= 0.95f);
                if (skill.skill_currentCastingProgress >= 0.95f)
                {
                    Utils.Skill_ResetCastingAudioSourceInstantly(skill);

                    skill.skill_currentCastingProgress = 0f;    //reset progressu po wycastowaniu Skilla

                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) //reset AnimatorIsCastingBooleana i CastingAudioClipa przy IsCastingFinished <- najlepiej żeby był taki sam jak długość AudioClipa przy HOLD
                    {                                                
                        live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, false);
                    }

                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerOnFinishedCastingName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill.skill_AnimatorTriggerOnFinishedCastingName);  //Animacja po wykonaniu casta                                           
                    skill.skill_currentCooldownRemaining = 1f;  //Ustawia cooldown po wycastowaniu żeby nie castował znów zbyt szybko, przydatnie jeśli mamy animację po wystrzeleniu
                }
                #endregion

                #region IsCasting -> Hold
                skill.skill_IsCastingHold = true;
                #endregion

            }
            else
            {

                Utils.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats);
            }

        }
        #endregion

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
                    if (skill.skill_currentCooldownRemaining <= 0.05f)  //Jeśli zostało 0.05f lub mniej cooldownu może użyć instanta
                    {
                        if (skill.skill_CastingVisualEffect != null) skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown

                        Utils.Skill_StopAllAnimatorMovement(scrObj_Skill, live_charStats);

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, true);
                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill.skill_AnimatorTriggerName);

                        for (int targetTypeIndex = 0; targetTypeIndex < scrObj_Skill.new_TargetType.Length; targetTypeIndex++)
                        {
                            switch (scrObj_Skill.new_TargetType[targetTypeIndex].new_EnumTargetType)
                            {
                                case ScrObj_skill.New_EnumTargetType.None:
                                    break;
                                case ScrObj_skill.New_EnumTargetType.Melee:
                                    TargetType.Skill_Melee_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                    break;
                                case ScrObj_skill.New_EnumTargetType.Cone:
                                    TargetType.Skill_Cone_Target(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                    break;
                                case ScrObj_skill.New_EnumTargetType.Projectile:
                                    break;
                                case ScrObj_skill.New_EnumTargetType.AreaOfEffectMouse:
                                    break;
                                case ScrObj_skill.New_EnumTargetType.Self:
                                    TargetType.Skill_Self_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                    break;
                                case ScrObj_skill.New_EnumTargetType.Pierce:
                                    break;
                                case ScrObj_skill.New_EnumTargetType.Chain:
                                    break;
                                case ScrObj_skill.New_EnumTargetType.Boom:
                                    break;
                            }
                        }

                        skill.skill_currentCooldownRemaining = 1f; //Ustawia cooldown czyli IsCastingInstant wychodzi z true (raz) ale przy następnej klatce już nie wejdzie do if bo ma cooldown

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill.skill_AnimatorFloatName, skill._currentComboProgress); //przed updatem comboProgress
                        skill._currentComboProgress += 0.5f;
                        if (skill._currentComboProgress >= 1) skill._currentComboProgress = 0f;

                        #region AudioClipy //Specjalnie puszczane przy instant ale po animatorze
                        if (scrObj_Skill.skill_OneShotOverlapAudioClip != null)
                        {
                            skill.skill_AudioSourceCaster.volume = scrObj_Skill.skill_CasterAudioVolume;
                            skill.skill_AudioSourceCaster.PlayOneShot(scrObj_Skill.skill_OneShotOverlapAudioClip, scrObj_Skill.skill_CasterAudioVolume);
                        }
                        #endregion
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
                    if (skill.skill_currentCooldownRemaining <= 0.05f)  //Jeśli zostało 0.05f lub mniej cooldownu może użyć instanta
                    {
                        if (skill.skill_CastingVisualEffect != null) skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown

                        Utils.Skill_StopAllAnimatorMovement(scrObj_Skill, live_charStats);

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, true);
                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill.skill_AnimatorTriggerName);
                        
                        skill.skill_currentCooldownRemaining = 1f; //Ustawia cooldown czyli IsCastingInstant wychodzi z true (raz) ale przy następnej klatce już nie wejdzie do if bo ma cooldown

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill.skill_AnimatorFloatName, skill._currentComboProgress); //przed updatem comboProgress
                        skill._currentComboProgress += 0.5f;
                        if (skill._currentComboProgress >= 1) skill._currentComboProgress = 0f;

                        #region AudioClipy //Specjalnie puszczane przy instant ale po animatorze
                        if (scrObj_Skill.skill_OneShotOverlapAudioClip != null)
                        {
                            skill.skill_AudioSourceCaster.volume = scrObj_Skill.skill_CasterAudioVolume;
                            skill.skill_AudioSourceCaster.PlayOneShot(scrObj_Skill.skill_OneShotOverlapAudioClip, scrObj_Skill.skill_CasterAudioVolume);
                        }
                        #endregion
                    }
                    live_charStats.charStatus._isCasting = true;
                }
                skill._isUsingSkill= true;

                for (int targetTypeIndex = 0; targetTypeIndex < scrObj_Skill.new_TargetType.Length; targetTypeIndex++)
                {
                    switch (scrObj_Skill.new_TargetType[targetTypeIndex].new_EnumTargetType)
                    {
                        case ScrObj_skill.New_EnumTargetType.None:
                            break;
                        case ScrObj_skill.New_EnumTargetType.Melee:
                            TargetType.Skill_Melee_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                            break;
                        case ScrObj_skill.New_EnumTargetType.Cone:
                            TargetType.Skill_Cone_Target(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                            break;
                        case ScrObj_skill.New_EnumTargetType.Projectile:
                            break;
                        case ScrObj_skill.New_EnumTargetType.AreaOfEffectMouse:
                            break;
                        case ScrObj_skill.New_EnumTargetType.Self:
                            TargetType.Skill_Self_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                            break;
                        case ScrObj_skill.New_EnumTargetType.Pierce:
                            break;
                        case ScrObj_skill.New_EnumTargetType.Chain:
                            break;
                        case ScrObj_skill.New_EnumTargetType.Boom:
                            break;
                    }
                } 
            }
            else
            {
                Utils.Skill_ResetCastingAndVFXAnimsAudio(scrObj_Skill, skill, live_charStats);
                //Utils.Skill_ResetTargetList(scrObj_Skill, skill, live_charStats, targetTypeIndex); //Reset TargetList w targetTypeIndexie
                skill._isUsingSkill= false;
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
                    if (skill.skill_currentCooldownRemaining <= 0.05f)  //Jeśli zostało 0.05f lub mniej cooldownu może użyć instanta
                    {
                        if (skill.skill_CastingVisualEffect != null) skill.skill_CastingVisualEffect.Play(); //może się odpalić tylko raz przy każdym inpucie, nie może się nadpisać -> taki sam efekt jak przy GetKeyDown

                        Utils.Skill_StopAllAnimatorMovement(scrObj_Skill, live_charStats);

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, true);
                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill.skill_AnimatorTriggerName);

                        skill.skill_currentCooldownRemaining = 1f; //Ustawia cooldown czyli IsCastingInstant wychodzi z true (raz) ale przy następnej klatce już nie wejdzie do if bo ma cooldown

                        if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill.skill_AnimatorFloatName, skill._currentComboProgress); //przed updatem comboProgress
                        skill._currentComboProgress += 0.5f;
                        if (skill._currentComboProgress >= 1) skill._currentComboProgress = 0f;

                        #region AudioClipy //Specjalnie puszczane przy instant ale po animatorze
                        if (scrObj_Skill.skill_OneShotOverlapAudioClip != null)
                        {
                            skill.skill_AudioSourceCaster.volume = scrObj_Skill.skill_CasterAudioVolume;
                            skill.skill_AudioSourceCaster.PlayOneShot(scrObj_Skill.skill_OneShotOverlapAudioClip, scrObj_Skill.skill_CasterAudioVolume);
                        }
                        #endregion
                    }
                    live_charStats.charStatus._isCasting = true;
                }               
               
                skill.skill_currentCastingProgress = Mathf.MoveTowards(skill.skill_currentCastingProgress, 1f, Time.deltaTime / scrObj_Skill.skill_TimeCast); //casting progress rośnie do 1 w (1sek * 1/TimeCast ) czyli (sek * "n" [np.0.5f] = "n" część sekundy)
                //skill.skill_IsCastingFinishedCastable = (skill.skill_currentCastingProgress >= 0.95f);
                if (skill.skill_currentCastingProgress >= 0.95f)
                {
                    Utils.Skill_ResetVFXAnimsAudio(scrObj_Skill, skill, live_charStats); //Reset Audio / VFX / Animator po wycastowaniu ale przed Triggerem OnFinished

                    //Odpalenie skilla po skończeniu castowania
                    for (int targetTypeIndex = 0; targetTypeIndex < scrObj_Skill.new_TargetType.Length; targetTypeIndex++)
                    {
                        switch (scrObj_Skill.new_TargetType[targetTypeIndex].new_EnumTargetType)
                        {
                            case ScrObj_skill.New_EnumTargetType.None:
                                break;
                            case ScrObj_skill.New_EnumTargetType.Melee:
                                TargetType.Skill_Melee_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                break;
                            case ScrObj_skill.New_EnumTargetType.Cone:
                                TargetType.Skill_Cone_Target(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                break;
                            case ScrObj_skill.New_EnumTargetType.Projectile:
                                break;
                            case ScrObj_skill.New_EnumTargetType.AreaOfEffectMouse:
                                break;
                            case ScrObj_skill.New_EnumTargetType.Self:
                                TargetType.Skill_Self_Target_new(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                                break;
                            case ScrObj_skill.New_EnumTargetType.Pierce:
                                break;
                            case ScrObj_skill.New_EnumTargetType.Chain:
                                break;
                            case ScrObj_skill.New_EnumTargetType.Boom:
                                break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerOnFinishedCastingName)) live_charStats.charComponents._Animator.SetTrigger(scrObj_Skill.skill_AnimatorTriggerOnFinishedCastingName);  //Animacja po wykonaniu casta                                           
                    
                    if (scrObj_Skill.skill_OneShotOverlapAudioClip != null)
                    {
                        skill.skill_AudioSourceCaster.volume = scrObj_Skill.skill_CasterAudioVolume;
                        skill.skill_AudioSourceCaster.PlayOneShot(scrObj_Skill.skill_OnFinishCastingAudioClip, scrObj_Skill.skill_CasterAudioVolume);
                    }

                    skill.skill_currentCastingProgress = 0f;    //reset progressu po wycastowaniu Skilla             
                    skill.skill_currentCooldownRemaining = 1f;  //Ustawia cooldown po wycastowaniu żeby nie castował znów zbyt szybko, przydatnie jeśli mamy animację po wystrzeleniu
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

        #region Działający Stary OverlapSphere
        /*/// <summary>Działający Stary OverlapSphere
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
        }*/
        #endregion
        #region OverlapSphereNonAloc
        /// <summary>nowy OverlapSphereNonAloc ma swoje plusy bo nie tworzy nowego array przy kazdym Cast ale ogólnie niewiele zmienia a trzeba robić dodatkową tablicę niedynamiczną
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
                for (int i = 0; i < Physics.OverlapSphereNonAlloc(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius, skill.skill_allLocalColliders); i++)
                {
                    for (int j = 0; j < skill._enemiesArray.Length; j++)
                    {
                        if (skill.skill_allLocalColliders[i].CompareTag(skill._enemiesArray[j]))
                        {
                            skill.skill_targetInRange = true; //target jest w breath range

                            Vector3 directionToTarget = (skill.skill_allLocalColliders[i].transform.position - skill.skill_casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector (kierunek w którym od niego znajduje się target)
                                                                                                                                                                         //sprawdzanie aktualnie ostatniego elementu z listy
                            if (Vector3.Angle(skill.skill_casterGameobject.transform.forward, directionToTarget) < skill.skill_currentAngle / 2)
                            //sprawdzanie angle wektora forward charactera i direction to target
                            //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                            {
                                if (!Physics.Raycast(skill.skill_casterGameobject.transform.position, directionToTarget, Vector3.Distance(skill.skill_casterGameobject.transform.position, skill.skill_allLocalColliders[i].transform.position), scrObj_Skill.skill_ObstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                                {                                   
                                    skill.skill_targetInAngle = true;

                                    if (skill.skill_targetColliders.IndexOf(skill.skill_allLocalColliders[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                    {
                                        skill.skill_targetColliders.Add(skill.skill_allLocalColliders[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies
                                    }
                                    else
                                    {
                                        skill.skill_targetColliders.Remove(skill.skill_allLocalColliders[i]);
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
                Utils.Skill_Target_Reset(skill);
            }
        }
        #endregion

        #region OverlapSphereNonAloc_NewMechanics
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

            skill.targetDynamicValues[targetTypeIndex]._allLocalColliders = new Collider[40];

            for (int i = 0; i < Physics.OverlapSphereNonAlloc(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius, skill.targetDynamicValues[targetTypeIndex]._allLocalColliders); i++)
            {
                Debug.Log(i);
                // trzeba zrobić tak żeby physics.overlapa używał tylko 1raz dla wszystkich casting types, a później z listy wyselekcjonować według kryteriów w targetTypach                 
                for (int j = 0; j < skill._enemiesArray.Length; j++)
                {
                    if (skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i].CompareTag(skill._enemiesArray[j]))
                    {
                        skill.iterationCounter++;
                        skill.targetDynamicValues[targetTypeIndex]._targetInRange = true; //target jest w breath range

                        Vector3 directionToTarget = (skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i].transform.position - skill.skill_casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector (kierunek w którym od niego znajduje się target)
                                                                                                                                                                                                        //sprawdzanie aktualnie ostatniego elementu z listy
                        if (Vector3.Angle(skill.skill_casterGameobject.transform.forward, directionToTarget) < skill.skill_currentAngle / 2)
                        //sprawdzanie angle wektora forward charactera i direction to target
                        //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                        {
                            if (!Physics.Raycast(skill.skill_casterGameobject.transform.position, directionToTarget, Vector3.Distance(skill.skill_casterGameobject.transform.position, skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i].transform.position), scrObj_Skill.new_TargetType[targetTypeIndex]._obstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                            {
                                skill.targetDynamicValues[targetTypeIndex]._targetInAngle = true;

                                if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.IndexOf(skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                {
                                    skill.targetDynamicValues[targetTypeIndex]._targetColliders.Add(skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies                                    
                                }
                                else
                                {
                                    skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i]);
                                    if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0) skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false; //jeśli nie ma żadnych targetów w Cone
                                }
                            }
                        }
                    }
                }
            }

            if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count > 0)
            {
                for (int effectTypeIndex = 0; effectTypeIndex < scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType.Length; effectTypeIndex++)
                {
                    switch (scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType[effectTypeIndex].new_EnumEffectType)
                    {
                        case ScrObj_skill.New_EnumEffectType.None:
                            break;

                        case ScrObj_skill.New_EnumEffectType.Hit:
                            EffectType.Skill_Hit_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.DamageOverTime:
                            EffectType.Skill_DamageOverTime_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.Heal:
                            EffectType.Skill_Heal_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.HealOverTime:
                            EffectType.Skill_HealOverTime_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.Summon:
                            break;
                    }
                }
            }


        }
            

        #endregion

        #endregion

        #region Skill_Melee_Target

        #region OLD_Skill_Melee_Target
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
                for (int i = 0; i < Physics.OverlapSphereNonAlloc(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius, skill.skill_allLocalColliders); i++)
                {
                    for (int j = 0; j < skill._enemiesArray.Length; j++)
                    {
                        if (skill.skill_allLocalColliders[i].CompareTag(skill._enemiesArray[j]))
                        {
                            skill.skill_targetInRange = true; //target jest w Radius range

                            Vector3 directionToTarget = (skill.skill_allLocalColliders[i].transform.position - skill.skill_casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector (kierunek w którym od niego znajduje się target)
                                                                                                                                                                            //sprawdzanie aktualnie ostatniego elementu z listy
                            if (Vector3.Angle(skill.skill_casterGameobject.transform.forward, directionToTarget) < skill.skill_currentAngle / 2)
                            //sprawdzanie angle wektora forward charactera i direction to target
                            //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                            {
                                if (!Physics.Raycast(skill.skill_casterGameobject.transform.position, directionToTarget, Vector3.Distance(skill.skill_casterGameobject.transform.position, skill.skill_allLocalColliders[i].transform.position), scrObj_Skill.skill_ObstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                                {
                                    skill.skill_targetInAngle = true;

                                    if (skill.skill_targetColliders.IndexOf(skill.skill_allLocalColliders[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                    {
                                        skill.skill_targetColliders.Add(skill.skill_allLocalColliders[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies
                                    }
                                    else
                                    {
                                        skill.skill_targetColliders.Remove(skill.skill_allLocalColliders[i]);
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
                Utils.Skill_Target_Reset(skill);
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

            for (int i = 0; i < Physics.OverlapSphereNonAlloc(skill.skill_casterGameobject.transform.position, skill.skill_currentRadius, skill.targetDynamicValues[targetTypeIndex]._allLocalColliders); i++)
            {
                for (int j = 0; j < skill._enemiesArray.Length; j++)
                {
                    if (skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i].CompareTag(skill._enemiesArray[j]))
                    {
                        skill.targetDynamicValues[targetTypeIndex]._targetInRange = true; //target jest w breath range

                        Vector3 directionToTarget = (skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i].transform.position - skill.skill_casterGameobject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector (kierunek w którym od niego znajduje się target)
                                                                                                                                                                                                        //sprawdzanie aktualnie ostatniego elementu z listy
                        if (Vector3.Angle(skill.skill_casterGameobject.transform.forward, directionToTarget) < skill.skill_currentAngle / 2)
                        //sprawdzanie angle wektora forward charactera i direction to target
                        //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                        {
                            if (!Physics.Raycast(skill.skill_casterGameobject.transform.position, directionToTarget, Vector3.Distance(skill.skill_casterGameobject.transform.position, skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i].transform.position), scrObj_Skill.new_TargetType[targetTypeIndex]._obstaclesMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                            {
                                skill.targetDynamicValues[targetTypeIndex]._targetInAngle = true;

                                if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.IndexOf(skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                                {
                                    skill.targetDynamicValues[targetTypeIndex]._targetColliders.Add(skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies  
                                }
                                else
                                {
                                    skill.targetDynamicValues[targetTypeIndex]._targetColliders.Remove(skill.targetDynamicValues[targetTypeIndex]._allLocalColliders[i]);
                                    if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count <= 0) skill.targetDynamicValues[targetTypeIndex]._targetInAngle = false; //jeśli nie ma żadnych targetów w Cone
                                }
                            }

                        }
                    }
                }
            }

            if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count > 0)
            {
                for (int effectTypeIndex = 0; effectTypeIndex < scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType.Length; effectTypeIndex++)
                {
                    switch (scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType[effectTypeIndex].new_EnumEffectType)
                    {
                        case ScrObj_skill.New_EnumEffectType.None:
                            break;

                        case ScrObj_skill.New_EnumEffectType.Hit:
                            EffectType.Skill_Hit_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.DamageOverTime:
                            EffectType.Skill_DamageOverTime_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.Heal:
                            EffectType.Skill_Heal_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.HealOverTime:
                            EffectType.Skill_HealOverTime_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.Summon:
                            break;
                    }
                }
            }
        }
        #endregion

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

        #region OLD_Skill_Self_Target
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
                Utils.Skill_Target_Reset(skill);
            }
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
                for (int effectTypeIndex = 0; effectTypeIndex < scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType.Length; effectTypeIndex++)
                {
                    switch (scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType[effectTypeIndex].new_EnumEffectType)
                    {
                        case ScrObj_skill.New_EnumEffectType.None:
                            break;

                        case ScrObj_skill.New_EnumEffectType.Hit:
                            EffectType.Skill_Hit_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.DamageOverTime:
                            EffectType.Skill_DamageOverTime_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.Heal:
                            EffectType.Skill_Heal_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.HealOverTime:
                            EffectType.Skill_HealOverTime_new(scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                            break;

                        case ScrObj_skill.New_EnumEffectType.Summon:
                            break;
                    }
                }
            }
        }
        #endregion

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

        #region OLD_Skill_DamageOverTime
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
                switch (scrObj_Skill._resourceType)
                {
                    case ScrObj_skill.Skill_ResourceType.health:
                        live_charStats.charStats._hp = Mathf.MoveTowards(live_charStats.charStats._hp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                        break;

                    case ScrObj_skill.Skill_ResourceType.mana:
                        live_charStats.charStats._mp = Mathf.MoveTowards(live_charStats.charStats._mp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                        break;

                    case ScrObj_skill.Skill_ResourceType.stamina:
                        live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, -skill._resourceCost, skill._resourceCost * Time.deltaTime);     // ResourceCost / Sek 
                        break;
                }
            }
        }
        #endregion

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
        public static void Skill_DamageOverTime_new(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex, int effectTypeIndex)
        {
            Utils.Skill_EffectValuesUpdate_new(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats, targetTypeIndex, effectTypeIndex); //nie chce mi się dopisywać linka do CharacterBonusStats :P

            for (int i = 0; i < skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count; i++)
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders[i].GetComponent<CharacterStatus>().TakeDamageOverTime(skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage, live_charStats);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill._resourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    live_charStats.charStats._hp = Mathf.MoveTowards(live_charStats.charStats._hp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    live_charStats.charStats._mp = Mathf.MoveTowards(live_charStats.charStats._mp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, -skill._resourceCost, skill._resourceCost * Time.deltaTime);     // ResourceCost / Sek 
                    break;
            }
        }
        #endregion
        #endregion

        #region Skill_Hit

        #region OLD_Skill_Hit
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
                switch (scrObj_Skill._resourceType)
                {
                    case ScrObj_skill.Skill_ResourceType.health:
                        live_charStats.charStats._hp -= skill._resourceCost; // ResourceCost Instant
                        break;

                    case ScrObj_skill.Skill_ResourceType.mana:
                        live_charStats.charStats._mp -= skill._resourceCost;  // ResourceCost Instant
                        break;

                    case ScrObj_skill.Skill_ResourceType.stamina:
                        live_charStats.charStats._stam -= skill._resourceCost; // ResourceCost Instant
                        break;
                }
            }
        }
        #endregion

        #region Skill_Hit_newMechanisc
        /// <summary>
        /// Hit Instant - Target Colliderów na Collider List
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        /// <param name="targetTypeIndex">Aktualny [targetTypeIndex] z targetType[]</param>    
        /// <param name="targetTypeIndex">Aktualny [effectTypeIndex] z effectType[]</param> 
        public static void Skill_Hit_new(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex, int effectTypeIndex)
        {
            Utils.Skill_EffectValuesUpdate_new(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats, targetTypeIndex, effectTypeIndex); //nie chce mi się dopisywać linka do CharacterBonusStats :P

            for (int i = 0; i < skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count; i++)
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders[i].GetComponent<CharacterStatus>().TakeDamgeInstant(skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage, live_charStats);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill._resourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    live_charStats.charStats._hp -= skill._resourceCost; // ResourceCost Instant
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    live_charStats.charStats._mp -= skill._resourceCost;  // ResourceCost Instant
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    live_charStats.charStats._stam -= skill._resourceCost; // ResourceCost Instant
                    break;
            }
        }
        #endregion

        #endregion

        #region Skill_HealOverTime

        #region OLD_Skill_HealOverTime
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
                switch (scrObj_Skill._resourceType)
                {
                    case ScrObj_skill.Skill_ResourceType.health:
                        live_charStats.charStats._hp = Mathf.MoveTowards(live_charStats.charStats._hp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                        break;

                    case ScrObj_skill.Skill_ResourceType.mana:
                        live_charStats.charStats._mp = Mathf.MoveTowards(live_charStats.charStats._mp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                        break;

                    case ScrObj_skill.Skill_ResourceType.stamina:
                        live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, -skill._resourceCost, skill._resourceCost * Time.deltaTime);     // ResourceCost / Sek 
                        break;
                }
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
        public static void Skill_HealOverTime_new(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex, int effectTypeIndex)
        {
            Utils.Skill_EffectValuesUpdate_new(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats, targetTypeIndex, effectTypeIndex); //nie chce mi się dopisywać linka do CharacterBonusStats :P

            for (int i = 0; i < skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count; i++)
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders[i].GetComponent<CharacterStatus>().TakeHealOverTime(skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill._resourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    live_charStats.charStats._hp = Mathf.MoveTowards(live_charStats.charStats._hp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    live_charStats.charStats._mp = Mathf.MoveTowards(live_charStats.charStats._mp, -skill._resourceCost, skill._resourceCost * Time.deltaTime);    // ResourceCost / Sek 
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, -skill._resourceCost, skill._resourceCost * Time.deltaTime);     // ResourceCost / Sek 
                    break;
            }
        }
        #endregion

        #endregion

        #region Skill_Heal

        #region OLD_Skill_Heal
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
                switch (scrObj_Skill._resourceType)
                {
                    case ScrObj_skill.Skill_ResourceType.health:
                        live_charStats.charStats._hp -= skill._resourceCost; // ResourceCost Instant
                        break;

                    case ScrObj_skill.Skill_ResourceType.mana:
                        live_charStats.charStats._mp -= skill._resourceCost;  // ResourceCost Instant
                        break;

                    case ScrObj_skill.Skill_ResourceType.stamina:
                        live_charStats.charStats._stam -= skill._resourceCost; // ResourceCost Instant
                        break;
                }
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
        public static void Skill_Heal_new(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, int targetTypeIndex, int effectTypeIndex)
        {
            Utils.Skill_EffectValuesUpdate_new(scrObj_Skill, skill, live_charStats, live_charStats.charComponents._characterBonusStats, targetTypeIndex, effectTypeIndex); //nie chce mi się dopisywać linka do CharacterBonusStats :P

            for (int i = 0; i < skill.targetDynamicValues[targetTypeIndex]._targetColliders.Count; i++)
            {
                skill.targetDynamicValues[targetTypeIndex]._targetColliders[i].GetComponent<CharacterStatus>().TakeHealInstant(skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage);
            }

            ///Metoda zużywania Resourca wybranego w scriptableObject
            ///Musi być "przypisana" do elementu objectu!!! (live_charStats.currentHP = wtedy trafia bezpośrednio tam gdzie ma być) Jeśli będzie return (float) trzeba użyć switcha wyżej (poza tą metodą/klasą) co może być niepotrzebne
            switch (scrObj_Skill._resourceType)
            {
                case ScrObj_skill.Skill_ResourceType.health:
                    live_charStats.charStats._hp -= skill._resourceCost; // ResourceCost Instant
                    break;

                case ScrObj_skill.Skill_ResourceType.mana:
                    live_charStats.charStats._mp -= skill._resourceCost;  // ResourceCost Instant
                    break;

                case ScrObj_skill.Skill_ResourceType.stamina:
                    live_charStats.charStats._stam -= skill._resourceCost; // ResourceCost Instant
                    break;
            }
        }
        #endregion

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

        #region Skill_EffectValuesUpdate_OLD
        /// <summary>
        /// Update wartości Damage / Cost  przed użyciem skilla (obliczanie z currentCharLevel i bonusCharStats )
        /// Wykorzystany w skill_Effects
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_EffectValuesUpdate(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, CharacterBonusStats currentCharacterBonusStats)
        {
            if (skill == live_charStats.fov._spellRangeSkill)
            {
                skill.skill_currentDamage = scrObj_Skill.skill_BaseDamage + (scrObj_Skill.skill_BaseDamage * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier)) + (scrObj_Skill.skill_BaseDamage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_Skill._multiplier)); //+bonus
                skill._resourceCost = scrObj_Skill._baseResourceCost + (scrObj_Skill._baseResourceCost * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier));
            }
            else if (skill == live_charStats.fov._closeRangeSkill)
            {
                skill.skill_currentDamage = scrObj_Skill.skill_BaseDamage + (scrObj_Skill.skill_BaseDamage * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier)) + (scrObj_Skill.skill_BaseDamage * (currentCharacterBonusStats.bonus_currentDamageCombo * scrObj_Skill._multiplier)); //+bonus
                skill._resourceCost = scrObj_Skill._baseResourceCost + (scrObj_Skill._baseResourceCost * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier));
            }
        }
        #endregion

        #region Skill_EffectValuesUpdate_NewMechanics
        /// <summary>
        /// Update wartości Damage / Cost  przed użyciem skilla (obliczanie z currentCharLevel i bonusCharStats )
        /// <br>Dziala z nową mechaniką targetType[]</br>
        /// Wykorzystany w skill_Effects
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_EffectValuesUpdate_new(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, CharacterBonusStats currentCharacterBonusStats, int targetTypeIndex, int effectTypeIndex)
        {
            if (skill == live_charStats.fov._spellRangeSkill)
            {
                skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage = scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType[effectTypeIndex]._baseDamage + (scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType[effectTypeIndex]._baseDamage * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier)) + (scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType[effectTypeIndex]._baseDamage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_Skill._multiplier)); //+bonus
                skill._resourceCost = scrObj_Skill._baseResourceCost + (scrObj_Skill._baseResourceCost * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier));
            }
            else if (skill == live_charStats.fov._closeRangeSkill)
            {
                skill.targetDynamicValues[targetTypeIndex].effectDynamicValues[effectTypeIndex]._currentDamage = scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType[effectTypeIndex]._baseDamage + (scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType[effectTypeIndex]._baseDamage * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier)) + (scrObj_Skill.new_TargetType[targetTypeIndex].new_EffectType[effectTypeIndex]._baseDamage * (currentCharacterBonusStats.bonus_currentDamageCombo * scrObj_Skill._multiplier)); //+bonus
                skill._resourceCost = scrObj_Skill._baseResourceCost + (scrObj_Skill._baseResourceCost * (live_charStats.charInfo._charLevel * scrObj_Skill._multiplier));
            }
        }
        #endregion

        #endregion

        #region Skill_EveryFrameValuesUpdate

        #region Skill_EveryFrameValuesUpdate_OLD
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
            if (skill.skill_currentCooldownRemaining <= 0.05f) skill._currentComboProgress = Mathf.MoveTowards(skill._currentComboProgress, 0f, Time.deltaTime / scrObj_Skill.skill_BaseCooldown);   //aktualny combo progress spada do zera w czasie (1sek * (1/basecooldown)) czyli 30f -> 0f w 30sek spada tylko jeśli nie jest na cooldownie

            if (skill.skill_currentCooldownRemaining >= 0.06f) { skill.skill_IsCastingInstant = false; } //Reset Instanta na początku każdej klatki jeśli jest na cooldown

        }
        #endregion

        #region Skill_EveryFrameValuesUpdate_NewMechanics

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

            skill.skill_currentCooldownRemaining = Mathf.MoveTowards(skill.skill_currentCooldownRemaining, 0f, Time.deltaTime / scrObj_Skill.skill_BaseCooldown); //cooldown remaining spada do zera w czasie (1sek * 1/basecooldown ) czyli 30f -> 0f w 10sek
            if (skill.skill_currentCooldownRemaining <= 0.05f) skill._currentComboProgress = Mathf.MoveTowards(skill._currentComboProgress, 0f, Time.deltaTime / scrObj_Skill.skill_BaseCooldown);   //aktualny combo progress spada do zera w czasie (1sek * (1/basecooldown)) czyli 30f -> 0f w 30sek spada tylko jeśli nie jest na cooldownie

            if (skill.skill_currentCooldownRemaining >= 0.06f) { live_charStats.charStatus._isCasting = false; } //Reset Instanta na początku każdej klatki jeśli jest na cooldown
        }
        #endregion

        #endregion

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
            switch (scrObj_Skill.new_TargetType[targetTypeIndex].new_EnumTargetType) // Switch wybiera jaki jest oznaczony EnumTargetType w TargetType[targetTypeIndex]
            {
                case ScrObj_skill.New_EnumTargetType.None:
                    break;

                case ScrObj_skill.New_EnumTargetType.Melee:
                    Skill_OnCombo_DynamicCone(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                    break;

                case ScrObj_skill.New_EnumTargetType.Cone:
                    Skill_OnTime_DynamicCone(scrObj_Skill, skill, live_charStats, targetTypeIndex);
                    break;

                case ScrObj_skill.New_EnumTargetType.Projectile:
                    break;

                case ScrObj_skill.New_EnumTargetType.AreaOfEffectMouse:
                    break;

                case ScrObj_skill.New_EnumTargetType.Self:
                    break;

                case ScrObj_skill.New_EnumTargetType.Pierce:
                    break;

                case ScrObj_skill.New_EnumTargetType.Chain:
                    break;

                case ScrObj_skill.New_EnumTargetType.Boom:
                    break;
            }
        }
        #endregion

        #region Skill_Cone_DynamicCone

        #region Skill_Cone_DynamicCone_OLD
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

        #region Skill_Cone_DynamicCone_NewMechanics

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
                skill.targetDynamicValues[targetTypeIndex]._currentRadius = Mathf.SmoothDamp(skill.targetDynamicValues[targetTypeIndex]._currentRadius, scrObj_Skill.new_TargetType[targetTypeIndex]._maxRadius, ref skill.targetDynamicValues[targetTypeIndex]._currentVectorRadius, scrObj_Skill.new_TargetType[targetTypeIndex]._timeMaxRadius);
                //dynamiczny BreathCone radius -> ++ on input

                skill.targetDynamicValues[targetTypeIndex]._currentAngle = Mathf.SmoothDamp(skill.targetDynamicValues[targetTypeIndex]._currentAngle, scrObj_Skill.new_TargetType[targetTypeIndex]._maxAngle, ref skill.targetDynamicValues[targetTypeIndex]._currentVectorAngle, scrObj_Skill.new_TargetType[targetTypeIndex]._timeMaxAngle);
                //dynamiczny BreathCone Angle -> ++ on input
            }
            else
            {
                skill.targetDynamicValues[targetTypeIndex]._currentRadius = Mathf.SmoothDamp(skill.targetDynamicValues[targetTypeIndex]._currentRadius, scrObj_Skill.new_TargetType[targetTypeIndex]._minRadius, ref skill.targetDynamicValues[targetTypeIndex]._currentVectorRadius, scrObj_Skill.new_TargetType[targetTypeIndex]._timeMaxRadius);
                //dynamiczny BreathCone radius -> -- off input

                skill.targetDynamicValues[targetTypeIndex]._currentAngle = Mathf.SmoothDamp(skill.targetDynamicValues[targetTypeIndex]._currentAngle, scrObj_Skill.new_TargetType[targetTypeIndex]._minAngle, ref skill.targetDynamicValues[targetTypeIndex]._currentVectorAngle, scrObj_Skill.new_TargetType[targetTypeIndex]._timeMaxAngle);
                //dynamiczny BreathCone Angle -> -- off 
            }
        }
        #endregion

        #endregion

        #region Skill_Melee_DynamicCone

        #region Skill_Melee_DynamicCone_OLD
        /// <summary>
        /// Mechanika Dynamicznego Range/Angle Cone dla TargetType Melee
        /// <br><i>Dynamicznie skaluje zasięg i kąt Skilla</i></br>
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param>
        public static void Skill_Melee_DynamicCone(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            if (live_charStats.charStatus._isCasting) { skill.skill_currentRadius = scrObj_Skill.skill_MaxRadius; }
            else skill.skill_currentRadius = scrObj_Skill.skill_MinRadius;

            skill.skill_currentAngle = Mathf.Lerp(scrObj_Skill.skill_MinAngle, scrObj_Skill.skill_MaxAngle, Mathf.InverseLerp(0f, 1.5f, skill._currentComboProgress));
        }
        #endregion

        #region Skill_Melee_DynamicCone_NewMechanics
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
            if (live_charStats.charStatus._isCasting) { skill.targetDynamicValues[targetTypeIndex]._currentRadius = scrObj_Skill.new_TargetType[targetTypeIndex]._maxRadius; }
            else { skill.targetDynamicValues[targetTypeIndex]._currentRadius = scrObj_Skill.new_TargetType[targetTypeIndex]._minRadius; }

            skill.targetDynamicValues[targetTypeIndex]._currentAngle = Mathf.Lerp(scrObj_Skill.new_TargetType[targetTypeIndex]._minAngle, scrObj_Skill.new_TargetType[targetTypeIndex]._maxAngle, Mathf.InverseLerp(0f, 1.5f, skill._currentComboProgress));
        }
        #endregion

        #endregion

        #region Skill_Target_ConeReset
        /// <summary>
        /// Resetuje Boole targetInRange/Angle oraz listę Colliderów
        /// </summary>
        /// <param name="skill">Ten GameObject skill</param>
        public static void Skill_Target_Reset(Skill skill)
        {
            skill.skill_targetInAngle = false;
            skill.skill_targetInRange = false;
            skill.skill_targetColliders.Clear();  //czyszczenie listy colliderów
        }

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
                skill.skill_AudioSourceInstant.volume = Mathf.MoveTowards(skill.skill_AudioSourceInstant.volume, 0, Time.deltaTime / 2); //obniza volume 1-> 0 w 2sek
                if (skill.skill_AudioSourceInstant.volume <= 0.05f) { skill.skill_AudioSourceInstant.Stop(); }
            }

            if (skill.skill_AudioSourceCastable != null)
            {
                skill.skill_AudioSourceCastable.volume = Mathf.MoveTowards(skill.skill_AudioSourceCastable.volume, 0, Time.deltaTime / 2); //obniza volume 1-> 0 w 2sek
                if (skill.skill_AudioSourceCastable.volume <= 0.05f) { skill.skill_AudioSourceCastable.Stop(); }
            }

            if (skill.skill_AudioSourceHold != null)
            {
                skill.skill_AudioSourceHold.volume = Mathf.MoveTowards(skill.skill_AudioSourceHold.volume, 0, Time.deltaTime / 2); //obniza volume 1-> 0 w 2sek
                if (skill.skill_AudioSourceHold.volume <= 0.05f) { skill.skill_AudioSourceHold.Stop(); }
            }

            live_charStats.charStatus._isCasting = false;

            skill.skill_IsCastingInstant = false;

            skill.skill_IsCastingHold = false;

            skill.skill_currentCastingProgress = 0f; //reset progressu przy przerwaniu casta / niespełnieniu warunków
            skill.skill_IsCastingFinishedCastable = false;

            Skill_Target_Reset(skill); //Reset TargetList
        }
        #endregion

        #region Skill_ResetCastingAndVFXAnimsAudio
        /// <summary>
        /// Reset wszystkich Static metod Casting oraz VFX i Animatora do Skilla
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

        #region Skill_ResetAnyCastingExceptInstant

        /// <summary>
        /// Reset wszystkich Static metod Casting
        /// </summary>
        /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats">Live_charStats Castera</param> 
        public static void Skill_ResetAnyCastingExceptInstant(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
        {
            if (scrObj_Skill.skill_EffectTypeArray[0] != ScrObj_skill.Skill_EffectTypeArray.none) //Podaje index Castowania z Enumeratora ScrObj_skill.Skill_EffectTypeArray jako argument
            {                
                if (skill.skill_AudioSourceCastable != null)
                {
                    if (skill.skill_CastingVisualEffect != null) skill.skill_CastingVisualEffect.Stop();
                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, false);
                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerName)) live_charStats.charComponents._Animator.ResetTrigger(scrObj_Skill.skill_AnimatorTriggerName);

                    skill.skill_AudioSourceCastable.volume = Mathf.MoveTowards(skill.skill_AudioSourceCastable.volume, 0, Time.deltaTime / 0.3f); //obniza volume 1-> 0 w 0.3sek
                    if (skill.skill_AudioSourceCastable.volume <= 0.05f) { skill.skill_AudioSourceCastable.Stop(); }

                    //live_charStats.charStatus._isCasting = false;

                    //skill.skill_IsCastingInstant = false;

                    //skill.skill_IsCastingHold = false;

                    skill.skill_currentCastingProgress = 0f; //reset progressu przy przerwaniu casta / niespełnieniu warunków
                    skill.skill_IsCastingFinishedCastable = false;

                    Skill_Target_Reset(skill); //Reset TargetList
                }
            }

            if (scrObj_Skill.skill_EffectTypeArray[2] != ScrObj_skill.Skill_EffectTypeArray.none)  //Podaje index Castowania z Enumeratora ScrObj_skill.Skill_EffectTypeArray jako argument
            {
                if (skill.skill_AudioSourceHold != null)
                {
                    if (skill.skill_CastingVisualEffect != null) skill.skill_CastingVisualEffect.Stop();
                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, false);
                    if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerName)) live_charStats.charComponents._Animator.ResetTrigger(scrObj_Skill.skill_AnimatorTriggerName);

                    skill.skill_AudioSourceHold.volume = Mathf.MoveTowards(skill.skill_AudioSourceHold.volume, 0, Time.deltaTime / 0.3f); //obniza volume 1-> 0 w 0.3sek
                    if (skill.skill_AudioSourceHold.volume <= 0.05f) { skill.skill_AudioSourceHold.Stop(); }

                    //live_charStats.charStatus._isCasting = false;

                    //skill.skill_IsCastingInstant = false;

                    skill.skill_IsCastingHold = false;

                    //skill.skill_currentCastingProgress = 0f; //reset progressu przy przerwaniu casta / niespełnieniu warunków
                    //skill.skill_IsCastingFinishedCastable = false;

                    Skill_Target_Reset(skill); //Reset TargetList
                }
            }
        }
        #endregion

        #region Skill_ResetCastingAudioSourceInstantly
        #region Skill_ResetCastingAudioSourceInstantly_OLD
        /// <summary>
        /// Natychmiastowo przerywa wszystkie Skill_AudioSource oprócz Instant, ponieważ instant ma krótki cast i cały zas by przerywało
        /// </summary>
        /// <param name="scrObj_Skill"></param>
        /// <param name="skill"></param>
        /// <param name="live_charStats"></param>
        public static void Skill_ResetCastingAudioSourceInstantly(Skill skill)
        {
            /*if (skill.skill_AudioSourceInstant != null)
            {
                //skill.skill_AudioSourceInstant.Stop();
            }*/

            if (skill.skill_AudioSourceCastable != null)
            {
                skill.skill_AudioSourceCastable.Stop();
            }

            if (skill.skill_AudioSourceHold != null)
            {
                skill.skill_AudioSourceHold.Stop();
            }
        }
        #endregion

        #region Skill_ResetCastingAudioSourceInstantly
        /// <summary>
        /// Natychmiastowo przerywa Skill_AudioSourceCaster
        /// </summary>
        /// <param name="scrObj_Skill"></param>
        /// <param name="skill">Ten GameObject skill</param>
        /// <param name="live_charStats"></param>
        public static void Skill_ResetCastingAudioSourceInstantly_new(Skill skill)
        {            
            if (skill.skill_AudioSourceCaster != null)
            {
                skill.skill_AudioSourceCaster.Stop();
            }
        }
        #endregion
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
            if (skill.skill_CastingVisualEffect != null) skill.skill_CastingVisualEffect.Stop();
            if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorFloatName)) live_charStats.charComponents._Animator.SetFloat(scrObj_Skill.skill_AnimatorFloatName, 0f);
            if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorBoolName)) live_charStats.charComponents._Animator.SetBool(scrObj_Skill.skill_AnimatorBoolName, false);
            if (!string.IsNullOrWhiteSpace(scrObj_Skill.skill_AnimatorTriggerName)) live_charStats.charComponents._Animator.ResetTrigger(scrObj_Skill.skill_AnimatorTriggerName);

            if (skill.skill_AudioSourceCaster != null)
            {
                skill.skill_AudioSourceCaster.volume = Mathf.MoveTowards(skill.skill_AudioSourceCaster.volume, 0, Time.deltaTime / 2); //obniza volume 1-> 0 w 2sek
                if (skill.skill_AudioSourceCaster.volume <= 0.05f) { skill.skill_AudioSourceCaster.Stop(); }
            }

            live_charStats.charStatus._isCasting = false;

            //skill.skill_currentCastingProgress = 0f; //reset progressu przy przerwaniu casta / niespełnieniu warunków    
        }
        #endregion

        #region Skill_ResetCastingAudioSourceInFixedTime
        /// <summary>
        /// Przerywa wszystkie Skill_AudioSource w 1sek, potrzebne do resetu AudioSource w wypadku kiedy kończy się resource(mana) podczas castowania spella
        /// </summary>
        /// <param name="skill"></param>  
        public static void Skill_ResetCastingAudioSourceInFixedTime(Skill skill)
        {
            if (skill.skill_AudioSourceInstant != null)
            {
                skill.skill_AudioSourceInstant.volume = Mathf.MoveTowards(skill.skill_AudioSourceInstant.volume, 0, Time.deltaTime / 2); //obniza volume 1-> 0 w 2sek
                if (skill.skill_AudioSourceInstant.volume <= 0.05f) { skill.skill_AudioSourceInstant.Stop(); }
            }

            if (skill.skill_AudioSourceCastable != null)
            {
                skill.skill_AudioSourceCastable.volume = Mathf.MoveTowards(skill.skill_AudioSourceCastable.volume, 0, Time.deltaTime / 2); //obniza volume 1-> 0 w 2sek
                if (skill.skill_AudioSourceCastable.volume <= 0.05f) { skill.skill_AudioSourceCastable.Stop(); }
            }

            if (skill.skill_AudioSourceHold != null)
            {
                skill.skill_AudioSourceHold.volume = Mathf.MoveTowards(skill.skill_AudioSourceHold.volume, 0, Time.deltaTime / 2); //obniza volume 1-> 0 w 2sek
                if (skill.skill_AudioSourceHold.volume <= 0.05f) { skill.skill_AudioSourceHold.Stop(); }
            }
        }
        #endregion

        #region Skill_CastingTypeCurrentFloatReadOnly
        /// <summary>
        /// <br>Przyjmuje currentCastingIndex z arraya EffetType [i] tego Skilla</br>
        /// <br>Zwraca Boola IsCastingType [FinishedCastable / Instant / Hold]</br>
        /// <br>Bool IsCastingType [FinishedCastable / Instant / Hold] przekazywany jest dalej do wybranej przez switcha Static metody z EffectType</br>
        /// <br>Kiedy trafia do EffectTypeArraya, który przeskakuje for[i] przez każdy z IsCastingTypów odpala odpowiedni switch EffectType</br>
        /// <br>Metoda wykorzystana w "for" EffectTypeArray Skill script</br> 
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
        /// <br>Klasa zatrzymująca cały movement postaci (Movement -> [yAnim = 0], ResetRrigger -> [Jump], Animator bool -> [false])</br>
        /// <br>Nazwy Zmiennych animatora pobierane ze scrObj_Skilla</br>
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

