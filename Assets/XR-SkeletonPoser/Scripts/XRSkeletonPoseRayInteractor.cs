using System.Linq;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace yellowyears.SkeletonPoser
{
    public class XRSkeletonPoseRayInteractor : XRRayInteractor
    {
        
        #region Editor storage

        [Space] [Tooltip("What hand is a child of the controller?")] public GameObject handObject;
        
        public enum HandType { Left, Right } // TODO: Could possibly be accessed and set from the XRController?
        [Tooltip("What hand is attached to the XR_SkeletonPoseInteractor?")] public HandType handType;
        
        #endregion

        private XRSkeletonPose _defaultPose;
        private Transform[] _handBones = null;

        private bool _isSkeletonPoseInteractable = false;

        protected override void Awake()
        {
            base.Awake();
            
            // Cache default pose at runtime
            _defaultPose = GetDefaultPose();
        }

        private XRSkeletonPose GetDefaultPose()
        {
            var pose = ScriptableObject.CreateInstance<XRSkeletonPose>();

            switch (handType)
            {
                case HandType.Left:
                    pose.leftHandPositions = handObject.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray();
                    pose.leftHandRotations = handObject.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
                    break;
                case HandType.Right:
                    pose.rightHandPositions = handObject.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray();
                    pose.rightHandRotations = handObject.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
                    break;
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
                        _handBones[i].localPosition = _defaultPose.leftHandPositions[i];
                        _handBones[i].localRotation = _defaultPose.leftHandRotations[i];
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
                        _handBones[i].localPosition = _defaultPose.rightHandPositions[i];
                        _handBones[i].localRotation = _defaultPose.rightHandRotations[i];
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
            var selectTargetAttach = ((XRGrabInteractable) selectTarget).attachTransform;

            // Move first index (hand model parent) to the grabbable's attach transform
            _handBones[0].localPosition = selectTargetAttach.localPosition;
            _handBones[0].localRotation = selectTargetAttach.localRotation;
        }
        
        private void SetPose(XRSkeletonPose pose)
        {
            // Get hand bones
            
            _handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            var leftPosePos = pose.leftHandPositions;
            var leftPoseRot = pose.leftHandRotations;

            var rightPosePos = pose.rightHandPositions;
            var rightPoseRot = pose.rightHandRotations;
            
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
            }
            
            // Reset main hand object to local 0,0,0

            _handBones[0].localPosition = Vector3.zero;
            _handBones[0].localRotation = Quaternion.identity;
        }
        
        protected override void OnSelectEnter(XRBaseInteractable interactable)
        {
            base.OnSelectEnter(interactable);
            
            // Do not run the below code if the object isn't a skeleton poser, ie do not pose hand if not a poser interactable
            if (!interactable.TryGetComponent(out XRSkeletonPoser poser)) return;
            
            var pose = poser.FetchPose();
                
            SetPose(pose);
            SetOffset();
            _isSkeletonPoseInteractable = true;
        }

        protected override void OnSelectExit(XRBaseInteractable interactable)
        {
            base.OnSelectExit(interactable);

            if(_isSkeletonPoseInteractable) SetDefaultPose(); // Reset back to default bone pose on select exit if it was a poser interactable

            _isSkeletonPoseInteractable = false;
        }
    }
}