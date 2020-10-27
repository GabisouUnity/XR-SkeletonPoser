using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XRSkeletonPose : ScriptableObject
    {
        
        public Vector3[] leftBonePositions = null;
        public Quaternion[] leftBoneRotations = null;
        
        [Space]
        
        public Vector3[] rightBonePositions = null;
        public Quaternion[] rightBoneRotations = null;
        
        [Space]
        [Space]
        
        public Vector3[] leftSecondaryPositions = null;
        public Quaternion[] leftSecondaryRotations = null;

        [Space]
        
        public Vector3[] rightSecondaryPositions = null;
        public Quaternion[] rightSecondaryRotations = null;

    } 
}
