using System.Collections.Generic;
using System.Linq;
using ARMagicBar.Resources.Scripts.Debugging;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace ARMagicBar.Resources.Scripts.TransformLogic
{
    
    //Sits on each transformable object, handles the "selected material" if clicked on
    [RequireComponent(typeof(TransformableObject))]
    public class SelectVisualLogic : MonoBehaviour
    {
        [SerializeField] private bool deactivateSelectionMaterial; 
        
        // [SerializeField] private Transform visual;
        [SerializeField] private Material selectedMaterial;
        [SerializeField] private Material selectedMaterialURP;

        private Material materialToApply;
        
        // private Material[] baseSharedMaterials;
        // private List<(Renderer renderer, Material[] baseMaterial, Material[] selectedMaterial)> rendererToMaterials = new List<(Renderer, Material[], Material[])>();
        // private Material[] selectMaterials;

        [FormerlySerializedAs("objectRenderer")]
        [Tooltip(
            "If the objects renderer is somewhere hidden in the hierarchy, rather drag it in, otherwise it will be automatically added.")]
        [SerializeField]
        private List<Renderer> objectRenderers = new();

        [SerializeField] private UnityEngine.Transform parentOfRenderer;
        
        private Dictionary<Renderer, (Material[], Material[])> rendererBaseSelectMaterialDict = new();
        
        private TransformableObject _transformableObject;


        private void Awake()
        {
            objectRenderers = GetRenderer();

            if (objectRenderers != null)
            {
                SetBaseMaterials();
                SetSelectMaterials();
            }
        }
        public List<Renderer> ReturnRenderer()
        {
            return objectRenderers;
        }
        
        //Get all renderer types
        List<Renderer> GetRenderer()
        {
            List<Renderer> childRenderer = new List<Renderer>();
            CustomLog.Instance.InfoLog("Get Renderer");
            
            if (GetComponentInChildren<PlacementObjectVisual.PlacementObjectVisual>())
            {
                PlacementObjectVisual.PlacementObjectVisual objectVisual = GetComponentInChildren<PlacementObjectVisual.PlacementObjectVisual>();
                parentOfRenderer = objectVisual.transform;
                
                
                if (objectVisual.GetComponent<Renderer>() && (!objectVisual.GetComponent<LODGroup>() && !objectVisual.GetComponentInChildren<LODGroup>()))
                {
                    if (!objectVisual.GetComponent<ParticleSystem>())
                    {
                        Renderer objectRenderer = objectVisual.GetComponent<Renderer>();
                        childRenderer.Add(objectVisual.GetComponent<Renderer>());
                    }
                }
                else
                {
                    UnityEngine.Transform[] childObjects = objectVisual.GetComponentsInChildren<UnityEngine.Transform>();
                    foreach (var childtransf in childObjects)
                    {
                        if(childtransf.GetComponent<Renderer>())
                        {
                            if(childtransf.GetComponent<ParticleSystem>()){ continue; }
                            CustomLog.Instance.InfoLog("Adding " + childtransf.GetComponent<Renderer>().name);
                            childRenderer.Add(childtransf.GetComponent<Renderer>());
                        }
                    }
                }
            }
            return childRenderer;
        }

        //Add base material state
        void SetBaseMaterials()
        {
            foreach (var renderer in objectRenderers)
            {
                Renderer rend = renderer;
                Material[] baseMaterials = renderer.sharedMaterials;
                Material[] selectedMaterials = null;
                
                rendererBaseSelectMaterialDict.Add(rend, (baseMaterials, selectedMaterials));
                
                // (Renderer renderer, Material[] baseMaterials, Material[] selectedMaterials) rendererToMaterial = new(renderer, renderer.sharedMaterials, null);
                // rendererToMaterials.Add(rendererToMaterial);
            }
        }


        private bool isURP;
        
        //Check for URP
        void SetSelectedMaterialDependingOnRP()
        {
            isURP = GraphicsSettings.renderPipelineAsset != null &&
                         GraphicsSettings.renderPipelineAsset.GetType().Name.Contains("Universal");
            
            if (isURP)
            {
                materialToApply = selectedMaterialURP;
            }
            else
            {
                materialToApply = selectedMaterial;
            }
            
            CustomLog.Instance.InfoLog("Detected Render Pipeline URP? " + isURP);
        }

        //Add selected Material state 
        private void SetSelectMaterials()
        {
            SetSelectedMaterialDependingOnRP();

            for (int i = 0; i < rendererBaseSelectMaterialDict.Count; i++)
            {
                //New Array at the materials length +1
                Material[] selectMaterials = new Material[rendererBaseSelectMaterialDict.ElementAt(i).Value.Item1.Length +1];
                
                for (int j = 0; j < rendererBaseSelectMaterialDict.ElementAt(i).Value.Item1.Length; j++)
                {
                    selectMaterials[j] =  rendererBaseSelectMaterialDict.ElementAt(i).Value.Item1[j];
                }
                
                selectMaterials[selectMaterials.Length - 1] = materialToApply;

                Renderer currentKey = rendererBaseSelectMaterialDict.Keys.ElementAt(i);
                Material[] currentBaseMaterials = rendererBaseSelectMaterialDict.Values.ElementAt(i).Item1;
                
                rendererBaseSelectMaterialDict.Remove(currentKey);
                
                
                rendererBaseSelectMaterialDict.Add(currentKey,(currentBaseMaterials, selectMaterials));
            }
        }



        private void Start()
        {
            // Hide();
            _transformableObject = GetComponent<TransformableObject>();
            _transformableObject.OnWasSelected += TransformableObjectWasSelected;
            SelectObjectsLogic.Instance.OnDeselectAll += Hide;
        }
        
        void TransformableObjectWasSelected(bool wasSelected)
        {
           CustomLog.Instance.InfoLog("transformable was selected " +  wasSelected +  " " + transform.name);
            if (wasSelected)
            {
                Show();
            }
            else
            {
                //check if the base materials have been changed
                // RefreshBaseMaterials(); 
                Hide();
            }
        }

        private void OnDestroy()
        {
            SelectObjectsLogic.Instance.OnDeselectAll -= Hide;
            
            if(_transformableObject)
                _transformableObject.OnWasSelected -= TransformableObjectWasSelected;
        }
        
        private void Show()
        {
            if (deactivateSelectionMaterial) return;

            for (int i = 0; i < objectRenderers.Count; i++)
            {
                Renderer renderer = objectRenderers[i];
                Material[] selectMaterials = rendererBaseSelectMaterialDict[renderer].Item2;
                renderer.sharedMaterials = selectMaterials;
            }
        }

        private void RefreshBaseMaterials()
        {
            for (int i = 0; i < rendererBaseSelectMaterialDict.Count; i++)
            {
                Renderer renderer = rendererBaseSelectMaterialDict.Keys.ElementAt(i);
                Material[] currentMaterials = renderer.sharedMaterials
                    .Where(mat => mat != selectedMaterial && mat != selectedMaterialURP)
                    .ToArray();

                CustomLog.Instance.InfoLog("Setting the material of  Renderer" +  rendererBaseSelectMaterialDict.Keys.ElementAt(i).name + " to "
                    );

                foreach (var currentMat in currentMaterials)
                {
                    CustomLog.Instance.InfoLog("currentMat name: "+ currentMat.name);
                }
                
                rendererBaseSelectMaterialDict[renderer] = (currentMaterials, rendererBaseSelectMaterialDict[renderer].Item2);
            }
        }

        private void Hide()
        {
            if (deactivateSelectionMaterial) return;
            
            RefreshBaseMaterials();
            SetSelectMaterials();


            for (int i = 0; i < objectRenderers.Count; i++)
            {
                Renderer renderer = objectRenderers[i];
                Material[] baseMaterials = rendererBaseSelectMaterialDict[renderer].Item1;
                renderer.sharedMaterials = baseMaterials;
            }
        }
    }
}