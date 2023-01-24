using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimations : MonoBehaviour
{
    
    public CharacterStatus live_charStats;
    public GameObject resourcePrefab;

    private void OnEnable()
    {
        live_charStats = GetComponent<CharacterStatus>();
        if (live_charStats.currentAnimator != null)
        {
            ResetAllTriggers();
            ResetInputsAndStates();
        }

    }


    private void FixedUpdate()
    {
        

    }



    // Update is called once per frame
    private void Update()
    {
        //Movement
        MovementAnimations();

        //MeeleAttack
        if (live_charStats.inputAttacking) StartCoroutine(MeeleAttack());

        //Jumping
        if (live_charStats.isJumping) JumpAnimation();

        //Death
        if (live_charStats.currentHP <= 0f)
        {
            if (!live_charStats.isDead)
            {

                StartCoroutine(DeathAction());

            }

        }
        //Reset Position
        if (GetComponentInParent<Transform>().transform.position.y <= -5f) live_charStats.ResetCharacterPosition();

    }

    public void PlayerUpdate()
    {
        if (live_charStats.isPlayer)
        {
            if (live_charStats.currentAnimator != null)
            {
                ResetAllTriggers();
                ResetInputsAndStates();
            }
            
        }
        
    }
    

    private void MovementAnimations()
    {
        if (live_charStats.currentMoveSpeed == 0f && live_charStats.isIdle) Idle();
        else if (live_charStats.currentMoveSpeed <= live_charStats.currentWalkSpeed && live_charStats.isWalking) Walk();
        else if (live_charStats.currentMoveSpeed <= live_charStats.currentRunSpeed && live_charStats.isRunning) Run();

    }


    private void Idle()
    {
        live_charStats.currentAnimator.SetFloat("yAnim", 0);
    }
    private void Walk()
    {
        live_charStats.currentAnimator.SetFloat("yAnim", 0.5f, 0.2f, Time.deltaTime);
    }
    private void Run()
    {
        live_charStats.currentAnimator.SetFloat("yAnim", 1, 0.2f, Time.deltaTime);
    }

    private IEnumerator MeeleAttack()
    {   //warunki ustalone tak �eby przerwa� coroutine
        if (live_charStats.isAttacking || live_charStats.currentStam <= live_charStats.currentAttackStamCost)
        {
            //Debug.Log("CoroutineBreak");
            yield break;
        }
        else
        {
            //je�li nie spe�ni warunk�w przerwania coroutine
            live_charStats.isAttacking = true;
            live_charStats.currentAnimator.ResetTrigger("Jump"); //�eby nie czeka� z jumpem w trakcie ataku w powietrzu
            live_charStats.currentAnimator.SetFloat("AttackCombo", live_charStats.currentComboMeele);
            live_charStats.currentAnimator.SetTrigger("MeeleAttack");
            live_charStats.currentStam -= live_charStats.currentAttackStamCost; //Koszt Stamy przy ataku
            yield return new WaitForSeconds(live_charStats.currentAttackCooldown); //cooldown pomi�dzy pojedynczymi atakami
            live_charStats.isAttacking = false;
            live_charStats.currentComboMeele += 0.5f;   //combo do animatora

            if (live_charStats.currentComboMeele > 1.0f) live_charStats.currentComboMeele = 0;  //reset combo po combo3
        }
    }


    public void JumpAnimation()
    {
        live_charStats.currentStam -= 10f; //Koszt Stamy przy skoku
        //live_charStats.isJumping = true;
        live_charStats.currentAnimator.ResetTrigger("MeeleAttack"); //�eby nie animowa� ataku w powietrzu           
        live_charStats.currentAnimator.SetFloat("yAnim", 0);
        live_charStats.currentAnimator.SetTrigger("Jump");
    }



    IEnumerator DeathAction()
    {
        live_charStats.isDead = true;
        SpawnResourceOrb();

        if (live_charStats.GetComponent<Animator>() != null)
        {
            ResetAllTriggers();
            live_charStats.currentAnimator.SetBool("ISDead", live_charStats.isDead);
        }
        if (live_charStats.GetComponent<CharacterMovement>() != null)
        {
            live_charStats.gameObject.GetComponent<CharacterMovement>().enabled = false;  //przy smierci wy��czanie component�w
        }
        if (live_charStats.GetComponent<NavMeshAgent>() != null)
        {
            live_charStats.gameObject.GetComponent<NavMeshAgent>().enabled = false;  //przy smierci wy��czanie component�w
        }
        if (live_charStats.GetComponent<NavMeshObstacle>() != null)
        {
            live_charStats.gameObject.GetComponent<NavMeshObstacle>().enabled = false;  //przy smierci wy��czanie component�w
        }

        yield return new WaitForSeconds(live_charStats.corpseTime);

        Invoke("RespawnAction", live_charStats.respawnTime); //musi by� invoke poniewa� coroutiny przestaj� dzia�a� kiedy object.SetActive(false)!!!
        ResetInputsAndStates();
        
        gameObject.SetActive(false); //lepiej wygl�da ale  czasami si� sypie przy respawnie

    }

    void SpawnResourceOrb()
    {
        GameObject resourceOrb = Instantiate(resourcePrefab);
        //GameObject resourceOrb = resourcePrefab;
        resourceOrb.transform.position = gameObject.transform.position;
        resourceOrb.GetComponent<Rigidbody>().AddForce(Random.Range(-2f, 2f), 2f, Random.Range(-2f, 2f), ForceMode.Impulse); //bounce orba
    }


    void RespawnAction()
    {
        gameObject.SetActive(true);

        live_charStats.currentHP = live_charStats.currentMaxHP;

        live_charStats.isDead = false;

        if (live_charStats.GetComponent<Animator>() != null)
        {
            live_charStats.currentAnimator.SetBool("ISDead", live_charStats.isDead);
        }

        if (live_charStats.GetComponent<NavMeshAgent>() != null)
        {
            live_charStats.gameObject.GetComponent<NavMeshAgent>().enabled = true;  //przy smierci wy��czanie component�w
            live_charStats.fov_isSearchingForTarget = false; //reset coroutine FOVRoutine przy respawnie, bo p�zniej goni playera lub przestaje biega�

        }
        if (live_charStats.GetComponent<NavMeshObstacle>() != null)
        {
            live_charStats.gameObject.GetComponent<NavMeshObstacle>().enabled = true;  //przy smierci wy��czanie component�w
        }
        if (live_charStats.GetComponent<CharacterMovement>() != null)
        {
            live_charStats.gameObject.GetComponent<CharacterMovement>().enabled = true;  //przy smierci wy��czanie component�w
        }
        if (live_charStats != null )
        {
            if (live_charStats.isPlayer) live_charStats.LoadGame();
            else
            {
                live_charStats.ResetCharacterPosition();
                live_charStats.LoadCharStats();
            }
             
            
        }
    }

    public void ResetAllTriggers() //resetuje wszytkie triggery przy �mierci �eby nie skaka� / uderza� po respwanie
    {
        foreach (var trigger in live_charStats.currentAnimator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
            {
                live_charStats.currentAnimator.ResetTrigger(trigger.name);
            }
        }
        live_charStats.currentAnimator.Rebind();
        live_charStats.currentAnimator.Update(0f);
    }
    public void ResetInputsAndStates()  //resetuje inputy i statsy przy �mierci �eby nie skaka� / uderza� po respwanie
    {        
        live_charStats.inputMoving = false;
        live_charStats.inputRunning = false;
        live_charStats.inputJumping = false;
        live_charStats.inputAttacking = false;
        live_charStats.inputCasting = false;       

        live_charStats.isMoving = false;
        live_charStats.isJumping = false;
        live_charStats.isAttacking = false;
        live_charStats.isRunning = false;
        live_charStats.isWalking = false;
        live_charStats.isIdle = false;
        live_charStats.isCasting = false;

        live_charStats.spell_OnCoroutine = false;

    }

}
