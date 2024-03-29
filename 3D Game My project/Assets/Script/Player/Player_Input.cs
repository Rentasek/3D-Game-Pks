using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player_Input : MonoBehaviour
{
    
    [Space]
    [Header("Character Objects")]
    public CharacterStatus live_charStats;

    public bool isPaused;
    public bool pauseKeyPressed;

    // Start is called before the first frame update
    private void OnEnable()
    {
        live_charStats = GetComponent<CharacterStatus>();        
    }

    /// <summary>
    /// PlayerInput tylko dla Playera
    /// </summary>
    public void PlayerInputClass() //wsyzstkie klasy maj� w sobie dodatkowe warunki do dzia�ania INPUTA
    {
        
        if(!isPaused || !live_charStats.charInfo._playerInputEnable) //�eby nie zmienia� stanu je�li jest zapauzowany lu playerInput w��czony
        {
            if (Cursor.lockState == CursorLockMode.Locked ) //live_charStats.charInput._enableMouseRotate tylko przetrzymuje dane aktualnego lock.state, nie zmienia go
            { live_charStats.charInput._enableMouseRotate = true; }
            else live_charStats.charInput._enableMouseRotate = false;
        }
        

        live_charStats.charInput._running = Input.GetKey(KeyCode.LeftShift);
        live_charStats.charInput._jumping = Input.GetKey(KeyCode.Space);

        live_charStats.charMove._moveInputDirection = new Vector3(Input.GetAxis("Horizontal")/*0*/, 0, Input.GetAxis("Vertical")); //input ruchu

        live_charStats.charInput._rotateHorizontal = Input.GetAxis("Horizontal") * live_charStats.charComponents._scrObj_GameSettings._inputSettings._rotateSensivity * Time.deltaTime;

        //PimaryAttack
        live_charStats.charSkillCombat._skillArray[0]._skillInput = Input.GetKey(KeyCode.Mouse0);
        live_charStats.charSkillCombat._skillArray[1]._skillOtherInput = Input.GetKey(KeyCode.Mouse0);
    }

    /// <summary>
    /// IsometricInput tylko dla Playera
    /// </summary>
    public void IsometricInputClass()
    {   
        // click �rodkowy mouse -> wskazanie destination dla agenta
        if (live_charStats.charInput._mouseCurrentMoving && live_charStats.charInfo._isPlayer && !live_charStats.charInfo._playerInputEnable)
        {
            LayerMask layerMask = 1 << 22; // ustawia jako layerMask tylko nr22 (Terrain)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit,40f,layerMask)) //Raycast dzia�a przez �ciany, zatrzymuje si� tylko na Terrain
            {
                live_charStats.navMeshAge._mouseWalkPoint = hit.point;
                live_charStats.navMeshAge._walkPoint = hit.point;                
            }
            live_charStats.navMeshAge._walkPointSet = true;
            
            if (live_charStats.charInput._mouseCurrentMoving)  //je�li poruszamy si� myszk� (�rodkowy przycisk) 
            {                
                live_charStats.fov._targetInAttackRange = false;      //debugging �eby nie blokowa� si� przy atakowaniu
                live_charStats.fov._targetInDynamicSightRange = false;      //debugging �eby nie blokowa� si� przy atakowaniu
                //live_charStats.charStatus._isAttacking = false;                     //debugging �eby nie blokowa� si� przy atakowaniu
                live_charStats.charSkillCombat._skillArray[0]._skillInput = false;                  //debugging �eby nie blokowa� si� przy atakowaniu
                live_charStats.charSkillCombat._skillArray[0]._skillOtherInput = false;                  //debugging �eby nie blokowa� si� przy atakowaniu
                live_charStats.charSkillCombat._skillArray[1]._skillInput = false;                  //debugging �eby nie blokowa� si� przy atakowaniu
                live_charStats.charSkillCombat._skillArray[1]._skillOtherInput = false;                  //debugging �eby nie blokowa� si� przy atakowaniu
                live_charStats.fov._targetAquired = true;             //debugging �eby nie blokowa� si� przy atakowaniu                
            }
        }

        if (live_charStats.charSkillCombat._skillArray[1]._skillInput)  //lub castujemy
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.LookAt(hit.point,Vector3.up);              //Obraca playera w stron� mouse Pointa przy breath //Tylko wok� osi up, dzi�ki temu nie atakuje w g�r� lub w d�
            }
            
            live_charStats.fov._targetInAttackRange = false;      //debugging �eby nie blokowa� si� przy atakowaniu
            live_charStats.fov._targetInDynamicSightRange = false;      //debugging �eby nie blokowa� si� przy atakowaniu
            //live_charStats.charStatus._isAttacking = false;                     //debugging �eby nie blokowa� si� przy atakowaniu
            live_charStats.charSkillCombat._skillArray[0]._skillInput = false;                  //debugging �eby nie blokowa� si� przy atakowaniu
            live_charStats.fov._targetAquired = true;             //debugging �eby nie blokowa� si� przy atakowaniu
            live_charStats.charStatus._isRunning= false;
            //live_charStats.charSkillCombat._secondarySkill._canCast = live_charStats.charInput._secondary;  //tylko dla gracza w isometric -> Mo�na canCastowa� przerywaj�c wszystko inne
            live_charStats.charSkillCombat._skillArray[1]._canCast = live_charStats.charSkillCombat._skillArray[1]._skillInput;  //tylko dla gracza w isometric -> Mo�na canCastowa� przerywaj�c wszystko inne
            live_charStats.navMeshAge._walkPoint = transform.position;
            live_charStats.navMeshAge._mouseWalkPoint = transform.position;
        }
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

            //dzia�a tylko z booleanem IsPlayer
            if (Input.GetKeyDown(KeyCode.P) && live_charStats.charInfo._isPlayer)
            {
                live_charStats.charInfo._playerInputEnable = !live_charStats.charInfo._playerInputEnable;//input Switch

                if (!live_charStats.charInfo._playerInputEnable)
                {
                    Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
                    live_charStats.navMeshAge._walkPoint = transform.position; // �eby po w��czeniu isometric nie szed� do punktu (0,0,0)
                }
                else
                {
                    if (live_charStats.charInput._enableMouseRotate) Cursor.lockState = CursorLockMode.Locked; //tutaj live_charStats.charInput._enableMouseRotate dzia�a jak przechowalnia lock.state i przywraca poprzedni stan
                    else Cursor.lockState = CursorLockMode.None;
                }
            }

            //casting Spells
            live_charStats.charSkillCombat._skillArray[1]._skillInput = Input.GetKey(KeyCode.Mouse1);
            live_charStats.charSkillCombat._skillArray[0]._skillOtherInput = Input.GetKey(KeyCode.Mouse1);            

            pauseKeyPressed = Input.GetKeyDown(KeyCode.Escape) && live_charStats.charInfo._isPlayer;

            if (!live_charStats.charInfo._playerInputEnable) IsometricInputClass();//je�li nie ma flagi player input //isometric
            if (live_charStats.charInfo._playerInputEnable) PlayerInputClass();//je�li zaznaczona jest flaga playerInputEnable //3rd person
                      
        }        
    }
    ////////////////////////////////////////////////////////////////////  

}
