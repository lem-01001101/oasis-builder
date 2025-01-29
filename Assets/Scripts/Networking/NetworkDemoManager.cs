using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.SharedAR.Colocalization;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class NetworkDemoManager : MonoBehaviour
{

   //log for mobile
   [SerializeField]
   private TMP_Text _logOutput;

   [SerializeField]
   private Texture2D _targetImage;

   [SerializeField]
   private float _targetImageSize;

   [SerializeField]
   private TMP_Text _statusText;

   [SerializeField]
   private TMP_Text _numConnected;

   [SerializeField]
   private TMP_InputField _roomCodeInput;

   [SerializeField]
   private Button _joinAsHostButton;

   [SerializeField]
   private Button _joinAsClientButton;

   [SerializeField]
   private SharedSpaceManager _sharedSpaceManager;

   [SerializeField]
   private GameObject _colocalizationIndicatorPrefab;

   [SerializeField]
   private GameObject _magicBar;

   private string _roomCode;

   private bool _startAsHost;

    // Start is called before the first frame update
    void Start()
    {
      // UI event listeners
      _joinAsHostButton.onClick.AddListener(OnJoinAsHostClicked);
      _joinAsClientButton.onClick.AddListener(OnJoinAsClientClicked);

      // SharedSpaceManager state change callback
      _sharedSpaceManager.sharedSpaceManagerStateChanged += OnColocalizationTrackingStateChanged;

      // Netcode connection event callback
      NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

      _logOutput.text = $"starting image tracking colocalization... colocalization type:{_sharedSpaceManager.GetColocalizationType()}";
      Debug.Log("Start!");

      HideButtons();
      _magicBar.gameObject.SetActive(false);

      /*
            // Set SharedSpaceManager and start it
      _sharedSpaceManager.sharedSpaceManagerStateChanged += OnColocalizationTrackingStateChanged;

             // Set room to join
        var mockTrackingArgs = ISharedSpaceTrackingOptions.CreateMockTrackingOptions();
        var roomArgs = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
            "ExampleRoom", // use fixed room name
            32, // set capacity to max
            "vps colocalization demo (mock mode)" // description
        );
        _sharedSpaceManager.StartSharedSpace(mockTrackingArgs, roomArgs);
        */

        var imageTrackingOptions = ISharedSpaceTrackingOptions.CreateImageTrackingOptions(
            _targetImage, _targetImageSize);
         var roomOptions = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
            "ImageTrackingDemoRoom",
            3, // Max capacity
            "image tracking colocalization demo"
         );

         _sharedSpaceManager.StartSharedSpace(imageTrackingOptions, roomOptions);

    }

   private void OnColocalizationTrackingStateChanged(SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs args)
   {

      
      _logOutput.text = $"Trying to track image... Tracking={args.Tracking}";
      //Debug.Log($"Trying to track image... Tracking={args.Tracking}");
      if (args.Tracking)
      {
         Debug.Log("Colocalization active! Players are synced.");
         _logOutput.text = "Colocalization Active!";

         _joinAsHostButton.gameObject.SetActive(true);
         _joinAsClientButton.gameObject.SetActive(true);

       
         Instantiate(_colocalizationIndicatorPrefab, _sharedSpaceManager.SharedArOriginObject.transform, false);
         // _logOutput.text = "Indicator Prefab Instantiated!";

         /*
         if(_startAsHost)
         {
            NetworkManager.Singleton.StartHost();
            HideButtons();
            //_statusText.text = $"Hosting room: {_roomCode}";
            //j_logOutput.text = "Hosting!";
         }
         else
         {
            NetworkManager.Singleton.StartClient();
            HideButtons();
            // why is this being called when you click host??
            //_logOutput.text = "Client";
         }
         */
      }
      else
      {
         _logOutput.text = "Image not tracking...";
      }
   }

   private void OnJoinAsHostClicked()
   {

      // Start the Shared Space as a host
      /*
      var imageTrackingOptions = ISharedSpaceTrackingOptions.CreateImageTrackingOptions(_targetImage, _targetImageSize);

      // Generate a random 3-letter room code
      _roomCode = GenerateRoomCode();
      Debug.Log($"Hosting room: {_roomCode}");

      var roomOptions = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(_roomCode, 3, "Host Room");

      _sharedSpaceManager.StartSharedSpace(imageTrackingOptions, roomOptions);

      _startAsHost = true;
      _statusText.text = $"Hosting room: {_roomCode}";
      */
      NetworkManager.Singleton.StartHost();
      HideButtons();
      _magicBar.gameObject.SetActive(true);
   }

   private void OnJoinAsClientClicked()
   {
      /*
      var imageTrackingOptions = ISharedSpaceTrackingOptions.CreateImageTrackingOptions(_targetImage, _targetImageSize);
      string roomCode = _roomCodeInput.text.Trim().ToUpper();
      if (string.IsNullOrEmpty(roomCode))
      {
         Debug.LogError("Room code is empty!");
         _statusText.text = "Enter a valid room code.";
         return;
      }

      Debug.Log($"Joining room: {roomCode}");

      var roomOptions = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(roomCode, 3, "Client Room");

      _sharedSpaceManager.StartSharedSpace(imageTrackingOptions, roomOptions);

      _startAsHost = false;
      _statusText.text = $"Joining room: {_roomCode}";
      */

      NetworkManager.Singleton.StartClient();
      HideButtons();
      _magicBar.gameObject.SetActive(true);
   }

   private string GenerateRoomCode()
   {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      return new string(Enumerable.Repeat(chars, 3).Select(s => s[Random.Range(0, s.Length)]).ToArray());
   }

   private void HideButtons()
   {
      _joinAsHostButton.gameObject.SetActive(false);
      _joinAsClientButton.gameObject.SetActive(false);
      _roomCodeInput.gameObject.SetActive(false);
   }

   private void OnClientConnectedCallback(ulong clientId)
   {
      Debug.Log($"Client connected: {clientId}");
      _numConnected.text = $"Connected: {clientId}";
   }
}
