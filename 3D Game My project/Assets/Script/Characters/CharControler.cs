using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CharControler : MonoBehaviour
{
    
    public CharacterStatus live_charStats;
    public GameObject resourcePrefab;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnValidate()
    {
        live_charStats = GetComponent<CharacterStatus>();

        if (live_charStats.isPlayer) live_charStats.LoadLevel();
        live_charStats.LoadCharStats();
    }

    private void Start()
    {
        live_charStats.SetCharacterPosition();
    }

    private void OnEnable()
    {
        live_charStats = GetComponent<CharacterStatus>();
        
        if (live_charStats.isPlayer) live_charStats.LoadLevel();
        live_charStats.LoadCharStats();

        if (live_charStats.currentAnimator != null)
        {
            ResetAllTriggers();
            ResetInputsAndStates();
        }

    }

    // Update is called once per frame
    private void Update()
    {        
        //PlayerInput
        if (live_charStats.currentPlayer_Input != null) live_charStats.currentPlayer_Input.InputClass();

        //AIController
        if (live_charStats.currentAIController != null && !live_charStats.playerInputEnable) { StartCoroutine(live_charStats.currentAIController.AIControllerRoutine()); }

        if (!live_charStats.isDead) //Je�li �yje
        { 
            //FieldOfView        
            if (live_charStats.currentFieldOfView != null) { StartCoroutine(live_charStats.currentFieldOfView.FOVRoutine()); }

            //MeeleAttack
            if (live_charStats.inputAttacking) StartCoroutine(MeeleAttack());

            //RotatePlayer
            if (live_charStats.playerInputEnable && live_charStats.isPlayer) { live_charStats.currentcharacterMovement.RotatePlayer(); }    //Rotate musi by� w update bo inaczej dziej� si� cyrki

            //Reset/SetPosition
            if (live_charStats.inputSetBackupPosition && live_charStats.isPlayer) live_charStats.SetCharacterPosition();
            if (live_charStats.inputResetPosition) live_charStats.ResetCharacterPosition();

        }

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

        //Testing
        live_charStats.Testing_Z_Key = !live_charStats.Testing_Z_Key;  //w��czanie i wy��czanie booleana przyciskiem ON/OFF
    }

    private void FixedUpdate()
    {
        if (!live_charStats.isDead)
        {
            //Character Movement
            if (live_charStats.currentcharacterMovement != null) { live_charStats.currentcharacterMovement.Movement(); }

            //Resource Regen
            live_charStats.ResourcesRegen();
        }
        
    }


    void LateUpdate()
    {        
        if (live_charStats.currentXP >= live_charStats.currentNeededXP && live_charStats.isPlayer) live_charStats.LevelGain();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void PlayerUpdate()
    {
        if (live_charStats.isPlayer)
        {            
            
            if (live_charStats.isPlayer) live_charStats.LoadLevel();
            live_charStats.LoadCharStats();

            if (live_charStats.currentAnimator != null)
            {
                ResetAllTriggers();
                ResetInputsAndStates();
            }
            
        }
        
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


    IEnumerator DeathAction()
    {
        live_charStats.isDead = true;
        SpawnResourceOrb();

        if (live_charStats.currentAnimator != null)
        {
            ResetAllTriggers();
            live_charStats.currentAnimator.SetBool("ISDead", live_charStats.isDead);
        }
        /*if (live_charStats.GetComponent<CharacterMovement>() != null)
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
        }*/
        
        live_charStats.currentHP = 0;

        yield return new WaitForSeconds(live_charStats.corpseTime);

        Invoke("RespawnAction", live_charStats.respawnTime); //musi by� invoke poniewa� coroutiny przestaj� dzia�a� kiedy object.SetActive(false)!!!
        ResetInputsAndStates();
        
        gameObject.SetActive(false); //lepiej wygl�da ale  czasami si� sypie przy respawnie

    }

    void SpawnResourceOrb()
    {
        GameObject resourceOrb = Instantiate(resourcePrefab);
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

        /*if (live_charStats.GetComponent<NavMeshAgent>() != null)
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
        }*/
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

        live_charStats.fov_isSearchingForTarget = false;
        live_charStats.navMeAge_isCheckingRoutine= false;
       
        live_charStats.spell_OnCoroutine = false;

    }

}
