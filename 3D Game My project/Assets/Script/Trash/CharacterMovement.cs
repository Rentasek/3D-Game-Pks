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
{/*
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
        live_NavMeshAgent = live_charStats.charComponents._navMeshAgent;
        characterController = live_charStats.charComponents._characterController;
        _GravityEnabled = live_charStats.charMove._gravityEnabled;
    }

    public void RotatePlayer()
    {       
        transform.Rotate(Vector3.up, live_charStats.charInput._rotateHorizontal);
    }

    public void Movement()
    {
        live_charStats.charStatus._isGrounded = Physics.CheckSphere(transform.position, live_charStats.charMove.currentGroundDistance, live_charStats.charMove.currentGroundMask); //sprawdzanie isGrounded
        if (live_charStats.charStatus._isGrounded) live_charStats.charComponents._Animator.ResetTrigger("Jump");       //resetowanie triggera Jump �eby nie wykonywa� animacji po wyl�dowaniu z op�nieniem
        live_charStats.charComponents._Animator.SetBool("IsGrounded", live_charStats.charStatus._isGrounded);          //Przeniesienie status isGrounded do animatora

        if (live_charStats.charInfo._playerInputEnable && live_charStats.charInfo._isPlayer)
        //je�li w inspektorze jest zaznaczona opcja PlayerInputEnable -> sterowanie z Player_Inputa         

        {
            live_NavMeshAgent.enabled = false;//Wy��czenie NavMeshAgenta

            Movement_PlayerInput(); //na fixedUpdate �eby posta� nie przyspiesza�a na wy�szych fps

            Jump(); //jump dzia�a z animacj� na fixedUpdate tylko jak jest GetKey w inpucie, na GetKeyDown nie dzia�a
            if (_GravityEnabled) Gravity();//grawitacja musi by� na fixed update inaczej za szybko spada, za cz�sty refresh klatki

            characterController.Move(live_charStats.charMove._moveVector * Time.deltaTime);

        }
        else if (!live_charStats.charInfo._playerInputEnable)
        {
            live_NavMeshAgent.enabled = true;//W��czenie NavMeshAgenta
            Movement_AgentInput();  //Je�li nie => AgentInput

        }

        MovementAnimations();

    }

    public void Movement_AgentInput()
    {
        live_charStats.charMove._moveInputDirection = live_NavMeshAgent.velocity;

        live_charStats.charStatus._isMoving = (live_NavMeshAgent.velocity != Vector3.zero);  //If moveVector.y > 0 = true <= w skr�cie, przypisanie do charStats


        live_NavMeshAgent.speed = live_charStats.charMove._runSpeed; //Zawsze stara si� porusza� RunningSpeed


        int localSpeedIndex = 0;
        //W�a�ciwy movement z animacjami
        if (live_charStats.charMove._moveInputDirection != Vector3.zero && live_NavMeshAgent.speed == live_charStats.charMove._runSpeed)
                                                                                                                    
        {
            if (live_charStats.charStats._stam > 0) live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, 0f, (10f + live_charStats.charInfo._charLevel) * Time.deltaTime); //zu�ywa f stamy / sekunde

            ////Running Speed 
            if (*//*live_charStats.currentMoveInputDirection != Vector3.zero &&*//* !live_charStats.charStatus._isJumping && !live_charStats.fov._closeRangeSkill.skill_input && live_charStats.charStats._stam > 5f
                && live_charStats.fov._targetAquired && !live_charStats.charInput._secondary && live_charStats.charComponents._navMeshAgent.remainingDistance > 2 * live_charStats.fov._closeRangeSkillMinRadius)
                //dodatnkowy warunek ->biega tylko jak targetAquired=true, kolejny warnek je�li nie castuje!!, Kolejny warunek je�li agent.eemainingDistance > 2* attack range
            {
                live_charStats.charComponents._Animator.ResetTrigger("MeeleAttack");
                localSpeedIndex = 2;
                live_NavMeshAgent.speed = live_charStats.charMove._runSpeed; //Run
                live_charStats.charMove._moveSpeed = live_charStats.charMove._runSpeed; //zmienna przekazywana do charStats a p�niej do Animatora
            }
            else
            {   ////Walking Speed
                localSpeedIndex = 1;
                live_NavMeshAgent.speed = live_charStats.charMove._walkSpeed; //Sprintowanie bez Staminy -> Walk
                live_charStats.charMove._moveSpeed = live_charStats.charMove._walkSpeed; //zmienna przekazywana do charStats a p�niej do Animatora
            }


        }       
        else if (live_charStats.charMove._moveInputDirection == Vector3.zero && !live_charStats.charStatus._isJumping && !live_charStats.fov._closeRangeSkill.skill_input)
        {   ///Idle Speed => speed = 0
            localSpeedIndex = 0;
            live_charStats.charMove._moveSpeed = 0f; //zmienna przekazywana do charStats a p�niej do Animatora


        }
        live_charStats.charStatus._isRunning = (localSpeedIndex == 2);
        live_charStats.charStatus._isWalking = (localSpeedIndex == 1);
        live_charStats.charStatus._isIdle = (localSpeedIndex == 0);
        //specjalnie zrobione na jednej zmiennej tak �eby si� ci�gle zmienia�a, troch� jak enumerator
    }
    
    private void Movement_PlayerInput()
    {
        //Movement -> vectory i transformacje        
        Vector3 transformDirection = transform.TransformDirection(live_charStats.charMove._moveInputDirection); //przeniesienie transform direction z World do Local
        Vector3 flatMovement = live_charStats.charMove._moveSpeed * transformDirection;
        live_charStats.charMove._moveVector = new Vector3(flatMovement.x, live_charStats.charMove._moveVector.y, flatMovement.z);

        live_charStats.charStatus._isMoving = (live_charStats.charMove._moveVector.z != 0f);  //If moveVector.y > 0 = true <= w skr�cie, przypisanie do charStats

        int localSpeedIndex = 0;
        //W�a�ciwy movement z animacjami
        if (live_charStats.charMove._moveInputDirection != Vector3.zero && live_charStats.charInput._running) 
        {

            if (live_charStats.charStats._stam > 0) live_charStats.charStats._stam = Mathf.MoveTowards(live_charStats.charStats._stam, 0f, (10f + live_charStats.charInfo._charLevel) * Time.deltaTime); //MoveTowards na ko�cu podaje czas maxymalny na zmian� warto�ci


            if (live_charStats.charMove._moveInputDirection != Vector3.zero && live_charStats.charMove._moveInputDirection != Vector3.back && !live_charStats.charStatus._isJumping && !live_charStats.fov._closeRangeSkill.skill_input && live_charStats.charStats._stam > 5f) 
            {                                                               //je�li nie sprintuje do ty�u
                ////Running Speed 
                live_charStats.charComponents._Animator.ResetTrigger("MeeleAttack");
                localSpeedIndex = 2;
                live_charStats.charMove._moveSpeed = live_charStats.charMove._runSpeed; //Run
            }
            else
            {
                ////Walking Speed
                localSpeedIndex = 1;
                live_charStats.charMove._moveSpeed = live_charStats.charMove._walkSpeed; //Sprintowanie bez Staminy -> Walk
            }


        }
        else if (live_charStats.charMove._moveInputDirection != Vector3.zero && !live_charStats.charInput._running && !live_charStats.charStatus._isJumping && !live_charStats.fov._closeRangeSkill.skill_input)
        {   ////Walking Speed
            localSpeedIndex = 1;
            live_charStats.charMove._moveSpeed = live_charStats.charMove._walkSpeed; //Walk
        }
        else if (live_charStats.charMove._moveInputDirection == Vector3.zero && !live_charStats.charStatus._isJumping && !live_charStats.fov._closeRangeSkill.skill_input)
        {   ///Idle Speed => speed = 0
            localSpeedIndex = 0;
            live_charStats.charMove._moveSpeed = 0; //Idle


        }
        live_charStats.charStatus._isRunning = (localSpeedIndex == 2);
        live_charStats.charStatus._isWalking = (localSpeedIndex == 1);
        live_charStats.charStatus._isIdle = (localSpeedIndex == 0);
        //specjalnie zrobione na jednej zmiennej tak �eby si� ci�gle zmienia�a, troch� jak enumerator


    }

    private void MovementAnimations()
    {
        if (live_charStats.charMove._moveSpeed == 0f && live_charStats.charStatus._isIdle) Idle();
        else if (live_charStats.charMove._moveSpeed <= live_charStats.charMove._walkSpeed && live_charStats.charStatus._isWalking) Walk();
        else if (live_charStats.charMove._moveSpeed <= live_charStats.charMove._runSpeed && live_charStats.charStatus._isRunning) Run();

    }


    private void Idle()
    {
        live_charStats.charComponents._Animator.SetFloat("yAnim", 0);
    }
    private void Walk()
    {
        live_charStats.charComponents._Animator.SetFloat("yAnim", 0.5f, 0.2f, Time.deltaTime);
    }
    private void Run()
    {
        live_charStats.charComponents._Animator.SetFloat("yAnim", 1, 0.2f, Time.deltaTime);
    }


    private void Jump()
    {
        float jumpSpeed = live_charStats.charMove._jumpPower;    //jumpPower
        if (live_charStats.charMove._moveSpeed != 0) jumpSpeed = (live_charStats.charMove._jumpPower + live_charStats.charMove._moveSpeed );  //w przypadku gdzie jest move speed, dodaje si�� do jump power

        if (live_charStats.charInput._jumping && live_charStats.charStatus._isGrounded && !live_charStats.fov._closeRangeSkill.skill_input && live_charStats.charStats._stam > 11f)
        {            
            //zmiana trybu skakania J key
            live_charStats.charMove._moveVector.y = jumpSpeed;
            live_charStats.charStatus._isJumping = true;
            JumpAnimation();
        }
        else
            live_charStats.charStatus._isJumping = false;
    }

    public void JumpAnimation()
    {
        live_charStats.charStats._stam -= (10f + live_charStats.charInfo._charLevel); //Koszt Stamy przy skoku        
        live_charStats.charComponents._Animator.ResetTrigger("MeeleAttack"); //�eby nie animowa� ataku w powietrzu           
        live_charStats.charComponents._Animator.SetFloat("yAnim", 0);
        live_charStats.charComponents._Animator.SetTrigger("Jump");
    }

    private void Gravity()
    {
        if (!live_charStats.charStatus._isGrounded)
        {
            //live_charStats.currentMoveVector.y -= live_charStats.currentGravity * Time.deltaTime/*(Time.deltaTime / 2f)*//*;  //moveVector to szybko�� poruszania si� wi�c gravity/delta.time (grawitacja/klatk�) jest prawid�owo jako przyspieszenie
                                                                                                                            //(Time.fixedDeltaTime / 1f) = klatki / sekunde     
            
            live_charStats.charMove._moveVector.y = Mathf.MoveTowards(live_charStats.charMove._moveVector.y, -live_charStats.charMove._gravity, 1f); //move towards dzia�a lepiej, wi�kszy przyrost na pocz�tku                                                                                                     ////(Time.deltaTime / 2f) => co 2 klatk� odejmuje warto��
            
        }

        if (live_charStats.charStatus._isGrounded && live_charStats.charMove._moveVector.y < 0)
        {
            live_charStats.charMove._moveVector.y = 0f;
        }
    }
*//**//**//**/
}
