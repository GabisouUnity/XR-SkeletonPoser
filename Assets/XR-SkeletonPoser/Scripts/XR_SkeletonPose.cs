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

    } 
}
