using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.PlacementBar;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using ARMagicBar.Resources.Scripts.PlacementObjectVisual;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEditor;
using UnityEngine;


namespace ARMagicBar.Resources.Editor.EditorScript
{
    public class ManagePlaceableObjectsEditor : EditorWindow
    {
        private PlaceableObjectSODatabase _placeableObjectSoDatabase;
    
        private List<GameObject> prefabs = new List<GameObject>();
        private List<Sprite> images = new List<Sprite>();

        private List<GameObject> loadedPrefabsRef = new List<GameObject>();
        private List<Sprite> loadedImageRefs = new List<Sprite>();
    
        private const int maxObjectCount = 9;
        private Vector2 scrollPosition;

 
        private const string maxPrefabMessage =
            "Max placement objects reached for this database. You can create another one or delete unused objects.";
    

        [SerializeField]
        private TransformableObject placeableObjectTemplate;
    
    

        [MenuItem("Window/AR Magic Bar/Prefab and Image Editor")]
        public static void ShowWindow()
        {
            GetWindow<ManagePlaceableObjectsEditor>("Prefab and Image Editor");
        }
    
        void AddToDatabase(PlacementObjectSO placementObjectSo)
        {
            CustomLog.EnsureInstance();
        
            _placeableObjectSoDatabase.PlacementObjectSos.Add(placementObjectSo);
            EditorUtility.SetDirty(_placeableObjectSoDatabase); // Mark it dirty to ensure it saves
            AssetDatabase.SaveAssets();
        }
        
        private void RefreshAssetDatabase()
        {
            RemoveMissingPrefabsFromDatabase();
            AssetDatabase.Refresh();
        }
        
        private string GetDatabaseDirectory()
        {
            if (_placeableObjectSoDatabase == null)
            {
                Debug.LogError("PlaceableObjectSODatabase is not assigned.");
                return null;
            }
            
            // Refresh the asset database to ensure all paths are up to date
            RefreshAssetDatabase();

            string assetPath = AssetDatabase.GetAssetPath(_placeableObjectSoDatabase);
    
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Could not find the asset path for PlaceableObjectSODatabase.");
                return null;
            }

            // Extract the directory from the asset path
            string directoryPath = Path.GetDirectoryName(assetPath);

            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.LogError("Could not determine the directory path from the asset path.");
                return null;
            }

