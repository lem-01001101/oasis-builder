using ARMagicBar.Resources.Scripts.PlacementBar;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;

namespace ARMagicBar.SampleScenes.SpellCaster
{
    public class ShootSpellsLogic : MonoBehaviour
    {
        [SerializeField] private Camera mainCam; 
        

        //In this script the "ARPlacementPlaneMesh.Instance.OnSpawnObjectWithScreenPos", event is being used to 
        // spawn a spell directly on screen. On the placeable object scriptable object the "isPlaceable", bool is
        //disabled. 
        void Start()
        {
            ARPlacementPlaneMesh.Instance.OnSpawnObjectWithScreenPos += InstanceOnOnSpawnObjectWithScreenPos;
        }

        //We can reference the object to spawn from the event and instead of letting it spawn at the hit position as usual, spawn it with this 
        //script on the screen and shoot it along the camera axis.
        private void InstanceOnOnSpawnObjectWithScreenPos((TransformableObject objectToSpawn, Vector2 screenPos) args)
        {
            Vector3 spawnPosition = mainCam.ScreenToWorldPoint(new Vector3(args.screenPos.x, args.screenPos.y, mainCam.nearClipPlane)) 
                                    + mainCam.transform.forward * 0.1f;

            TransformableObject gameObject = Instantiate(args.objectToSpawn, 
                spawnPosition, 
                Quaternion.LookRotation(mainCam.transform.forward));
    
            Destroy(gameObject.gameObject, 3f);
        }
        
    }
}
