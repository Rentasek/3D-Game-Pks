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
    }

    // Start is called before the first frame update
    void Start()
    {        
        
    }

    private void Update() //Update najszybszy update => do inputa
    {
      /*  if (live_charStats.playerInputEnable && live_charStats.isPlayer)
        {
            Rotate(); 
            //Jump();
        }*/
    }

    private void FixedUpdate() //Na FixedUpdate lepiej chodzi, p�ynnie !!! //Update razem z Physics
    {
     //   Movement();
    }

    public void RotatePlayer()
    {       
        transform.Rotate(Vector3.up, live_charStats.inputRotateHorizontal);
    }

    public void Movement()
    {
        live_charStats.isGrounded = Physics.CheckSphere(transform.position, live_charStats.currentGroundDistance, live_charStats.currentGroundMask); //sprawdzanie isGrounded
        if (live_charStats.isGrounded) live_charStats.currentAnimator.ResetTrigger("Jump");       //resetowanie triggera Jump �eby nie wykonywa� animacji po wyl�dowaniu z op�nieniem
        live_charStats.currentAnimator.SetBool("IsGrounded", live_charStats.isGrounded);          //Przeniesienie status isGrounded do animatora

        if (live_charStats.playerInputEnable && live_charStats.isPlayer)
        //je�li w inspektorze jest zaznaczona opcja PlayerInputEnable -> sterowanie z Player_Inputa         

        {
            live_NavMeshAgent.enabled = false;//Wy��czenie NavMeshAgenta

            Movement_PlayerInput(); //na fixedUpdate �eby posta� nie przyspiesza�a na wy�szych fps

            Jump(); //jump dzia�a z animacj� na fixedUpdate tylko jak jest GetKey w inpucie, na GetKeyDown nie dzia�a
            if (_GravityEnabled) Gravity();//grawitacja musi by� na fixed update inaczej za szybko spada, za cz�sty refresh klatki

            characterController.Move(live_charStats.currentMoveVector * Time.deltaTime);

        }
        else if (!live_charStats.playerInputEnable)
        {
            live_NavMeshAgent.enabled = true;//W��czenie NavMeshAgenta
            Movement_AgentInput();  //Je�li nie => AgentInput

        }

        MovementAnimations();

    }

    public void Movement_AgentInput()
    {
        live_charStats.currentMoveInputDirection = live_NavMeshAgent.velocity;

        live_charStats.isMoving = (live_NavMeshAgent.velocity != Vector3.zero);  //If moveVector.y > 0 = true <= w skr�cie, przypisanie do charStats


        live_NavMeshAgent.speed = live_charStats.currentRunSpeed; //Zawsze stara si� porusza� RunningSpeed


        int localSpeedIndex = 0;
        //W�a�ciwy movement z animacjami
        if (live_charStats.currentMoveInputDirection != Vector3.zero && live_NavMeshAgent.speed == live_charStats.currentRunSpeed)
                                                                                                                    
        {
            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, (10f + live_charStats.currentCharLevel) * Time.deltaTime); //zu�ywa f stamy / sekunde

            ////Running Speed 
            if (/*live_charStats.currentMoveInputDirection != Vector3.zero &&*/ !live_charStats.isJumping && !live_charStats.isAttacking && live_charStats.currentStam > 5f
                && live_charStats.navMeAge_targetAquired && !live_charStats.inputCasting && live_charStats.currentNavMeshAgent.remainingDistance > 2 * live_charStats.navMeAge_attackRange)
                //dodatnkowy warunek ->biega tylko jak targetAquired=true, kolejny warnek je�li nie castuje!!, Kolejny warunek je�li agent.eemainingDistance > 2* attack range
            {
                live_charStats.currentAnimator.ResetTrigger("MeeleAttack");
                localSpeedIndex = 2;
                live_NavMeshAgent.speed = live_charStats.currentRunSpeed; //Run
                live_charStats.currentMoveSpeed = live_charStats.currentRunSpeed; //zmienna przekazywana do charStats a p�niej do Animatora
            }
            else
            {   ////Walking Speed
                localSpeedIndex = 1;
                live_NavMeshAgent.speed = live_charStats.currentWalkSpeed; //Sprintowanie bez Staminy -> Walk
                live_charStats.currentMoveSpeed = live_charStats.currentWalkSpeed; //zmienna przekazywana do charStats a p�niej do Animatora
            }


        }       
        else if (live_charStats.currentMoveInputDirection == Vector3.zero && !live_charStats.isJumping && !live_charStats.isAttacking)
        {   ///Idle Speed => speed = 0
            localSpeedIndex = 0;
            live_charStats.currentMoveSpeed = 0f; //zmienna przekazywana do charStats a p�niej do Animatora


        }
        live_charStats.isRunning = (localSpeedIndex == 2);
        live_charStats.isWalking = (localSpeedIndex == 1);
        live_charStats.isIdle = (localSpeedIndex == 0);
        //specjalnie zrobione na jednej zmiennej tak �eby si� ci�gle zmienia�a, troch� jak enumerator
    }
    
    private void Movement_PlayerInput()
    {
        //Movement -> vectory i transformacje        
        Vector3 transformDirection = transform.TransformDirection(live_charStats.currentMoveInputDirection); //przeniesienie transform direction z World do Local
        Vector3 flatMovement = live_charStats.currentMoveSpeed * transformDirection;
        live_charStats.currentMoveVector = new Vector3(flatMovement.x, live_charStats.currentMoveVector.y, flatMovement.z);

        live_charStats.isMoving = (live_charStats.currentMoveVector.z != 0f);  //If moveVector.y > 0 = true <= w skr�cie, przypisanie do charStats

        int localSpeedIndex = 0;
        //W�a�ciwy movement z animacjami
        if (live_charStats.currentMoveInputDirection != Vector3.zero && live_charStats.inputRunning) 
        {

            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, (10f + live_charStats.currentCharLevel) * Time.deltaTime); //MoveTowards na ko�cu podaje czas maxymalny na zmian� warto�ci


            if (live_charStats.currentMoveInputDirection != Vector3.zero && live_charStats.currentMoveInputDirection != Vector3.back && !live_charStats.isJumping && !live_charStats.isAttacking && live_charStats.currentStam > 5f) 
            {                                                               //je�li nie sprintuje do ty�u
                ////Running Speed 
                live_charStats.currentAnimator.ResetTrigger("MeeleAttack");
                localSpeedIndex = 2;
                live_charStats.currentMoveSpeed = live_charStats.currentRunSpeed; //Run
            }
            else
            {
                ////Walking Speed
                localSpeedIndex = 1;
                live_charStats.currentMoveSpeed = live_charStats.currentWalkSpeed; //Sprintowanie bez Staminy -> Walk
            }


        }
        else if (live_charStats.currentMoveInputDirection != Vector3.zero && !live_charStats.inputRunning && !live_charStats.isJumping && !live_charStats.isAttacking)
        {   ////Walking Speed
            localSpeedIndex = 1;
            live_charStats.currentMoveSpeed = live_charStats.currentWalkSpeed; //Walk
        }
        else if (live_charStats.currentMoveInputDirection == Vector3.zero && !live_charStats.isJumping && !live_charStats.isAttacking)
        {   ///Idle Speed => speed = 0
            localSpeedIndex = 0;
            live_charStats.currentMoveSpeed = 0; //Idle


        }
        live_charStats.isRunning = (localSpeedIndex == 2);
        live_charStats.isWalking = (localSpeedIndex == 1);
        live_charStats.isIdle = (localSpeedIndex == 0);
        //specjalnie zrobione na jednej zmiennej tak �eby si� ci�gle zmienia�a, troch� jak enumerator


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


    private void Jump()
    {
        float jumpSpeed = live_charStats.currentJumpPower;    //jumpPower
        if (live_charStats.currentMoveSpeed != 0) jumpSpeed = (live_charStats.currentJumpPower + live_charStats.currentMoveSpeed );  //w przypadku gdzie jest move speed, dodaje si�� do jump power

        if (live_charStats.inputJumping && live_charStats.isGrounded && !live_charStats.isAttacking && live_charStats.currentStam > 11f)
        {            
            //zmiana trybu skakania J key
            live_charStats.currentMoveVector.y = live_charStats.currentJumpMode_J_ ? (jumpSpeed) : Mathf.SmoothDamp(live_charStats.currentMoveVector.y, jumpSpeed, ref live_charStats.currentMoveVector.y, live_charStats.currentJumpDeltaTime);
            //je�li (bool) jumpMode "On"=jumpSpeed(sta�y): "OFF" = Mathf.SmootDamp <=====
                
            live_charStats.isJumping = true;
            JumpAnimation();
        }
        else
            live_charStats.isJumping = false;
    }

    public void JumpAnimation()
    {
        live_charStats.currentStam -= (10f + live_charStats.currentCharLevel); //Koszt Stamy przy skoku        
        live_charStats.currentAnimator.ResetTrigger("MeeleAttack"); //�eby nie animowa� ataku w powietrzu           
        live_charStats.currentAnimator.SetFloat("yAnim", 0);
        live_charStats.currentAnimator.SetTrigger("Jump");
    }

    private void Gravity()
    {
        if (!live_charStats.isGrounded)
        {
            //live_charStats.currentMoveVector.y -= live_charStats.currentGravity * Time.deltaTime/*(Time.deltaTime / 2f)*/;  //moveVector to szybko�� poruszania si� wi�c gravity/delta.time (grawitacja/klatk�) jest prawid�owo jako przyspieszenie
                                                                                                                            //(Time.fixedDeltaTime / 1f) = klatki / sekunde     
            
            live_charStats.currentMoveVector.y = Mathf.MoveTowards(live_charStats.currentMoveVector.y, -live_charStats.currentGravity, 1f); //move towards dzia�a lepiej, wi�kszy przyrost na pocz�tku                                                                                                     ////(Time.deltaTime / 2f) => co 2 klatk� odejmuje warto��
            
        }

        if (live_charStats.isGrounded && live_charStats.currentMoveVector.y < 0)
        {
            live_charStats.currentMoveVector.y = 0f;
        }
    }

}
