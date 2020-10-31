using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XRSkeletonPose : ScriptableObject
    {
        
        public Vector3[] leftHandPositions = null;
        public Quaternion[] leftHandRotations = null;
        
        [Space]
        
        public Vector3[] rightHandPositions = null;
        public Quaternion[] rightHandRotations = null;
        
        [Space]
        [Space]
        
        public Vector3[] leftSecondaryPositions = null;
        public Quaternion[] leftSecondaryRotations = null;

        [Space]
        
        public Vector3[] rightSecondaryPositions = null;
        public Quaternion[] rightSecondaryRotations = null;

    } 
}
