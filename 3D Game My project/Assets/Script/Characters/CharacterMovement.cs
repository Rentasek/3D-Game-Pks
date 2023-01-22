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
            Rotate(); //Rotate musi byæ w update bo inaczej dziej¹ siê cyrki

            //Jump();
            

            //Reset/SetPosition
            if (live_charStats.inputSetBackupPosition && live_charStats.isPlayer) live_charStats.SetCharacterPosition();
            if (live_charStats.inputResetPosition) live_charStats.ResetCharacterPosition();
        }
        

        //MeeleAttack
        if (live_charStats.inputAttacking) StartCoroutine(MeeleAttack());

        //Testing
        live_charStats.Testing_Z_Key = !live_charStats.Testing_Z_Key;  //w³¹czanie i wy³¹czanie booleana przyciskiem ON/OFF
         
    }



    private void FixedUpdate() //Na FixedUpdate lepiej chodzi, p³ynnie !!! //Update razem z Physics
    {
        Movement();

        if (live_charStats.playerInputEnable && live_charStats.isPlayer)
        //jeœli w inspektorze jest zaznaczona opcja PlayerInputEnable -> sterowanie z Player_Inputa         

        {
            live_NavMeshAgent.enabled = false;//Wy³¹czenie NavMeshAgenta
            
            Movement_PlayerInput(); //na fixedUpdate ¿eby postaæ nie przyspiesza³a na wy¿szych fps

            Jump(); //jump dzia³a z animacj¹ na fixedUpdate tylko jak jest GetKey w inpucie, na GetKeyDown nie dzia³a
            if (_GravityEnabled) Gravity();//grawitacja musi byæ na fixed update inaczej za szybko spada, za czêsty refresh klatki

            characterController.Move(live_charStats.currentMoveVector * Time.deltaTime);

        }
        else if (!live_charStats.playerInputEnable)
        {
            live_NavMeshAgent.enabled = true;//W³¹czenie NavMeshAgenta
            Movement_AgentInput();  //Jeœli nie => AgentInput
           
        }



    }



    private void Rotate()
    {       
        transform.Rotate(Vector3.up, live_charStats.inputRotateHorizontal);
    }


    private void Movement()
    {
        live_charStats.isGrounded = Physics.CheckSphere(transform.position, live_charStats.currentGroundDistance, live_charStats.currentGroundMask); //sprawdzanie isGrounded
        if (live_charStats.isGrounded) live_charStats.currentAnimator.ResetTrigger("Jump");       //resetowanie triggera Jump ¿eby nie wykonywa³ animacji po wyl¹dowaniu z opóŸnieniem
        live_charStats.currentAnimator.SetBool("IsGrounded", live_charStats.isGrounded);          //Przeniesienie status isGrounded do animatora

        if (live_charStats.playerInputEnable) Movement_PlayerInput(); //jeœli w inspektorze jest zaznaczona opcja PlayerInputEnable -> sterowanie z Player_Inputa
        else Movement_AgentInput();  //Jeœli nie => AgentInput 


    }

    public void Movement_AgentInput()
    {
        live_charStats.currentMoveInputDirection = live_NavMeshAgent.velocity;

        live_charStats.isMoving = (live_NavMeshAgent.velocity != Vector3.zero);  //If moveVector.y > 0 = true <= w skrócie, przypisanie do charStats


        live_NavMeshAgent.speed = live_charStats.currentRunSpeed; //Zawsze stara siê poruszaæ RunningSpeed


        int localSpeedIndex = 0;
        //W³aœciwy movement z animacjami
        if (live_charStats.currentMoveInputDirection != Vector3.zero && live_NavMeshAgent.speed == live_charStats.currentRunSpeed)
                                                                                                                    
        {                                                                                                                                           
            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, 5f * Time.deltaTime); //zu¿ywa f stamy / sekunde

            ////Running Speed 
            if (/*live_charStats.currentMoveInputDirection != Vector3.zero &&*/ !live_charStats.isJumping && !live_charStats.isAttacking && live_charStats.currentStam > 5f
                && live_charStats.navMeAge_targetAquired && !live_charStats.inputCasting)//dodatnkowy warunek ->biega tylko jak targetAquired=true, kolejny warnek jeœli nie castuje!!
            {
                live_charStats.currentAnimator.ResetTrigger("MeeleAttack");
                localSpeedIndex = 2;
                live_NavMeshAgent.speed = live_charStats.currentRunSpeed; //Run
                live_charStats.currentMoveSpeed = live_charStats.currentRunSpeed; //zmienna przekazywana do charStats a póŸniej do Animatora
            }
            else
            {   ////Walking Speed
                localSpeedIndex = 1;
                live_NavMeshAgent.speed = live_charStats.currentWalkSpeed; //Sprintowanie bez Staminy -> Walk
                live_charStats.currentMoveSpeed = live_charStats.currentWalkSpeed; //zmienna przekazywana do charStats a póŸniej do Animatora
            }


        }       
        else if (live_charStats.currentMoveInputDirection == Vector3.zero && !live_charStats.isJumping && !live_charStats.isAttacking)
        {   ///Idle Speed => speed = 0
            localSpeedIndex = 0;
            live_charStats.currentMoveSpeed = 0f; //zmienna przekazywana do charStats a póŸniej do Animatora


        }
        live_charStats.isRunning = (localSpeedIndex == 2);
        live_charStats.isWalking = (localSpeedIndex == 1);
        live_charStats.isIdle = (localSpeedIndex == 0);
        //specjalnie zrobione na jednej zmiennej tak ¿eby siê ci¹gle zmienia³a, trochê jak enumerator



    }
    
    private void Movement_PlayerInput()
    {
        //Movement -> vectory i transformacje        
        Vector3 transformDirection = transform.TransformDirection(live_charStats.currentMoveInputDirection); //przeniesienie transform direction z World do Local
        Vector3 flatMovement = live_charStats.currentMoveSpeed * transformDirection;
        live_charStats.currentMoveVector = new Vector3(flatMovement.x, live_charStats.currentMoveVector.y, flatMovement.z);

        live_charStats.isMoving = (live_charStats.currentMoveVector.z != 0f);  //If moveVector.y > 0 = true <= w skrócie, przypisanie do charStats

        int localSpeedIndex = 0;
        //W³aœciwy movement z animacjami
        if (live_charStats.currentMoveInputDirection != Vector3.zero && live_charStats.inputRunning) 
        {

            if (live_charStats.currentStam > 0) live_charStats.currentStam = Mathf.MoveTowards(live_charStats.currentStam, 0f, 5f * Time.deltaTime); //MoveTowards na koñcu podaje czas maxymalny na zmianê wartoœci


            if (live_charStats.currentMoveInputDirection != Vector3.zero && live_charStats.currentMoveInputDirection != Vector3.back && !live_charStats.isJumping && !live_charStats.isAttacking && live_charStats.currentStam > 5f) 
            {                                                               //jeœli nie sprintuje do ty³u
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
        //specjalnie zrobione na jednej zmiennej tak ¿eby siê ci¹gle zmienia³a, trochê jak enumerator


    }

    private void Jump()
    {
        float jumpSpeed = live_charStats.currentJumpPower;    //jumpPower
        if (live_charStats.currentMoveSpeed != 0) jumpSpeed = (live_charStats.currentJumpPower + live_charStats.currentMoveSpeed );  //w przypadku gdzie jest move speed, dodaje si³ê do jump power

        if (live_charStats.inputJumping && live_charStats.isGrounded && !live_charStats.isAttacking && live_charStats.currentStam > 11f)
        {            
            //zmiana trybu skakania J key
            live_charStats.currentMoveVector.y = live_charStats.currentJumpMode_J_ ? (jumpSpeed) : Mathf.SmoothDamp(live_charStats.currentMoveVector.y, jumpSpeed, ref live_charStats.currentMoveVector.y, live_charStats.currentJumpDeltaTime);
            //jeœli (bool) jumpMode "On"=jumpSpeed(sta³y): "OFF" = Mathf.SmootDamp <=====
                
            live_charStats.isJumping = true;                 
        }
        else
            live_charStats.isJumping = false;
    }



    
   

    private void Gravity()
    {
        if (!live_charStats.isGrounded)
        {
            //live_charStats.currentMoveVector.y -= live_charStats.currentGravity * Time.deltaTime/*(Time.deltaTime / 2f)*/;  //moveVector to szybkoœæ poruszania siê wiêc gravity/delta.time (grawitacja/klatkê) jest prawid³owo jako przyspieszenie
                                                                                                                            //(Time.fixedDeltaTime / 1f) = klatki / sekunde     
            
            live_charStats.currentMoveVector.y = Mathf.MoveTowards(live_charStats.currentMoveVector.y, -live_charStats.currentGravity, 1f); //move towards dzia³a lepiej, wiêkszy przyrost na pocz¹tku                                                                                                     ////(Time.deltaTime / 2f) => co 2 klatkê odejmuje wartoœæ
            
        }

        if (live_charStats.isGrounded && live_charStats.currentMoveVector.y < 0)
        {
            live_charStats.currentMoveVector.y = 0f;
        }
    }  

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

}
