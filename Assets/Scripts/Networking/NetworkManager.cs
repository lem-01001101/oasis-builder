using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.SharedAR.Colocalization;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class NetworkDemoManager : MonoBehaviour
{

   // remove
   [SerializeField]
   private TMP_Text _logOutput;


   [SerializeField]
   private Texture2D _targetImage;


   [SerializeField]
   private float _targetImageSize;


   // remove
   [SerializeField]
   private TMP_Text _statusText;


   // remove
   [SerializeField]
   private TMP_Text _numConnected;

   
   [SerializeField]
   private TMP_InputField _roomCodeInput;

   [SerializeField]
   private TMP_Text _roomCodeOutput;

   [SerializeField]
   private Button _joinAsHostButton;

   [SerializeField]
   private Button _joinAsClientButton;

   [SerializeField]
   private SharedSpaceManager _sharedSpaceManager;

   [SerializeField]
   private GameObject _colocalizationIndicatorPrefab;

   [SerializeField]
   private TMP_Text _scanningText;

   [SerializeField]
   private TMP_Text _scanningSuccess;

   [SerializeField]
   private GameObject _startMultiplayerGameButton;

   [SerializeField]
   private GameObject _magicBar;

   [SerializeField]
   private Button _generateCodeButton;

   private string _roomCode;

   private int _MAXPLAYERS = 5;

   private bool _startAsHost;

    // Start is called before the first frame update
    void Start()
    {
      _joinAsHostButton.onClick.AddListener(OnJoinAsHostClicked);
      _joinAsClientButton.onClick.AddListener(OnJoinAsClientClicked);

      // generate code
      _generateCodeButton.onClick.AddListener(GenerateRoomCode);


      // SharedSpaceManager state change callback
      _sharedSpaceManager.sharedSpaceManagerStateChanged += OnColocalizationTrackingStateChanged;

      NetworkManager.Singleton.OnServerStarted += OnServerStarted;

      // Netcode connection event callback
      NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

      // Disconnection Call back
      NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;

      Debug.Log("Start!");

    }

   private void OnColocalizationTrackingStateChanged(SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs args)
   {
      if (args.Tracking)
      {
         Debug.Log("Colocalization active! Players are synced.");

         _scanningText.gameObject.SetActive(false);
         _scanningSuccess.gameObject.SetActive(true);
         _startMultiplayerGameButton.SetActive(true);
               
         // instantiate an object to signify target image have been scanned
         Instantiate(_colocalizationIndicatorPrefab, _sharedSpaceManager.SharedArOriginObject.transform, false);

         if(_startAsHost)
         {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Hosting!");

         }
         else
         {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client!");
         }
      }
      else
      {
         _logOutput.text = "Image not tracking...";
      }
   }

   private void OnJoinAsHostClicked()
   {
      _startAsHost = true;
      var imageTrackingOptions = ISharedSpaceTrackingOptions.CreateImageTrackingOptions(_targetImage, _targetImageSize);

      var roomOptions = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(_roomCode, _MAXPLAYERS, "session");

      _sharedSpaceManager.StartSharedSpace(imageTrackingOptions, roomOptions);


      Debug.Log($"Session started -> (Room Code: {_roomCode})");
      Debug.Log($"New Target Image: {_targetImage.name}");

   }

   private void OnJoinAsClientClicked()
   {
      var imageTrackingOptions = ISharedSpaceTrackingOptions.CreateImageTrackingOptions(_targetImage, _targetImageSize);

      string _curRoomName = _roomCodeInput.text;
      var _curRoomOptions = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(_curRoomName, _MAXPLAYERS, "session");

      _sharedSpaceManager.StartSharedSpace(imageTrackingOptions, _curRoomOptions);

      _scanningText.text = "Scanning Target Image... If Target Image not tracking, recheck Code and/or retake Target Image.";

      _startAsHost = false;
   }

   private void GenerateRoomCode()
   {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      _roomCode = new string(Enumerable.Repeat(chars, 3).Select(s => s[Random.Range(0, s.Length)]).ToArray());

      _roomCodeOutput.text = _roomCode;
   }



   // blit calls
   private void OnEnable()
   {
      // Subscribe to the blitting script's event
      BlittingColocalization.OnTextureRendered += HandleTextureRendered;
   }

   private void OnDisable()
   {
    // Unsubscribe to avoid memory leaks or null refs
      BlittingColocalization.OnTextureRendered -= HandleTextureRendered;
   }

   private void HandleTextureRendered(Texture2D capturedTex)
   {
      _targetImage = capturedTex;
   }


   // networking checks
   private void OnServerStarted()
   {
      Debug.Log("Netcode server is ready.");
   }

   private void OnClientConnectedCallback(ulong clientId)
   {
      Debug.Log($"Client connected: {clientId}");
      _numConnected.text = $"{clientId}";
   }

   private void OnClientDisconnectedCallback(ulong clientId)
   {
      var selfId = NetworkManager.Singleton.LocalClientId;
         if (NetworkManager.Singleton)
         {
            if (NetworkManager.Singleton.IsHost && clientId != NetworkManager.ServerClientId)
            {
                    // ignore other clients' disconnect event
               return;
            }
                // show the UI panel for ending
               //_endPanelText.text = "Disconnected from network";
               //_endPanel.SetActive(true);
         }
   }
}






/*

flow
- generate room code (function) from a user clicking "HOST"
-- user clicks continue button
- do blitting, grab image
-- user clicks start button
- start game (function) when user clicked "START"

functions
- GenerateRoomCode() -> returns nothing, sets _roomCode variable
- GrabImageBlitting
- StartMultiplayerGame()



notifications


*/