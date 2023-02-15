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
        if (live_charStats.charInfo._isPlayer)  GameObject.Find("Player_Charmander/UI_Screen/OptionsPanel");
    }


    /*// Update is called once per frame  //update u¿ywaæ do input GetKeyDown, dzia³aj¹ bez laga
    void Update()
    {      

    }*/

    /// <summary>
    /// PlayerInput tylko dla Playera
    /// </summary>
    public void PlayerInputClass() //wsyzstkie klasy maj¹ w sobie dodatkowe warunki do dzia³ania INPUTA
    {
        if (live_charStats.charInput._enableMouseRotate) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;

        live_charStats.charInput._running = Input.GetKey(KeyCode.LeftShift);
        live_charStats.charInput._jumping = Input.GetKey(KeyCode.Space);

        live_charStats.charMove._moveInputDirection = new Vector3(Input.GetAxis("Horizontal")/*0*/, 0, Input.GetAxis("Vertical")); //input ruchu

        live_charStats.charInput._rotateHorizontal = Input.GetAxis("Horizontal") * live_charStats.charInput._rotateSensivity * Time.deltaTime;

        //MeeleAttack
        live_charStats.charInput._primary = Input.GetKey(KeyCode.Mouse0) && !live_charStats.charStatus._isRunning;

    }

    /// <summary>
    /// IsometricInput tylko dla Playera
    /// </summary>
    public void IsometricInputClass()
    {              
        //live_charStats.inputEnableMouseRotate = false;
        // click œrodkowy mouse -> wskazanie destination dla agenta
        if (live_charStats.charInput._mouseCurrentMoving && live_charStats.charInfo._isPlayer && !live_charStats.charInfo._playerInputEnable)
        {            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                live_charStats.navMeshAge._mouseWalkPoint = hit.point;
                live_charStats.navMeshAge._walkPoint = hit.point;
            }
            live_charStats.navMeshAge._walkPointSet = true;
            
            if (live_charStats.charInput._mouseCurrentMoving)  //jeœli poruszamy siê myszk¹ (Œrodkowy przycisk) 
            {                
                live_charStats.fov._targetInAttackRange = false;      //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.fov._targetInDynamicSightRange = false;      //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.charStatus._isAttacking = false;                     //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.charInput._primary = false;                  //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.charInput._secondary = false;                  //debugging ¿eby nie blokowa³ siê przy atakowaniu
                live_charStats.fov._targetAquired = true;             //debugging ¿eby nie blokowa³ siê przy atakowaniu                
            }
        }

        if (live_charStats.charInput._secondary)  //lub castujemy
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.LookAt(hit.point);              //Obraca playera w stronê mouse Pointa przy breath
            }
            
            live_charStats.fov._targetInAttackRange = false;      //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.fov._targetInDynamicSightRange = false;      //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.charStatus._isAttacking = false;                     //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.charInput._primary = false;                  //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.fov._targetAquired = true;             //debugging ¿eby nie blokowa³ siê przy atakowaniu
            live_charStats.charStatus._isRunning= false;
            live_charStats.charSkillCombat._secondarySkill.skill_CanCast = live_charStats.charInput._secondary;  //tylko dla gracza w isometric -> Mo¿na canCastowaæ przerywaj¹c wszystko inne
            live_charStats.charComponents._navMeshAgent.isStopped = true;

        }

        //if (live_charStats.inputEnableMouseRotate) live_charStats.inputEnableMouseRotate = false; //odblokowanie mouse positiona w isometric
    }
   

    public void InputClass()
    {
        if (live_charStats.charInfo._isPlayer)
        {

            //MouseMoving
            live_charStats.charInput._mouseCurrentMoving = Input.GetKey(KeyCode.Mouse2);

            //MouseScroll
            live_charStats.charInput._mouseScroll = Input.GetAxis("Mouse ScrollWheel");

            //GameSave/Load
            live_charStats.charInput._saveGame = Input.GetKeyDown(KeyCode.Alpha5);
            if (live_charStats.charInput._saveGame) live_charStats.SaveGame();

            live_charStats.charInput._loadGame = Input.GetKeyDown(KeyCode.Alpha6);
            if (live_charStats.charInput._loadGame) live_charStats.LoadGame();

            //ResetLevel
            if (Input.GetKeyDown(KeyCode.L)) live_charStats.ResetLevel();

            //Reset/SetPosition
            live_charStats.charInput._setBackupPosition = Input.GetKeyDown(KeyCode.B);
            live_charStats.charInput._resetPosition = Input.GetKeyDown(KeyCode.R) || GetComponentInParent<Transform>().transform.position.y <= -5f;

            live_charStats.charInput._testing_Z_Key = Input.GetKeyDown(KeyCode.Z);

            //dzia³a tylko z booleanem IsPlayer
            if (Input.GetKeyDown(KeyCode.P) && live_charStats.charInfo._isPlayer)
            {
                live_charStats.charInfo._playerInputEnable = !live_charStats.charInfo._playerInputEnable;//input Switch

                if (!live_charStats.charInfo._playerInputEnable)
                {
                    mouseRotateLocalBool = live_charStats.charInput._enableMouseRotate;
                    live_charStats.charInput._enableMouseRotate = false;
                }
                else live_charStats.charInput._enableMouseRotate = mouseRotateLocalBool;

                if (!live_charStats.charInput._enableMouseRotate) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
                else Cursor.lockState = CursorLockMode.Locked;
            }

            //casting Spells
            if (live_charStats.charInfo._isPlayer) live_charStats.charInput._secondary = Input.GetKey(KeyCode.Mouse1);

            pauseKeyPressed = Input.GetKeyDown(KeyCode.Escape) && live_charStats.charInfo._isPlayer;

            if (!live_charStats.charInfo._playerInputEnable) IsometricInputClass();//jeœli nie ma flagi player input //isometric
            if (live_charStats.charInfo._playerInputEnable) PlayerInputClass();//jeœli zaznaczona jest flaga playerInputEnable //3rd person

            //options panel
            if (Input.GetKeyDown(KeyCode.O)) OptionsPanel_enabling();
        }        
    }
    ////////////////////////////////////////////////////////////////////  

    
    public void OptionsPanel_enabling()
    {
        if (live_charStats.charInfo._isPlayer)
        {
            optionsActivated = !optionsActivated;
            optionsPanel.SetActive(optionsActivated);
        }

    }
}
