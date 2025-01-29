using System;
using System.Collections.Generic;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.Other;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using ARMagicBar.Resources.Scripts.TransformLogic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMagicBar.Resources.Scripts.PlacementBar
{
    public class ARPlacementPlaneMesh : MonoBehaviour
    {
        // Start is called before the first frame update
        
        private TransformableObject placementObject; 
        private List<TransformableObject> instantiatedObjects = new();

        public List<TransformableObject> getTransformableObjects
        {
            get => instantiatedObjects;
        }
        
        private Camera mainCam;
        private bool placed;

        
        [Header("Plane detection requires an AR Raycast Manager to be in the scene.")]
       [SerializeField] public ARPlacementMethod placementMethod;
        

       public static ARPlacementPlaneMesh Instance;
       public event Action<TransformableObject> OnSpawnObject;
       
       
       /// <summary>
       /// Can be used for example when not placing objects but using it for something else
       /// </summary>

       
       //Fires when tapping on the screen, gives info about screenpos & objectToSpawn
       public event Action<(TransformableObject objectToSpawn, Vector2 screenPos)> OnSpawnObjectWithScreenPos; 
       
       //Fires when the raycast after tapping the screen hits something, gives info about hitPosition, rotation & objectToSpawn
       public event Action<(TransformableObject objectToSpawn, Vector3 hitPointPosition, Quaternion hitPointRotation)> OnSpawnObjectWithHitPosAndRotation;
       
       //Fires after an object has been spawned with reference to the object
       public event Action<PlacementObjectSO, GameObject> OnObjectSpawnedWithSO;
       
       
       //Fires when the screen was tapped with info about the positoin
       public event Action<Vector3> OnHitScreenAt;
       
       //Fires when a plane or mesh was hit with info about position and normal 
       public event Action<(Vector3 position, Quaternion normal)> OnHitPlaneOrMeshAt;
       
       //Returns reference to the object that was hit by a raycast
       public event Action<GameObject> OnHitMeshObject; 
       
       [Header("Use deactivate spawning, when you want to use the tap for something else, e.g. select a spell in the bar and cast it")]
       [SerializeField] bool deactivateSpawning; 
       

       public bool SetDeactivateSpawning
       {
           set;
           get;
       }

       #if UNITY_XR_ARKIT_LOADER_ENABLED || NIANTIC_LIGHTSHIP_AR_LOADER_ENABLED
        private ARRaycastManager arRaycastManager;
        #endif

       private void Awake()
       {
           if (!FindObjectOfType<EventSystem>())
           {
               Debug.LogError(AssetName.NAME + ": No event system found, please add an event system to the scene");
           }
            
#if !UNITY_XR_ARKIT_LOADER_ENABLED && !NIANTIC_LIGHTSHIP_AR_LOADER_ENABLED
            Debug.Log($"{AssetName.NAME}: No AR Loader enabled");
#endif
            
           mainCam = FindObjectOfType<Camera>();
           Instance = this; 
       }

       private void Start()
       {
           
#if !UNITY_XR_ARKIT_LOADER_ENABLED && !NIANTIC_LIGHTSHIP_AR_LOADER_ENABLED
           placementMethod = ARPlacementMethod.meshDetection;
#else
           PreparePlacementMethod();
#endif
           
           CheckIfPlacementBarInScene();
       }

       private static void CheckIfPlacementBarInScene()
        {
            if (PlacementBarLogic.Instance == null)
            {
                CustomLog.Instance.InfoLog("Please add PlacementBarLogic to scene");
            }
        }
       
       
#if  UNITY_XR_ARKIT_LOADER_ENABLED || NIANTIC_LIGHTSHIP_AR_LOADER_ENABLED
        void PreparePlacementMethod()
        {
            switch (placementMethod)
            {
                case ARPlacementMethod.planeDetection:
                    FindAndAssignRaycastManager();
                    break;
                case ARPlacementMethod.meshDetection:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //returns a raycast positon
        public (Vector3 hitPos, Vector3 hitNorm) GetRaycastHitpointFromScreenPosition(Vector2 screenPos)
        {
            (Vector3 hitPosition, Quaternion hitNormal) raycastHit;
            
            if (placementMethod == ARPlacementMethod.meshDetection)
            {
                return GetPositionRayMeshing(screenPos);
                
            } else if (placementMethod == ARPlacementMethod.planeDetection)
            {
                return GetPositionARPlaneDetection(screenPos);
            }

            return (Vector3.zero, Vector3.zero);
        }

        void FindAndAssignRaycastManager()
        {
            if(placementMethod == ARPlacementMethod.meshDetection) return;
            CustomLog.Instance.InfoLog("Find assign Raycast => placemethod = " + placementMethod);

            arRaycastManager = FindObjectOfType<ARRaycastManager>();
            
            if (arRaycastManager == null)
            {
                Debug.LogError("Please add a AR raycast manager to your scene");
            }
        }
#endif


        private void Update()
        {
            CheckForPlacement();
        }

        // Update is called once per frame
        void CheckForPlacement()
        {
            if(EventSystem.current == null) return;
    #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                   CustomLog.Instance.InfoLog("Click / Tap over UI object");
                    return;
                }
                
                if (placementMethod == ARPlacementMethod.planeDetection)
                {
                    if (!TouchToRayMeshing(Input.mousePosition))
                    {
                        TouchToRayPlaneDetection(Input.mousePosition);
                    }
                }
                else
                {
                    // Debug.Log("Placement Method =>  Meshing");
                    TouchToRayMeshing(Input.mousePosition);
                    OnHitScreenAt?.Invoke(Input.mousePosition);
                }
                
                OnSpawnObjectWithScreenPos?.Invoke((PlacementBarLogic.Instance.GetPlacementObject(), Input.mousePosition));
            }
    #endif
    #if UNITY_IOS || UNITY_ANDROID
            
            if (Input.touchCount > 0 && Input.touchCount < 2 &&
                Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Touch touch = Input.GetTouch(0);
                
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = touch.position;

                List<RaycastResult> results = new List<RaycastResult>();

                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0) {
                    // We hit a UI element
                    Debug.Log("We hit an UI Element");
                    return;
                }
                
                if (placementMethod == ARPlacementMethod.planeDetection)
                {
                    if (!TouchToRayMeshing(touch.position))
                    {
                        TouchToRayPlaneDetection(touch.position);
                    }
                }
                else
                {
                    TouchToRayMeshing(touch.position);
                    OnHitScreenAt?.Invoke(touch.position);
                }
                
                OnSpawnObjectWithScreenPos?.Invoke((PlacementBarLogic.Instance.GetPlacementObject(), touch.position));
            }
    #endif
        }
        
        
        //Shoot ray against AR planes
        void TouchToRayPlaneDetection(Vector3 touch)
        {
    #if UNITY_XR_ARKIT_LOADER_ENABLED || NIANTIC_LIGHTSHIP_AR_LOADER_ENABLED
            if (deactivateSpawning)
            {
                OnHitScreenAt?.Invoke(touch);
            }
            
            
            Ray ray = mainCam.ScreenPointToRay(touch);
            List<ARRaycastHit> hits = new();

            arRaycastManager.Raycast(ray, hits, TrackableType.Planes);
            CustomLog.Instance.InfoLog("ShootingRay Plane Detection, hitcount => " + hits.Count);
            if (hits.Count > 0)
            {
                InstantiateObjectAtPosition(hits[0].pose.position, Quaternion.LookRotation(Vector3.forward));
                    // hits[0].pose.rotation);
            }
    #endif
        }

        //Shoot ray against procedural AR Mesh
        bool TouchToRayMeshing(Vector3 touch)
        {
            if (deactivateSpawning)
            {
                OnHitScreenAt?.Invoke(touch);
            }
            
            CustomLog.Instance.InfoLog("ShootingRay AR Meshing");

            Ray ray = mainCam.ScreenPointToRay(touch);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                OnHitMeshObject?.Invoke(hit.transform.gameObject);
                InstantiateObjectAtPosition(hit.point, hit.transform.rotation);
                return true;
            }

            return false;
        }
        
        (Vector3 pos, Vector3 norm) GetPositionRayMeshing(Vector3 touch)
        {
            CustomLog.Instance.InfoLog("ShootingRay AR Meshing");

            Ray ray = mainCam.ScreenPointToRay(touch);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                OnHitMeshObject?.Invoke(hit.transform.gameObject);
                InstantiateObjectAtPosition(hit.point, hit.transform.rotation);
            }
            
            return (hit.point, hit.normal);
        }

        (Vector3 pos, Vector3 rot) GetPositionARPlaneDetection(Vector2 touch)
        {
#if UNITY_XR_ARKIT_LOADER_ENABLED || NIANTIC_LIGHTSHIP_AR_LOADER_ENABLED
            if (deactivateSpawning)
            {
                OnHitScreenAt?.Invoke(touch);
            }
            
            
            Ray ray = mainCam.ScreenPointToRay(touch);
            List<ARRaycastHit> hits = new();

            arRaycastManager.Raycast(ray, hits, TrackableType.Planes);
            CustomLog.Instance.InfoLog("ShootingRay Plane Detection, hitcount => " + hits.Count);
            if (hits.Count > 0)
            {
                return (hits[0].pose.position, hits[0].pose.rotation.eulerAngles);
                // hits[0].pose.rotation);
            }
#endif
            return (Vector3.zero, Vector3.zero);

        }

        
        /// <summary>
        /// Method to externally call to spawn an object, Invokes the OnSpawnObject event 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SpawnObjectAtPosition(Vector3 position, Quaternion rotation)
        {
            InstantiateObjectAtPosition(position, rotation);
        }

        //Instantiate Object at the raycast position
        void InstantiateObjectAtPosition(Vector3 position, Quaternion rotation)
        {
            /*
            CustomLog.Instance.InfoLog("Should instantiate object at position " + position);


            if (deactivateSpawning)
            {
                CustomLog.Instance.InfoLog("Preventing Spawning as deactivate Spawning is enabled");
                (Vector3, Quaternion) positionRotation = (position, rotation);
                OnHitPlaneOrMeshAt?.Invoke(positionRotation);
                return;
            }
            
            OnSpawnObjectWithHitPosAndRotation?.Invoke((PlacementBarLogic.Instance.GetPlacementObject(), position, rotation));
            
            placementObject = PlacementBarLogic.Instance.GetPlacementObject();
            
            CustomLog.Instance.InfoLog("Placement-Object: " + placementObject);
            
            if(!placementObject) return;
            
            ReferenceToSO soToObject = placementObject.gameObject.GetComponent<ReferenceToSO>();
            
            CustomLog.Instance.InfoLog("soToObject " + soToObject);


            CustomLog.Instance.InfoLog("Corresponding SO to Object: " + 
                                       soToObject.correspondingObject);

            //When Placing is disabled
            if (soToObject != null && !soToObject.correspondingObject.IsPlaceable || 
                !soToObject.correspondingObject.GetIsEnabledInUI())
            {
                CustomLog.Instance.InfoLog($"Placement is disabled for {soToObject.correspondingObject}");
                return;
            }
            
            
            CustomLog.Instance.InfoLog("PlacementBarLogic.Instance-GetPlacementObj => " + placementObject + " "
                + "functionReturns: " + PlacementBarLogic.Instance.GetPlacementObject());
            
            //Check if it should place
            if(placementObject == null) return;
            
            TransformableObject placeObject = Instantiate(placementObject);
            CustomLog.Instance.InfoLog("Placeobject => Instantiate " + placeObject.name);
            
            OnSpawnObject?.Invoke(placeObject);
            
            placeObject.transform.position = position;
            placeObject.transform.rotation = rotation;
            
            instantiatedObjects.Add(placeObject);
            OnObjectSpawnedWithSO?.Invoke(placeObject.GetCorrespondingPlacementObject, placeObject.gameObject);
            */

            // networking version
            CustomLog.Instance.InfoLog("Should instantiate object at position " + position);

            if (deactivateSpawning)
            {
                CustomLog.Instance.InfoLog("Preventing Spawning as deactivate Spawning is enabled");
                (Vector3, Quaternion) positionRotation = (position, rotation);
                OnHitPlaneOrMeshAt?.Invoke(positionRotation);
                return;
            }

            placementObject = PlacementBarLogic.Instance.GetPlacementObject();
            if (!placementObject) return;

            TransformableObject placeObject = Instantiate(placementObject);
            CustomLog.Instance.InfoLog("Placeobject => Instantiate " + placeObject.name);

            // Networking logic
            var networkObject = placeObject.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                {
                    networkObject.Spawn(); // Spawn for clients
                    CustomLog.Instance.InfoLog($"Spawned network object: {placeObject.name}");
                }
                else
                {
                    Debug.LogError($"Failed to spawn object: {placeObject.name} is missing a NetworkObject component.");
                }
            }

            placeObject.transform.position = position;
            placeObject.transform.rotation = rotation;

            instantiatedObjects.Add(placeObject);
            OnObjectSpawnedWithSO?.Invoke(placeObject.GetCorrespondingPlacementObject, placeObject.gameObject);            
        }
    }
    
    public enum ARPlacementMethod
    {
        planeDetection, 
        meshDetection
    }
}