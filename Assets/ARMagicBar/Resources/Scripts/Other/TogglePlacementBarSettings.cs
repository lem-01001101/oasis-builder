using System;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ARMagicBar.Resources.Scripts.Other
{
    public class TogglePlacementBarSettings : MonoBehaviour
    {
        [FormerlySerializedAs("disableTransform")]
        [Header("Disable the selection, transformation and interaction for all objects")]
        [SerializeField] private bool disableInteraction;

        private bool currentDisableTransform; 

        public bool SetDisableTransform
        {
            set => disableInteraction = value;
            get => disableInteraction;
        }

        private void Update()
        {
            if (disableInteraction != currentDisableTransform)
            {
                SelectObjectsLogic.Instance.DisableTransformOptions = disableInteraction;
                currentDisableTransform = disableInteraction;
            }
        }

        private void Start()
        {
            currentDisableTransform = disableInteraction;
            CustomLog.Instance.InfoLog("Setting DisableTransfomr to => " +  disableInteraction);
            // TransformableObjectsSelectLogic.Instance.DisableTransformOptions = disableTransform;
            SelectObjectsLogic.Instance.DisableTransformOptions = disableInteraction;
        }
        
        
        
    }
} 