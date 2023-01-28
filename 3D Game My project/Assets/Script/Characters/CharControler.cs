using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
        
        live_charStats.SetCharacterPosition();
    }

    private void Start()
    {
        if (live_charStats.isPlayer) live_charStats.LoadLevel();
        live_charStats.LoadCharStats();
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

        //FieldOfView        
        if (live_charStats.currentFieldOfView != null) { StartCoroutine(live_charStats.currentFieldOfView.FOVRoutine()); }

        //AIController
        if (live_charStats.currentAIController != null && !live_charStats.playerInputEnable)
        {
            //StartCoroutine(LiveCharStats_Base.FOVRoutine(live_charStats));

            StartCoroutine(live_charStats.currentAIController.AIControllerRoutine()); 
        }

        if (!live_charStats.isDead) //Jeœli ¿yje
        {   
            //MeeleAttack
            if (live_charStats.inputAttacking) StartCoroutine(MeeleAttack());

            //RotatePlayer
            if (live_charStats.playerInputEnable && live_charStats.isPlayer) { live_charStats.currentcharacterMovement.RotatePlayer(); }    //Rotate musi byæ w update bo inaczej dziej¹ siê cyrki

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

        ///////////////////////////////////////////////////////

        //Testing
        //live_charStats.Testing_Z_Key = !live_charStats.Testing_Z_Key;  //w³¹czanie i wy³¹czanie booleana przyciskiem ON/OFF
        if (live_charStats.isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Comma)) //Testing class get / set 
            {
                StaticTestingClass.SimpleDebugTestingCharStats(live_charStats, false);
                StaticTestingClass.Attacking2(live_charStats);
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                StaticTestingClass.AddCurrentHP(live_charStats, 100f);
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                live_charStats.currentHP = StaticTestingClass.AddCurrentFloat(live_charStats.currentHP, -100f);
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus) && live_charStats.inputRunning)
            {
                live_charStats.currentHP = StaticTestingClass.AddCurrentFloat(live_charStats.currentHP, -100f, 1.2f);
            }
        }

       /* //FieldOfView        
        StartCoroutine(StaticTestingClass.FOVRoutine(live_charStats));
        //StaticTestingClass.FieldOfViewTarget(live_charStats); 
*/
        ///////////////////////////////////////////////////////
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

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private IEnumerator MeeleAttack()
    {   //warunki ustalone tak ¿eby przerwaæ coroutine
        if (live_charStats.isAttacking || live_charStats.currentStam <= live_charStats.currentAttackStamCost)
        {
            //Debug.Log("CoroutineBreak");
            yield break;
        }
        else
        {
            //jeœli nie spe³ni warunków przerwania coroutine
            live_charStats.isAttacking = true;
            live_charStats.currentAnimator.ResetTrigger("Jump"); //¿eby nie czeka³ z jumpem w trakcie ataku w powietrzu
            live_charStats.currentAnimator.SetFloat("AttackCombo", live_charStats.currentComboMeele);
            live_charStats.currentAnimator.SetTrigger("MeeleAttack");
            live_charStats.currentStam -= live_charStats.currentAttackStamCost; //Koszt Stamy przy ataku
            yield return new WaitForSeconds(live_charStats.currentAttackCooldown); //cooldown pomiêdzy pojedynczymi atakami
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
            live_charStats.gameObject.GetComponent<CharacterMovement>().enabled = false;  //przy smierci wy³¹czanie componentów
        }
        if (live_charStats.GetComponent<NavMeshAgent>() != null)
        {
            live_charStats.gameObject.GetComponent<NavMeshAgent>().enabled = false;  //przy smierci wy³¹czanie componentów
        }
        if (live_charStats.GetComponent<NavMeshObstacle>() != null)
        {
            live_charStats.gameObject.GetComponent<NavMeshObstacle>().enabled = false;  //przy smierci wy³¹czanie componentów
        }*/
        
        live_charStats.currentHP = 0;

        yield return new WaitForSeconds(live_charStats.corpseTime);

        Invoke("RespawnAction", live_charStats.respawnTime); //musi byæ invoke poniewa¿ coroutiny przestaj¹ dzia³aæ kiedy object.SetActive(false)!!!
        ResetInputsAndStates();
        
        gameObject.SetActive(false); //lepiej wygl¹da ale  czasami siê sypie przy respawnie

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
            live_charStats.gameObject.GetComponent<NavMeshAgent>().enabled = true;  //przy smierci wy³¹czanie componentów
            live_charStats.fov_isSearchingForTarget = false; //reset coroutine FOVRoutine przy respawnie, bo pózniej goni playera lub przestaje biegaæ

        }
        if (live_charStats.GetComponent<NavMeshObstacle>() != null)
        {
            live_charStats.gameObject.GetComponent<NavMeshObstacle>().enabled = true;  //przy smierci wy³¹czanie componentów
        }
        if (live_charStats.GetComponent<CharacterMovement>() != null)
        {
            live_charStats.gameObject.GetComponent<CharacterMovement>().enabled = true;  //przy smierci wy³¹czanie componentów
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

    public void ResetAllTriggers() //resetuje wszytkie triggery przy œmierci ¿eby nie skaka³ / uderza³ po respwanie
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
    public void ResetInputsAndStates()  //resetuje inputy i statsy przy œmierci ¿eby nie skaka³ / uderza³ po respwanie
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



/// <summary>
/// GizmosDrawers
/// </summary>
#if UNITY_EDITOR //zamiast skryptu w Editor

    /*private void OnDrawGizmos() //rusyje wszystkie
    {
        GizmosDrawer();
    }*/
    private void OnDrawGizmosSelected() //rysuje tylko zaznaczone
    {
        GizmosDrawer();
    }

    private void GizmosDrawer()
    {
        Handles.color = live_charStats.fov_editorRadiusColor;
        //Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov_coneRadius, live_charStats.fov_editorLineThickness); //rysowanie lini po okrêgu
        Handles.DrawSolidArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov_MaxSightRadius);  //rysowanie solid okrêgu

        Handles.color = live_charStats.fov_editorDynamicRadiusColor; //closeSightRadius      
        Handles.DrawSolidArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov_CurrentDynamicSightRadius);  //rysowanie solid okrêgu

        Handles.color = live_charStats.spell_editorAISpellRadiusColor; //SpellAIRange      
        Handles.DrawSolidArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.spell_MaxRadius * live_charStats.spell_AISpellRangeFromMax);  //rysowanie solid okrêgu


        /*  
          Vector3 viewAngleLeft = DirectionFromAngle(transform.eulerAngles.y, -live_charStats.fov_coneAngle / 2); //tworzy view angle w lewo od vectora transform.forward do coneAngle/2
          Vector3 viewAngleRight = DirectionFromAngle(transform.eulerAngles.y, live_charStats.fov_coneAngle / 2); //tworzy view angle w prawo od vectora transform.forward do coneAngle/2
  */

        Handles.color = live_charStats.fov_editorAngleLineColor;
        Handles.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-(live_charStats.fov_CurrentDynamicSightAngle / 2), Vector3.up) * transform.forward * live_charStats.fov_MaxSightRadius, live_charStats.fov_editorLineThickness);//rysowanie lini left
        Handles.DrawLine(transform.position, transform.position + Quaternion.AngleAxis((live_charStats.fov_CurrentDynamicSightAngle / 2), Vector3.up) * transform.forward * live_charStats.fov_MaxSightRadius, live_charStats.fov_editorLineThickness);//rysowanie lini right

        Handles.color = live_charStats.fov_editorAngleColor;
        Handles.DrawSolidArc(transform.position, Vector3.up, /*viewAngleLeft*/Quaternion.AngleAxis(-(live_charStats.fov_CurrentDynamicSightAngle / 2), Vector3.up) * transform.forward, live_charStats.fov_CurrentDynamicSightAngle, live_charStats./*fov_MaxSightRadius*/fov_CurrentDynamicSightRadius); //rysuje coneAngle view               
                                                                                                                                                                                                                                                                                                          //Quaternion.AngleAxis korzysta z lokalnego transforma zamiast skomplikowanego Mathf.sin/cos

        if (live_charStats.navMeAge_walkPointSet)
        {
            Handles.color = live_charStats.fov_editorRaycastColor;
            Handles.DrawLine(transform.position, live_charStats.navMeAge_walkPoint, live_charStats.fov_editorLineThickness);
        }

        if (live_charStats.navMeAge_targetAquired && live_charStats.fov_aquiredTargetGameObject != null)
        {
            Handles.color = live_charStats.fov_editorRaycastColor;
            Handles.DrawLine(transform.position, live_charStats.fov_aquiredTargetGameObject.transform.position, live_charStats.fov_editorLineThickness); //rysowanie lini w kierunku playera jeœli nie zas³ania go obstacle Layer
        }
    }
    
#endif

}
