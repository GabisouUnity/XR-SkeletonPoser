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

        public enum HandType { Left, Right } // TODO: Could possibly be accessed and set from the XRController?

        [Tooltip("What hand is attached to the XR_SkeletonPoseInteractor?")]
        public HandType handType;

        #endregion

        private XRSkeletonPose _defaultPose;
        private XRSkeletonPoser _selectedPoser;
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
            _defaultPose = GetDefaultPose();
        }

        private void Update()
        {
            CheckForBlendInput();
        }

        private XRSkeletonPose GetDefaultPose()
        {
            var pose = ScriptableObject.CreateInstance<XRSkeletonPose>();

            switch (handType)
            {
                case HandType.Left:
                    pose.leftBonePositions = handObject.GetComponentsInChildren<Transform>()
                        .Select(x => x.localPosition).ToArray();
                    pose.leftBoneRotations = handObject.GetComponentsInChildren<Transform>()
                        .Select(x => x.localRotation).ToArray();
                    break;
                case HandType.Right:
                    pose.rightBonePositions = handObject.GetComponentsInChildren<Transform>()
                        .Select(x => x.localPosition).ToArray();
                    pose.rightBoneRotations = handObject.GetComponentsInChildren<Transform>()
                        .Select(x => x.localRotation).ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return pose;
        }

        private void SetDefaultPose()
        {
            switch (handType)
            {
                case HandType.Left:
                {
                    for (int i = 0; i < _handBones.Length; i++)
                    {
                        _handBones[i].localPosition = _defaultPose.leftBonePositions[i];
                        _handBones[i].localRotation = _defaultPose.leftBoneRotations[i];
                    }

                    // Reset main hand object to local 0,0,0

                    _handBones[0].localPosition = Vector3.zero;
                    _handBones[0].localRotation = Quaternion.identity;
                    break;
                }
                case HandType.Right:
                {
                    for (int i = 0; i < _handBones.Length; i++)
                    {
                        _handBones[i].localPosition = _defaultPose.rightBonePositions[i];
                        _handBones[i].localRotation = _defaultPose.rightBoneRotations[i];
                    }

                    // Reset main hand object to local 0,0,0

                    _handBones[0].localPosition = Vector3.zero;
                    _handBones[0].localRotation = Quaternion.identity;
                    break;
                }
            }
        }

        private void SetOffset()
        {
            // Get grabbable's attach point
            var selectTargetVar = ((XRGrabInteractable) selectTarget);
            
            // Move first index (hand model parent) to the grabbable's attach transform
            if (selectTargetVar.attachTransform == null) return;

            _handBones[0].localPosition = selectTargetVar.attachTransform.localPosition;
            _handBones[0].localRotation = selectTargetVar.attachTransform.localRotation;
        }

        private void SetPose(XRSkeletonPose pose)
        {
            // Get hand bones

            _handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            var leftPosePos = pose.leftBonePositions;
            var leftPoseRot = pose.leftBoneRotations;

            var rightPosePos = pose.rightBonePositions;
            var rightPoseRot = pose.rightBoneRotations;

            // Set values to loaded pose

            switch (handType)
            {
                case HandType.Left:
                {
                    for (int i = 0; i < _handBones.Length; i++)
                    {
                        _handBones[i].localPosition = leftPosePos[i];
                        _handBones[i].localRotation = leftPoseRot[i];
                    }

                    break;
                }
                case HandType.Right:
                {
                    for (int i = 0; i < _handBones.Length; i++)
                    {
                        _handBones[i].localPosition = rightPosePos[i];
                        _handBones[i].localRotation = rightPoseRot[i];
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Reset main hand object to local 0,0,0

            _handBones[0].localPosition = Vector3.zero;
            _handBones[0].localRotation = Quaternion.identity;
        }
        
        protected override void OnSelectEnter(XRBaseInteractable interactable)
        {
            base.OnSelectEnter(interactable);

            // Do not run the below code if the object isn't a skeleton poser, ie do not pose hand if not a poser interactable
            if (!interactable.TryGetComponent(out _selectedPoser)) return;

            var pose = _selectedPoser.FetchPose();
            
            SetPose(pose);
            
            SetOffset();
            
            _isSkeletonPoseInteractable = true;
            _shouldCheckForBlendInput = _selectedPoser.useBlend;
        }
        
        protected override void OnSelectExit(XRBaseInteractable interactable)
        {
            base.OnSelectExit(interactable);

            if (_isSkeletonPoseInteractable)
                
                SetDefaultPose(); // Reset back to default bone pose on select exit if it was a skeleton poser

            _isSkeletonPoseInteractable = false;
            _shouldCheckForBlendInput = false;
        }

        private void CheckForBlendInput()
        {
            if (!_shouldCheckForBlendInput) return;

            var device = _inputController.inputDevice;
            
            // Get input and convert to common usages
            var triggerUsage = CommonUsages.trigger;

            _handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            if (handType == HandType.Left)
            {
                // Check for input
                switch (_selectedPoser.blendInput)
                {
                    case XRSkeletonPoser.BlendInput.Trigger:
                        // Get value
                        device.TryGetFeatureValue(triggerUsage, out var triggerValue);
            
                        // Blend Pose
                        _selectedPoser.BlendLeftPose(_handBones, triggerValue);
                        SetOffset();
                        break;
                }
            }
            else if (handType == HandType.Right)
            {
                switch (_selectedPoser.blendInput)
                {
                    case XRSkeletonPoser.BlendInput.Trigger:
                        // Get value
                        device.TryGetFeatureValue(triggerUsage, out var triggerValue);
            
                        // Blend Pose
                        _selectedPoser.BlendRightPose(_handBones, triggerValue);
                        SetOffset();
                        break;
                }
            }
            
        }
        
    }
}