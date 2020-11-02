using System;
using System.Collections;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.XR;

namespace yellowyears.SkeletonPoser
{
    public class XRSkeletonPoseDirectInteractor : XRDirectInteractor
    {

        #region Editor storage

        [Space] [Tooltip("What hand is a child of the controller?")]
        public GameObject handObject;
        
        [Tooltip("What hand is attached to the XR_SkeletonPoseInteractor?")]
        public HandType handType;

        #endregion

        private XRSkeletonPose _defaultPose;
        private XRSkeletonPoser _poser;
        private XRController _inputController;
        private Transform[] _handBones = null;

        private XRSkeletonPoserSettings _poserSettings;
        private bool _isSkeletonPoseInteractable = false;
        private bool _shouldCheckForBlendInput = false;

        protected override void Awake()
        {
            base.Awake();

            _inputController = GetComponent<XRController>();
            _poserSettings = XRSkeletonPoserSettings.Instance;

            // Cache default pose at runtime
            _defaultPose = XRSkeletonPoser.GetDefaultPose(handType, handObject);
        }

        private void Update()
        {
            if (!_poser) return;
            
            _poser.CheckForBlendInput(_shouldCheckForBlendInput, _inputController, _handBones, handObject, handType, selectTarget);
        }
        
        protected override void OnSelectEnter(XRBaseInteractable interactable)
        {
            base.OnSelectEnter(interactable);

            // Do not run the below code if the object isn't a skeleton poser, ie do not pose hand if not a poser interactable
            if (!interactable.TryGetComponent(out _poser)) return;

            var pose = _poser.FetchPose();
            
            _poser.SetPose(pose, handObject, handType);
            
            _poser.SetOffset(selectTarget, _handBones);
            
            _isSkeletonPoseInteractable = true;
            _shouldCheckForBlendInput = _poser.useBlend;
        }
        
        protected override void OnSelectExit(XRBaseInteractable interactable)
        {
            base.OnSelectExit(interactable);

            if (_isSkeletonPoseInteractable)
            {
                _poser.SetDefaultPose(handType, _handBones, _defaultPose); // Reset back to default bone pose on select exit if it was a skeleton poser
            }
            
            _isSkeletonPoseInteractable = false;
            _shouldCheckForBlendInput = false;
        }
        
    }
}