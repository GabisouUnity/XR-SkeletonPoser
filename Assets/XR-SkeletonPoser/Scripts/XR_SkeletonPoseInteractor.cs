using System.Linq;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XR_SkeletonPoseInteractor : XRDirectInteractor
    {

        // public XR_SkeletonPose pose;
        
        private Transform[] _handBones = null;

        private Vector3[] _defaultBonePos = null;
        private Quaternion[] _defaultBoneRot = null;
        
        [Space]
        public GameObject handObject;

        private void SetDefaultPose()
        {
            _handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            for (int i = 0; i < _handBones.Length; i++)
            {
                // Error here, value is null. Stops select exit :/
                _defaultBonePos[i] = _handBones[i].localPosition;
                _defaultBoneRot[i] = _handBones[i].localRotation;
            }
        }
        
        private void SetPose(XR_SkeletonPose pose)
        {
            // Get hand bones
            
            _handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            Vector3[] posePos = pose.leftBonePositions;
            Quaternion[] poseRot = pose.leftBoneRotations;
            
            // Set values to loaded pose
            for (int i = 0; i < _handBones.Length; i++)
            {
                _handBones[i].localPosition = posePos[i];
                _handBones[i].localRotation = poseRot[i];
            }
            
            // Reset main hand object to local 0,0,0

            _handBones[0].localPosition = Vector3.zero;
            _handBones[0].localRotation = Quaternion.identity;
        }

        private void OffsetHand()
        {
            
        }
        
        protected override void OnSelectEnter(XRBaseInteractable interactable)
        {
            base.OnSelectEnter(interactable);
            
            if(interactable.TryGetComponent(out XR_SkeletonPoser poser))
            {
                XR_SkeletonPose pose = poser.GetLoadedPose();
                
                SetPose(pose);
            }
        }

        protected override void OnSelectExit(XRBaseInteractable interactable)
        {
            base.OnSelectExit(interactable);
            
            SetDefaultPose();
        }
    }
}