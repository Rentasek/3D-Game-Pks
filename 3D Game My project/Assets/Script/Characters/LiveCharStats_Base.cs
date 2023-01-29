using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



/// <summary>
/// Statyczna klasa powiązana z komponentem Character Status
/// </summary>

public static class LiveCharStats_Base
{

    //////FieldOfView//////

    /// <summary>
    /// <br>Metoda szuka przeciwnika w dynamicznym Range    
    /// <br><i>Metoda pobiera CharacterStatus(live_charStats) i korzystając z watości z live_charStats przypisuje live_charStats.fov_aquiredTargetGameObject</i></br>>
    /// </summary>
    /// <param name="live_charStats">Attached CharacterStatus object / Przypięty obiekt CharacterStatus</param>
    /// <returns></returns>    
    public static void FieldOfViewTarget(CharacterStatus live_charStats)
    {
        DynamicSightRangeScalling(live_charStats);

        CheckForTargetInDynamicSightRange(live_charStats);        
        if (live_charStats.FOV__enemyInDynamicSightRange)
        {
           
            CheckForTargetInSpellRange(live_charStats);
            CheckForTargetInAttackRange(live_charStats);            
        }
        
    }

    public static void CheckForTargetInDynamicSightRange(CharacterStatus live_charStats)
    {
        live_charStats.FOV__allTargetsInDynamicSightRange = Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.FOV__CurrentDynamicSightRadius);

        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.FOV__CurrentDynamicSightRadius)) //dla każdego collidera w zasięgu wzroku
        {
            if (live_charStats.currentEnemiesArray.Contains(collider.tag))    //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.FOV__enemyInDynamicSightRange = true;    //target jest w sight range 

                Vector3 directionToTarget = (collider.transform.position - live_charStats.gameObject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem

                if (Vector3.Angle(live_charStats.gameObject.transform.forward, directionToTarget) < live_charStats.FOV__CurrentDynamicSightAngle / 2) //sprawdzanie angle wektora forward charactera i direction to target
                                                                                                                                                      //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                {
                    if (!Physics.Raycast(live_charStats.gameObject.transform.position, directionToTarget, live_charStats.FOV__CurrentDynamicSightRadius, live_charStats.FOV__obstaclesLayerMask))
                    {
                        //jeśli raycast do targetu nie jest zasłonięty przez jakiekolwiek obstacles!!
                        live_charStats.FOV__aquiredTargetGameObject = collider.gameObject;           //ustawia znaleziony colliderem game objecta jako target
                        live_charStats.FOV__targetAquired = true;
                        break;          //najważniejszy jest brake, nie da się jednocześnie porównać listy colliderów z Overlapa z listą tagów z enemiesArray
                                        //foreach sprawdza wszystkie collidery jeśli trafi => target bool = true, ale że sprawdza wszystkie collidery
                                        //to trafia i pudłuje cały czas, dlatego trzeba foreacha zatrzymać breakiem kiedy trafi !!!
                    }
                    else
                    {
                        //live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                        live_charStats.FOV__targetAquired = false;
                    }
                }
                else
                {
                    //live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                    live_charStats.FOV__targetAquired = false;
                }
            }
            else
            {
                live_charStats.FOV__enemyInDynamicSightRange = false;                     //target nie jest w sight range
                live_charStats.FOV__aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                live_charStats.FOV__targetAquired = false;
            }
        }
    }

    private static void DynamicSightRangeScalling(CharacterStatus live_charStats)
    {
        //Dynamiczny Sight Range
        if (live_charStats.FOV__enemyInDynamicSightRange)
        {
            live_charStats.FOV__CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.FOV__CurrentDynamicSightRadius, live_charStats.FOV__MinSightRadius, ref live_charStats.FOV__CurrentVectorDynamicSightRadius, live_charStats.FOV__TimeDynamicSightRadius);
            //dynamiczny sight range zmniejsza się żeby player nie lockował się na stałe na targecie

            live_charStats.FOV__CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.FOV__CurrentDynamicSightAngle, live_charStats.FOV__MaxSightAngle, ref live_charStats.FOV__CurrentVectorDynamicSightAngle, live_charStats.FOV__TimeDynamicSightAngle);
            //dynamiczny sight Angle jeśli jest w range zwiększa kąt jeśli jest poza range zmniejsza kąt
        }
        else
        {
            live_charStats.FOV__CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.FOV__CurrentDynamicSightRadius, live_charStats.FOV__MaxSightRadius, ref live_charStats.FOV__CurrentVectorDynamicSightRadius, live_charStats.FOV__TimeDynamicSightRadius);
            //dynamiczny sight range -> wraca do maxymalnego sight range

            live_charStats.FOV__CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.FOV__CurrentDynamicSightAngle, live_charStats.FOV__MinSightAngle, ref live_charStats.FOV__CurrentVectorDynamicSightAngle, live_charStats.FOV__TimeDynamicSightAngle);
            //dynamiczny sight Angle, poza sight range wraca do minimalnego Sight Angle
        }
    }

    private static void CheckForTargetInAttackRange(CharacterStatus live_charStats)
    {
        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.FOV__attackRange)) //dla każdego collidera w zasięgu wzroku
        {
            if (live_charStats.currentEnemiesArray.Contains(collider.tag))                                         //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.FOV__inAttackRange = true;                          //ustawia znaleziony colliderem game objecta jako target

                if (live_charStats.FOV__aquiredTargetGameObject == null) live_charStats.FOV__aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie złapał targeta w attack range
                break;
            }
            else live_charStats.FOV__inAttackRange = false;
        }
        //Jeśli jest w zasięgu ataku, triggeruje booleana inputAttacking w charStats, które posyła go dalej => CharacterMovement
        live_charStats.inputAttacking = live_charStats.FOV__inAttackRange;
    }

    private static void CheckForTargetInSpellRange(CharacterStatus live_charStats)
    {
        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.spell_MaxRadius * live_charStats.spell_AISpellRangeFromMax)) //dla każdego collidera w zasięgu spella
        {
            if (live_charStats.currentEnemiesArray.Contains(collider.tag))                                         //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.spell_targetInSpellRange = true;

                if (live_charStats.FOV__aquiredTargetGameObject == null) live_charStats.FOV__aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie złapał targeta w attack range
                break;
            }
            else live_charStats.spell_targetInSpellRange = false;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////AIController//////////////////////////////////////


    public static void AIControllerCheck(CharacterStatus live_charStats)
    {
        //wyłączone player input na postaci gracza
        if (!live_charStats.playerInputEnable && live_charStats.currentNavMeshAgent.enabled && live_charStats.isPlayer)
        {
            live_charStats.currentNavMeshAgent.SetDestination(live_charStats.navMeAge_walkPoint);

            if (!live_charStats.inputMouseCurrentMoving && !live_charStats.inputCasting)     //mouse input -> wyłącza CheckTagetInRange //dodatkowo input casting
            {
                //LiveCharStats_Base.FieldOfViewTarget(live_charStats);
                CheckForTargetInDynamicSightRange(live_charStats);
                CheckForTargetInAttackRange(live_charStats);
            }

            if (live_charStats.isAttacking)
            {
                //debugging if not null
                if (live_charStats.fov_aquiredTargetGameObject != null && !live_charStats.inputMouseCurrentMoving) //przy wciskaniu przucisku porusazania się nie lockuje targetu
                {
                    live_charStats.gameObject.transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform);//Enemy jest zwrócony w stronę Playera
                    live_charStats.navMeAge_walkPoint = live_charStats.fov_aquiredTargetGameObject.transform.position;
                }
            }
            // Jeśli dystans do walkPoint jest mniejszy niż 1f resetuje walkPointSet (wykorzystanie live_charStats.currentNavMeshAgent.*)            
            if (live_charStats.currentNavMeshAgent.remainingDistance <= live_charStats.navMeAge_attackRange) { live_charStats.navMeAge_walkPoint = live_charStats.gameObject.transform.position; }

            if (live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetInAttackRange) Chasing(live_charStats); // player bez inputa ściga tylko przy close sight range

            if (live_charStats.inputMouseCurrentMoving) live_charStats.navMeAge_walkPoint = live_charStats.navMeAge_mouseWalkPoint; //mouse input -> ovveriride walkPointSet z CheckRange
        }

        else
        //wyłączone player input na każdej innej postaci
        if (!live_charStats.playerInputEnable && live_charStats.currentNavMeshAgent.enabled && live_charStats.isPlayer == false)
        {
            //LiveCharStats_Base.FieldOfViewTarget(live_charStats);
            CheckForTargetInDynamicSightRange(live_charStats);
            CheckForTargetInSpellRange(live_charStats);
            CheckForTargetInAttackRange(live_charStats);

            if (!live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetAquired && !live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Patrolling(live_charStats);

            //Routine AI_Castowanie Spelli, live_charStats.navMeAge_targetAquired zablokowane ponieważ DynamicRange jeśli trafi to nadpisuje target Aquired  
            if (live_charStats.spell != null /*&& live_charStats.navMeAge_targetAquired*/ && live_charStats.spell_targetInSpellRange && !live_charStats.navMeAge_targetInAttackRange
                && live_charStats.currentMP >= 10f) ;// StartCoroutine(AI_SpellRoutine(live_charStats));         <----------------Powinnya być coroutine ale nie działa
            else
            {
                live_charStats.inputCasting = false;
                //Jabky coś nie działało to odblokować
                /*if (live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Chasing();
                if (live_charStats.navMeAge_targetInDynamicSightRange && live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Attacking();*/
            }
            if (live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Chasing(live_charStats);
            if (live_charStats.navMeAge_targetInDynamicSightRange && live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Attacking(live_charStats);
        }

        if (live_charStats.isDead) { StopMovementNavMeshAgent(live_charStats); }  //Stop Nav Mesh Agent przy śmierci

    }

    private static void Patrolling(CharacterStatus live_charStats)
    {
        //jeśli walkPoint nie jest ustawiony        
        if (!live_charStats.navMeAge_walkPointSet) SearchForWalkPoint(live_charStats);
        //jeśli walkPoint jest ustawiony
        if (live_charStats.navMeAge_walkPointSet)
        {
            live_charStats.currentNavMeshAgent.SetDestination(live_charStats.navMeAge_walkPoint);
        }

        // Jeśli dystans do walkPoint jest mniejszy niż 1f resetuje walkPointSet i szuka nowego (wykorzystanie live_charStats.currentNavMeshAgent.*)
        if (live_charStats.currentNavMeshAgent.remainingDistance < live_charStats.navMeAge_attackRange) live_charStats.navMeAge_walkPointSet = false;
    }

    private static void SearchForWalkPoint(CharacterStatus live_charStats)
    {
        //wyliczanie randomowego punktu w zasięgu walkPointRange
        float randomZ = Random.Range(-live_charStats.navMeAge_walkPointRange, live_charStats.navMeAge_walkPointRange);
        float randomX = Random.Range(-live_charStats.navMeAge_walkPointRange, live_charStats.navMeAge_walkPointRange);

        live_charStats.navMeAge_walkPoint = new Vector3(live_charStats.gameObject.transform.position.x + randomX, live_charStats.gameObject.transform.position.y, live_charStats.gameObject.transform.position.z + randomZ);

        //Raycast sprawdza czy walkpoint trafia na terrain, vector osi po której sprawdza =>(-transform.up)
        if (Physics.Raycast(live_charStats.navMeAge_walkPoint, -live_charStats.gameObject.transform.up, 2f, live_charStats.navMeAge_whatIsGround) &&
            Vector3.Distance(live_charStats.navMeAge_walkPoint, live_charStats.navMeAge_spawnPoint) < live_charStats.navMeAge_wanderingRange)
        //Dodatkowo srawdza czy walkPoint ustawił się w zasięgu wanderingRange od spawnPointa
        {
            live_charStats.navMeAge_walkPointSet = true;
            live_charStats.navMeAge_failsafeCounter = 0;
        }
        else
        {   //mechanizm ochronny w wypadku poscigu za przeciwnikiem, resetuje walkPointSet do spawnPoint
            live_charStats.navMeAge_failsafeCounter++;
            if (live_charStats.navMeAge_failsafeCounter == 10)
            {
                live_charStats.navMeAge_walkPoint = live_charStats.navMeAge_spawnPoint;
                live_charStats.navMeAge_walkPointSet = true;
                live_charStats.navMeAge_failsafeCounter = 0;
            }
        }
    }

    public static void Chasing(CharacterStatus live_charStats) //w Sight Range poza Attack Range
    {
        if (live_charStats.fov_aquiredTargetGameObject != null)//debug
        {
            live_charStats.currentNavMeshAgent.SetDestination(live_charStats.fov_aquiredTargetGameObject.transform.position);
        }
    }

    private static IEnumerator AI_SpellRoutine(CharacterStatus live_charStats)   //Routine AI_Castowanie Spelli
    {
        if (live_charStats.spell_OnCoroutine)// jeśli true to odbija aż nie minie delay              
        {
            live_charStats.gameObject.transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform); //żeby trafiał
            yield break; // żeby nie nadpisywał coroutine co klatke
        }
        else
        {
            live_charStats.spell_OnCoroutine = true;
            live_charStats.gameObject.transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform);
            live_charStats.currentNavMeshAgent.SetDestination(live_charStats.gameObject.transform.position);
            if (!Physics.Raycast(live_charStats.gameObject.transform.position, live_charStats.gameObject.transform.forward, live_charStats.spell_MaxRadius * live_charStats.spell_AISpellRangeFromMax, live_charStats.fov_obstaclesLayerMask)) //raycast żeby nie bił przez ściany
            {
                live_charStats.inputCasting = true;
            }

            yield return new WaitForSeconds(live_charStats.spell_coroutineDelay);
            live_charStats.spell_OnCoroutine = false;
            live_charStats.inputCasting = false;
            if (live_charStats.fov_aquiredTargetGameObject != null && live_charStats.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled == true)
            {
                live_charStats.currentNavMeshAgent.SetDestination(live_charStats.fov_aquiredTargetGameObject.transform.position);    //ustawia target tak żeby nie biegać na Patrolling
            }
        }
    }

    public static void StopMovementNavMeshAgent(CharacterStatus live_charStats)
    {
        live_charStats.navMeAge_walkPoint = live_charStats.gameObject.transform.position; //debugging żeby nie próbował uciekać

        live_charStats.currentNavMeshAgent.SetDestination(live_charStats.navMeAge_walkPoint);
        /*agent.stoppingDistance = live_charStats.navMeAge_attackRange;
        //Zatrzymanie agenta przy ataku
        //agent.isStopped = true;*/
    }

    private static void Attacking(CharacterStatus live_charStats)
    {
        StopMovementNavMeshAgent(live_charStats);

        //Enemy jest zwrócony w stronę Playera
        //transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform);        

    }



}