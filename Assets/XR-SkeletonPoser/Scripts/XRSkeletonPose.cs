using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XRSkeletonPose : ScriptableObject
    {
        
        // Left hand values

        public Vector3[] leftBonePositions = null;
        public Quaternion[] leftBoneRotations = null;

        // Right hand values

        public Vector3[] rightBonePositions = null;
        public Quaternion[] rightBoneRotations = null;
        
        // Blending stuff
        
        // public string blendName = "My Blend";
        // [Tooltip("The pose that we blend to, from this pose")] public XRSkeletonPose blendTo;

        public Vector3[] leftSecondaryPositions = null;
        public Quaternion[] leftSecondaryRotations = null;

        public Vector3[] rightSecondaryPositions = null;
        public Quaternion[] rightSecondaryRotations = null;

    } 
}
