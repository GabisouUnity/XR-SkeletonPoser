using System.Linq;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XR_SkeletonPoseInteractor : XRDirectInteractor
    {

        public Transform[] _handBones = null;

        private void SetPose(XR_SkeletonPose pose)
        {
            // Get hand bones
            
            // Bug: Gets attach models, and also the controller. Need to remove everything that isn't under the glove model
            _handBones = GetComponentsInChildren<Transform>().ToArray();

            Vector3[] posePos = pose.leftBonePositions;
            Quaternion[] poseRot = pose.leftBoneRotations;
            
            // Vector3[] rightPosePos = pose.rightBonePositions;
            // Quaternion[] rightPoseRot = pose.rightBoneRotations;

            // Set values to loaded pose
            for (int i = 0; i < _handBones.Length; i++)
            {
                _handBones[i].localPosition = posePos[i];
                _handBones[i].localRotation = poseRot[i];
            }
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
    }
}