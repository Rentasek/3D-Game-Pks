using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player_Input : MonoBehaviour
{
    
    [Space]
    [Header("Character Objects")]
    public CharacterStatus live_charStats;


    public bool pauseKeyPressed;
    public bool optionsActivated = true;
    [SerializeField] private GameObject optionsPanel;
    private bool mouseRotateLocalBool;


    // Start is called before the first frame update
    private void OnEnable()
    {
        live_charStats = GetComponent<CharacterStatus>();        
        if (live_charStats.isPlayer)  GameObject.Find("Player_Charmander/UI_Screen/OptionsPanel");
    }


    // Update is called once per frame  //update u¿ywaæ do input GetKeyDown, dzia³aj¹ bez laga
    void Update()
    {      

    }
   

    public void PlayerInputClass() //wsyzstkie klasy maj¹ w sobie dodatkowe warunki do dzia³ania INPUTA
    {
        if (live_charStats.inputEnableMouseRotate) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;

        live_charStats.inputRunning = Input.GetKey(KeyCode.LeftShift);
        live_charStats.inputJumping = Input.GetKey(KeyCode.Space);

        live_charStats.currentMoveInputDirection = new Vector3(Input.GetAxis("Horizontal")/*0*/, 0, Input.GetAxis("Vertical")); //input ruchu

        live_charStats.inputRotateHorizontal = Input.GetAxis("Horizontal") * live_charStats.inputRotateSensivity * Time.deltaTime;

        //MeeleAttack
        live_charStats.inputPrimary = Input.GetKey(KeyCode.Mouse0) && !live_charStats.isRunning;

    }
    public void IsometricInputClass()
    {              
        //live_charStats.inputEnableMouseRotate = false;
        // click œrodkowy mouse -> wskazanie destination dla agenta
        if (live_charStats.inputMouseCurrentMoving && live_charStats.isPlayer && !live_charStats.playerInputEnable)
        {            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                live_charStats.navMeAge_mouseWalkPoint = hit.point;
                live_charStats.navMeAge_walkPoint = hit.point;
            }
            live_charStats.navMeAge_walkPointSet = true;
            
            if (live_charStats.inputMouseCurrentMoving)  //jeœli poruszamy siê myszk¹ (Œrodkowy przycisk) 
            {                
                live_charStats.fov_targetInAttackRange = false;      //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.fov_targetInDynamicSightRange = false;      //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.isAttacking = false;                     //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.inputPrimary = false;                  //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.inputSecondary = false;                  //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.fov_targetAquired = true;             //debugging ¿eby nie blokowa³ siê przy atakowaniu                
            }
        }

        if (live_charStats.inputSecondary)  //lub castujemy
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.LookAt(hit.point);              //Obraca playera w stronê mouse Pointa przy breath
            }
            
            live_charStats.fov_targetInAttackRange = false;      //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.fov_targetInDynamicSightRange = false;      //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.isAttacking = false;                     //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.inputPrimary = false;                  //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.fov_targetAquired = true;             //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.isRunning= false;
            live_charStats.skill_CanCast = live_charStats.inputSecondary;  //tylko dla gracza w isometric -> Mo¿na canCastowaæ przerywaj¹c wszystko inne
            live_charStats.currentNavMeshAgent.isStopped = true;

        }

        //if (live_charStats.inputEnableMouseRotate) live_charStats.inputEnableMouseRotate = false; //odblokowanie mouse positiona w isometric
    }
   

    public void InputClass()
    {
        if (live_charStats.isPlayer)
        {

            //MouseMoving
            live_charStats.inputMouseCurrentMoving = Input.GetKey(KeyCode.Mouse2);

            //MouseScroll
            live_charStats.inputMouseScroll = Input.GetAxis("Mouse ScrollWheel");

            //GameSave/Load
            live_charStats.inputSaveGame = Input.GetKeyDown(KeyCode.Alpha5);
            if (live_charStats.inputSaveGame) live_charStats.SaveGame();

            live_charStats.inputLoadGame = Input.GetKeyDown(KeyCode.Alpha6);
            if (live_charStats.inputLoadGame) live_charStats.LoadGame();

            //ResetLevel
            if (Input.GetKeyDown(KeyCode.L)) live_charStats.ResetLevel();

            //Reset/SetPosition
            live_charStats.inputSetBackupPosition = Input.GetKeyDown(KeyCode.B);
            live_charStats.inputResetPosition = Input.GetKeyDown(KeyCode.R) || GetComponentInParent<Transform>().transform.position.y <= -5f;

            live_charStats.Testing_Z_Key = Input.GetKeyDown(KeyCode.Z);
            if (Input.GetKeyDown(KeyCode.J)) live_charStats.currentJumpMode_J_ = !live_charStats.currentJumpMode_J_;  //w³¹czanie i wy³¹czanie booleana przyciskiem ON/OFF
                                                                                                                      //w³¹czanie i wy³¹czanie booleana przyciskiem ON/OFF

            //dzia³a tylko z booleanem IsPlayer
            if (Input.GetKeyDown(KeyCode.P) && live_charStats.isPlayer)
            {
                live_charStats.playerInputEnable = !live_charStats.playerInputEnable;//input Switch

                if (!live_charStats.playerInputEnable)
                {
                    mouseRotateLocalBool = live_charStats.inputEnableMouseRotate;
                    live_charStats.inputEnableMouseRotate = false;
                }
                else live_charStats.inputEnableMouseRotate = mouseRotateLocalBool;

                if (!live_charStats.inputEnableMouseRotate) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
                else Cursor.lockState = CursorLockMode.Locked;
            }

            //casting Spells
            if (live_charStats.isPlayer) live_charStats.inputSecondary = Input.GetKey(KeyCode.Mouse1);

            pauseKeyPressed = Input.GetKeyDown(KeyCode.Escape) && live_charStats.isPlayer;

            if (!live_charStats.playerInputEnable) IsometricInputClass();//jeœli nie ma flagi player input //isometric
            if (live_charStats.playerInputEnable) PlayerInputClass();//jeœli zaznaczona jest flaga playerInputEnable //3rd person

            //options panel
            if (Input.GetKeyDown(KeyCode.O)) OptionsPanel_enabling();
        }        
    }
    ////////////////////////////////////////////////////////////////////  

    
    public void OptionsPanel_enabling()
    {
        if (live_charStats.isPlayer)
        {
            optionsActivated = !optionsActivated;
            optionsPanel.SetActive(optionsActivated);
        }

    }
}
