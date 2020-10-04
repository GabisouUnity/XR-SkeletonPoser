using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XR_SkeletonPose : ScriptableObject
    {

        // Left hand values

        public Vector3[] leftBonePositions;
        public Quaternion[] leftBoneRotations;

        // Right hand values

        public Vector3[] rightBonePositions;
        public Quaternion[] rightBoneRotations;
        
        // Blending stuff

        public string blendName = "My Blend";
        [Tooltip("The pose that we blend to, from this pose")] public XR_SkeletonPose blendTo;

    } 
}
