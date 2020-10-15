using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

namespace yellowyears.SkeletonPoser
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class XRSkeletonPoser : MonoBehaviour
    {

        // [Tooltip("Left hand model to be spawned in as a left preview. Should be the same as your in game left hand")]
        // public GameObject leftHand = null;
        //
        // [Tooltip("Left hand model to be spawned in as a left preview. Should be the same as your in game left hand")] 
        // public GameObject rightHand = null;

        // #region Editor Stuff
        //
        // public string[] poseNames;
        //
        // #endregion
        
        // Serialized Properties
        
        public enum ActivePose { Main, Secondary }

        public enum BlendInput { Trigger, Grip }
        
        [SerializeField] private XRSkeletonPose mainPose;
        [SerializeField] private XRSkeletonPose secondaryPose;
        public XRSkeletonPose selectedPose;
        [SerializeField] private BlendBehaviour blendBehaviour = new BlendBehaviour();
        public bool blendWasCreated = false;
        [SerializeField] private BlendInput blendInput;
        // [SerializeField] private InputHelpers.Button blendButton;
            
        [SerializeField] private ActivePose activePoseEnum;

        // [SerializeField] private BlendBehaviour blend = null;

        [SerializeField] private bool showPoses = false;
        [SerializeField] private bool showPoseEditor = true; // Used in editor foldout
        [SerializeField] private bool showBlendEditor = false;

        [SerializeField] private float scale;
        
        [SerializeField] private bool showLeft;
        [SerializeField] private GameObject tempLeft;

        [SerializeField] private bool showRight;
        [SerializeField] private GameObject tempRight;

        private void Awake()
        {
            // Destroy preview hands on awake so they are not visible as we play the game.
            DestroyLeftPreview(tempLeft);
            DestroyRightPreview(tempRight);
        }
        
        // Functions used by XR_SkeletonPoserEditor.

        public GameObject ShowLeftPreview()
        {
            var poserSettings = XRSkeletonPoserSettings.Instance;
            
            var preview = Instantiate(poserSettings.leftHand);

            if (poserSettings.defaultExpandPreview)
            {
                SetExpandedRecursive(preview, true);
            }

            return preview;
        }

        public void DestroyLeftPreview(GameObject obj)
        {
            DestroyImmediate(obj);
        }

        public GameObject ShowRightPreview()
        {
            var poserSettings = XRSkeletonPoserSettings.Instance;
            
            var preview = Instantiate(poserSettings.rightHand);
            
            if (poserSettings.defaultExpandPreview)
            {
                SetExpandedRecursive(preview, true);
            }
            
            return preview;
        }

        public void DestroyRightPreview(GameObject obj)
        {
            DestroyImmediate(obj);
        }

        private static void SetExpandedRecursive(GameObject gameObject, bool expand)
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var methodInfo = type.GetMethod("SetExpandedRecursive");
 
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");            
            var window = EditorWindow.focusedWindow;

            methodInfo?.Invoke(window, new object[] {gameObject.GetInstanceID(), expand});
        }
        
        public XRSkeletonPose FetchSelectedPose()
        {
            return selectedPose;
        }

        public XRSkeletonPose FetchMainPose()
        {
            return mainPose;
        }
        
        public XRSkeletonPose FetchSecondaryPose()
        {
            return secondaryPose;
        }
        
        public Vector3[] GetBonePositions(GameObject target)
        {
            return target != null ? target.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray() : null;
        }
        
        public Quaternion[] GetBoneRotations(GameObject target)
        {
            return target != null ? target.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray() : null;
        }

        public void BlendPose(float blendValue)
        {
            
        }
        
        // public XRSkeletonPose GetBlendToPose(XRSkeletonPose inputPose)
        // {
        //     return inputPose.blendTo;
        // }
        //
        // public string GetBlendName(XRSkeletonPose inputPose)
        // {
        //     return inputPose.blendName;
        // }
        
        // public Quaternion InverseBoneRotations(Quaternion boneRot)
        // {
        //     Quaternion normalQuat = boneRot;
        //     float mirrorY = -normalQuat.y;
        //     float mirrorZ = -normalQuat.z;
        //     
        //     Quaternion newQuat = new Quaternion(boneRot.x, mirrorY, mirrorZ, boneRot.w);
        //     return newQuat;
        // }
        
        // public Vector3 InverseBonePositions(Vector3 bonePos)
        // {
        //     Vector3 normalVector3 = bonePos;
        //     float mirrorY = -normalVector3.y;
        //     float mirrorZ = normalVector3.z;
        //     
        //     Vector3 newVector3 = new Vector3(bonePos.x, mirrorY, mirrorZ);
        //     return newVector3;
        // }

        // public Quaternion MirrorBoneRotation(Quaternion boneRot)
        // {
        //     boneRot.y = boneRot.y * -1;
        //     boneRot.z = boneRot.z * -1;
        //
        //     return boneRot;
        // }
        //
        // public Vector3 MirrorBonePosition(Vector3 bonePos)
        // {
        //     bonePos = bonePos * -1;
        //
        //     return bonePos;
        // }

        [Serializable]
        public class BlendBehaviour
        {
            public string blendName;
            public bool enabled = false;
            public XRSkeletonPose from;
            public XRSkeletonPose to;
        }
        
    }
}