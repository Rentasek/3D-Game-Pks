using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public NavMeshAgent agent;
    //public Transform playerTransform;
    /*public LayerMask whatIsGround;
    //public LayerMask whatIsPlayer;*/


    public string[] enemiesArray;
    
    [SerializeField] private CharacterStatus live_charStats;
    
    ///////////////////////////////////////////////////////////////////////////////////////

    private void OnEnable()
    {
        live_charStats= GetComponent<CharacterStatus>();
        enemiesArray = live_charStats.currentEnemiesArray;
        agent = live_charStats.currentNavMeshAgent;
        live_charStats.navMeAge_spawnPoint = transform.position;
        agent.stoppingDistance = live_charStats.navMeAge_attackRange;
    }

    ///////////////////////////////////////////////////////////////////////////////////////

    public IEnumerator AIControllerRoutine()
    {
        if (live_charStats.navMeAge_isCheckingRoutine)
        {
            yield break; // ¿eby nie nadpisywa³ coroutine co klatke
        }
        else
        {
            live_charStats.navMeAge_isCheckingRoutine = true;

            yield return new WaitForSeconds(live_charStats.navMeAge_AIRoutineDelay);

            AIControllerCheck();
            live_charStats.navMeAge_isCheckingRoutine = false;
        }
    }

    private void AIControllerCheck()
    {
        //wy³¹czone player input na postaci gracza
        if (!live_charStats.playerInputEnable && GetComponent<NavMeshAgent>().enabled && live_charStats.isPlayer)
        {
            agent.SetDestination(live_charStats.navMeAge_walkPoint);

            if (!live_charStats.inputMouseCurrentMoving && !live_charStats.inputCasting)     //mouse input -> wy³¹cza CheckTagetInRange //dodatkowo input casting
            {
                //LiveCharStats_Base.FieldOfViewTarget(live_charStats);
                CheckForTargetInDynamicSightRange();
                CheckForTargetInAttackRange();
            }

            if (live_charStats.isAttacking)
            {
                //debugging if not null
                if (live_charStats.fov_aquiredTargetGameObject != null && !live_charStats.inputMouseCurrentMoving) //przy wciskaniu przucisku porusazania siê nie lockuje targetu
                {
                    transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform);//Enemy jest zwrócony w stronê Playera
                    live_charStats.navMeAge_walkPoint = live_charStats.fov_aquiredTargetGameObject.transform.position;
                }
            }
            // Jeœli dystans do walkPoint jest mniejszy ni¿ 1f resetuje walkPointSet (wykorzystanie agent.*)            
            if (agent.remainingDistance <= live_charStats.navMeAge_attackRange) { live_charStats.navMeAge_walkPoint = transform.position; }

            if (live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetInAttackRange) Chasing(); // player bez inputa œciga tylko przy close sight range

            if (live_charStats.inputMouseCurrentMoving) live_charStats.navMeAge_walkPoint = live_charStats.navMeAge_mouseWalkPoint; //mouse input -> ovveriride walkPointSet z CheckRange
        }

        else
        //wy³¹czone player input na ka¿dej innej postaci
        if (!live_charStats.playerInputEnable && GetComponent<NavMeshAgent>().enabled && live_charStats.isPlayer == false)
        {
            //LiveCharStats_Base.FieldOfViewTarget(live_charStats);
            CheckForTargetInDynamicSightRange();
            CheckForTargetInSpellRange();
            CheckForTargetInAttackRange();

            if (!live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetAquired && !live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Patrolling();

            //Routine AI_Castowanie Spelli, live_charStats.navMeAge_targetAquired zablokowane poniewa¿ DynamicRange jeœli trafi to nadpisuje target Aquired  
            if (live_charStats.spell != null /*&& live_charStats.navMeAge_targetAquired*/ && live_charStats.spell_targetInSpellRange && !live_charStats.navMeAge_targetInAttackRange
                && live_charStats.currentMP >= 10f) StartCoroutine(AI_SpellRoutine());
            else
            {
                live_charStats.inputCasting = false;
                //Jabky coœ nie dzia³a³o to odblokowaæ
                /*if (live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Chasing();
                if (live_charStats.navMeAge_targetInDynamicSightRange && live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Attacking();*/
            }
            if (live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Chasing();
            if (live_charStats.navMeAge_targetInDynamicSightRange && live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Attacking();
        }

        if (live_charStats.isDead) { StopMovementNavMeshAgent(); }  //Stop Nav Mesh Agent przy œmierci

    }

    private void CheckForTargetInDynamicSightRange() //sprawdzanie czy enemies s¹ w zasiêgu dynamic sight range / angle i zmiana rozmiaru sight range / angle 
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, live_charStats.fov_CurrentDynamicSightRadius)) //dla ka¿dego collidera w zasiêgu wzroku
        {
            if (enemiesArray.Contains(collider.tag))                                         //jeœli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.navMeAge_targetInDynamicSightRange = true;                          //ustawia znaleziony colliderem game objecta jako target               


                if (live_charStats.fov_aquiredTargetGameObject == null) live_charStats.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie z³apa³ targeta w DynamicSight range
                break;
            }
            else live_charStats.navMeAge_targetInDynamicSightRange = false;            
        }
        
        if (live_charStats.navMeAge_targetInDynamicSightRange)
        {
            live_charStats.fov_CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov_CurrentDynamicSightRadius, live_charStats.fov_MinSightRadius, ref live_charStats.fov_CurrentVectorDynamicSightRadius, live_charStats.fov_TimeDynamicSightRadius);
            //dynamiczny sight range zmniejsza siê ¿eby player nie lockowa³ siê na sta³e na targecie
            
            live_charStats.fov_CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov_CurrentDynamicSightAngle, live_charStats.fov_MaxSightAngle, ref live_charStats.fov_CurrentVectorDynamicSightAngle, live_charStats.fov_TimeDynamicSightAngle);
            //dynamiczny sight Angle jeœli jest w range zwiêksza k¹t jeœli jest poza range zmniejsza k¹t
        }
        else
        {
            live_charStats.fov_CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov_CurrentDynamicSightRadius, live_charStats.fov_MaxSightRadius, ref live_charStats.fov_CurrentVectorDynamicSightRadius, live_charStats.fov_TimeDynamicSightRadius);
            //dynamiczny sight range -> wraca do maxymalnego sight range
            
            live_charStats.fov_CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov_CurrentDynamicSightAngle, live_charStats.fov_MinSightAngle, ref live_charStats.fov_CurrentVectorDynamicSightAngle, live_charStats.fov_TimeDynamicSightAngle);
            //dynamiczny sight Angle, poza sight range wraca do minimalnego Sight Angle
        }
    }

    private void CheckForTargetInAttackRange()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, live_charStats.navMeAge_attackRange)) //dla ka¿dego collidera w zasiêgu wzroku
        {
            if (enemiesArray.Contains(collider.tag))                                         //jeœli ma tag zawarty w arrayu enemiesArray
            {                                                      
                live_charStats.navMeAge_targetInAttackRange = true;                          //ustawia znaleziony colliderem game objecta jako target
                
                if (live_charStats.fov_aquiredTargetGameObject == null) live_charStats.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie z³apa³ targeta w attack range
                break;
            }
            else live_charStats.navMeAge_targetInAttackRange = false;
        }
        //Jeœli jest w zasiêgu ataku, triggeruje booleana inputAttacking w charStats, które posy³a go dalej => CharacterMovement
        live_charStats.inputAttacking = live_charStats.navMeAge_targetInAttackRange;
    }

    private void CheckForTargetInSpellRange()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, live_charStats.spell_MaxRadius * live_charStats.spell_AISpellRangeFromMax)) //dla ka¿dego collidera w zasiêgu spella
        {
            if (enemiesArray.Contains(collider.tag))                                         //jeœli ma tag zawarty w arrayu enemiesArray
            {                
                    live_charStats.spell_targetInSpellRange = true;  

                if (live_charStats.fov_aquiredTargetGameObject == null) live_charStats.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie z³apa³ targeta w attack range
                break;
            }
            else live_charStats.spell_targetInSpellRange = false;
        }
    }

    private void Patrolling()
    {        
        //jeœli walkPoint nie jest ustawiony        
        if (!live_charStats.navMeAge_walkPointSet) SearchForWalkPoint();
        //jeœli walkPoint jest ustawiony
        if (live_charStats.navMeAge_walkPointSet)
        {           
            agent.SetDestination(live_charStats.navMeAge_walkPoint);
        }

        // Jeœli dystans do walkPoint jest mniejszy ni¿ 1f resetuje walkPointSet i szuka nowego (wykorzystanie agent.*)
        if (agent.remainingDistance < live_charStats.navMeAge_attackRange) live_charStats.navMeAge_walkPointSet = false;                
    }

    private void SearchForWalkPoint()
    {        
        //wyliczanie randomowego punktu w zasiêgu walkPointRange
        float randomZ = Random.Range(-live_charStats.navMeAge_walkPointRange, live_charStats.navMeAge_walkPointRange);
        float randomX = Random.Range(-live_charStats.navMeAge_walkPointRange, live_charStats.navMeAge_walkPointRange);        

        live_charStats.navMeAge_walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);                                   
        
        //Raycast sprawdza czy walkpoint trafia na terrain, vector osi po której sprawdza =>(-transform.up)
        if (Physics.Raycast(live_charStats.navMeAge_walkPoint, -transform.up, 2f, live_charStats.navMeAge_whatIsGround)&&
            Vector3.Distance(live_charStats.navMeAge_walkPoint, live_charStats.navMeAge_spawnPoint) < live_charStats.navMeAge_wanderingRange)
            //Dodatkowo srawdza czy walkPoint ustawi³ siê w zasiêgu wanderingRange od spawnPointa
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

    public void Chasing() //w Sight Range poza Attack Range
    {
        if (live_charStats.fov_aquiredTargetGameObject != null)//debug
        {            
            agent.SetDestination(live_charStats.fov_aquiredTargetGameObject.transform.position);            
        }
    }

    private IEnumerator AI_SpellRoutine()   //Routine AI_Castowanie Spelli
    {
        if (live_charStats.spell_OnCoroutine)// jeœli true to odbija a¿ nie minie delay              
        {
            transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform); //¿eby trafia³
            yield break; // ¿eby nie nadpisywa³ coroutine co klatke
        }
        else
        {
            live_charStats.spell_OnCoroutine = true;
            transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform);
            agent.SetDestination(transform.position);            
            if (!Physics.Raycast(transform.position, transform.forward, live_charStats.spell_MaxRadius * live_charStats.spell_AISpellRangeFromMax, live_charStats.fov_obstaclesLayerMask)) //raycast ¿eby nie bi³ przez œciany
            {                
                live_charStats.inputCasting = true;                
            }

            yield return new WaitForSeconds(live_charStats.spell_coroutineDelay);            
            live_charStats.spell_OnCoroutine = false;
            live_charStats.inputCasting = false;    
            if (live_charStats.fov_aquiredTargetGameObject != null && live_charStats.gameObject.GetComponent<NavMeshAgent>().enabled == true) 
            {
                agent.SetDestination(live_charStats.fov_aquiredTargetGameObject.transform.position);    //ustawia target tak ¿eby nie biegaæ na Patrolling
            } 
        }
    }

    public void StopMovementNavMeshAgent()
    {
        live_charStats.navMeAge_walkPoint = transform.position; //debugging ¿eby nie próbowa³ uciekaæ

        agent.SetDestination(live_charStats.navMeAge_walkPoint);
        /*agent.stoppingDistance = live_charStats.navMeAge_attackRange;
        //Zatrzymanie agenta przy ataku
        //agent.isStopped = true;*/
    }

    private void Attacking()
    {
        StopMovementNavMeshAgent();
        
        //Enemy jest zwrócony w stronê Playera
        //transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform);        
        
    }
}