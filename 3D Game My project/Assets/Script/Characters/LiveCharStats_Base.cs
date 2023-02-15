﻿using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Statyczna klasa powiązana z komponentem Character Status
/// </summary>

public static class LiveCharStats_Base
{
    #region FieldOfView

    //////FieldOfView//////

    /// <summary>
    /// <br>Metoda szuka przeciwnika w dynamicznym Range    
    /// <br><i>Metoda pobiera CharacterStatus(live_charStats) i korzystając z wartości z live_charStats przypisuje live_charStats.fov_aquiredTargetGameObject</i></br>>
    /// </summary>
    /// <param name="live_charStats">Attached CharacterStatus object / Przypięty obiekt CharacterStatus</param>
    /// <returns></returns>    
    public static void FieldOfViewTarget(CharacterStatus live_charStats)
    {
        if (!live_charStats.charSkillCombat.currentSpellRangeSkill.skill_input) //nie zmniejsza range jeśli castuje
        {
            DynamicSightRangeScalling(live_charStats);
        }

        CheckForTargetInDynamicSightRange(live_charStats);
        CheckForTargetInSpellRange(live_charStats, live_charStats.charSkillCombat.currentSpellRangeSkill);
        CheckForTargetInAttackRange(live_charStats, live_charStats.charSkillCombat.currentCloseRangeSkill);
    }

    public static void CheckForTargetInDynamicSightRange(CharacterStatus live_charStats)
    {
        live_charStats.fov.fov_allTargetsInDynamicSightRange = Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov.fov_CurrentDynamicSightRadius);

