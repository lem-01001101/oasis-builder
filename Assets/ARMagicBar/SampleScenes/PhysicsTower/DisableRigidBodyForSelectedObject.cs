using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;

namespace ARMagicBar.SampleScenes.PhysicsTower
{
    public class DisableRigidBodyForSelectedObject : MonoBehaviour
    {
        void Start()
        {
            SelectObjectsLogic.Instance.OnSelectObjectInfo += DisableRigidbodyOnSelection;
            SelectObjectsLogic.Instance.OnDeselectAll += EnableRigidbodyOnDeselect;
        }
    
    
        Rigidbody rb; 


        private void EnableRigidbodyOnDeselect()
        {
            if (rb)
            {
                rb.isKinematic = false;
            }
        }

        //Transformable object is a script that sits on the main parent of our placeable objects.
        //Through this reference we can access a component in its children. 
        //This is not very efficient, we could also attach a script to each placeable object,
        //but it's more convenient to do it here. 
    
        private void DisableRigidbodyOnSelection(TransformableObject obj)
        {
            rb = obj.GetComponentInChildren<Rigidbody>();

            if (rb)
            {
                rb.isKinematic = true; 
            }
        }


    }
}
