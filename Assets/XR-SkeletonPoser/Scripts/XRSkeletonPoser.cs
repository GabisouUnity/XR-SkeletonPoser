using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

namespace yellowyears.SkeletonPoser
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class XRSkeletonPoser : MonoBehaviour
    {
        
        // Serialized Properties
        
        public enum SelectedPose { Main, Secondary }
        
        // Only trigger right now, will be adding more soon along with boolean / analogue input options.
        public enum BlendInput { Trigger }
        
        [HideInInspector] public XRSkeletonPose pose;
        [HideInInspector] public SelectedPose selectedPose;
        
        [HideInInspector] public BlendInput blendInput;
        
        [HideInInspector] public bool showPoses = false;
        [HideInInspector] public bool showPoseEditor = true; // Used in editor foldout
        [HideInInspector] public bool showBlendEditor = false;
        [HideInInspector] public bool useBlend = false;
        
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
        
        public XRSkeletonPose FetchPose()
        {
            return pose;
        }
        
        public Vector3[] GetBonePositions(GameObject target)
        {
            return target != null ? target.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray() : null;
        }
        
        public Quaternion[] GetBoneRotations(GameObject target)
        {
            return target != null ? target.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray() : null;
        }

        public void BlendLeftPose(Transform[] fingers, float blendValue)
        {
            var mainPosePos = pose.leftBonePositions;
            var mainPoseRot = pose.leftSecondaryRotations;

            var secondaryPosePos = pose.leftSecondaryPositions;
            var secondaryPoseRot = pose.leftSecondaryRotations;

            for (int i = 0; i < fingers.Length; i++)
            {
                fingers[i].localPosition = Vector3.Slerp(mainPosePos[i], secondaryPosePos[i], blendValue);
                fingers[i].localRotation = Quaternion.Slerp(mainPoseRot[i], secondaryPoseRot[i], blendValue);
            }
        }
        
        public void BlendRightPose(Transform[] fingers, float blendValue)
        {
            var mainPosePos = pose.rightBonePositions;
            var mainPoseRot = pose.rightSecondaryRotations;

            var secondaryPosePos = pose.rightSecondaryPositions;
            var secondaryPoseRot = pose.rightSecondaryRotations;

            for (int i = 0; i < fingers.Length; i++)
            {
                fingers[i].localPosition = Vector3.Slerp(mainPosePos[i], secondaryPosePos[i], blendValue);
                fingers[i].localRotation = Quaternion.Slerp(mainPoseRot[i], secondaryPoseRot[i], blendValue);
            }
        }
        
    }
}