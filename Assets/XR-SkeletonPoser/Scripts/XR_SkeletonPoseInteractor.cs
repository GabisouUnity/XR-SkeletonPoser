using System.Linq;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XR_SkeletonPoseInteractor : XRDirectInteractor
    {

        // public XR_SkeletonPose pose;
        
        private Transform[] _handBones = null;
        public GameObject handObject;

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
                pose = poser.GetLoadedPose();
                
                SetPose(pose);
            }
        }
    }
}