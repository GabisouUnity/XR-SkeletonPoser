using System.Linq;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XR_SkeletonPoseInteractor : XRDirectInteractor
    {
        public Transform[] _handBones = null;

        public enum HandType
        {
            Left, Right
        }

        public HandType handType;
        
        private XR_SkeletonPose _defaultPose;
        
        [Space]
        public GameObject handObject;

        protected override void Awake()
        {
            base.Awake();
            
            // Cache default pose at runtime
            _defaultPose = GetDefaultPose();
        }

        private XR_SkeletonPose GetDefaultPose()
        {
            XR_SkeletonPose pose = ScriptableObject.CreateInstance<XR_SkeletonPose>();

            if (handType == HandType.Left)
            {
                pose.leftBonePositions = handObject.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray();
                pose.leftBoneRotations = handObject.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
            }
            else if (handType == HandType.Right)
            {
                pose.rightBonePositions = handObject.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray();
                pose.rightBoneRotations = handObject.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
            }

            return pose;
        }
        
        private void SetDefaultPose()
        {
            if (handType == HandType.Left)
            {
                for (int i = 0; i < _handBones.Length; i++)
                {
                    _handBones[i].localPosition = _defaultPose.leftBonePositions[i];
                    _handBones[i].localRotation = _defaultPose.leftBoneRotations[i];
                }
                
                // Reset main hand object to local 0,0,0

                _handBones[0].localPosition = Vector3.zero;
                _handBones[0].localRotation = Quaternion.identity;
            } 
            else if(handType == HandType.Right)
            {
                for (int i = 0; i < _handBones.Length; i++)
                {
                    _handBones[i].localPosition = _defaultPose.rightBonePositions[i];
                    _handBones[i].localRotation = _defaultPose.rightBoneRotations[i];
                }
                
                // Reset main hand object to local 0,0,0

                _handBones[0].localPosition = Vector3.zero;
                _handBones[0].localRotation = Quaternion.identity;
            }
        }

        private void SetOffset()
        {
            // Set offset of transform index 0 on the hand to the cube
            
            handObject.transform.position = ((XRGrabInteractable) selectTarget).attachTransform.position;
        }
        
        private void SetPose(XR_SkeletonPose pose)
        {
            // Get hand bones
            
            _handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            Vector3[] leftPosePos = pose.leftBonePositions;
            Quaternion[] leftPoseRot = pose.leftBoneRotations;

            Vector3[] rightPosePos = pose.rightBonePositions;
            Quaternion[] rightPoseRot = pose.rightBoneRotations;
            
            // Set values to loaded pose

            if (handType == HandType.Left)
            {
                for (int i = 0; i < _handBones.Length; i++)
                {
                    _handBones[i].localPosition = leftPosePos[i];
                    _handBones[i].localRotation = leftPoseRot[i];
                }
            }
            else if (handType == HandType.Right)
            {
                for (int i = 0; i < _handBones.Length; i++)
                {
                    _handBones[i].localPosition = rightPosePos[i];
                    _handBones[i].localRotation = rightPoseRot[i];
                }
            }
            
            // Reset main hand object to local 0,0,0

            _handBones[0].localPosition = Vector3.zero;
            _handBones[0].localRotation = Quaternion.identity;
        }
        
        protected override void OnSelectEnter(XRBaseInteractable interactable)
        {
            base.OnSelectEnter(interactable);
            
            if(interactable.TryGetComponent(out XR_SkeletonPoser poser))
            {
                XR_SkeletonPose pose = poser.GetLoadedPose();
                
                SetPose(pose);
                SetOffset();
            }
        }

        protected override void OnSelectExit(XRBaseInteractable interactable)
        {
            base.OnSelectExit(interactable);
            
            SetDefaultPose(); // Reset back to default bone pose on select exit
        }

    }
}