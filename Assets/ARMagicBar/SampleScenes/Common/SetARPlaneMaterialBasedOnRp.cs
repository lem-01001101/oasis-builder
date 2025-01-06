using ARMagicBar.Resources.Scripts.Debugging;
using UnityEngine;
using UnityEngine.Rendering;

namespace ARMagicBar.SampleScenes.Common
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SetARPlaneMaterialBasedOnRp : MonoBehaviour
    {
        [Header("Material to use on URP")]
        [SerializeField] private Material planeMaterialURP;
    
        [Header("Material to use on BiRP")]
        [SerializeField] private Material planeMaterialBirp;

        private Material materialToApply;
        private MeshRenderer meshRenderer;
        // Start is called before the first frame update
        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            SetSelectedMaterialDependingOnRP();
            ApplyMaterial();
        }


        void ApplyMaterial()
        {
            meshRenderer.sharedMaterial = materialToApply;
        }
    
        //Check for URP
        void SetSelectedMaterialDependingOnRP()
        {
            bool isURP = GraphicsSettings.renderPipelineAsset != null &&
                         GraphicsSettings.renderPipelineAsset.GetType().Name.Contains("Universal");
            
            if (isURP)
            {
                materialToApply = planeMaterialURP;
            }
            else
            {
                materialToApply = planeMaterialBirp;
            }
            
            CustomLog.Instance.InfoLog("Detected Render Pipeline URP? " + isURP);
        }
    }
}
