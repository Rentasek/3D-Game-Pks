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

/*
    // Imported from live_charStats
    [Space]
    private float walkSpeed;
    private float runSpeed;
    private float jumpPower;
    private float gravity;

    private float attackCooldown = 0.15f;

    private LayerMask groundMask;
    private float groundDistance;                   //pobierane z live_charStats
    ////////////////////////////////////////////////////////////////////////////////////////*/

    /*[Header("Booleany/Vectory z klasy Input")]
    //[SerializeField] private Vector3 inputMovingVector;
    //[SerializeField] private float inputRotateHorizontal;
    //[SerializeField] private bool inputMoving;
    //[SerializeField] private bool inputRunning;
    //[SerializeField] private bool inputJumping;
    //[SerializeField] private bool inputAttacking;

    [Space]
    //[SerializeField] private bool inputSaveGame;
    //[SerializeField] private bool inputLoadGame;
    //[SerializeField] private bool inputResetPosition;
    //[SerializeField] private bool inputSetBackupPosition;
    //[SerializeField] private bool Testing_Z_Key;*/
                                                               //pobierane z Player_input
    /////////////////////////////////////////////////////////////////////////////////////////

    /*[Space]
      [Header("Character State")]    
      [SerializeField] private float comboMeele = 0f;
      [SerializeField] private float moveSpeed;*/

    // [SerializeField] private bool runningTesting_LShift_Key;

                                                                //pobierane z live_charStats
    ///////////////////////////////////////////////////////////////////////////////////////////


    /*  [Space]
      [Header("Character Current Movement")]
      [SerializeField] private Vector3 inputDirection;
      [SerializeField] private Vector3 moveVector;
      [SerializeField] private Vector3 velocity;*/
                                                                //pobierane z live_charStats
    ///////////////////////////////////////////////////////////////////////////////////////


    //[SerializeField] private float rotateSensivity;

    //[SerializeField] private float jumpDeltaTime;
                                                                 //pobierane z Player_input
    ////////////////////////////////////////////////////////////////////////////////////////

    /*public Animator animator;
    [SerializeField] private CharacterController characterController;
    public ScrObj_charStats scrObj_charStats;*/
                                                                //pobierane z live_charStats
    ///////////////////////////////////////////////////////////////////////////////////////
    [Space]
    [Header("Character Components")]
    public CharacterStatus live_charStats;
    //public Player_Input live_Player_Input;
    public NavMeshAgent live_NavMeshAgent;

    [SerializeField] private CharacterController characterController;    

    [SerializeField] private bool _GravityEnabled = true;

    //[SerializeField] private Rigidbody rb;

    ///////////////////////////////////// Implementacja ///////////////////////////////////////

    private void OnValidate()
    {        
        live_charStats = GetComponent<CharacterStatus>();        
        live_NavMeshAgent = live_charStats.currentNavMeshAgent;//live_charStats.GetComponent<NavMeshAgent>();         
        characterController = live_charStats.currentCharacterController;//GetComponent<CharacterController>();
    }



    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    private void Update() //Update najszybszy update => do inputa
    {
        

        if (live_charStats.playerInputEnable && live_charStats.isPlayer)
        {
            Rotate(); //Rotate musi by� w update bo inaczej dziej� si� cyrki

            //Jump();
            

            //Reset/SetPosition
            if (live_charStats.inputSetBackupPosition && live_charStats.isPlayer) live_charStats.SetCharacterPosition();
            if (live_charStats.inputResetPosition) live_charStats.ResetCharacterPosition();
        }
        

        //MeeleAttack
        if (live_charStats.inputAttacking) StartCoroutine(MeeleAttack());

        //Testing
        live_charStats.Testing_Z_Key = !live_charStats.Testing_Z_Key;  //w��czanie i wy��czanie booleana przyciskiem ON/OFF
         
    }



    private void FixedUpdate() //Na FixedUpdate lepiej chodzi, p�ynnie !!! //Update razem z Physics
    {
        Movement();

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



    }



    private void Rotate()
    {       
        transform.Rotate(Vector3.up, live_charStats.inputRotateHorizontal);
    }


    private void Movement()
    {
        live_charStats.isGrounded = Physics.CheckSphere(transform.position, live_charStats.currentGroundDistance, live_charStats.currentGroundMask); //sprawdzanie isGrounded
        if (live_charStats.isGrounded) live_charStats.currentAnimator.ResetTrigger("Jump");       //resetowanie triggera Jump �eby nie wykonywa� animacji po wyl�dowaniu z op�nieniem
        live_charStats.currentAnimator.SetBool("IsGrounded", live_charStats.isGrounded);          //Przeniesienie status isGrounded do animatora

        if (live_charStats.playerInputEnable) Movement_PlayerInput(); //je�li w inspektorze jest zaznaczona opcja PlayerInputEnable -> sterowanie z Player_Inputa
        else Movement_AgentInput();  //Je�li nie => AgentInput 


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
            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, 5f * Time.deltaTime); //zu�ywa f stamy / sekunde

            ////Running Speed 
            if (/*live_charStats.currentMoveInputDirection != Vector3.zero &&*/ !live_charStats.isJumping && !live_charStats.isAttacking && live_charStats.currentStam > 5f
                && live_charStats.navMeAge_targetAquired && !live_charStats.inputCasting)//dodatnkowy warunek ->biega tylko jak targetAquired=true, kolejny warnek je�li nie castuje!!
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

            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, 5f * Time.deltaTime); //MoveTowards na ko�cu podaje czas maxymalny na zmian� warto�ci


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
        }
        else
            live_charStats.isJumping = false;
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

}
