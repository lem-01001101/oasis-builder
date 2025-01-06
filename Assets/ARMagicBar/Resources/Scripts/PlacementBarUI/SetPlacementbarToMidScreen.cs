using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ARMagicBar.Resources.Scripts.Debugging;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace ARMagicBar.Resources.Scripts.PlacementBarUI
{
    public class SetPlacementBarToMidScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform placementBarParent;
        private List<RectTransform> allRectTransforms = new List<RectTransform>();
        private Vector2 lastScreenSize;
        private int lastChildCount = 0;
        private float anchoredPositionY;

        private void Awake()
        { 
            anchoredPositionY = placementBarParent.anchoredPosition.y;
        }

        void Start()
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            StartCoroutine(DelayedUpate());
        }
    
        IEnumerator DelayedUpate()
        {
            yield return new WaitForSeconds(.1f);
            UpdatePlacement();
        }

        void Update()
        {
            // Check if the screen size has changed (due to orientation or other reasons)
            if (lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
            {
                lastScreenSize = new Vector2(Screen.width, Screen.height);
                UpdatePlacement();
            }
        
            //If the amount if childregn changes,  reset positoin
            if (lastChildCount != placementBarParent.childCount)
            {
                lastChildCount = placementBarParent.childCount;
                UpdatePlacement();
            }
        }

        void UpdatePlacement()
        {
            GetAllChildRectTransforms();
            SetParentPositionToMinusHalfXOfLastObject();
            // AdjustYPosition();
        }

        void GetAllChildRectTransforms()
        {
            allRectTransforms = placementBarParent.GetComponentsInChildren<RectTransform>().ToList();
            CustomLog.Instance.InfoLog("All Rect Transforms => " +  allRectTransforms.Count);
        }

        void AdjustYPosition()
        {
            if (allRectTransforms.Count == 0)
            {
                return;
            }
            
            Vector2 newPosition = placementBarParent.anchoredPosition;
            newPosition.y = 35;
            
            placementBarParent.anchoredPosition = newPosition;
            
        }

        void SetParentPositionToMinusHalfXOfLastObject()
        {
            if (allRectTransforms.Count == 0)
                return;

            float furthestPosition = Mathf.NegativeInfinity;
            RectTransform furthestObject = null;

            //find the furthest right object's position
            foreach (var rectObject in allRectTransforms)
            {
                // Use the position in the parent's coordinate system
                float rightEdgePosition = rectObject.anchoredPosition.x + rectObject.rect.width * rectObject.pivot.x;
                if (rightEdgePosition > furthestPosition)
                {
                    furthestPosition = rightEdgePosition;
                    furthestObject = rectObject;
                }
            }

            if (furthestObject != null)
            {
                // Adjust the parent's position based on the furthest object found
                placementBarParent.anchoredPosition = new Vector2(-furthestPosition / 2, anchoredPositionY);
            }
        }
    }
}