        //sprawdzanie czy jest enemy przed wrzucaniem targetu, musi być brake po (targetInDynamicRange = true) i zwrócony boolean, bez tego prawdopodobnie się nadpisuje i przeciwnik nie reaguje na gracza

        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov.fov_CurrentDynamicSightRadius)) //dla każdego collidera w zasięgu wzroku
        {
            if (live_charStats.charInfo.currentEnemiesArray.Contains(collider.tag))    //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.fov.fov_targetInDynamicSightRange = true;    //target jest w sight range                 
                break;
            }
            else
            {
                live_charStats.fov.fov_targetInDynamicSightRange = false;                     //target nie jest w sight range
                live_charStats.fov.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                live_charStats.fov.fov_targetAquired = false;
            }
        }

        if (live_charStats.fov.fov_targetInDynamicSightRange)
        {
            foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov.fov_CurrentDynamicSightRadius)) //dla każdego collidera w zasięgu wzroku
            {
                if (live_charStats.charInfo.currentEnemiesArray.Contains(collider.tag))    //jeśli ma tag zawarty w arrayu enemiesArray
                {
                    //live_charStats.fov_targetInDynamicSightRange = true;    //target jest w sight range 

                    Vector3 directionToTarget = (collider.transform.position - live_charStats.gameObject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem

                    if (Vector3.Angle(live_charStats.gameObject.transform.forward, directionToTarget) < live_charStats.fov.fov_CurrentDynamicSightAngle / 2) //sprawdzanie angle wektora forward charactera i direction to target
                                                                                                                                                         //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                    {
                        if (!Physics.Raycast(live_charStats.gameObject.transform.position, directionToTarget, Vector3.Distance(live_charStats.gameObject.transform.position, collider.transform.position), live_charStats.fov.fov_obstaclesLayerMask))  //zmiana DynamicRange na Distance ponieważ wysyłał Raycasta na taką odległość że patrząc na target -> patrzył w ziemie(Terrain / Environment LayerMask)
                        /*                     OKO                                                  
                                                |\                      Tak było, teraz Raycast zatrzymuje się na Targecie -> Vector3.Distance(( Char, Target)
                                                | \  | < - Target       
                                                |  \ |   
                                                |   \|
                                                |    \
                                                |    |\
                                                |    | \< - Raycast
                                                |    |  \
                        ________________________|____|___\____(ziemia(Terrain/Envrironment)) 
                        */
                        {
                            //jeśli raycast do targetu nie jest zasłonięty przez jakiekolwiek obstacles!!
                            live_charStats.fov.fov_aquiredTargetGameObject = collider.gameObject;           //ustawia znaleziony colliderem game objecta jako target
                            live_charStats.fov.fov_targetAquired = true;
                            break;          //najważniejszy jest brake, nie da się jednocześnie porównać listy colliderów z Overlapa z listą tagów z enemiesArray
                                            //foreach sprawdza wszystkie collidery jeśli trafi => target bool = true, ale że sprawdza wszystkie collidery
                                            //to trafia i pudłuje cały czas, dlatego trzeba foreacha zatrzymać breakiem kiedy trafi !!!
                        }
                        else
                        {
                            //live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                            live_charStats.fov.fov_targetAquired = false;
                        }
                    }
                    else
                    {
                        //live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                        live_charStats.fov.fov_targetAquired = false;
                    }
                }
                /*else
                {
                    live_charStats.fov_targetInDynamicSightRange = false;                     //target nie jest w sight range
                    live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                    live_charStats.fov_targetAquired = false;
                }*/
            }
        }

        #region Nie działa do końca i jest wolniejsze od foreach
        ///Nie działa do końca i jest wolniejsze od foreach !!!
        ///
        /*live_charStats.fov_enemyTargetsInDynamicSightRange.Clear; 
       
        
        if(live_charStats.fov_targetInDynamicSightRange == true)
        {
            for (int i = 0; i < Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius).Length; i++)
            {
                for (int j = 0; j < live_charStats.currentEnemiesArray.Length; j++)
                {
                    if (Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)[i].CompareTag(live_charStats.currentEnemiesArray[j]))
                    {
                        Vector3 directionToTarget = (Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)[i].transform.position - live_charStats.gameObject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem Vector3.normalized ==> vector wyrażony w radianach
                                                                                                                                                                                                                                                         //sprawdzanie aktualnie ostatniego elementu z listy
                        if (Vector3.Angle(live_charStats.gameObject.transform.forward, directionToTarget) < live_charStats.fov_CurrentDynamicSightAngle / 2)
                        //sprawdzanie angle wektora forward charactera i direction to target
                        //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                        {
                            //if (!Physics.Raycast(transform.position, directionToTarget, breath_currentFireRadius, live_charStats.fov_obstaclesLayerMask))    //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                            if (live_charStats.fov_enemyTargetsInDynamicSightRange.IndexOf(Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)[i]) < 0) //sprawdza czy nie ma na liście. Jeżeli IndexOf < 0 czyli nie ma obiektów z tym indexem
                            {
                                live_charStats.fov_enemyTargetsInDynamicSightRange.Add(Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)[i]); //przypisuje do listy colliders jeśli ma taga z listy enemies                           

                            }
                            else
                            {
                                live_charStats.fov_enemyTargetsInDynamicSightRange.Remove(Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)[i]);
                            }

                            if (!Physics.Raycast(live_charStats.gameObject.transform.position, directionToTarget, live_charStats.fov_CurrentDynamicSightRadius, live_charStats.fov_obstaclesLayerMask))
                            {
                                //jeśli raycast do targetu nie jest zasłonięty przez jakiekolwiek obstacles!!
                                live_charStats.fov_aquiredTargetGameObject = live_charStats.fov_enemyTargetsInDynamicSightRange.ElementAt(0).gameObject;           //ustawia znaleziony colliderem game objecta jako target
                                live_charStats.fov_targetAquired = true;
                            }
                            else
                            {
                                live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                                live_charStats.fov_targetAquired = false;
                            }
                        }
                    }
                    else
                    {
                        live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                        live_charStats.fov_targetAquired = false;
                    }
                }
            }

            if (live_charStats.fov_enemyTargetsInDynamicSightRange.Count() <= 0)
            {
                live_charStats.fov_targetInDynamicSightRange = false;       //jeśli nie ma żadnych targetów w zasięgu
                live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                live_charStats.fov_targetAquired = false;
            }
        }*/
        #endregion
    }

    private static void DynamicSightRangeScalling(CharacterStatus live_charStats)
    {
        //Dynamiczny Sight Range
        if (live_charStats.fov.fov_targetInDynamicSightRange)
        {
            live_charStats.fov.fov_CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov.fov_CurrentDynamicSightRadius, live_charStats.fov.fov_MinSightRadius, ref live_charStats.fov.fov_CurrentVectorDynamicSightRadius, live_charStats.fov.fov_TimeDynamicSightRadius);
            //dynamiczny sight range zmniejsza się żeby player nie lockował się na stałe na targecie

            live_charStats.fov.fov_CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov.fov_CurrentDynamicSightAngle, live_charStats.fov.fov_MaxSightAngle, ref live_charStats.fov.fov_CurrentVectorDynamicSightAngle, live_charStats.fov.fov_TimeDynamicSightAngle);
            //dynamiczny sight Angle jeśli jest w range zwiększa kąt jeśli jest poza range zmniejsza kąt
        }
        else
        {
            live_charStats.fov.fov_CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov.fov_CurrentDynamicSightRadius, live_charStats.fov.fov_MaxSightRadius, ref live_charStats.fov.fov_CurrentVectorDynamicSightRadius, live_charStats.fov.fov_TimeDynamicSightRadius);
            //dynamiczny sight range -> wraca do maxymalnego sight range

            live_charStats.fov.fov_CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov.fov_CurrentDynamicSightAngle, live_charStats.fov.fov_MinSightAngle, ref live_charStats.fov.fov_CurrentVectorDynamicSightAngle, live_charStats.fov.fov_TimeDynamicSightAngle);
            //dynamiczny sight Angle, poza sight range wraca do minimalnego Sight Angle
        }
    }    


    /// <summary>
    /// Bedzie inputował skilla który jest oznaczony jako close range, niezależnie od ustawionego w scrObj inputa 
    /// </summary>
    /// <param name="live_charStats"></param>
    /// <param name="skill_CloseRange">Skill o bliższym Range z live_charStats</param>
    private static void CheckForTargetInAttackRange(CharacterStatus live_charStats, Skill skill_CloseRange)
    {
        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, skill_CloseRange.scrObj_Skill.skill_MaxRadius)) //dla każdego collidera w zasięgu wzroku
        {
            if (live_charStats.charInfo.currentEnemiesArray.Contains(collider.tag))                                         //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.fov.fov_targetInAttackRange = true;                          //ustawia znaleziony colliderem game objecta jako target

                if (live_charStats.fov.fov_aquiredTargetGameObject == null) live_charStats.fov.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie złapał targeta w attack range
                break;
            }
            else live_charStats.fov.fov_targetInAttackRange = false;
        }
        //Jeśli jest w zasięgu ataku, triggeruje booleana inputAttacking w charStats, które posyła go dalej => CharacterMovement

        Utils.CloseRangeInputSwitcher(live_charStats, skill_CloseRange);
    }

    private static void CheckForTargetInSpellRange(CharacterStatus live_charStats, Skill skill_SpellRange)
    {
        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, skill_SpellRange.scrObj_Skill.skill_MaxRadius * live_charStats.charSkillCombat.spell_AISpellRangeFromMax)) //dla każdego collidera w zasięgu spella
        {
            if (live_charStats.charInfo.currentEnemiesArray.Contains(collider.tag))                                         //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.charSkillCombat.spell_targetInSpellRange = true;

                if (live_charStats.fov.fov_aquiredTargetGameObject == null) live_charStats.fov.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie złapał targeta w attack range
                break;
            }
            else live_charStats.charSkillCombat.spell_targetInSpellRange = false;
        }
    }

    #endregion

    #region AI Controller

    ////////////////////////AIController//////////////////////////////////////
    /// <summary>
    /// <br>Metoda porusza postacią AI   
    /// <br><i>Metoda pobiera CharacterStatus(live_charStats) i korzystając z wartości z live_charStats przypisuje live_charStats.fov_aquiredTargetGameObject</i></br>>
    /// <br><i>Poruszanie postacią AI przy pomocy FieldOfView, używa NavMeshAgent</i></br>>
    /// </summary>
    /// <param name="live_charStats">Attached CharacterStatus object / Przypięty obiekt CharacterStatus</param>
    /// <returns></returns>    
    public static void AIControllerCheck(CharacterStatus live_charStats)
    {
        /* if (!live_charStats.inputCasting) //nie zmniejsza range jeśli castuje
         {
             DynamicSightRangeScalling(live_charStats);
         }*/

        //wyłączone player input na postaci gracza
        if (!live_charStats.charInfo.playerInputEnable && live_charStats.currentNavMeshAgent.enabled && live_charStats.charInfo.isPlayer)
        {
            live_charStats.currentNavMeshAgent.SetDestination(live_charStats.navMeshAge.navMeAge_walkPoint);

            if (!live_charStats.characterInput.inputMouseCurrentMoving && !live_charStats.characterInput.inputSecondary)     //mouse input -> wyłącza CheckTagetInRange //dodatkowo input casting
            {
                /*CheckForTargetInDynamicSightRange(live_charStats);
                CheckForTargetInSpellRange(live_charStats);
                CheckForTargetInAttackRange(live_charStats);   */

                FieldOfViewTarget(live_charStats);
            }

            if (live_charStats.currentCharStatus.isAttacking)
            {
                //debugging if not null
                if (live_charStats.fov.fov_aquiredTargetGameObject != null && !live_charStats.characterInput.inputMouseCurrentMoving) //przy wciskaniu przucisku porusazania się nie lockuje targetu
                {
                    live_charStats.gameObject.transform.LookAt(live_charStats.fov.fov_aquiredTargetGameObject.transform);//Enemy jest zwrócony w stronę Playera
                    live_charStats.navMeshAge.navMeAge_walkPoint = live_charStats.fov.fov_aquiredTargetGameObject.transform.position;
                    StopMovementNavMeshAgent(live_charStats);
                }
            }
            else live_charStats.currentNavMeshAgent.isStopped = false; //żeby odblokować agenta po atakowaniu

            // Jeśli dystans do walkPoint jest mniejszy niż 1f resetuje walkPointSet (wykorzystanie live_charStats.currentNavMeshAgent.*)            
            if (live_charStats.currentNavMeshAgent.remainingDistance <= live_charStats.fov.fov_attackRange)
            {
                live_charStats.navMeshAge.navMeAge_walkPoint = live_charStats.gameObject.transform.position;
            }

            if (live_charStats.fov.fov_targetInDynamicSightRange && !live_charStats.fov.fov_targetInAttackRange) Chasing(live_charStats); // player bez inputa ściga tylko przy close sight range

            if (live_charStats.characterInput.inputMouseCurrentMoving) live_charStats.navMeshAge.navMeAge_walkPoint = live_charStats.navMeshAge.navMeAge_mouseWalkPoint; //mouse input -> ovveriride walkPointSet z CheckRange
        }
        else
        //wyłączone player input na każdej innej postaci
        if (!live_charStats.charInfo.playerInputEnable && live_charStats.currentNavMeshAgent.enabled && !live_charStats.charInfo.isPlayer)
        {
            /* CheckForTargetInDynamicSightRange(live_charStats);
             CheckForTargetInSpellRange(live_charStats);
             CheckForTargetInAttackRange(live_charStats);    */
            FieldOfViewTarget(live_charStats);

            if (!live_charStats.fov.fov_targetInDynamicSightRange && !live_charStats.fov.fov_targetAquired && !live_charStats.fov.fov_targetInAttackRange && !live_charStats.charSkillCombat.currentSpellRangeSkill.skill_input) Patrolling(live_charStats);

            //AI_Castowanie Spelli
            if (live_charStats.charSkillCombat.currentSpellRangeSkill != null && live_charStats.fov.fov_targetAquired && live_charStats.charSkillCombat.spell_targetInSpellRange && !live_charStats.fov.fov_targetInAttackRange)
            {
                if (live_charStats.currentMP <= 10f)    //jeśli zejdzie do 10 many to nie castuje
                {
                    switch (live_charStats.charSkillCombat.currentSpellRangeSkill.scrObj_Skill.skill_InputType)   //to tak jakby komuś chciało się podmienic na inputPrimary dla AI dać Casta a na secondary Melle
                    {
                        case ScrObj_skill.Skill_InputType.primary:
                            live_charStats.characterInput.inputPrimary = false;
                            break;
                        case ScrObj_skill.Skill_InputType.secondary:
                            live_charStats.characterInput.inputSecondary = false;
                            break;                            
                    }
                }
                else if (live_charStats.currentMP >= 70f)   //jeśli nie osiągnie 70 many to nie castuje
                {
                    AI_SpellCast(live_charStats);
                }
            }
            else
            {
                switch (live_charStats.charSkillCombat.currentSpellRangeSkill.scrObj_Skill.skill_InputType)
                {
                    case ScrObj_skill.Skill_InputType.primary:
                        live_charStats.characterInput.inputPrimary = false;
                        break;
                    case ScrObj_skill.Skill_InputType.secondary:
                        live_charStats.characterInput.inputSecondary = false;
                        break;
                }
            }
            if (live_charStats.fov.fov_targetInDynamicSightRange && !live_charStats.fov.fov_targetInAttackRange && !live_charStats.charSkillCombat.currentSpellRangeSkill.skill_input) Chasing(live_charStats);
            if (live_charStats.fov.fov_targetInDynamicSightRange && live_charStats.fov.fov_targetInAttackRange && !live_charStats.charSkillCombat.currentSpellRangeSkill.skill_input) Attacking(live_charStats);
        }

        if (live_charStats.currentCharStatus.isDead) { StopMovementNavMeshAgent(live_charStats); } //Stop Nav Mesh Agent przy śmierci        
    }

    private static void Patrolling(CharacterStatus live_charStats)
    {
        live_charStats.currentNavMeshAgent.isStopped = false; //żeby odblokować agenta
        //jeśli walkPoint nie jest ustawiony        
        if (!live_charStats.navMeshAge.navMeAge_walkPointSet) SearchForWalkPoint(live_charStats);
        //jeśli walkPoint jest ustawiony
        if (live_charStats.navMeshAge.navMeAge_walkPointSet)
        {
            live_charStats.currentNavMeshAgent.SetDestination(live_charStats.navMeshAge.navMeAge_walkPoint);
        }

        // Jeśli dystans do walkPoint jest mniejszy niż 1f resetuje walkPointSet i szuka nowego (wykorzystanie live_charStats.currentNavMeshAgent.*)
        if (live_charStats.currentNavMeshAgent.remainingDistance < live_charStats.fov.fov_attackRange) live_charStats.navMeshAge.navMeAge_walkPointSet = false;
    }

    private static void SearchForWalkPoint(CharacterStatus live_charStats)
    {
        //wyliczanie randomowego punktu w zasięgu walkPointRange
        float randomZ = Random.Range(-live_charStats.navMeshAge.navMeAge_walkPointRange, live_charStats.navMeshAge.navMeAge_walkPointRange);
        float randomX = Random.Range(-live_charStats.navMeshAge.navMeAge_walkPointRange, live_charStats.navMeshAge.navMeAge_walkPointRange);

        live_charStats.navMeshAge.navMeAge_walkPoint = new Vector3(live_charStats.gameObject.transform.position.x + randomX, live_charStats.gameObject.transform.position.y, live_charStats.gameObject.transform.position.z + randomZ);

        //Raycast sprawdza czy walkpoint trafia na terrain, vector osi po której sprawdza =>(-transform.up)
        if (Physics.Raycast(live_charStats.navMeshAge.navMeAge_walkPoint, -live_charStats.gameObject.transform.up, 2f, live_charStats.navMeshAge.navMeAge_whatIsGround) &&
            Vector3.Distance(live_charStats.navMeshAge.navMeAge_walkPoint, live_charStats.navMeshAge.navMeAge_spawnPoint) < live_charStats.navMeshAge.navMeAge_wanderingRange)
        //Dodatkowo srawdza czy walkPoint ustawił się w zasięgu wanderingRange od spawnPointa
        {
            live_charStats.navMeshAge.navMeAge_walkPointSet = true;
            live_charStats.navMeshAge.navMeAge_failsafeCounter = 0;
        }
        else
        {   //mechanizm ochronny w wypadku poscigu za przeciwnikiem, resetuje walkPointSet do spawnPoint
            live_charStats.navMeshAge.navMeAge_failsafeCounter++;
            if (live_charStats.navMeshAge.navMeAge_failsafeCounter == 10)
            {
                live_charStats.navMeshAge.navMeAge_walkPoint = live_charStats.navMeshAge.navMeAge_spawnPoint;
                live_charStats.navMeshAge.navMeAge_walkPointSet = true;
                live_charStats.navMeshAge.navMeAge_failsafeCounter = 0;
            }
        }
    }

    public static void Chasing(CharacterStatus live_charStats) //w Sight Range poza Attack Range
    {
        live_charStats.currentNavMeshAgent.isStopped = false; //żeby odblokować agenta
        if (live_charStats.fov.fov_aquiredTargetGameObject != null)//debug
        {
            live_charStats.currentNavMeshAgent.SetDestination(live_charStats.fov.fov_aquiredTargetGameObject.transform.position);
        }
    }

    private static void AI_SpellCast(CharacterStatus live_charStats)   //AI_Castowanie Spelli
    {
        if (!Physics.Raycast(live_charStats.gameObject.transform.position, live_charStats.gameObject.transform.forward, live_charStats.charSkillCombat.currentSpellRangeSkill.scrObj_Skill.skill_MaxRadius * live_charStats.charSkillCombat.spell_AISpellRangeFromMax, live_charStats.fov.fov_obstaclesLayerMask)) //raycast żeby nie bił przez ściany
        {
            switch (live_charStats.charSkillCombat.currentSpellRangeSkill.scrObj_Skill.skill_InputType)
            {
                case ScrObj_skill.Skill_InputType.primary:
                    live_charStats.characterInput.inputPrimary = true;
                    break;
                case ScrObj_skill.Skill_InputType.secondary:
                    live_charStats.characterInput.inputSecondary = true;
                    break;
            }

            live_charStats.gameObject.transform.LookAt(live_charStats.fov.fov_aquiredTargetGameObject.transform);
            live_charStats.currentNavMeshAgent.SetDestination(live_charStats.gameObject.transform.position);
        }
    }

    public static void StopMovementNavMeshAgent(CharacterStatus live_charStats) //debugging żeby nie próbował uciekać
    {
        live_charStats.navMeshAge.navMeAge_walkPoint = live_charStats.gameObject.transform.position;
        live_charStats.currentNavMeshAgent.SetDestination(live_charStats.navMeshAge.navMeAge_walkPoint);

        //live_charStats.currentNavMeshAgent.isStopped = true;
    }

    private static void Attacking(CharacterStatus live_charStats)
    {
        StopMovementNavMeshAgent(live_charStats);
    }

    #endregion

    #region Character Movement


    ////////////////////////CharacterMovement//////////////////////////////////////
    /// <summary>
    /// <br>Metoda obraca postacią przy pomocy myszki</br>
    /// </summary>
    /// <param name="live_charStats">Attached CharacterStatus object / Przypięty obiekt CharacterStatus</param>
    /// <returns></returns>    
    public static void RotatePlayer(CharacterStatus live_charStats)
    {
        live_charStats.gameObject.transform.Rotate(Vector3.up, live_charStats.characterInput.inputRotateHorizontal);
    }

    /// <summary>
    /// <br>Metoda porusza postacią CharacterControllerem / nadaje speed i velocity do NavMeshAgent</br>
    /// </summary>
    /// <param name="live_charStats">Attached CharacterStatus object / Przypięty obiekt CharacterStatus</param>
    /// <returns></returns>    
    public static void Movement(CharacterStatus live_charStats)
    {
        live_charStats.currentCharStatus.isGrounded = Physics.CheckSphere(live_charStats.gameObject.transform.position, live_charStats.currentCharMove.currentGroundDistance, live_charStats.currentCharMove.currentGroundMask); //sprawdzanie isGrounded
        if (live_charStats.currentCharStatus.isGrounded) live_charStats.currentAnimator.ResetTrigger("Jump");       //resetowanie triggera Jump żeby nie wykonywał animacji po wylądowaniu z opóźnieniem
        live_charStats.currentAnimator.SetBool("IsGrounded", live_charStats.currentCharStatus.isGrounded);          //Przeniesienie status isGrounded do animatora

        if (live_charStats.charInfo.playerInputEnable && live_charStats.charInfo.isPlayer)
        //jeśli w inspektorze jest zaznaczona opcja PlayerInputEnable -> sterowanie z Player_Inputa         

        {
            live_charStats.currentNavMeshAgent.enabled = false;//Wyłączenie NavMeshAgenta

            Movement_PlayerInput(live_charStats); //na fixedUpdate żeby postać nie przyspieszała na wyższych fps

            Jump(live_charStats); //jump działa z animacją na fixedUpdate tylko jak jest GetKey w inpucie, na GetKeyDown nie działa
            if (live_charStats.currentCharMove.currentGravityEnabled) Gravity(live_charStats);//grawitacja musi być na fixed update inaczej za szybko spada, za częsty refresh klatki

            live_charStats.currentCharacterController.Move(live_charStats.currentCharMove.currentMoveVector * Time.deltaTime);

        }
        else if (!live_charStats.charInfo.playerInputEnable)
        {
            live_charStats.currentNavMeshAgent.enabled = true;//Włączenie NavMeshAgenta
            Movement_AgentInput(live_charStats);  //Jeśli nie => AgentInput
        }
        MovementAnimations(live_charStats);
    }

    public static void Movement_AgentInput(CharacterStatus live_charStats)
    {
        live_charStats.currentCharMove.currentMoveInputDirection = live_charStats.currentNavMeshAgent.velocity;

        live_charStats.currentCharStatus.isMoving = (live_charStats.currentNavMeshAgent.velocity != Vector3.zero);  //If moveVector.y > 0 = true <= w skrócie, przypisanie do charStats


        live_charStats.currentNavMeshAgent.speed = live_charStats.currentCharMove.currentRunSpeed; //Zawsze stara się poruszać RunningSpeed


        int localSpeedIndex = 0;
        //Właściwy movement z animacjami
        if (live_charStats.currentCharMove.currentMoveInputDirection != Vector3.zero && live_charStats.currentNavMeshAgent.speed == live_charStats.currentCharMove.currentRunSpeed)

        {
            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, (10f + live_charStats.charInfo.currentCharLevel) * Time.deltaTime); //zużywa f stamy / sekunde

            ////Running Speed 
            if (live_charStats.currentCharMove.currentMoveInputDirection != Vector3.zero && !live_charStats.currentCharStatus.isJumping && !live_charStats.currentCharStatus.isAttacking && live_charStats.currentStam > 5f
                && live_charStats.fov.fov_targetAquired && !live_charStats.charSkillCombat.currentSpellRangeSkill.skill_input && live_charStats.currentNavMeshAgent.remainingDistance > 2 * live_charStats.fov.fov_attackRange)
            //dodatnkowy warunek ->biega tylko jak targetAquired=true, kolejny warnek jeśli nie castuje!!, Kolejny warunek jeśli agent.eemainingDistance > 2* attack range
            {
                live_charStats.currentAnimator.ResetTrigger("MeeleAttack");
                localSpeedIndex = 2;
                live_charStats.currentNavMeshAgent.speed = live_charStats.currentCharMove.currentRunSpeed; //Run
                live_charStats.currentCharMove.currentMoveSpeed = live_charStats.currentCharMove.currentRunSpeed; //zmienna przekazywana do charStats a później do Animatora
            }
            else
            {   ////Walking Speed
                localSpeedIndex = 1;
                live_charStats.currentNavMeshAgent.speed = live_charStats.currentCharMove.currentWalkSpeed; //Sprintowanie bez Staminy -> Walk
                live_charStats.currentCharMove.currentMoveSpeed = live_charStats.currentCharMove.currentWalkSpeed; //zmienna przekazywana do charStats a później do Animatora
            }


        }
        else if (live_charStats.currentCharMove.currentMoveInputDirection == Vector3.zero && !live_charStats.currentCharStatus.isJumping && !live_charStats.currentCharStatus.isAttacking)
        {   ///Idle Speed => speed = 0
            localSpeedIndex = 0;
            live_charStats.currentCharMove.currentMoveSpeed = 0f; //zmienna przekazywana do charStats a później do Animatora
        }
        live_charStats.currentCharStatus.isRunning = (localSpeedIndex == 2);
        live_charStats.currentCharStatus.isWalking = (localSpeedIndex == 1);
        live_charStats.currentCharStatus.isIdle = (localSpeedIndex == 0);
        //specjalnie zrobione na jednej zmiennej tak żeby się ciągle zmieniała, trochę jak enumerator
    }

    private static void Movement_PlayerInput(CharacterStatus live_charStats)
    {
        //Movement -> vectory i transformacje        
        Vector3 transformDirection = live_charStats.gameObject.transform.TransformDirection(live_charStats.currentCharMove.currentMoveInputDirection); //przeniesienie transform direction z World do Local
        Vector3 flatMovement = live_charStats.currentCharMove.currentMoveSpeed * transformDirection;
        live_charStats.currentCharMove.currentMoveVector = new Vector3(flatMovement.x, live_charStats.currentCharMove.currentMoveVector.y, flatMovement.z);

        live_charStats.currentCharStatus.isMoving = (live_charStats.currentCharMove.currentMoveVector.z != 0f);  //If moveVector.y > 0 = true <= w skrócie, przypisanie do charStats

        int localSpeedIndex = 0;
        //Właściwy movement z animacjami
        if (live_charStats.currentCharMove.currentMoveInputDirection != Vector3.zero && live_charStats.characterInput.inputRunning)
        {
            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, (10f + live_charStats.charInfo.currentCharLevel) * Time.deltaTime); //MoveTowards na końcu podaje czas maxymalny na zmianę wartości


            if (live_charStats.currentCharMove.currentMoveInputDirection != Vector3.zero && live_charStats.currentCharMove.currentMoveInputDirection != Vector3.back && !live_charStats.currentCharStatus.isJumping && !live_charStats.currentCharStatus.isAttacking && live_charStats.currentStam > 5f)
            {                                                               //jeśli nie sprintuje do tyłu
                ////Running Speed 
                live_charStats.currentAnimator.ResetTrigger("MeeleAttack");
                localSpeedIndex = 2;
                live_charStats.currentCharMove.currentMoveSpeed = live_charStats.currentCharMove.currentRunSpeed; //Run
            }
            else
            {
                ////Walking Speed
                localSpeedIndex = 1;
                live_charStats.currentCharMove.currentMoveSpeed = live_charStats.currentCharMove.currentWalkSpeed; //Sprintowanie bez Staminy -> Walk
            }
        }
        else if (live_charStats.currentCharMove.currentMoveInputDirection != Vector3.zero && !live_charStats.characterInput.inputRunning && !live_charStats.currentCharStatus.isJumping && !live_charStats.currentCharStatus.isAttacking)
        {   ////Walking Speed
            localSpeedIndex = 1;
            live_charStats.currentCharMove.currentMoveSpeed = live_charStats.currentCharMove.currentWalkSpeed; //Walk
        }
        else if (live_charStats.currentCharMove.currentMoveInputDirection == Vector3.zero && !live_charStats.currentCharStatus.isJumping && !live_charStats.currentCharStatus.isAttacking)
        {   ///Idle Speed => speed = 0
            localSpeedIndex = 0;
            live_charStats.currentCharMove.currentMoveSpeed = 0; //Idle
        }
        live_charStats.currentCharStatus.isRunning = (localSpeedIndex == 2);
        live_charStats.currentCharStatus.isWalking = (localSpeedIndex == 1);
        live_charStats.currentCharStatus.isIdle = (localSpeedIndex == 0);
        //specjalnie zrobione na jednej zmiennej tak żeby się ciągle zmieniała, trochę jak enumerator
    }

    private static void MovementAnimations(CharacterStatus live_charStats)
    {
        if (live_charStats.currentCharMove.currentMoveSpeed == 0f && live_charStats.currentCharStatus.isIdle) Idle(live_charStats);
        else if (live_charStats.currentCharMove.currentMoveSpeed <= live_charStats.currentCharMove.currentWalkSpeed && live_charStats.currentCharStatus.isWalking) Walk(live_charStats);
        else if (live_charStats.currentCharMove.currentMoveSpeed <= live_charStats.currentCharMove.currentRunSpeed && live_charStats.currentCharStatus.isRunning) Run(live_charStats);
    }

    private static void Idle(CharacterStatus live_charStats)
    {
        live_charStats.currentAnimator.SetFloat("yAnim", 0);
    }
    private static void Walk(CharacterStatus live_charStats)
    {
        live_charStats.currentAnimator.SetFloat("yAnim", 0.5f, 0.2f, Time.deltaTime);
    }
    private static void Run(CharacterStatus live_charStats)
    {
        live_charStats.currentAnimator.SetFloat("yAnim", 1, 0.2f, Time.deltaTime);
    }

    private static void Jump(CharacterStatus live_charStats)
    {
        float jumpSpeed = live_charStats.currentCharMove.currentJumpPower;    //jumpPower
        if (live_charStats.currentCharMove.currentMoveSpeed != 0) jumpSpeed = (live_charStats.currentCharMove.currentJumpPower + live_charStats.currentCharMove.currentMoveSpeed);  //w przypadku gdzie jest move speed, dodaje siłę do jump power

        if (live_charStats.characterInput.inputJumping && live_charStats.currentCharStatus.isGrounded && !live_charStats.currentCharStatus.isAttacking && live_charStats.currentStam > 11f)
        {
            //zmiana trybu skakania J key
            live_charStats.currentCharMove.currentMoveVector.y = live_charStats.currentCharMove.currentJumpMode_J_ ? (jumpSpeed) : Mathf.SmoothDamp(live_charStats.currentCharMove.currentMoveVector.y, jumpSpeed, ref live_charStats.currentCharMove.currentMoveVector.y, live_charStats.currentCharMove.currentJumpDeltaTime);
            //jeśli (bool) jumpMode "On"=jumpSpeed(stały): "OFF" = Mathf.SmootDamp <=====

            live_charStats.currentCharStatus.isJumping = true;
            JumpAnimation(live_charStats);
        }
        else
            live_charStats.currentCharStatus.isJumping = false;
    }

    public static void JumpAnimation(CharacterStatus live_charStats)
    {
        live_charStats.currentStam -= (10f + live_charStats.charInfo.currentCharLevel); //Koszt Stamy przy skoku        
        live_charStats.currentAnimator.ResetTrigger("MeeleAttack"); //żeby nie animował ataku w powietrzu           
        live_charStats.currentAnimator.SetFloat("yAnim", 0);
        live_charStats.currentAnimator.SetTrigger("Jump");
    }

    private static void Gravity(CharacterStatus live_charStats)
    {
        if (!live_charStats.currentCharStatus.isGrounded)
        {
            live_charStats.currentCharMove.currentMoveVector.y = Mathf.MoveTowards(live_charStats.currentCharMove.currentMoveVector.y, -live_charStats.currentCharMove.currentGravity, 1f); //move towards działa lepiej, większy przyrost na początku   
        }

        if (live_charStats.currentCharStatus.isGrounded && live_charStats.currentCharMove.currentMoveVector.y < 0)
        {
            live_charStats.currentCharMove.currentMoveVector.y = 0f;
        }
    }


    #region UTILSY
    private static class Utils
    {
        /// <summary>
        /// Uruchamia targetInAttackRange dla  skill_CloseRange
        /// </summary>
        /// <param name="live_charStats"></param>
        /// <param name="skill_CloseRange"></param>
        public static void CloseRangeInputSwitcher(CharacterStatus live_charStats, Skill skill_CloseRange)
        {
            switch (skill_CloseRange.scrObj_Skill.skill_InputType)
            {
                case ScrObj_skill.Skill_InputType.primary:
                    live_charStats.characterInput.inputPrimary = live_charStats.fov.fov_targetInAttackRange;
                    break;
                case ScrObj_skill.Skill_InputType.secondary:
                    live_charStats.characterInput.inputSecondary = live_charStats.fov.fov_targetInAttackRange;
                    break;
            }
        }

        /// <summary>
        /// Uruchamia Input dla  skill_SpellRange
        /// </summary>
        /// <param name="live_charStats"></param>
        /// <param name="skill_CloseRange"></param>
        public static void SpellRangeInputSwitcher(CharacterStatus live_charStats)
        {
            switch (live_charStats.charSkillCombat.currentSpellRangeSkill.scrObj_Skill.skill_InputType)
            {
                case ScrObj_skill.Skill_InputType.primary:
                    live_charStats.characterInput.inputPrimary = true;
                    break;
                case ScrObj_skill.Skill_InputType.secondary:
                    live_charStats.characterInput.inputSecondary = true;
                    break;
            }
        }



        #region StopAllAnimatorMovement
        /// <summary>
        /// Klasa zatrzymująca cały movement postaci [Jump, Movement -> yAnim]
        /// </summary>
        /// <param name="live_charStats"></param>
        public static void StopAllAnimatorMovement(CharacterStatus live_charStats)
        {
            live_charStats.currentAnimator.SetFloat("yAnim", 0);
            live_charStats.currentAnimator.ResetTrigger("Jump");
        }
        #endregion

        #endregion
    }
    #endregion
}