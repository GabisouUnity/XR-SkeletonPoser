using System.Linq;
using System.Net.Configuration;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XR_SkeletonPoseDirectInteractor : XRDirectInteractor
    {

        #region Editor storage

        [Space] [Tooltip("What hand is a child of the controller?")] public GameObject handObject;
        
        public enum HandType { Left, Right } // TODO: Could possibly be accessed and set from the XRController?
        [Tooltip("What hand is attached to the XR_SkeletonPoseInteractor?")] public HandType handType;
        
        #endregion
        
        private XR_SkeletonPose _defaultPose;
        private Transform[] _handBones = null;

        private bool _isSkeletonPoseInteractable = false;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Cache default pose at runtime
            _defaultPose = GetDefaultPose();
        }

        private XR_SkeletonPose GetDefaultPose()
        {
            var pose = ScriptableObject.CreateInstance<XR_SkeletonPose>();

            switch (handType)
            {
                case HandType.Left:
                    pose.leftBonePositions = handObject.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray();
                    pose.leftBoneRotations = handObject.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
                    break;
                case HandType.Right:
                    pose.rightBonePositions = handObject.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray();
                    pose.rightBoneRotations = handObject.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
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
            
            // var selectTargetAttach = ((XRGrabInteractable) selectTarget).attachTransform;

            // Move first index (hand model parent) to the grabbable's attach transform
            if (selectTargetVar.attachTransform != null)
            {
                _handBones[0].localPosition = selectTargetVar.attachTransform.localPosition;
                _handBones[0].localRotation = selectTargetVar.attachTransform.localRotation;
            }
        }
        
        private void SetPose(XR_SkeletonPose pose)
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
            }
            
            // Reset main hand object to local 0,0,0

            _handBones[0].localPosition = Vector3.zero;
            _handBones[0].localRotation = Quaternion.identity;
        }
        
        protected override void OnSelectEnter(XRBaseInteractable interactable)
        {
            base.OnSelectEnter(interactable);
            
            // Do not run the below code if the object isn't a skeleton poser, ie do not pose hand if not a poser interactable
            if (!interactable.TryGetComponent(out XR_SkeletonPoser poser)) return;
            
            var pose = poser.GetLoadedPose();
            
            SetPose(pose);
            SetOffset();
            _isSkeletonPoseInteractable = true;
        }

        protected override void OnSelectExit(XRBaseInteractable interactable)
        {
            base.OnSelectExit(interactable);

            if(_isSkeletonPoseInteractable) SetDefaultPose(); // Reset back to default bone pose on select exit if it was a skeleton poser
            
            _isSkeletonPoseInteractable = false;
        }

    }
}