using ARMagicBar.Resources.Scripts.GizmoUI.Custom_Interactions;
using ARMagicBar.Resources.Scripts.PlacementBar;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;

namespace ARMagicBar.SampleScenes.PickUpAndPlace
{
    /// <summary>
    /// This script uses custom interactions to let you pick up an object. 
    /// Please find further information on custom interactions in the documentation.
    /// https://zealous-system-734.notion.site/AR-Magic-bar-1-0-documentation-d9c9be0653ce4b919a845e3ac59060ae
    /// </summary>
    public class CustomInteractionAddPlaceableObjectsAtRuntime : MonoBehaviour
    {
        //Defining which interaction is the pick-up interaction by dragging it in. 
        //Custom interactions itself have no "effect" they are simply being called as an event. 
        [SerializeField] CustomInteractionDataSO pickUpInteraction;
        
        //Placeable object database 
        [SerializeField] private PlaceableObjectSODatabase objectDatabase;  

        void Start()
        {
            //Subscribe to this event, it will be triggered upon any custom interaction and contain different information (see below)
            CustomInteractionUI.OnCustomInteractionTriggered += PickUpObject;
            
            //Set our inventory amount of each object to 5 in the beginning. As the placement objects are scripteable objects
            //any changed in-game are being saved. Thus we have to reset the amount at any time. 
            foreach (var placementObject in objectDatabase.PlacementObjectSos)
            {
                placementObject.SetIsEnabledInUI(true);
                placementObject.SetItemEnableToSelectInUI(true);
                placementObject.SetEnableAmountInInventory(true); 
                placementObject.SetAmountInInventory(5);
            }
        }

        
        //Event args contain information on the name of the custom interaction that has been triggered, the reference to the game object that
        //the custom interaction got triggered on, the reference to it's scriptable object and the scriptable object of the custom interaction.
        private void PickUpObject((string nameOfInteraction, 
            GameObject referenceToObject, ReferenceToSO referenceToSo, 
            CustomInteractionDataSO customInteractionDataSo) eventArgs)
        {
            //This particular script is handling object collection, so adding an object in the scene to your inventory. 
        
            //Firstly we are checking if the custom interaction triggered is the pickup interaction we would like to trigger
            if (eventArgs.customInteractionDataSo == pickUpInteraction)
            {
                foreach (var placementObject in objectDatabase.PlacementObjectSos)
                {
                    if (eventArgs.referenceToSo.correspondingObject == placementObject)
                    {
                        Destroy(eventArgs.referenceToObject);
                
                        //Enable the Object that has been collected in UI
                        eventArgs.referenceToSo.correspondingObject.SetIsEnabledInUI(true);
                        eventArgs.referenceToSo.correspondingObject.SetAmountInInventory(placementObject.GetAmountInInventory() + 1);
                    }
                }
                
                //Make sure to trigger the deselection of all objects, since the instantiated object 
                SelectObjectsLogic.Instance.DeselectAllObjects();
            } 
        }
    
    }
}
