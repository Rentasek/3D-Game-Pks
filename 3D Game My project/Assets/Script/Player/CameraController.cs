using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float mouseSensivity;

    private Transform playerTransform;
    [SerializeField] public GameObject player;
    //[SerializeField] private bool enableMouseMovement;
    [SerializeField] public CharacterStatus live_charStats;
    [SerializeField] public Player_Input player_Input;

    [SerializeField] private Vector3 standardCameraOffset;
    [SerializeField] private Vector3 isometricCameraOffset;

   /* [SerializeField] private float xOffset = 0;
    [SerializeField] private float yOffset = 2;
    [SerializeField] private float zOffset = 4;*/
    
    [SerializeField] private float timeFollowOffset = 1f;
    [SerializeField] private Vector3 currentCameraVelocity = Vector3.zero;
    [SerializeField] private Vector3 cameraReferencePoint = Vector3.zero;

    private void OnValidate()
    {
        live_charStats = player.GetComponent<CharacterStatus>();
        player_Input = player.GetComponent<Player_Input>();
    }


    // Start is called before the first frame update
    void Start()
    {

        
        playerTransform =  player.transform;
        SwitchCursorOptions();
        
    }


    // Update is called once per frame  //update u�ywa� do input GetKeyDown, dzia�aj� bez laga
    private void Update()  
    {


        if (Input.GetKeyDown(KeyCode.X)) SwitchCursorOptions();
        if (live_charStats.inputEnableMouseRotate) MouseRotate();
        
        CameraFollowPlayer();



       /* ///Przeniesione z Player_Input
        ///
                

        //dzia�a tylko z komponentem z przypi�t� main.camera
        if (Input.GetKeyDown(KeyCode.P) && live_charStats.isPlayer) live_charStats.playerInputEnable = !live_charStats.playerInputEnable;//input Switch

        player_Input.InputClass();



        if (!live_charStats.playerInputEnable) player_Input.IsometricInputClass();//je�li nie ma flagi player input //isometric
        if (live_charStats.playerInputEnable) player_Input.PlayerInputClass();//je�li zaznaczona jest flaga playerInputEnable //3rd person

        //GameSave/Load
        if (live_charStats.inputSaveGame && live_charStats.isPlayer) { player_Input.SaveState(); live_charStats.GetComponent<CharacterMovement>().SetCharacterPosition(); } //przy save state ustawia backup pozycje 
        if (live_charStats.inputLoadGame && live_charStats.isPlayer) { player_Input.LoadState(); live_charStats.GetComponent<CharacterMovement>().ResetCharacterPosition(); } //przy load state resetuje do backup pozycji 

*/

    }
    private void MouseRotate()
    {
        float mouseX = Input.GetAxis("Mouse X")*mouseSensivity*Time.deltaTime;

        playerTransform.Rotate(Vector3.up,mouseX);
    }
    void CameraFollowPlayer()
    {

        transform.LookAt(new Vector3(playerTransform.position.x + cameraReferencePoint.x, playerTransform.position.y + cameraReferencePoint.y, playerTransform.position.z + cameraReferencePoint.z), Vector3.up);
        //funkcja lookAt z mo�liwo�ci� dopasowania punktu na kt�ry si� patrzy

        //transform.position = Vector3.MoveTowards(transform.position, (playerTransform.position - playerTransform.forward * zOffset + playerTransform.up * yOffset), timeFollowOffset * Time.deltaTime);

        if (live_charStats.playerInputEnable)
        {
            transform.position = Vector3.SmoothDamp(transform.position, (playerTransform.right * standardCameraOffset.x + (playerTransform.position - playerTransform.forward * standardCameraOffset.z) + playerTransform.up * standardCameraOffset.y), ref currentCameraVelocity, timeFollowOffset);
            // smoothDamp jest p�ynniejsze od MoveTowards
            // transform position z offsetami -> wektor .forward w osi Z jest !!lokalny!! dla playerTransform, tak samo wektor .up w osi Y
            // w tym przypadku dost�pne s� tylko wektory -> .forward (blue axis) / .up (green axis) / .right(red axis)
        }
        else
        {
            transform.position = new Vector3(playerTransform.position.x + isometricCameraOffset.x, playerTransform.position.y + isometricCameraOffset.y, playerTransform.position.z + isometricCameraOffset.z);
        }
    }
    public void SwitchCursorOptions()
    {
        if (live_charStats.isPlayer)
        {     
            if (live_charStats.playerInputEnable) //jesli 3rd person view
            {                
                live_charStats.inputEnableMouseRotate = !live_charStats.inputEnableMouseRotate; //w��czanie i wy��czanie booleana przyciskiem on/off

                if (live_charStats.inputEnableMouseRotate) Cursor.lockState = CursorLockMode.Locked; //kursor zablokwany na �rodku ekranu
                else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
            }
            else //je�li isometric
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } //je�li player steruje -> lock cursora, je�li nie cursor widoczny i zamkni�ty w okienku
        }
            
    }





}
