using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{/*
    public NavMeshAgent agent;
    //public Transform playerTransform;
    *//*public LayerMask whatIsGround;
    //public LayerMask whatIsPlayer;*//*


    public string[] enemiesArray;
    
    [SerializeField] private CharacterStatus live_charStats;
    
    ///////////////////////////////////////////////////////////////////////////////////////

    private void OnEnable()
    {
        live_charStats= GetComponent<CharacterStatus>();
        enemiesArray = live_charStats.charInfo._enemiesArray;
        agent = live_charStats.charComponents._navMeshAgent;
        live_charStats.navMeshAge._spawnPoint = transform.position;
        agent.stoppingDistance = live_charStats.fov._closeRangeSkillMinRadius;
    }

    ///////////////////////////////////////////////////////////////////////////////////////

    public IEnumerator AIControllerRoutine()
    {
        if (live_charStats.navMeshAge._isCheckingAIRoutine)
        {
            yield break; // �eby nie nadpisywa� coroutine co klatke
        }
        else
        {
            live_charStats.navMeshAge._isCheckingAIRoutine = true;

            yield return new WaitForSeconds(live_charStats.navMeshAge._AIRoutineDelay);

            AIControllerCheck();
            live_charStats.navMeshAge._isCheckingAIRoutine = false;
        }
    }

    private void AIControllerCheck()
    {
        //wy��czone player input na postaci gracza
        if (!live_charStats.charInfo._playerInputEnable && GetComponent<NavMeshAgent>().enabled && live_charStats.charInfo._isPlayer)
        {
            agent.SetDestination(live_charStats.navMeshAge._walkPoint);

            if (!live_charStats.charInput._mouseCurrentMoving && !live_charStats.charInput._secondary)     //mouse input -> wy��cza CheckTagetInRange //dodatkowo input casting
            {
                //LiveCharStats_Base.FieldOfViewTarget(live_charStats);
                CheckForTargetInDynamicSightRange();
                CheckForTargetInAttackRange();
            }

            if (live_charStats.fov._closeRangeSkill.skill_input)
            {
                //debugging if not null
                if (live_charStats.fov._aquiredTargetGameObject != null && !live_charStats.charInput._mouseCurrentMoving) //przy wciskaniu przucisku porusazania si� nie lockuje targetu
                {
                    transform.LookAt(live_charStats.fov._aquiredTargetGameObject.transform);//Enemy jest zwr�cony w stron� Playera
                    live_charStats.navMeshAge._walkPoint = live_charStats.fov._aquiredTargetGameObject.transform.position;
                }
            }
            // Je�li dystans do walkPoint jest mniejszy ni� 1f resetuje walkPointSet (wykorzystanie agent.*)            
            if (agent.remainingDistance <= live_charStats.fov._closeRangeSkillMinRadius) { live_charStats.navMeshAge._walkPoint = transform.position; }

            if (live_charStats.fov._targetInDynamicSightRange && !live_charStats.fov._targetInAttackRange) Chasing(); // player bez inputa �ciga tylko przy close sight range

            if (live_charStats.charInput._mouseCurrentMoving) live_charStats.navMeshAge._walkPoint = live_charStats.navMeshAge._mouseWalkPoint; //mouse input -> ovveriride walkPointSet z CheckRange
        }

        else
        //wy��czone player input na ka�dej innej postaci
        if (!live_charStats.charInfo._playerInputEnable && GetComponent<NavMeshAgent>().enabled && live_charStats.charInfo._isPlayer == false)
        {
            //LiveCharStats_Base.FieldOfViewTarget(live_charStats);
            CheckForTargetInDynamicSightRange();
            CheckForTargetInSpellRange();
            CheckForTargetInAttackRange();

            if (!live_charStats.fov._targetInDynamicSightRange && !live_charStats.fov._targetAquired && !live_charStats.fov._targetInAttackRange && !live_charStats.charInput._secondary) Patrolling();

            //AI_Castowanie Spelli  
            if (live_charStats.charSkillCombat._secondarySkill != null && live_charStats.fov._targetInSpellRange && !live_charStats.fov._targetInAttackRange)
            {
                if (live_charStats.charStats._mp <= 10f)    //je�li zejdzie do 10 many to nie castuje
                {
                    live_charStats.charInput._secondary = false;
                }
                else if (live_charStats.charStats._mp >= 70f)   //je�li nie osi�gnie 70 many to nie castuje
                {
                    AI_SpellCast();
                }
            }
            else
            {
                live_charStats.charInput._secondary = false;
                //Jabky co� nie dzia�a�o to odblokowa�
                *//*if (live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Chasing();
                if (live_charStats.navMeAge_targetInDynamicSightRange && live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Attacking();*//*
            }
            if (live_charStats.fov._targetInDynamicSightRange && !live_charStats.fov._targetInAttackRange && !live_charStats.charInput._secondary) Chasing();
            if (live_charStats.fov._targetInDynamicSightRange && live_charStats.fov._targetInAttackRange && !live_charStats.charInput._secondary) Attacking();
        }

        if (live_charStats.charStatus._isDead) { StopMovementNavMeshAgent(); }  //Stop Nav Mesh Agent przy �mierci

    }

    private void CheckForTargetInDynamicSightRange() //sprawdzanie czy enemies s� w zasi�gu dynamic sight range / angle i zmiana rozmiaru sight range / angle 
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, live_charStats.fov._currentDynamicSightRadius)) //dla ka�dego collidera w zasi�gu wzroku
        {
            if (enemiesArray.Contains(collider.tag))                                         //je�li ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.fov._targetInDynamicSightRange = true;                          //ustawia znaleziony colliderem game objecta jako target               


                if (live_charStats.fov._aquiredTargetGameObject == null) live_charStats.fov._aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie z�apa� targeta w DynamicSight range
                break;
            }
            else live_charStats.fov._targetInDynamicSightRange = false;            
        }
        
        if (live_charStats.fov._targetInDynamicSightRange)
        {
            live_charStats.fov._currentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov._currentDynamicSightRadius, live_charStats.fov._minSightRadius, ref live_charStats.fov._currentVectorDynamicSightRadius, live_charStats.fov._timeDynamicSightRadius);
            //dynamiczny sight range zmniejsza si� �eby player nie lockowa� si� na sta�e na targecie
            
            live_charStats.fov._currentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov._currentDynamicSightAngle, live_charStats.fov._maxSightAngle, ref live_charStats.fov._currentVectorDynamicSightAngle, live_charStats.fov._timeDynamicSightAngle);
            //dynamiczny sight Angle je�li jest w range zwi�ksza k�t je�li jest poza range zmniejsza k�t
        }
        else
        {
            live_charStats.fov._currentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov._currentDynamicSightRadius, live_charStats.fov._maxSightRadius, ref live_charStats.fov._currentVectorDynamicSightRadius, live_charStats.fov._timeDynamicSightRadius);
            //dynamiczny sight range -> wraca do maxymalnego sight range
            
            live_charStats.fov._currentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov._currentDynamicSightAngle, live_charStats.fov._minSightAngle, ref live_charStats.fov._currentVectorDynamicSightAngle, live_charStats.fov._timeDynamicSightAngle);
            //dynamiczny sight Angle, poza sight range wraca do minimalnego Sight Angle
        }
    }

    private void CheckForTargetInAttackRange()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, live_charStats.fov._closeRangeSkillMinRadius)) //dla ka�dego collidera w zasi�gu wzroku
        {
            if (enemiesArray.Contains(collider.tag))                                         //je�li ma tag zawarty w arrayu enemiesArray
            {                                                      
                live_charStats.fov._targetInAttackRange = true;                          //ustawia znaleziony colliderem game objecta jako target
                
                if (live_charStats.fov._aquiredTargetGameObject == null) live_charStats.fov._aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie z�apa� targeta w attack range
                break;
            }
            else live_charStats.fov._targetInAttackRange = false;
        }
        //Je�li jest w zasi�gu ataku, triggeruje booleana inputAttacking w charStats, kt�re posy�a go dalej => CharacterMovement
        live_charStats.charInput._primary = live_charStats.fov._targetInAttackRange;
    }

    private void CheckForTargetInSpellRange()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, live_charStats.fov._spellRangeSkillMaxRadius * live_charStats.fov._AISpellRangeSkillRadiusFromMax)) //dla ka�dego collidera w zasi�gu spella
        {
            if (enemiesArray.Contains(collider.tag))                                         //je�li ma tag zawarty w arrayu enemiesArray
            {                
                    live_charStats.fov._targetInSpellRange = true;  

                if (live_charStats.fov._aquiredTargetGameObject == null) live_charStats.fov._aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie z�apa� targeta w attack range
                break;
            }
            else live_charStats.fov._targetInSpellRange = false;
        }
    }

    private void Patrolling()
    {        
        //je�li walkPoint nie jest ustawiony        
        if (!live_charStats.navMeshAge._walkPointSet) SearchForWalkPoint();
        //je�li walkPoint jest ustawiony
        if (live_charStats.navMeshAge._walkPointSet)
        {           
            agent.SetDestination(live_charStats.navMeshAge._walkPoint);
        }

        // Je�li dystans do walkPoint jest mniejszy ni� 1f resetuje walkPointSet i szuka nowego (wykorzystanie agent.*)
        if (agent.remainingDistance < live_charStats.fov._closeRangeSkillMinRadius) live_charStats.navMeshAge._walkPointSet = false;                
    }

    private void SearchForWalkPoint()
    {        
        //wyliczanie randomowego punktu w zasi�gu walkPointRange
        float randomZ = Random.Range(-live_charStats.navMeshAge._walkPointRange, live_charStats.navMeshAge._walkPointRange);
        float randomX = Random.Range(-live_charStats.navMeshAge._walkPointRange, live_charStats.navMeshAge._walkPointRange);        

        live_charStats.navMeshAge._walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);                                   
        
        //Raycast sprawdza czy walkpoint trafia na terrain, vector osi po kt�rej sprawdza =>(-transform.up)
        if (Physics.Raycast(live_charStats.navMeshAge._walkPoint, -transform.up, 2f, live_charStats.navMeshAge._whatIsGround)&&
            Vector3.Distance(live_charStats.navMeshAge._walkPoint, live_charStats.navMeshAge._spawnPoint) < live_charStats.navMeshAge._wanderingRange)
            //Dodatkowo srawdza czy walkPoint ustawi� si� w zasi�gu wanderingRange od spawnPointa
            {
                live_charStats.navMeshAge._walkPointSet = true;
                live_charStats.navMeshAge._failsafeCounter = 0;
            }
        else
        {   //mechanizm ochronny w wypadku poscigu za przeciwnikiem, resetuje walkPointSet do spawnPoint
            live_charStats.navMeshAge._failsafeCounter++;
            if (live_charStats.navMeshAge._failsafeCounter == 10)
            {
                live_charStats.navMeshAge._walkPoint = live_charStats.navMeshAge._spawnPoint;
                live_charStats.navMeshAge._walkPointSet = true;
                live_charStats.navMeshAge._failsafeCounter = 0;
            }
        }
    }

    public void Chasing() //w Sight Range poza Attack Range
    {
        if (live_charStats.fov._aquiredTargetGameObject != null)//debug
        {            
            agent.SetDestination(live_charStats.fov._aquiredTargetGameObject.transform.position);            
        }
    }

    private  void AI_SpellCast()   //AI_Castowanie Spelli
    {
        if (!Physics.Raycast(live_charStats.gameObject.transform.position, live_charStats.gameObject.transform.forward, live_charStats.fov._spellRangeSkillMaxRadius * live_charStats.fov._AISpellRangeSkillRadiusFromMax, live_charStats.fov._obstaclesLayerMask)) //raycast �eby nie bi� przez �ciany
        {
            live_charStats.charInput._secondary = true;
            live_charStats.gameObject.transform.LookAt(live_charStats.fov._aquiredTargetGameObject.transform);
            live_charStats.charComponents._navMeshAgent.SetDestination(live_charStats.gameObject.transform.position);
        }
    }

    public void StopMovementNavMeshAgent()
    {
        live_charStats.navMeshAge._walkPoint = transform.position; //debugging �eby nie pr�bowa� ucieka�

        agent.SetDestination(live_charStats.navMeshAge._walkPoint);
        *//*agent.stoppingDistance = live_charStats.navMeAge_attackRange;
        //Zatrzymanie agenta przy ataku
        //agent.isStopped = true;*//*
    }

    private void Attacking()
    {
        StopMovementNavMeshAgent();
        
        //Enemy jest zwr�cony w stron� Playera
        //transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform);        
        
    }
*/
}