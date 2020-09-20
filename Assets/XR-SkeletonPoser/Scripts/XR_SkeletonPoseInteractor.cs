using System.Linq;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XRSkeletonPoseInteractor : XRDirectInteractor
    {
        public Transform[] handBones = null;
        
        [Space]
        public GameObject handObject;

        public enum HandType { Left, Right }
        public HandType handType;
        
        private XR_SkeletonPose _defaultPose;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Cache default pose at runtime
            _defaultPose = GetDefaultPose();
        }

        private XR_SkeletonPose GetDefaultPose()
        {
            XR_SkeletonPose pose = ScriptableObject.CreateInstance<XR_SkeletonPose>();

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
                    for (int i = 0; i < handBones.Length; i++)
                    {
                        handBones[i].localPosition = _defaultPose.leftBonePositions[i];
                        handBones[i].localRotation = _defaultPose.leftBoneRotations[i];
                    }
                
                    // Reset main hand object to local 0,0,0

                    handBones[0].localPosition = Vector3.zero;
                    handBones[0].localRotation = Quaternion.identity;
                    break;
                }
                case HandType.Right:
                {
                    for (int i = 0; i < handBones.Length; i++)
                    {
                        handBones[i].localPosition = _defaultPose.rightBonePositions[i];
                        handBones[i].localRotation = _defaultPose.rightBoneRotations[i];
                    }
                
                    // Reset main hand object to local 0,0,0

                    handBones[0].localPosition = Vector3.zero;
                    handBones[0].localRotation = Quaternion.identity;
                    break;
                }
            }
        }

        private void SetOffset()
        {
            // Get grabbable's attach point
            var selectTargetAttach = ((XRGrabInteractable) selectTarget).attachTransform;

            // Set offset of transform index 0 on the hand to the cube
            handBones[0].localPosition = selectTargetAttach.localPosition;
            handBones[0].localRotation = selectTargetAttach.localRotation;
        }
        
        private void SetPose(XR_SkeletonPose pose)
        {
            // Get hand bones
            
            handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            var leftPosePos = pose.leftBonePositions;
            var leftPoseRot = pose.leftBoneRotations;

            var rightPosePos = pose.rightBonePositions;
            var rightPoseRot = pose.rightBoneRotations;
            
            // Set values to loaded pose

            switch (handType)
            {
                case HandType.Left:
                {
                    for (int i = 0; i < handBones.Length; i++)
                    {
                        handBones[i].localPosition = leftPosePos[i];
                        handBones[i].localRotation = leftPoseRot[i];
                    }

                    break;
                }
                case HandType.Right:
                {
                    for (int i = 0; i < handBones.Length; i++)
                    {
                        handBones[i].localPosition = rightPosePos[i];
                        handBones[i].localRotation = rightPoseRot[i];
                    }

                    break;
                }
            }
            
            // Reset main hand object to local 0,0,0

            handBones[0].localPosition = Vector3.zero;
            handBones[0].localRotation = Quaternion.identity;
        }
        
        protected override void OnSelectEnter(XRBaseInteractable interactable)
        {
            base.OnSelectEnter(interactable);

            if (!interactable.TryGetComponent(out XR_SkeletonPoser poser)) return;
            XR_SkeletonPose pose = poser.GetLoadedPose();
                
            SetPose(pose);
            SetOffset();
        }

        protected override void OnSelectExit(XRBaseInteractable interactable)
        {
            base.OnSelectExit(interactable);

            // attachTransform.position = _defaultOffset.position;
            SetDefaultPose(); // Reset back to default bone pose on select exit
        }

    }
}