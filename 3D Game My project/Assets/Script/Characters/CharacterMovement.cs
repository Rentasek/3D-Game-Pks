using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CharacterMovement : MonoBehaviour
{
    [Space]
    [Header("Character Components")]
    public CharacterStatus live_charStats;   
    public NavMeshAgent live_NavMeshAgent;

    [SerializeField] private CharacterController characterController;    

    [SerializeField] private bool _GravityEnabled = true;

    

    ///////////////////////////////////// Implementacja ///////////////////////////////////////

    private void OnEnable()
    {
        live_charStats = GetComponent<CharacterStatus>();
        live_NavMeshAgent = live_charStats.currentNavMeshAgent;
        characterController = live_charStats.currentCharacterController;
        _GravityEnabled = live_charStats.currentCharMove.currentGravityEnabled;
    }

    public void RotatePlayer()
    {       
        transform.Rotate(Vector3.up, live_charStats.characterInput.inputRotateHorizontal);
    }

    public void Movement()
    {
        live_charStats.currentCharStatus.isGrounded = Physics.CheckSphere(transform.position, live_charStats.currentCharMove.currentGroundDistance, live_charStats.currentCharMove.currentGroundMask); //sprawdzanie isGrounded
        if (live_charStats.currentCharStatus.isGrounded) live_charStats.currentAnimator.ResetTrigger("Jump");       //resetowanie triggera Jump ¿eby nie wykonywa³ animacji po wyl¹dowaniu z opóŸnieniem
        live_charStats.currentAnimator.SetBool("IsGrounded", live_charStats.currentCharStatus.isGrounded);          //Przeniesienie status isGrounded do animatora

        if (live_charStats.charInfo.playerInputEnable && live_charStats.charInfo.isPlayer)
        //jeœli w inspektorze jest zaznaczona opcja PlayerInputEnable -> sterowanie z Player_Inputa         

        {
            live_NavMeshAgent.enabled = false;//Wy³¹czenie NavMeshAgenta

            Movement_PlayerInput(); //na fixedUpdate ¿eby postaæ nie przyspiesza³a na wy¿szych fps

            Jump(); //jump dzia³a z animacj¹ na fixedUpdate tylko jak jest GetKey w inpucie, na GetKeyDown nie dzia³a
            if (_GravityEnabled) Gravity();//grawitacja musi byæ na fixed update inaczej za szybko spada, za czêsty refresh klatki

            characterController.Move(live_charStats.currentCharMove.currentMoveVector * Time.deltaTime);

        }
        else if (!live_charStats.charInfo.playerInputEnable)
        {
            live_NavMeshAgent.enabled = true;//W³¹czenie NavMeshAgenta
            Movement_AgentInput();  //Jeœli nie => AgentInput

        }

        MovementAnimations();

    }

    public void Movement_AgentInput()
    {
        live_charStats.currentCharMove.currentMoveInputDirection = live_NavMeshAgent.velocity;

        live_charStats.currentCharStatus.isMoving = (live_NavMeshAgent.velocity != Vector3.zero);  //If moveVector.y > 0 = true <= w skrócie, przypisanie do charStats


        live_NavMeshAgent.speed = live_charStats.currentCharMove.currentRunSpeed; //Zawsze stara siê poruszaæ RunningSpeed


        int localSpeedIndex = 0;
        //W³aœciwy movement z animacjami
        if (live_charStats.currentCharMove.currentMoveInputDirection != Vector3.zero && live_NavMeshAgent.speed == live_charStats.currentCharMove.currentRunSpeed)
                                                                                                                    
        {
            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, (10f + live_charStats.charInfo.currentCharLevel) * Time.deltaTime); //zu¿ywa f stamy / sekunde

            ////Running Speed 
            if (/*live_charStats.currentMoveInputDirection != Vector3.zero &&*/ !live_charStats.currentCharStatus.isJumping && !live_charStats.currentCharStatus.isAttacking && live_charStats.currentStam > 5f
                && live_charStats.fov.fov_targetAquired && !live_charStats.characterInput.inputSecondary && live_charStats.currentNavMeshAgent.remainingDistance > 2 * live_charStats.fov.fov_attackRange)
                //dodatnkowy warunek ->biega tylko jak targetAquired=true, kolejny warnek jeœli nie castuje!!, Kolejny warunek jeœli agent.eemainingDistance > 2* attack range
            {
                live_charStats.currentAnimator.ResetTrigger("MeeleAttack");
                localSpeedIndex = 2;
                live_NavMeshAgent.speed = live_charStats.currentCharMove.currentRunSpeed; //Run
                live_charStats.currentCharMove.currentMoveSpeed = live_charStats.currentCharMove.currentRunSpeed; //zmienna przekazywana do charStats a póŸniej do Animatora
            }
            else
            {   ////Walking Speed
                localSpeedIndex = 1;
                live_NavMeshAgent.speed = live_charStats.currentCharMove.currentWalkSpeed; //Sprintowanie bez Staminy -> Walk
                live_charStats.currentCharMove.currentMoveSpeed = live_charStats.currentCharMove.currentWalkSpeed; //zmienna przekazywana do charStats a póŸniej do Animatora
            }


        }       
        else if (live_charStats.currentCharMove.currentMoveInputDirection == Vector3.zero && !live_charStats.currentCharStatus.isJumping && !live_charStats.currentCharStatus.isAttacking)
        {   ///Idle Speed => speed = 0
            localSpeedIndex = 0;
            live_charStats.currentCharMove.currentMoveSpeed = 0f; //zmienna przekazywana do charStats a póŸniej do Animatora


        }
        live_charStats.currentCharStatus.isRunning = (localSpeedIndex == 2);
        live_charStats.currentCharStatus.isWalking = (localSpeedIndex == 1);
        live_charStats.currentCharStatus.isIdle = (localSpeedIndex == 0);
        //specjalnie zrobione na jednej zmiennej tak ¿eby siê ci¹gle zmienia³a, trochê jak enumerator
    }
    
    private void Movement_PlayerInput()
    {
        //Movement -> vectory i transformacje        
        Vector3 transformDirection = transform.TransformDirection(live_charStats.currentCharMove.currentMoveInputDirection); //przeniesienie transform direction z World do Local
        Vector3 flatMovement = live_charStats.currentCharMove.currentMoveSpeed * transformDirection;
        live_charStats.currentCharMove.currentMoveVector = new Vector3(flatMovement.x, live_charStats.currentCharMove.currentMoveVector.y, flatMovement.z);

        live_charStats.currentCharStatus.isMoving = (live_charStats.currentCharMove.currentMoveVector.z != 0f);  //If moveVector.y > 0 = true <= w skrócie, przypisanie do charStats

        int localSpeedIndex = 0;
        //W³aœciwy movement z animacjami
        if (live_charStats.currentCharMove.currentMoveInputDirection != Vector3.zero && live_charStats.characterInput.inputRunning) 
        {

            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, (10f + live_charStats.charInfo.currentCharLevel) * Time.deltaTime); //MoveTowards na koñcu podaje czas maxymalny na zmianê wartoœci


            if (live_charStats.currentCharMove.currentMoveInputDirection != Vector3.zero && live_charStats.currentCharMove.currentMoveInputDirection != Vector3.back && !live_charStats.currentCharStatus.isJumping && !live_charStats.currentCharStatus.isAttacking && live_charStats.currentStam > 5f) 
            {                                                               //jeœli nie sprintuje do ty³u
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
        //specjalnie zrobione na jednej zmiennej tak ¿eby siê ci¹gle zmienia³a, trochê jak enumerator


    }

    private void MovementAnimations()
    {
        if (live_charStats.currentCharMove.currentMoveSpeed == 0f && live_charStats.currentCharStatus.isIdle) Idle();
        else if (live_charStats.currentCharMove.currentMoveSpeed <= live_charStats.currentCharMove.currentWalkSpeed && live_charStats.currentCharStatus.isWalking) Walk();
        else if (live_charStats.currentCharMove.currentMoveSpeed <= live_charStats.currentCharMove.currentRunSpeed && live_charStats.currentCharStatus.isRunning) Run();

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


    private void Jump()
    {
        float jumpSpeed = live_charStats.currentCharMove.currentJumpPower;    //jumpPower
        if (live_charStats.currentCharMove.currentMoveSpeed != 0) jumpSpeed = (live_charStats.currentCharMove.currentJumpPower + live_charStats.currentCharMove.currentMoveSpeed );  //w przypadku gdzie jest move speed, dodaje si³ê do jump power

        if (live_charStats.characterInput.inputJumping && live_charStats.currentCharStatus.isGrounded && !live_charStats.currentCharStatus.isAttacking && live_charStats.currentStam > 11f)
        {            
            //zmiana trybu skakania J key
            live_charStats.currentCharMove.currentMoveVector.y = live_charStats.currentCharMove.currentJumpMode_J_ ? (jumpSpeed) : Mathf.SmoothDamp(live_charStats.currentCharMove.currentMoveVector.y, jumpSpeed, ref live_charStats.currentCharMove.currentMoveVector.y, live_charStats.currentCharMove.currentJumpDeltaTime);
            //jeœli (bool) jumpMode "On"=jumpSpeed(sta³y): "OFF" = Mathf.SmootDamp <=====
                
            live_charStats.currentCharStatus.isJumping = true;
            JumpAnimation();
        }
        else
            live_charStats.currentCharStatus.isJumping = false;
    }

    public void JumpAnimation()
    {
        live_charStats.currentStam -= (10f + live_charStats.charInfo.currentCharLevel); //Koszt Stamy przy skoku        
        live_charStats.currentAnimator.ResetTrigger("MeeleAttack"); //¿eby nie animowa³ ataku w powietrzu           
        live_charStats.currentAnimator.SetFloat("yAnim", 0);
        live_charStats.currentAnimator.SetTrigger("Jump");
    }

    private void Gravity()
    {
        if (!live_charStats.currentCharStatus.isGrounded)
        {
            //live_charStats.currentMoveVector.y -= live_charStats.currentGravity * Time.deltaTime/*(Time.deltaTime / 2f)*/;  //moveVector to szybkoœæ poruszania siê wiêc gravity/delta.time (grawitacja/klatkê) jest prawid³owo jako przyspieszenie
                                                                                                                            //(Time.fixedDeltaTime / 1f) = klatki / sekunde     
            
            live_charStats.currentCharMove.currentMoveVector.y = Mathf.MoveTowards(live_charStats.currentCharMove.currentMoveVector.y, -live_charStats.currentCharMove.currentGravity, 1f); //move towards dzia³a lepiej, wiêkszy przyrost na pocz¹tku                                                                                                     ////(Time.deltaTime / 2f) => co 2 klatkê odejmuje wartoœæ
            
        }

        if (live_charStats.currentCharStatus.isGrounded && live_charStats.currentCharMove.currentMoveVector.y < 0)
        {
            live_charStats.currentCharMove.currentMoveVector.y = 0f;
        }
    }

}
