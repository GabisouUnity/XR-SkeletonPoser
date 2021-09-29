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

    } 
}
