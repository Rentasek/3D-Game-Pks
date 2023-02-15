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

        live_charStats.LoadCharStats();
        if (live_charStats.charInfo._isPlayer) live_charStats.LoadLevel();

        live_charStats.Utils_RestoreResourcesToAll();

        live_charStats.SetCharacterPosition();


    }

    private void Start()
    {
        if (live_charStats.charInfo._isPlayer) live_charStats.LoadLevel();
        live_charStats.LoadCharStats();
    }

    private void OnEnable()
    {
        //live_charStats = GetComponent<CharacterStatus>();
        
        if (live_charStats.charInfo._isPlayer) live_charStats.LoadLevel();
        live_charStats.LoadCharStats();

        if (live_charStats.charComponents._Animator != null)
        {
            ResetAllTriggers();
            ResetInputsAndStates();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //PlayerInput
        if (live_charStats.charComponents._player_Input != null)
        {
            live_charStats.charComponents._player_Input.InputClass();
        }

        if (!live_charStats.charStatus._isDead) //Jeœli ¿yje
        {
            //AIController
            if (live_charStats.charComponents._navMeshAgent != null && !live_charStats.charInfo._playerInputEnable) //Jeœli PplayerInput (3rd person view) nie jest w³¹czony
            {
                StartCoroutine(AIControllerRoutine());
            }

            /*//MeeleAttack - unused
            //if (live_charStats.inputPrimary) StartCoroutine(MeeleAttack());*/

            //RotatePlayer
            if (live_charStats.charInfo._playerInputEnable && live_charStats.charInfo._isPlayer) { LiveCharStats_Base.RotatePlayer(live_charStats); }    //Rotate musi byæ w update bo inaczej dziej¹ siê cyrki            
            //if (live_charStats.playerInputEnable && live_charStats.isPlayer) { live_charStats.currentcharacterMovement.RotatePlayer(); }    //Rotate musi byæ w update bo inaczej dziej¹ siê cyrki

            //Reset/SetPosition
            if (live_charStats.charInput._setBackupPosition && live_charStats.charInfo._isPlayer) live_charStats.SetCharacterPosition();
            if (live_charStats.charInput._resetPosition) live_charStats.ResetCharacterPosition();

        }

        //Death
        if (live_charStats.charStats._hp <= 0f)
        {
            if (!live_charStats.charStatus._isDead)
            {
                StartCoroutine(DeathAction());
            }
        }

        //Reset Position
        if (GetComponentInParent<Transform>().transform.position.y <= -5f) live_charStats.ResetCharacterPosition();

        //Testing
        Testing();
    }
    private void FixedUpdate()
    {
        if (!live_charStats.charStatus._isDead)
        {
            //Character Movement
            if (live_charStats.charComponents._characterController != null ) { LiveCharStats_Base.Movement(live_charStats); }
            

            //Resource Regen
            live_charStats.ResourcesRegen();
        }        
    }

    void LateUpdate()
    {        
        if (live_charStats.charStats._xp >= live_charStats.charStats._neededXP && live_charStats.charInfo._isPlayer) live_charStats.LevelGain();
    }


    /// <summary>
    /// <br>PlayerUpdate class - reload character stars from scriptable object and Character Level</br>
    /// <br>Also resets all triggers, states and inputs</br>
    /// </summary>
    /// <returns></returns>
    public void PlayerUpdate()
    {
        if (live_charStats.charInfo._isPlayer)
        {            
            
            if (live_charStats.charInfo._isPlayer) live_charStats.LoadLevel();
            live_charStats.LoadCharStats();

            if (live_charStats.charComponents._Animator != null)
            {
                ResetAllTriggers();
                ResetInputsAndStates();
            }            
        }        
    }

    /// <summary>
    /// <br>Perform Coroutine FOVRoutine </br>
    /// <br>Use static LiveCharStats_Base class, contains only: </br>
    /// <br>Dynamic FieldOfViewCheck</br>
    /// </summary>
    /// <returns></returns>
    private IEnumerator FOVRoutine()
    {
        if (live_charStats.fov._isSearchingForTarget)
        {
            yield break; // ¿eby nie nadpisywa³ coroutine co klatke
        }
        else
        {
            live_charStats.fov._isSearchingForTarget = true;

            yield return new WaitForSeconds(live_charStats.fov._routineDelay);

            LiveCharStats_Base.FieldOfViewTarget(live_charStats);

            live_charStats.fov._isSearchingForTarget = false;
        }
    }

    /// <summary>
    /// <br>Perform Coroutine AIControllerRoutine </br>
    /// <br>Use static LiveCharStats_Base class, contains: </br>
    /// <br>Dynamic FieldOfViewCheck, NavMeshAgent Controll, AI Movement</br>
    /// </summary>
    /// <returns></returns>
    public IEnumerator AIControllerRoutine()
    {
        if (live_charStats.navMeshAge._isCheckingAIRoutine)
        {
            yield break; // ¿eby nie nadpisywa³ coroutine co klatke
        }
        else
        {
            live_charStats.navMeshAge._isCheckingAIRoutine = true;

            yield return new WaitForSeconds(live_charStats.navMeshAge._AIRoutineDelay);

            LiveCharStats_Base.AIControllerCheck(live_charStats);
            live_charStats.navMeshAge._isCheckingAIRoutine = false;
        }
    }

    /*#region MeleeAttackRoutine - unused
    /// <summary>
    /// Perform Coroutine MeeleAttack action current Animator involved
    /// </summary>
    /// <returns></returns>
    private IEnumerator MeeleAttack()
    {   //warunki ustalone tak ¿eby przerwaæ coroutine
        if (live_charStats.currentCharStatus.isAttacking || live_charStats.currentStam <= live_charStats.charSkillCombat.currentAttackStamCost)
        {
            //Debug.Log("CoroutineBreak");
            yield break;
        }
        else
        {
            //jeœli nie spe³ni warunków przerwania coroutine
            live_charStats.currentCharStatus.isAttacking = true;
            live_charStats.currentAnimator.ResetTrigger("Jump"); //¿eby nie czeka³ z jumpem w trakcie ataku w powietrzu
            live_charStats.currentAnimator.SetFloat("AttackCombo", live_charStats.charSkillCombat.currentComboMeele);
            live_charStats.currentAnimator.SetTrigger("MeeleAttack");
            live_charStats.currentStam -= live_charStats.charSkillCombat.currentAttackStamCost; //Koszt Stamy przy ataku
            yield return new WaitForSeconds(live_charStats.charSkillCombat.currentAttackCooldown); //cooldown pomiêdzy pojedynczymi atakami
            live_charStats.currentCharStatus.isAttacking = false;
            live_charStats.charSkillCombat.currentComboMeele += 0.5f;   //combo do animatora

            if (live_charStats.charSkillCombat.currentComboMeele > 1.0f) live_charStats.charSkillCombat.currentComboMeele = 0;  //reset combo po combo3
        }
    }

    /// <summary>
    /// Perform Coroutine Death Action with Respawn Invoke and reset all Triggers / Inputs / States for current char
    /// </summary>
    /// <returns></returns> 
	#endregion*/

    IEnumerator DeathAction()
    {
        live_charStats.charStatus._isDead = true;
        SpawnResourceOrb();

        if (live_charStats.charComponents._Animator != null)
        {
            ResetAllTriggers();
            live_charStats.charComponents._Animator.SetBool("ISDead", live_charStats.charStatus._isDead);
        }

        //if (live_charStats.inputEnableMouseRotate && live_charStats.isPlayer) { Camera.main.GetComponent<CameraController>().SwitchCursorOptions(); }
        
        live_charStats.charStats._hp = 0;

        yield return new WaitForSeconds(live_charStats.charStats._corpseTime);

        Invoke(nameof(RespawnAction), live_charStats.charStats._respawnTime); //musi byæ invoke poniewa¿ coroutiny przestaj¹ dzia³aæ kiedy object.SetActive(false)!!!
        ResetInputsAndStates();
        
        gameObject.SetActive(false); //lepiej wygl¹da ale  czasami siê sypie przy respawnie
    }

    /// <summary>
    /// Spawn Resource orb after char death
    /// </summary>
    void SpawnResourceOrb()
    {
        GameObject resourceOrb = Instantiate(resourcePrefab);
        resourceOrb.transform.position = gameObject.transform.position;
        resourceOrb.GetComponent<Rigidbody>().AddForce(Random.Range(-2f, 2f), 2f, Random.Range(-2f, 2f), ForceMode.Impulse); //bounce orba
    }

    /// <summary>
    /// <br>Perform Invoked Respawn Action for current char</br>
    /// <br>Also resets all triggers, states and inputs</br>
    /// <br>Reload character stats from scriptable object and Character Level</br>
    /// </summary>
    void RespawnAction()
    {
        gameObject.SetActive(true);

        live_charStats.charStats._hp = live_charStats.charStats._maxHP;

        live_charStats.charStatus._isDead = false;

        if (live_charStats.GetComponent<Animator>() != null)
        {
            live_charStats.charComponents._Animator.SetBool("ISDead", live_charStats.charStatus._isDead);
        }

        if (live_charStats != null )
        {
            if (live_charStats.charInfo._isPlayer) live_charStats.LoadGame();
            else
            {
                live_charStats.ResetCharacterPosition();
                live_charStats.LoadCharStats();
            } 
        }
    }

    /// <summary>
    /// Resets all Animator Triggers from current char
    /// </summary>
    public void ResetAllTriggers() //resetuje wszytkie triggery przy œmierci ¿eby nie skaka³ / uderza³ po respwanie
    {
        foreach (var trigger in live_charStats.charComponents._Animator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
            {
                live_charStats.charComponents._Animator.ResetTrigger(trigger.name);
            }
        }
        live_charStats.charComponents._Animator.Rebind();
        live_charStats.charComponents._Animator.Update(0f);

    }

    /// <summary>
    /// Resets all inputs and states from current char / live_charStats
    /// </summary>
    public void ResetInputsAndStates()  //resetuje inputy i statsy przy œmierci ¿eby nie skaka³ / uderza³ po respwanie
    {        
        live_charStats.charInput._moving = false;
        live_charStats.charInput._running = false;
        live_charStats.charInput._jumping = false;
        live_charStats.charInput._primary = false;
        live_charStats.charInput._secondary = false;       

        live_charStats.charStatus._isMoving = false;
        live_charStats.charStatus._isJumping = false;
        live_charStats.charStatus._isAttacking = false;
        live_charStats.charStatus._isRunning = false;
        live_charStats.charStatus._isWalking = false;
        live_charStats.charStatus._isIdle = false;
        live_charStats.charStatus._isCasting = false;

        live_charStats.fov._isSearchingForTarget = false;
        live_charStats.navMeshAge._isCheckingAIRoutine = false;

        //live_charStats.charSkillCombat.spell_OnCoroutine = false; // old - unused

        //if (live_charStats.isPlayer) live_charStats.inputEnableMouseRotate = true;
    }

    /// <summary>
    /// TestingMethod
    /// </summary>
    private void Testing()
    {
        //live_charStats.Testing_Z_Key = !live_charStats.Testing_Z_Key;  //w³¹czanie i wy³¹czanie booleana przyciskiem ON/OFF
        if (live_charStats.charInfo._isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Comma)) //Testing class get / set 
            {
                StaticTestingClass.SimpleDebugTestingCharStats(live_charStats, false);
                StaticTestingClass.Attacking2(live_charStats);
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                StaticTestingClass.AddCurrentHP(live_charStats, 9999f);
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                live_charStats.charStats._hp = StaticTestingClass.AddCurrentFloat(live_charStats.charStats._hp, -100f);
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus) && live_charStats.charInput._running)
            {
                live_charStats.charStats._hp = StaticTestingClass.AddCurrentFloat(live_charStats.charStats._hp, -100f, 1.2f);
            }
            
            if (Input.GetKey(KeyCode.Alpha8))
            {
                StaticTestingClass.ResetLocalFloat();
            }
            if (Input.GetKey(KeyCode.Alpha9))
            {
                StaticTestingClass.MoveTowardsLocalFloat(100f, 1f);
            }
            if (Input.GetKey(KeyCode.Alpha0))
            {
                StaticTestingClass.MoveTowardsLocalFloat(200f, 0.5f);
            }
        }
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
        Handles.color = live_charStats.fov._editorMaxRadiusColor;
        //Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov_coneRadius, live_charStats.fov_editorLineThickness); //rysowanie lini po okrêgu
        Handles.DrawSolidArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov._maxSightRadius);  //rysowanie solid okrêgu

        Handles.color = live_charStats.fov._editorDynamicRadiusColor; //closeSightRadius      
        Handles.DrawSolidArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov._currentDynamicSightRadius);  //rysowanie solid okrêgu

        Handles.color = live_charStats.charSkillCombat._editorAISpellRadiusColor; //SpellAIRange      
        Handles.DrawSolidArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov._spellRangeSkillMaxRadius * live_charStats.fov._AISpellRangeSkillRadiusFromMax);  //rysowanie solid okrêgu

        Handles.color = live_charStats.fov._editorAngleLineColor;
        Handles.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-(live_charStats.fov._currentDynamicSightAngle / 2), Vector3.up) * transform.forward * live_charStats.fov._maxSightRadius, live_charStats.fov._editorLineThickness);//rysowanie lini left
        Handles.DrawLine(transform.position, transform.position + Quaternion.AngleAxis((live_charStats.fov._currentDynamicSightAngle / 2), Vector3.up) * transform.forward * live_charStats.fov._maxSightRadius, live_charStats.fov._editorLineThickness);//rysowanie lini right

        Handles.color = live_charStats.fov._editorAngleColor;
        Handles.DrawSolidArc(transform.position, Vector3.up, /*viewAngleLeft*/Quaternion.AngleAxis(-(live_charStats.fov._currentDynamicSightAngle / 2), Vector3.up) * transform.forward, live_charStats.fov._currentDynamicSightAngle, live_charStats.fov._currentDynamicSightRadius); //rysuje coneAngle view               
                                                                                                                                                                                                                                                                                                          //Quaternion.AngleAxis korzysta z lokalnego transforma zamiast skomplikowanego Mathf.sin/cos

        if (live_charStats.navMeshAge._walkPointSet)
        {
            Handles.color = live_charStats.fov._editorRaycastColor;
            Handles.DrawLine(transform.position, live_charStats.navMeshAge._walkPoint, live_charStats.fov._editorLineThickness);
        }

        if (live_charStats.fov._targetAquired && live_charStats.fov._aquiredTargetGameObject != null)
        {
            Handles.color = live_charStats.fov._editorRaycastColor;
            Handles.DrawLine(transform.position, live_charStats.fov._aquiredTargetGameObject.transform.position, live_charStats.fov._editorLineThickness); //rysowanie lini w kierunku playera jeœli nie zas³ania go obstacle Layer
        }
    }
#endif
}
