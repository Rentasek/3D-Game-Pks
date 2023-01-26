using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public partial class CameraController : MonoBehaviour
{

    [SerializeField] private float mouseSensivity;

    private Transform playerTransform;
    [SerializeField] public GameObject player;    
    [SerializeField] public CharacterStatus live_charStats;
    [SerializeField] public Player_Input player_Input;
    [SerializeField] List<IPlayerUpdate> playerObjectsList;

    [SerializeField] private Vector3 standardCameraOffset;
    [SerializeField] private Vector3 isometricCameraOffset;

    [SerializeField] private float timeFollowOffset = 1f;
    [SerializeField] private Vector3 currentCameraVelocity = Vector3.zero;
    [SerializeField] private Vector3 cameraReferencePoint = Vector3.zero;
    [SerializeField, CanBeNull] private UI_StatusBar[] playerStatusBars;

    [SerializeField] private float cameraDistanceYMin, cameraDistanceYMax, cameraDistanceZMin, cameraDistanceZMax;
    [SerializeField] private Vector3 cameraDistanceIsometricMin, cameraDistanceIsometricMax;
   
    private void OnValidate()
    {
        SearchForPlayer();
    }

    // Start is called before the first frame update
    void Start()
    {
        SearchForPlayer();
    }

    // Update is called once per frame  //update u�ywa� do input GetKeyDown, dzia�aj� bez laga
    private void Update()
    {
        CameraDistance();

        if (Input.GetKeyDown(KeyCode.X)) SwitchCursorOptions();
        if (live_charStats.inputEnableMouseRotate) MouseRotate();

        CameraFollowPlayer();
    }

    private void MouseRotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;

        playerTransform.Rotate(Vector3.up, mouseX);
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
            } //je�li player steruje -> lock cursora, je�li nie cursor widoczny
        }
    }

    void CameraDistance()
    {
        //Mouse Roll zmienie odleg�o�� kamery
        float mouseScroll = -1 * live_charStats.inputMouseScroll; //InvertScrollMouse
        if (live_charStats.playerInputEnable)
        {
            standardCameraOffset.y = Mathf.Clamp(standardCameraOffset.y + (cameraDistanceYMax - cameraDistanceYMin) * mouseScroll, cameraDistanceYMin, cameraDistanceYMax);
            standardCameraOffset.z = Mathf.Clamp(standardCameraOffset.z + (cameraDistanceZMax - cameraDistanceZMin) * mouseScroll, cameraDistanceZMin, cameraDistanceZMax);
        }
        else
        {
            isometricCameraOffset = isometricCameraOffset + mouseScroll * new Vector3(1f, 1f, -1f);
        }
    }    

    public void SearchForPlayer()
    {        
        player = GameObject.FindGameObjectWithTag("Player");       //szuka tylko po aktywnych objectach z Tagiem Player
        
        if (player != null)
        {
            playerTransform = player.transform;

            live_charStats = Camera.main.GetComponent<CameraController>().player.GetComponent<CharacterStatus>();
            player_Input = Camera.main.GetComponent<CameraController>().player.GetComponent<Player_Input>();

            //Dla wszystkich Object�w zawieraj�cych komponent Interface -> IPlayerUpdate        
            playerObjectsList = new List<IPlayerUpdate>(FindObjectsOfType<Object>().OfType<IPlayerUpdate>());
            foreach (IPlayerUpdate playerUpdate in playerObjectsList)
            {
                playerUpdate.PlayerUpdate();
            }
            
            live_charStats.isPlayer = true;
            player.GetComponent<CharControler>().PlayerUpdate();

            //Podmianka UI Status bar�w na aktulnego playera
            if (playerStatusBars != null)
            {
                foreach (UI_StatusBar playerStatusBar in playerStatusBars)
                {
                    playerStatusBar.live_charStats = live_charStats;
                }
            }
        } 
    }
}
