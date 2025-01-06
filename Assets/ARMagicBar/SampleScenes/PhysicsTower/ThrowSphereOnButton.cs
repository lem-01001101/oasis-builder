using UnityEngine;
using UnityEngine.UI;

namespace ARMagicBar.SampleScenes.PhysicsTower
{
    public class ThrowSphereOnButton : MonoBehaviour
    {
        [SerializeField] private Camera cam; 
        [SerializeField] private Rigidbody sphere; 
        [SerializeField] private Button throwButton;
        // Start is called before the first frame update
        void Start()
        {
            throwButton.onClick.AddListener(ThrowSphere);
        }

        void ThrowSphere()
        {
            Rigidbody spawnedSphere = Instantiate(sphere, cam.transform.position + cam.transform.forward, 
                Quaternion.identity);
        
            spawnedSphere.AddForce(cam.transform.forward * 220f);
        
            Destroy(spawnedSphere, 10f);
        }
    
    }
}
