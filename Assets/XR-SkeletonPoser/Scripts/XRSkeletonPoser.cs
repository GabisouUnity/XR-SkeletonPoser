using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace yellowyears.SkeletonPoser
{
    public enum HandType { Left, Right } // TODO: Could possibly be accessed and set from the XRController?

    [RequireComponent(typeof(XRGrabInteractable))]
    public class XRSkeletonPoser : MonoBehaviour
    {
        
        // Serialized Properties
        
        public enum SelectedPose { Main, Secondary }

        public enum BlendInput { Trigger } // Only trigger right now, will be adding more soon along with boolean / analogue input options.
        
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
            
            tempLeft = Instantiate(poserSettings.leftHand);

            if (poserSettings.defaultExpandPreview)
            {
                SetExpandedRecursive(tempLeft, true);
            }

            showLeft = true;
            
            return tempLeft;
        }

        public void DestroyLeftPreview(GameObject obj)
        {
            DestroyImmediate(obj);
            showLeft = false;
        }

        public GameObject ShowRightPreview()
        {
            var poserSettings = XRSkeletonPoserSettings.Instance;
            
            tempLeft = Instantiate(poserSettings.rightHand);
            
            if (poserSettings.defaultExpandPreview)
            {
                SetExpandedRecursive(tempLeft, true);
            }

            showRight = true;
            
            return tempLeft;
        }

        public void DestroyRightPreview(GameObject obj)
        {
            DestroyImmediate(obj);
            showRight = false;
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

        private void BlendLeftPose(Transform[] fingers, float blendValue)
        {
            var mainPosePos = pose.leftHandPositions;
            var mainPoseRot = pose.leftSecondaryRotations;

            var secondaryPosePos = pose.leftSecondaryPositions;
            var secondaryPoseRot = pose.leftSecondaryRotations;

            for (int i = 0; i < fingers.Length; i++)
            {
                fingers[i].localPosition = Vector3.Slerp(mainPosePos[i], secondaryPosePos[i], blendValue);
                fingers[i].localRotation = Quaternion.Slerp(mainPoseRot[i], secondaryPoseRot[i], blendValue);
            }
            
            Debug.Log(blendValue);
        }
        
        private void BlendRightPose(Transform[] fingers, float blendValue)
        {
            var mainPosePos = pose.rightHandPositions;
            var mainPoseRot = pose.rightSecondaryRotations;

            var secondaryPosePos = pose.rightSecondaryPositions;
            var secondaryPoseRot = pose.rightSecondaryRotations;

            for (int i = 0; i < fingers.Length; i++)
            {
                fingers[i].localPosition = Vector3.Slerp(mainPosePos[i], secondaryPosePos[i], blendValue);
                fingers[i].localRotation = Quaternion.Slerp(mainPoseRot[i], secondaryPoseRot[i], blendValue);
            }
        }
     
        public void CheckForBlendInput(bool shouldCheckForBlendInput, XRController inputController, GameObject handObject, HandType handType, XRBaseInteractable selectTarget)
        {
            if (!shouldCheckForBlendInput) return;

            var device = inputController.inputDevice;
            
            // Get input and convert to common usages
            var triggerUsage = CommonUsages.trigger;

            var handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            if (handType == HandType.Left)
            {
                // Check for input
                switch (blendInput)
                {
                    case BlendInput.Trigger:
                        // Get value
                        device.TryGetFeatureValue(triggerUsage, out var triggerValue);
            
                        // Blend Pose
                        BlendLeftPose(handBones, triggerValue);
                        SetOffset(selectTarget, handObject);
                        break;
                }
            }
            else if (handType == HandType.Right)
            {
                switch (blendInput)
                {
                    case BlendInput.Trigger:
                        // Get value
                        device.TryGetFeatureValue(triggerUsage, out var triggerValue);
            
                        // Blend Pose
                        BlendRightPose(handBones, triggerValue);
                        SetOffset(selectTarget, handObject);
                        break;
                }
            }
            
        }

        public void SetDefaultPose(HandType handType, GameObject handObject, XRSkeletonPose defaultPose)
        {
            var handBones = handObject.GetComponentsInChildren<Transform>().ToArray();
            
            switch (handType)
            {
                case HandType.Left:
                {
                    for (int i = 0; i < handBones.Length; i++)
                    {
                        handBones[i].localPosition = defaultPose.leftHandPositions[i];
                        handBones[i].localRotation = defaultPose.leftHandRotations[i];
                    }
                
                    // Reset main hand object to local 0,0,0

                    handBones[0].localPosition = Vector3.zero;
                    handBones[0].localRotation = Quaternion.identity;
                    break;
                }
                case HandType.Right:
                {
                    for (int i = 0; i < handBones.Length; i++)
                    {
                        handBones[i].localPosition = defaultPose.rightHandPositions[i];
                        handBones[i].localRotation = defaultPose.rightHandRotations[i];
                    }
                
                    // Reset main hand object to local 0,0,0

                    handBones[0].localPosition = Vector3.zero;
                    handBones[0].localRotation = Quaternion.identity;
                    break;
                }
            }
        }
        
        public static XRSkeletonPose GetDefaultPose(HandType handType, GameObject handObject)
        {
            var defaultPose = ScriptableObject.CreateInstance<XRSkeletonPose>();

            switch (handType)
            {
                case HandType.Left:
                    defaultPose.leftHandPositions = handObject.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray();
                    defaultPose.leftHandRotations = handObject.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
                    break;
                case HandType.Right:
                    defaultPose.rightHandPositions = handObject.GetComponentsInChildren<Transform>().Select(x => x.localPosition).ToArray();
                    defaultPose.rightHandRotations = handObject.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
                    break;
            }

            return defaultPose;
        }

        public void SetOffset(XRBaseInteractable selectTarget, GameObject handObject)
        {
            // Get grabbable's attach point
            var selectTargetAttach = ((XRGrabInteractable) selectTarget).attachTransform;
            
            var handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            // Move first index (hand model parent) to the grabbable's attach transform
            handBones[0].localPosition = selectTargetAttach.localPosition;
            handBones[0].localRotation = selectTargetAttach.localRotation;
        }

        public void SetPose(XRSkeletonPose inputPose, GameObject handObject, HandType handType)
        {
            // Get hand bones
            
            var handBones = handObject.GetComponentsInChildren<Transform>().ToArray();

            var leftPosePos = inputPose.leftHandPositions;
            var leftPoseRot = inputPose.leftHandRotations;

            var rightPosePos = inputPose.rightHandPositions;
            var rightPoseRot = inputPose.rightHandRotations;
            
            // Set values to loaded pose

            switch (handType)
            {
                case HandType.Left:
                {
                    for (int i = 0; i < handBones.Length; i++)
                    {
                        handBones[i].localPosition = leftPosePos[i];
                        handBones[i].localRotation = leftPoseRot[i];
                    }

                    break;
                }
                case HandType.Right:
                {
                    for (int i = 0; i < handBones.Length; i++)
                    {
                        handBones[i].localPosition = rightPosePos[i];
                        handBones[i].localRotation = rightPoseRot[i];
                    }

                    break;
                }
            }
            
            // Reset main hand object to local 0,0,0

            handBones[0].localPosition = Vector3.zero;
            handBones[0].localRotation = Quaternion.identity;
        }
        
    }
}