            return directoryPath;
        }

        private void CheckSaveDirectory()
        {
            string directoryPath = GetDatabaseDirectory();

            if (directoryPath == null)
            {
                Debug.LogError("Failed to determine the save directory path.");
                return;
            }

            if (Directory.Exists(directoryPath))
            {
                CustomLog.Instance.InfoLog("Save directory exists: " + directoryPath);
            }
            else
            {
                Debug.LogError("Error: Save directory does not exist - " + directoryPath);
            }
        }

        private void OnInspectorUpdate()
        {
            this.Repaint();
            CheckForNulls();
        }

        private void Awake()
        {
            CustomLog.EnsureInstance();

            if(_placeableObjectSoDatabase == null)
                Debug.LogWarning("AR Magic Bar has no database. You can create a new one on your asset window, right click -> Create -> AR Magic Bar -> PlaceableobjectDatabase and assign it.");
            //     FindOrCreateDatabase();
        
        
            // LoadPlaceableObjectTemplate();
            InitializePlaceableObjectTemplate();
            RefreshExistingPlaceableObjects();
        }

        private int selectedIndex = -1;
        
        private void InitializePlaceableObjectTemplate()
        {
            CustomLog.EnsureInstance();

            if (placeableObjectTemplate == null)
            {
                GameObject prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/PlaceableObject/PlaceableObjectTemplate");
                if (prefab != null)
                {
                    placeableObjectTemplate = prefab.GetComponent<TransformableObject>();
                }
                else
                {
                    Debug.LogError("Could not load the PlaceableObject prefab. Please check the path.");
                }
            }
        }
        
    void OnGUI()
    {        
        CustomLog.EnsureInstance();

        EditorGUILayout.BeginVertical(); // Begin Vertical Layout
        GUILayout.Label("Manage Placeable Objects", EditorStyles.boldLabel);
        // Object field for the database
        _placeableObjectSoDatabase = EditorGUILayout.ObjectField("Placeable Object Database", _placeableObjectSoDatabase, typeof(PlaceableObjectSODatabase), false) as PlaceableObjectSODatabase;

        
        GUILayout.Label("Template for the placeable object", EditorStyles.boldLabel);
        placeableObjectTemplate = EditorGUILayout.ObjectField("Placeable Object Template", placeableObjectTemplate, typeof(TransformableObject), false) as TransformableObject;
        
        if (_placeableObjectSoDatabase == null)
        {
            EditorGUILayout.HelpBox("Please assign a PlaceableObjectSODatabase.", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("Refresh Database"))
            {
                RefreshExistingPlaceableObjects();
            }

            GUILayout.Label("Prefab and Image Editor", EditorStyles.boldLabel);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition); // Begin Scroll View

            if (prefabs.Count <= 30)
            {
                if (GUILayout.Button("Add Pair"))
                {
                    CustomLog.Instance.InfoLog("Adding prefab, count is" + prefabs.Count);
                    prefabs.Add(null);
                    images.Add(null);
                }
            }

            for (int i = 0; i < prefabs.Count; i++)
            {
                GUILayout.BeginHorizontal(); // Begin Horizontal Layout

                prefabs[i] = EditorGUILayout.ObjectField("Prefab", prefabs[i], typeof(GameObject), false) as GameObject;
                images[i] = EditorGUILayout.ObjectField("Image", images[i], typeof(Sprite), false) as Sprite;

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    // Safely remove elements here (handled below)
                    SafeRemoveAt(i);
                }

                GUILayout.EndHorizontal(); // End Horizontal Layout
            }
            GUILayout.EndScrollView(); // End Scroll View

            if (GUILayout.Button("Create Placeable Objects"))
            {
                CreatePlaceableObjects();
            }
            
            GUILayout.Space(20); // Add space before the Recreate Prefabs button

            HandleRecreatePrefabsButton(); // Add the Recreate Prefabs button with double-click functionality

            GUILayout.Space(20); // Add space after the Recreate Prefabs button
        }
        EditorGUILayout.EndVertical(); // End Vertical Layout
    }
    
    
    private double lastClickTime = 0;
    private const double doubleClickDelay = 0.3;
    private bool isAwaitingSecondClick = false;
    private double lastInteractionTime = 0;


    private void HandleRecreatePrefabsButton()
    {
        Event e = Event.current;
        
        // Reset button color after rendering it
        GUI.backgroundColor = Color.white;

        // Change the color of the button if awaiting the second click
        if (isAwaitingSecondClick)
        {
            GUI.backgroundColor = Color.red;
        }
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Recreate Prefabs", GUILayout.Height(20), GUILayout.Width(200)))
        {
            double timeSinceLastClick = EditorApplication.timeSinceStartup - lastClickTime;
            if (timeSinceLastClick < doubleClickDelay && isAwaitingSecondClick)
            {
                RecreatePrefabs(); // Only execute if it's a double-click
                ResetButtonState(); // Reset the button state and color
            }
            else
            {
                isAwaitingSecondClick = true;
                lastInteractionTime = EditorApplication.timeSinceStartup;
                Debug.Log("Click again to confirm Recreate Prefabs.");
                // Start coroutine to reset after 5 seconds
                EditorApplication.update += CheckForResetState;
            }
            lastClickTime = EditorApplication.timeSinceStartup;
        }
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Reset button color after rendering it
        GUI.backgroundColor = Color.white;
    }

    private void ResetButtonState()
    {
        isAwaitingSecondClick = false;
        GUI.backgroundColor = Color.white; // Reset the color
        EditorApplication.update -= CheckForResetState; // Stop the coroutine
    }

    private void CheckForResetState()
    {
        // Check if 5 seconds have passed since the last interaction
        if (EditorApplication.timeSinceStartup - lastInteractionTime > 5f && isAwaitingSecondClick)
        {
            Debug.Log("Recreate Prefabs button reset due to inactivity.");
            ResetButtonState();
        }
    }
    
        private void CreatePlaceableObjects()
        {
            HashSet<GameObject> uniquePrefabs = new HashSet<GameObject>();
            HashSet<Sprite> uniqueImages = new HashSet<Sprite>();

            loadedPrefabsRef = prefabs.Distinct().ToList();
            loadedImageRefs = images.Distinct().ToList();

            for (int i = 0; i < loadedPrefabsRef.Count; i++)
            {
                if (loadedPrefabsRef[i] == null) continue;

                if (!uniquePrefabs.Contains(loadedPrefabsRef[i]) && loadedImageRefs[i] != null)
                {
                    uniquePrefabs.Add(loadedPrefabsRef[i]);
                    CreatePlaceableObject(loadedPrefabsRef[i], loadedImageRefs[i].texture);
                }
            }

            RefreshExistingPlaceableObjects(); // Refresh list to reflect the new state
        }
        
        private void RecreatePrefabs()
        {
            if (_placeableObjectSoDatabase == null)
            {
                Debug.LogError("PlaceableObjectSODatabase is not assigned.");
                return;
            }

            int countOfDatabaseSO = _placeableObjectSoDatabase.PlacementObjectSos.Count;

            for(int i = 0; i < countOfDatabaseSO; i++)
                // (var placeableObjectSO in _placeableObjectSoDatabase.PlacementObjectSos)
            {
                var placeableObjectSO = _placeableObjectSoDatabase.PlacementObjectSos[i];
                
                if (placeableObjectSO == null || placeableObjectSO.placementObject == null)
                {
                    continue;
                }

                // Instantiate the prefab
                GameObject instantiatedPrefab = Instantiate(placeableObjectSO.placementObject.gameObject);

                // Find the child with the PlacementObjectVisual script
                var placementVisual = instantiatedPrefab.GetComponentInChildren<PlacementObjectVisual>();
                if (placementVisual == null)
                {
                    Debug.LogWarning($"Prefab {instantiatedPrefab.name} does not have a child with PlacementObjectVisual script attached.");
                    continue;
                }

                // Get the GameObject that contains the PlacementObjectVisual script
                GameObject visualObject = placementVisual.gameObject;
                visualObject.name = instantiatedPrefab.name;

                // Remove the PlacementObjectVisual script
                DestroyImmediate(placementVisual);

                // Recreate the placeable object using the visualObject as the new prefab
                CreatePlaceableObject(visualObject, placeableObjectSO.uiSprite);
                
                //Remove the old asset
                SafeRemoveAt(i);

                // Remove the "(Clone)" suffix from the name
                instantiatedPrefab.name = instantiatedPrefab.name.Replace("(Clone)", "").Trim();
                
                // Destroy the instantiated prefab after use
                DestroyImmediate(instantiatedPrefab);
                

                

            }

            // Refresh the editor to show the updated prefabs
            RefreshExistingPlaceableObjects();
            Debug.Log("Prefabs have been recreated successfully.");
        }
    
        void CheckForNulls()
        {
            CustomLog.EnsureInstance();

            if (prefabs.Count <= 0 && images.Count <= 0) return;

        
            for (int i = 0; i < images.Count; i++)
            {
                if (images[i] == null)
                {
                    // Make sure there is a corresponding element in the database list
                    if (i < _placeableObjectSoDatabase.PlacementObjectSos.Count)
                    {
                        if(_placeableObjectSoDatabase.PlacementObjectSos[i] == null) return;
                    
                        images[i] = TextureToSprite(_placeableObjectSoDatabase.PlacementObjectSos[i].uiSprite);
                    }
                }
            }
        }

        void RefreshExistingPlaceableObjects() {
            CustomLog.EnsureInstance();

            // Clear current lists to avoid duplicates if this method is called multiple times
            RemoveMissingPrefabsFromDatabase();

            prefabs.Clear();
            images.Clear();
            InitializePlaceableObjectTemplate();
            
            // Iterate over the PlaceableObjectSODatabase and populate the lists
            if (_placeableObjectSoDatabase != null) {
                foreach (var placeableObjectSO in _placeableObjectSoDatabase.PlacementObjectSos) {
                    if (placeableObjectSO.placementObject != null && placeableObjectSO.uiSprite != null) {
                    
                        // Assuming the prefab is the parent of the TransformableObject component
                        GameObject prefab = placeableObjectSO.placementObject.gameObject;
                        if (prefab != null) {
                            // Add the prefab to the list
                            prefabs.Add(prefab);
                            // Add the associated sprite to the list
                            images.Add(TextureToSprite(placeableObjectSO.uiSprite));
                        
                            CustomLog.Instance.InfoLog("Loading prefab " + prefab.name);
                        }
                    }
                }
            }

        }
        
    
        /// <summary>
        /// </summary>
        /// <param name="texture2D">Takes Texture2D</param>
        /// <returns>Sprite</returns>
        Sprite TextureToSprite(Texture2D texture2D)
        {
            CustomLog.EnsureInstance();

            Texture2D texture = texture2D; // Assuming this is a Texture2D
            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return newSprite;
        }

        private void RemoveMissingPrefabsFromDatabase()
        {
            if (_placeableObjectSoDatabase != null && _placeableObjectSoDatabase.PlacementObjectSos != null)
                for (int i = 0; i < _placeableObjectSoDatabase.PlacementObjectSos.Count; i++)
                {
                    if (_placeableObjectSoDatabase.PlacementObjectSos[i] == null)
                        _placeableObjectSoDatabase.PlacementObjectSos.RemoveAt(i);
                }
        }
        
        private void CreatePlaceableObject(GameObject prefab, Texture2D image)
        {
            RemoveMissingPrefabsFromDatabase();
            
            CustomLog.EnsureInstance();

            if (placeableObjectTemplate == null || prefab == null || image == null)
            {
                Debug.Log("Missing template, prefab, or image. Skipping creation.");
                return;
            }
            
        
        
            if(_placeableObjectSoDatabase == null || _placeableObjectSoDatabase.PlacementObjectSos == null)
            {
                Debug.LogError("PlaceableObjectSODatabase or its list is null. Cannot create placeable object.");
                return;
            }

            CheckSaveDirectory();


            CustomLog.Instance.InfoLog($"_placeableObjectSoDatabase => {_placeableObjectSoDatabase} " +
                                       $"PlacementObjectSos: {_placeableObjectSoDatabase.PlacementObjectSos}"
                                       + $"Len: {_placeableObjectSoDatabase.PlacementObjectSos.Count}");
            //Duplicate check
            if (_placeableObjectSoDatabase.PlacementObjectSos.Count != 0)
            {
                CustomLog.Instance.InfoLog("_placeableObjectSoDatabase.PlacementObjectSos.Count => " 
                    + _placeableObjectSoDatabase.PlacementObjectSos.Count + " _placeableObjectSoDatabase is " + 
                    _placeableObjectSoDatabase.name);
                
                var existingObject = _placeableObjectSoDatabase.PlacementObjectSos
                    .FirstOrDefault(po => po.placementObject.gameObject.name == prefab.name);
                
                
                //Maybe exchange Image
                if (existingObject != null)
                {
                    CustomLog.Instance.InfoLog($"Skipping creation: An object for {prefab.name} already exists.");

                    if (existingObject.placementObject.gameObject == prefab && existingObject.uiSprite != image)
                    {
                        CustomLog.Instance.InfoLog("Swapping out image");
                        existingObject.uiSprite = image;
                    }
                    
                    return;

                } 
            }
        
            //Instantiate the prefab template
            GameObject newPrefab = Instantiate(placeableObjectTemplate.gameObject);
            
            EditorUtility.SetDirty(newPrefab);
        
        
            //Attach the ReferenceToSO Script
            CustomLog.Instance.InfoLog("Is new prefab active? " + newPrefab.activeSelf);

        
            GameObject childPrefab = Instantiate(prefab, newPrefab.transform);
            childPrefab.transform.localPosition = Vector3.zero;
        
            // childPrefab.transform.localRotation = Quaternion.identity;
            // childPrefab.transform.localPosition += new Vector3(0, childPrefab.transform.localScale.y / 2, 0);
        
            childPrefab.AddComponent<PlacementObjectVisual>();

            // Save the new prefab
            //Assets/PlaceAndManipulateObjects/Resources/PlaceableObjects
            string prefabPath =
                $"{GetDatabaseDirectory()}/{prefab.name}_Placeable.prefab";
            GameObject savedPrefabAsset = PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath);
        
            // Load the saved prefab asset to get the TransformableObject component
            GameObject loadedPrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            TransformableObject transformableObjectComponent = loadedPrefabAsset.GetComponentInChildren<TransformableObject>();

            if (transformableObjectComponent == null)
            {
                Debug.LogError($"Failed to find TransformableObject component in prefab at path: {prefabPath}");
                return;
            }
        
            // Create and set up the ScriptableObject
            PlacementObjectSO placeableObject = ScriptableObject.CreateInstance<PlacementObjectSO>();

            placeableObject.placementObject = transformableObjectComponent;
            placeableObject.uiSprite = image;
            placeableObject.nameOfObject = childPrefab.name;

            if (!placeableObject.name.Contains("_PlaceableObject"))
            {
                placeableObject.name = $"{prefab.name}_PlaceableObject";
            }
        
            // Save the ScriptableObject
            string assetPath = $"{ GetDatabaseDirectory()}/{prefab.name}_PlaceableObject.asset";
            
            AssetDatabase.CreateAsset(placeableObject, assetPath);
            AssetDatabase.SaveAssets(); 
            AssetDatabase.Refresh(); 
        
            CustomLog.Instance.InfoLog($"Created placeable object for {prefab.name}");
            AddToDatabase(placeableObject);
        
            ReferenceToSO referenceToSo = loadedPrefabAsset.GetComponent<ReferenceToSO>() ?? newPrefab.AddComponent<ReferenceToSO>();

            if (referenceToSo == null) {
                Debug.LogError("ReferenceToSO component is null.");
            } else if (placeableObject == null) {
                Debug.LogError("placeableObject is null.");
            } else
            {
                CustomLog.Instance.InfoLog("Should set correspondingObj to => " + placeableObject);
                CustomLog.Instance.InfoLog("Reference Object ToSo => " + referenceToSo);
            
                referenceToSo.SetPlacementObjectSO(placeableObject);
                referenceToSo.SetCorrespondingDatabaseSO(_placeableObjectSoDatabase);
                
                EditorUtility.SetDirty(referenceToSo);
                EditorUtility.SetDirty(referenceToSo.gameObject);
            }
        
            CustomLog.Instance.InfoLog("referencetoSO GetPlacementObejctSO =>" + referenceToSo.GetPlacementObejctSO());
        
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            DestroyImmediate(newPrefab); 
        
        }
        
        private void SafeRemoveAt(int index)
        {
            if (index < 0 || index >= prefabs.Count)
                return;

            // If the prefab and image are not null, remove from the database
            if (prefabs[index] != null && images[index] != null)
            {
                PlacementObjectSO toDelete = _placeableObjectSoDatabase.PlacementObjectSos.FirstOrDefault(so => so.placementObject.gameObject == prefabs[index]);
        
                if (toDelete != null)
                {
                    string soPath = AssetDatabase.GetAssetPath(toDelete);
                    string prefabPath = AssetDatabase.GetAssetPath(toDelete.placementObject.gameObject);

                    _placeableObjectSoDatabase.PlacementObjectSos.Remove(toDelete);

                    AssetDatabase.DeleteAsset(prefabPath);
                    AssetDatabase.DeleteAsset(soPath);
                }
            }
    
            prefabs.RemoveAt(index);
            images.RemoveAt(index);
        }
    
    
        void DebugList(List<GameObject> list)
        {
            CustomLog.EnsureInstance();
        
            foreach (var goj in list)
            {
                if(goj == null) continue;
                CustomLog.Instance.InfoLog("LoadedPrefRefs=> " + goj.name);
            }
        
            CustomLog.Instance.InfoLog("Debug End");
        }
    }
}