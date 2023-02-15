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
        enemiesArray = live_charStats.charInfo.currentEnemiesArray;
        agent = live_charStats.currentNavMeshAgent;
        live_charStats.navMeshAge.navMeAge_spawnPoint = transform.position;
        agent.stoppingDistance = live_charStats.fov.fov_attackRange;
    }

    ///////////////////////////////////////////////////////////////////////////////////////

    public IEnumerator AIControllerRoutine()
    {
        if (live_charStats.navMeshAge.navMeAge_isCheckingAIRoutine)
        {
            yield break; // �eby nie nadpisywa� coroutine co klatke
        }
        else
        {
            live_charStats.navMeshAge.navMeAge_isCheckingAIRoutine = true;

            yield return new WaitForSeconds(live_charStats.navMeshAge.navMeAge_AIRoutineDelay);

            AIControllerCheck();
            live_charStats.navMeshAge.navMeAge_isCheckingAIRoutine = false;
        }
    }

    private void AIControllerCheck()
    {
        //wy��czone player input na postaci gracza
        if (!live_charStats.charInfo.playerInputEnable && GetComponent<NavMeshAgent>().enabled && live_charStats.charInfo.isPlayer)
        {
            agent.SetDestination(live_charStats.navMeshAge.navMeAge_walkPoint);

            if (!live_charStats.characterInput.inputMouseCurrentMoving && !live_charStats.characterInput.inputSecondary)     //mouse input -> wy��cza CheckTagetInRange //dodatkowo input casting
            {
                //LiveCharStats_Base.FieldOfViewTarget(live_charStats);
                CheckForTargetInDynamicSightRange();
                CheckForTargetInAttackRange();
            }

            if (live_charStats.currentCharStatus.isAttacking)
            {
                //debugging if not null
                if (live_charStats.fov.fov_aquiredTargetGameObject != null && !live_charStats.characterInput.inputMouseCurrentMoving) //przy wciskaniu przucisku porusazania si� nie lockuje targetu
                {
                    transform.LookAt(live_charStats.fov.fov_aquiredTargetGameObject.transform);//Enemy jest zwr�cony w stron� Playera
                    live_charStats.navMeshAge.navMeAge_walkPoint = live_charStats.fov.fov_aquiredTargetGameObject.transform.position;
                }
            }
            // Je�li dystans do walkPoint jest mniejszy ni� 1f resetuje walkPointSet (wykorzystanie agent.*)            
            if (agent.remainingDistance <= live_charStats.fov.fov_attackRange) { live_charStats.navMeshAge.navMeAge_walkPoint = transform.position; }

            if (live_charStats.fov.fov_targetInDynamicSightRange && !live_charStats.fov.fov_targetInAttackRange) Chasing(); // player bez inputa �ciga tylko przy close sight range

            if (live_charStats.characterInput.inputMouseCurrentMoving) live_charStats.navMeshAge.navMeAge_walkPoint = live_charStats.navMeshAge.navMeAge_mouseWalkPoint; //mouse input -> ovveriride walkPointSet z CheckRange
        }

        else
        //wy��czone player input na ka�dej innej postaci
        if (!live_charStats.charInfo.playerInputEnable && GetComponent<NavMeshAgent>().enabled && live_charStats.charInfo.isPlayer == false)
        {
            //LiveCharStats_Base.FieldOfViewTarget(live_charStats);
            CheckForTargetInDynamicSightRange();
            CheckForTargetInSpellRange();
            CheckForTargetInAttackRange();

            if (!live_charStats.fov.fov_targetInDynamicSightRange && !live_charStats.fov.fov_targetAquired && !live_charStats.fov.fov_targetInAttackRange && !live_charStats.characterInput.inputSecondary) Patrolling();

            //AI_Castowanie Spelli  
            if (live_charStats.charSkillCombat.skill_secondarySkill != null && live_charStats.charSkillCombat.spell_targetInSpellRange && !live_charStats.fov.fov_targetInAttackRange)
            {
                if (live_charStats.currentMP <= 10f)    //je�li zejdzie do 10 many to nie castuje
                {
                    live_charStats.characterInput.inputSecondary = false;
                }
                else if (live_charStats.currentMP >= 70f)   //je�li nie osi�gnie 70 many to nie castuje
                {
                    AI_SpellCast();
                }
            }
            else
            {
                live_charStats.characterInput.inputSecondary = false;
                //Jabky co� nie dzia�a�o to odblokowa�
                /*if (live_charStats.navMeAge_targetInDynamicSightRange && !live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Chasing();
                if (live_charStats.navMeAge_targetInDynamicSightRange && live_charStats.navMeAge_targetInAttackRange && !live_charStats.inputCasting) Attacking();*/
            }
            if (live_charStats.fov.fov_targetInDynamicSightRange && !live_charStats.fov.fov_targetInAttackRange && !live_charStats.characterInput.inputSecondary) Chasing();
            if (live_charStats.fov.fov_targetInDynamicSightRange && live_charStats.fov.fov_targetInAttackRange && !live_charStats.characterInput.inputSecondary) Attacking();
        }

        if (live_charStats.currentCharStatus.isDead) { StopMovementNavMeshAgent(); }  //Stop Nav Mesh Agent przy �mierci

    }

    private void CheckForTargetInDynamicSightRange() //sprawdzanie czy enemies s� w zasi�gu dynamic sight range / angle i zmiana rozmiaru sight range / angle 
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, live_charStats.fov.fov_CurrentDynamicSightRadius)) //dla ka�dego collidera w zasi�gu wzroku
        {
            if (enemiesArray.Contains(collider.tag))                                         //je�li ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.fov.fov_targetInDynamicSightRange = true;                          //ustawia znaleziony colliderem game objecta jako target               


                if (live_charStats.fov.fov_aquiredTargetGameObject == null) live_charStats.fov.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie z�apa� targeta w DynamicSight range
                break;
            }
            else live_charStats.fov.fov_targetInDynamicSightRange = false;            
        }
        
        if (live_charStats.fov.fov_targetInDynamicSightRange)
        {
            live_charStats.fov.fov_CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov.fov_CurrentDynamicSightRadius, live_charStats.fov.fov_MinSightRadius, ref live_charStats.fov.fov_CurrentVectorDynamicSightRadius, live_charStats.fov.fov_TimeDynamicSightRadius);
            //dynamiczny sight range zmniejsza si� �eby player nie lockowa� si� na sta�e na targecie
            
            live_charStats.fov.fov_CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov.fov_CurrentDynamicSightAngle, live_charStats.fov.fov_MaxSightAngle, ref live_charStats.fov.fov_CurrentVectorDynamicSightAngle, live_charStats.fov.fov_TimeDynamicSightAngle);
            //dynamiczny sight Angle je�li jest w range zwi�ksza k�t je�li jest poza range zmniejsza k�t
        }
        else
        {
            live_charStats.fov.fov_CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov.fov_CurrentDynamicSightRadius, live_charStats.fov.fov_MaxSightRadius, ref live_charStats.fov.fov_CurrentVectorDynamicSightRadius, live_charStats.fov.fov_TimeDynamicSightRadius);
            //dynamiczny sight range -> wraca do maxymalnego sight range
            
            live_charStats.fov.fov_CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov.fov_CurrentDynamicSightAngle, live_charStats.fov.fov_MinSightAngle, ref live_charStats.fov.fov_CurrentVectorDynamicSightAngle, live_charStats.fov.fov_TimeDynamicSightAngle);
            //dynamiczny sight Angle, poza sight range wraca do minimalnego Sight Angle
        }
    }

    private void CheckForTargetInAttackRange()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, live_charStats.fov.fov_attackRange)) //dla ka�dego collidera w zasi�gu wzroku
        {
            if (enemiesArray.Contains(collider.tag))                                         //je�li ma tag zawarty w arrayu enemiesArray
            {                                                      
                live_charStats.fov.fov_targetInAttackRange = true;                          //ustawia znaleziony colliderem game objecta jako target
                
                if (live_charStats.fov.fov_aquiredTargetGameObject == null) live_charStats.fov.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie z�apa� targeta w attack range
                break;
            }
            else live_charStats.fov.fov_targetInAttackRange = false;
        }
        //Je�li jest w zasi�gu ataku, triggeruje booleana inputAttacking w charStats, kt�re posy�a go dalej => CharacterMovement
        live_charStats.characterInput.inputPrimary = live_charStats.fov.fov_targetInAttackRange;
    }

    private void CheckForTargetInSpellRange()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, live_charStats.charSkillCombat.spell_MaxRadius * live_charStats.charSkillCombat.spell_AISpellRangeFromMax)) //dla ka�dego collidera w zasi�gu spella
        {
            if (enemiesArray.Contains(collider.tag))                                         //je�li ma tag zawarty w arrayu enemiesArray
            {                
                    live_charStats.charSkillCombat.spell_targetInSpellRange = true;  

                if (live_charStats.fov.fov_aquiredTargetGameObject == null) live_charStats.fov.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie z�apa� targeta w attack range
                break;
            }
            else live_charStats.charSkillCombat.spell_targetInSpellRange = false;
        }
    }

    private void Patrolling()
    {        
        //je�li walkPoint nie jest ustawiony        
        if (!live_charStats.navMeshAge.navMeAge_walkPointSet) SearchForWalkPoint();
        //je�li walkPoint jest ustawiony
        if (live_charStats.navMeshAge.navMeAge_walkPointSet)
        {           
            agent.SetDestination(live_charStats.navMeshAge.navMeAge_walkPoint);
        }

        // Je�li dystans do walkPoint jest mniejszy ni� 1f resetuje walkPointSet i szuka nowego (wykorzystanie agent.*)
        if (agent.remainingDistance < live_charStats.fov.fov_attackRange) live_charStats.navMeshAge.navMeAge_walkPointSet = false;                
    }

    private void SearchForWalkPoint()
    {        
        //wyliczanie randomowego punktu w zasi�gu walkPointRange
        float randomZ = Random.Range(-live_charStats.navMeshAge.navMeAge_walkPointRange, live_charStats.navMeshAge.navMeAge_walkPointRange);
        float randomX = Random.Range(-live_charStats.navMeshAge.navMeAge_walkPointRange, live_charStats.navMeshAge.navMeAge_walkPointRange);        

        live_charStats.navMeshAge.navMeAge_walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);                                   
        
        //Raycast sprawdza czy walkpoint trafia na terrain, vector osi po kt�rej sprawdza =>(-transform.up)
        if (Physics.Raycast(live_charStats.navMeshAge.navMeAge_walkPoint, -transform.up, 2f, live_charStats.navMeshAge.navMeAge_whatIsGround)&&
            Vector3.Distance(live_charStats.navMeshAge.navMeAge_walkPoint, live_charStats.navMeshAge.navMeAge_spawnPoint) < live_charStats.navMeshAge.navMeAge_wanderingRange)
            //Dodatkowo srawdza czy walkPoint ustawi� si� w zasi�gu wanderingRange od spawnPointa
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

    public void Chasing() //w Sight Range poza Attack Range
    {
        if (live_charStats.fov.fov_aquiredTargetGameObject != null)//debug
        {            
            agent.SetDestination(live_charStats.fov.fov_aquiredTargetGameObject.transform.position);            
        }
    }

    private  void AI_SpellCast()   //AI_Castowanie Spelli
    {
        if (!Physics.Raycast(live_charStats.gameObject.transform.position, live_charStats.gameObject.transform.forward, live_charStats.charSkillCombat.spell_MaxRadius * live_charStats.charSkillCombat.spell_AISpellRangeFromMax, live_charStats.fov.fov_obstaclesLayerMask)) //raycast �eby nie bi� przez �ciany
        {
            live_charStats.characterInput.inputSecondary = true;
            live_charStats.gameObject.transform.LookAt(live_charStats.fov.fov_aquiredTargetGameObject.transform);
            live_charStats.currentNavMeshAgent.SetDestination(live_charStats.gameObject.transform.position);
        }
    }

    public void StopMovementNavMeshAgent()
    {
        live_charStats.navMeshAge.navMeAge_walkPoint = transform.position; //debugging �eby nie pr�bowa� ucieka�

        agent.SetDestination(live_charStats.navMeshAge.navMeAge_walkPoint);
        /*agent.stoppingDistance = live_charStats.navMeAge_attackRange;
        //Zatrzymanie agenta przy ataku
        //agent.isStopped = true;*/
    }

    private void Attacking()
    {
        StopMovementNavMeshAgent();
        
        //Enemy jest zwr�cony w stron� Playera
        //transform.LookAt(live_charStats.fov_aquiredTargetGameObject.transform);        
        
    }
}