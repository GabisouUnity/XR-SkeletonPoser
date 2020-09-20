using UnityEngine;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;

namespace yellowyears.SkeletonPoser
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class XR_SkeletonPoser : MonoBehaviour
    {

        [Tooltip("Left hand model to be spawned in as a left preview. Should be the same as your in game left hand")]
        public GameObject leftHand = null;
        
        [Tooltip("Left hand model to be spawned in as a left preview. Should be the same as your in game left hand")] 
        public GameObject rightHand = null;

        // Serialized Properties
        
        [HideInInspector] public XR_SkeletonPose activePose;
        [HideInInspector] public bool showPoseEditor = false; // Used in editor foldout
        
        [HideInInspector] public bool showLeft;
        [HideInInspector] public GameObject tempLeft;

        [HideInInspector] public bool showRight;
        [HideInInspector] public GameObject tempRight;

        private void Awake()
        {
            // Destroy preview hands on awake so they are not visible as we play the game.
            DestroyLeftPreview(tempLeft);
            DestroyRightPreview(tempRight);
        }
        
        // Functions used by XR_SkeletonPoserEditor.

        public GameObject ShowLeftPreview()
        {
            return Instantiate(leftHand);
        }

        public void DestroyLeftPreview(GameObject obj)
        {
            DestroyImmediate(obj);
        }

        public GameObject ShowRightPreview()
        {
            return Instantiate(rightHand);
        }

        public void DestroyRightPreview(GameObject obj)
        {
            DestroyImmediate(obj);
        }

        public XR_SkeletonPose GetLoadedPose()
        {
            return activePose;
        }
        
        public Vector3[] GetBonePositions(GameObject target)
        {
            if (target != null)
            {
                return target.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray();
            }

            return null;
        }
        
        public Quaternion[] GetBoneRotations(GameObject target)
        {
            if (target != null)
            {
                return target.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
            }

            return null;
        }

        // public Quaternion InverseBoneRotations(Quaternion boneRot)
        // {
        //     Quaternion normalQuat = boneRot;
        //     float mirrorY = -normalQuat.y;
        //     float mirrorZ = -normalQuat.z;
        //     
        //     Quaternion newQuat = new Quaternion(boneRot.x, mirrorY, mirrorZ, boneRot.w);
        //     return newQuat;
        // }
        //
        // public Vector3 InverseBonePositions(Vector3 bonePos)
        // {
        //     Vector3 normalVector3 = bonePos;
        //     float mirrorY = -normalVector3.y;
        //     float mirrorZ = normalVector3.z;
        //     
        //     Vector3 newVector3 = new Vector3(bonePos.x, mirrorY, mirrorZ);
        //     return newVector3;
        // }

    }
}