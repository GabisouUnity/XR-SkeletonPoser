using System;
using System.Linq;

using UnityEditor;
using UnityEngine;
using yellowyears.SkeletonPoser;

namespace yellowyears.SkeletonPoser
{
    [CustomEditor(typeof(XRSkeletonPoser))]
    public class XRSkeletonPoserEditor : Editor
    {
        private XRSkeletonPoser _poser = null;
        
        private XRSkeletonPose _defaultPose = null;

        private SerializedProperty _propertyPose = null;
        // private SerializedProperty _propertySecondaryPose = null;
        private SerializedProperty _propertySelectedPose = null;
        // private SerializedProperty _propertyActivePoseEnum = null;
        // private SerializedProperty _propertyBlendBehaviour = null;
        // private SerializedProperty _propertyBlendWasCreated = null;
        private SerializedProperty _propertyBlendInput = null;

        private SerializedProperty _propertyShowPoses = null;
        private SerializedProperty _propertyShowPoseEditor = null;
        private SerializedProperty _propertyShowBlendEditor = null;
        private SerializedProperty _propertyUseBlend = null;
        // private SerializedProperty _propertyScale = null;

        // private bool _updateHands = false;
        
        private SerializedProperty _propertyShowLeft = null;
        private SerializedProperty _propertyTempLeft = null;

        private SerializedProperty _propertyShowRight = null;
        private SerializedProperty _propertyTempRight = null;

        private XRSkeletonPoserSettings _poserSettings = null;
        
        private void OnEnable()
        {
            _poser = (XRSkeletonPoser) target;
            _poserSettings = XRSkeletonPoserSettings.Instance;

            _defaultPose = CreateInstance<XRSkeletonPose>();
            GetDefaultPose();

            _propertyPose = serializedObject.FindProperty("pose");
            // _propertySecondaryPose = serializedObject.FindProperty("secondaryPose");
            _propertySelectedPose = serializedObject.FindProperty("selectedPose");
            // _propertyActivePoseEnum = serializedObject.FindProperty("activePoseEnum");
            // _propertyBlendBehaviour = serializedObject.FindProperty("blendBehaviour");
            // _propertyBlendWasCreated = serializedObject.FindProperty("blendWasCreated");
            _propertyBlendInput = serializedObject.FindProperty("blendInput");
            
            _propertyShowPoses = serializedObject.FindProperty("showPoses");
            _propertyShowPoseEditor = serializedObject.FindProperty("showPoseEditor");
            _propertyShowBlendEditor = serializedObject.FindProperty("showBlendEditor");
            _propertyUseBlend = serializedObject.FindProperty("useBlend");
            // _propertyScale = serializedObject.FindProperty("scale");
            
            _propertyShowLeft = serializedObject.FindProperty("showLeft");
            _propertyTempLeft = serializedObject.FindProperty("tempLeft");

            _propertyShowRight = serializedObject.FindProperty("showRight");
            _propertyTempRight = serializedObject.FindProperty("tempRight");
        }
        
        private void GetDefaultPose()
        {
            // Get default values from the hand prefab.
            
            _defaultPose.leftHandPositions = _poser.GetBonePositions(XRSkeletonPoserSettings.Instance.leftHand);
            _defaultPose.leftHandRotations = _poser.GetBoneRotations(XRSkeletonPoserSettings.Instance.leftHand);

            _defaultPose.rightHandPositions = _poser.GetBonePositions(XRSkeletonPoserSettings.Instance.rightHand);
            _defaultPose.rightHandRotations = _poser.GetBoneRotations(XRSkeletonPoserSettings.Instance.rightHand);
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            DrawAdditionalPoses();
            
            DrawPoseEditor();
            
            DrawBlendEditor();
            
            serializedObject.ApplyModifiedProperties();
        }
        
        public void OnSceneGUI()
        {
            if (Application.isPlaying || !XRSkeletonPoserSettings.Instance.useBoneGizmos) return;

            if (_propertyShowLeft.boolValue)
            {
                // CustomGizmos.DrawLeftBones(this, GizmoType.Pickable);
                DrawBoneHandles(_propertyTempLeft.objectReferenceValue as GameObject);
            }

            if (_propertyShowRight.boolValue)
            {
                DrawBoneHandles(_propertyTempRight.objectReferenceValue as GameObject);
            } 
        }

        private void DrawBoneHandles(GameObject targetHand)
        {
            if (!targetHand) return;
            
            var bones = targetHand.GetComponentsInChildren<Transform>();

            Handles.color = XRSkeletonPoserSettings.Instance.boneGizmoColour;
            
            foreach (var bone in bones)
            {
                if (!IsValidBone(bone)) continue;
                
                if (Handles.Button(bone.position, bone.rotation, 0.01f, 0.01f, Handles.SphereHandleCap))
                {
                    Selection.activeGameObject = bone.gameObject;
                }
            }
        }

        private bool IsValidBone(Transform bone)
        {
            if (bone == null) return false;
            
            var ignoredBones = _poserSettings.ignoredBoneKeywords;

            foreach (var ignoredBone in ignoredBones)
            {
                if (bone.name.Contains(ignoredBone)) return false;
            }

            return true;
        }

        private bool IsValidMainPose(XRSkeletonPose pose)
        {
            return pose.leftHandPositions != null && pose.leftHandRotations != null && pose.rightHandPositions != null && pose.rightHandRotations != null;
        }
        
        private bool IsValidSecondaryPose(XRSkeletonPose pose)
        {
            return pose.leftSecondaryPositions != null && pose.leftSecondaryRotations != null && pose.rightSecondaryPositions != null && pose.rightSecondaryRotations != null;
        }
        
        private void DrawAdditionalPoses()
        {
            if (Application.isPlaying) return;
            
            EditorGUILayout.BeginVertical("box");
            
            _propertyShowPoses.boolValue =
                IndentedFoldoutHeader(_propertyShowPoses.boolValue, "Pose Manager");

            if (_propertyShowPoses.boolValue)
            {

                EditorGUILayout.PropertyField(_propertyPose);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(!_propertyShowLeft.boolValue || !_propertyShowRight.boolValue || _poser.pose == null || !IsValidMainPose(_poser.pose) || !IsValidSecondaryPose(_poser.pose));

                // TODO: disable buttons if there is no saved data

                GUI.backgroundColor = _poserSettings.loadPoseColour;
                
                if (GUILayout.Button(_propertyPose.name + " (MAIN)"))
                {
                    // _propertySelectedPose = _propertyPose;
                    _propertySelectedPose.enumValueIndex = 0; // Main
                    LoadPose(_propertyPose.objectReferenceValue as XRSkeletonPose);
                }

                if (GUILayout.Button(_propertyPose.name + " (SECONDARY)"))
                {
                    // _propertySelectedPose = _propertySecondaryPose;
                    _propertySelectedPose.enumValueIndex = 1; // Secondary
                    LoadSecondaryPose(_propertyPose.objectReferenceValue as XRSkeletonPose);
                }
                
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
                
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawPoseEditor()
        {
            if (Application.isPlaying) // Check if in playmode and skip if not.
            {
                EditorGUILayout.LabelField("Cannot modify pose while in play mode.");
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                
                if (_poserSettings.guiFont != null)
                {
                    GUI.skin.font = _poserSettings.guiFont;
                }
                
                _propertyShowPoseEditor.boolValue =
                    IndentedFoldoutHeader(_propertyShowPoseEditor.boolValue, "Pose Editor");
        
                if (_propertyShowPoseEditor.boolValue)
                {
                   EditorGUILayout.BeginHorizontal(); EditorGUI.BeginDisabledGroup(XRSkeletonPoserSettings.Instance.leftHand == null);
                   
                   if (!_propertyShowLeft.boolValue)
                   {
                       GUI.backgroundColor = _poserSettings.showLeftHandColour;
                       
                       if (GUILayout.Button("Show Left Hand"))
                       {
                           var leftGameObject = _poser.ShowLeftPreview();
        
                           leftGameObject.transform.parent = null;
                           leftGameObject.transform.parent = _poser.transform;
                           leftGameObject.transform.localPosition = Vector3.zero;
                           leftGameObject.transform.localRotation = Quaternion.identity;
                           
                           _propertyShowLeft.boolValue = true;
        
                           _propertyTempLeft.objectReferenceValue = leftGameObject;
                       }
        
                   }
                   else
                   {
                       GUI.backgroundColor = _poserSettings.hideLeftHandColour;
                       
                       if (GUILayout.Button("Hide Left Hand"))
                       {
                           _poser.DestroyLeftPreview(_propertyTempLeft.objectReferenceValue as GameObject);
                           _propertyShowLeft.boolValue = false;
                       }
                   }
                   
                   EditorGUI.EndDisabledGroup();
                   
                   // Preview Right Button
                   
                   EditorGUI.BeginDisabledGroup(XRSkeletonPoserSettings.Instance.rightHand == null);
                   
                   if (!_propertyShowRight.boolValue)
                   {
                       GUI.backgroundColor = _poserSettings.showRightHandColour;
                       
                       if (GUILayout.Button("Show Right Hand"))
                       {
                           var rightGameObject = _poser.ShowRightPreview();
        
                           rightGameObject.transform.parent = null;
                           rightGameObject.transform.parent = _poser.transform;
                           rightGameObject.transform.localPosition = Vector3.zero;
                           rightGameObject.transform.localRotation = Quaternion.identity;
                           
                           _propertyShowRight.boolValue = true;
        
                           _propertyTempRight.objectReferenceValue = rightGameObject;
                       }
                   }
                   else
                   {
                       GUI.backgroundColor = _poserSettings.hideRightHandColour;
                       
                       if (GUILayout.Button("Hide Right Hand"))
                       {
                           _poser.DestroyRightPreview(_propertyTempRight.objectReferenceValue as GameObject);
                           _propertyShowRight.boolValue = false;
                       }
                   }
        
                   EditorGUI.EndDisabledGroup(); EditorGUILayout.EndHorizontal();
                   
                   EditorGUILayout.BeginHorizontal();
                   
                   // Grey it out if hands aren't active
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false || _propertyShowRight.boolValue == false);
        
                   GUI.backgroundColor = _poserSettings.savePoseColour;
                   
                   if (GUILayout.Button("Save Main Pose", "button"))
                   {
                       SaveMainPose();
                   }

                   if (GUILayout.Button("Save Secondary Pose", "button"))
                   {
                       SaveSecondaryPose();
                   }
                   
                   EditorGUILayout.EndHorizontal();
                   
                   EditorGUI.EndDisabledGroup();
        
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false);
        
                   EditorGUILayout.BeginHorizontal();
                   
                   GUI.backgroundColor = _poserSettings.resetPoseColour;
                   
                   if (GUILayout.Button("Reset Pose", "button"))
                   {
                       // Make sure we warn the user before they reset their pose
                       if (EditorUtility.DisplayDialog("Reset Pose?",
                           "Are you sure you want to do this? You will lose your pose on this object!", "Yes", "No"))
                       {
                           // They are sure, reset pose
                           ResetPose(); // Reset new pose instance to default pose
                       }
                   }
                   
                   GUI.backgroundColor = Color.white;
                   
                   EditorGUI.EndDisabledGroup();
                   
                   // EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false || XRSkeletonPoserSettings.Instance.referencePose == null);
                   //
                   // GUI.backgroundColor = _poserSettings.resetToReferencePoseColour;
                   //
                   // if (GUILayout.Button("Reset To Reference Pose", "button"))
                   // {
                   //     // Make sure we warn the user before they reset their pose
                   //     if (EditorUtility.DisplayDialog("Reset Pose?",
                   //         "Are you sure you want to do this? You will lose your pose on this object!", "Yes", "No"))
                   //     {
                   //         // They are sure, reset pose
                   //         ResetToReferencePose(); // Reset new pose instance to reference pose
                   //     }
                   // }
                   //
                   // GUI.backgroundColor = Color.white;
                   //
                   // EditorGUI.EndDisabledGroup();
                   
                   EditorGUILayout.EndHorizontal();
                }
        
                EditorGUILayout.EndFoldoutHeaderGroup();
        
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawBlendEditor()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Cannot modify blends in playmode.");
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                
                _propertyShowBlendEditor.boolValue =
                    IndentedFoldoutHeader(_propertyShowBlendEditor.boolValue, "Blend Editor");
                
                if (_propertyShowBlendEditor.boolValue)
                {
                    EditorGUILayout.PropertyField(_propertyUseBlend);

                    if (_propertyUseBlend.boolValue)
                    {
                        EditorGUILayout.PropertyField(_propertyBlendInput);
                    }
                }
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private bool IndentedFoldoutHeader(bool fold, string text, int indent = 1)
        {
            // Taken from the steamvr unity plugin code, just looks too good :p
            
            GUILayout.BeginHorizontal();
            var boldFoldoutStyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
            GUILayout.Space(14f * indent);
            fold = EditorGUILayout.Foldout(fold, text, boldFoldoutStyle);
            GUILayout.EndHorizontal();
            return fold;
        }
        
        private void SaveMainPose()
        {
            var pose = _propertyPose.objectReferenceValue as XRSkeletonPose;

            if (pose == null) pose = ScriptableObject.CreateInstance<XRSkeletonPose>();
            
            var copy = Instantiate(pose);
            
            copy.leftHandPositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
            copy.leftHandRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
        
            copy.rightHandPositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
            copy.rightHandRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);
        
            // Don't overwrite secondary stuff
        
            if (copy.leftSecondaryPositions != null)
            {
                copy.leftSecondaryPositions = pose.leftSecondaryPositions;
            }
        
            if (copy.leftSecondaryRotations != null)
            {
                copy.leftSecondaryRotations = pose.leftSecondaryRotations;
            }
        
            if (copy.rightSecondaryPositions != null)
            {
                copy.rightSecondaryPositions = pose.rightSecondaryPositions;
            }
        
            if (copy.rightSecondaryRotations != null)
            {
                copy.rightSecondaryRotations = pose.rightSecondaryRotations;
            }

            _poser.pose = copy;
            
            // LoadPose(copy);
            
            if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
            {
                // Folder doesn't exist, create new
                AssetDatabase.CreateFolder("Assets", "XRPoses");
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poserSettings.poseNamePrefix}{_poser.gameObject.name}.asset");
            }
            else
            {
                // Folder exists
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poserSettings.poseNamePrefix}{_poser.gameObject.name}.asset");
            }
        }
        
        private void SaveSecondaryPose()
        {
            var pose = _propertyPose.objectReferenceValue as XRSkeletonPose;
            
            if (pose == null) pose = ScriptableObject.CreateInstance<XRSkeletonPose>();

            var copy = Instantiate(pose);
            
            copy.leftSecondaryPositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
            copy.leftSecondaryRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
        
            copy.rightSecondaryPositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
            copy.rightSecondaryRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);
        
            // Don't overwrite main stuff
        
            if (copy.leftHandPositions != null)
            {
                copy.leftHandPositions = pose.leftHandPositions;
            }
        
            if (copy.leftHandRotations != null)
            {
                copy.leftHandRotations = pose.leftHandRotations;
            }
        
            if (copy.rightHandPositions != null)
            {
                copy.rightHandPositions = pose.rightHandPositions;
            }
        
            if (copy.rightHandRotations != null)
            {
                copy.rightHandRotations = pose.rightHandRotations;
            }

            _poser.pose = copy;
            
            if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
            {
                // Folder doesn't exist, create new
                AssetDatabase.CreateFolder("Assets", "XRPoses");
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poserSettings.poseNamePrefix}{_poser.gameObject.name}.asset");
            }
            else
            {
                // Folder exists
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poserSettings.poseNamePrefix}{_poser.gameObject.name}.asset");
            }
        }

        private void ResetPose()
        {
            // Set pose to new pose data to avoid the need for reassignment after saving the file
            
            // Create copy of pose to stop error whilst saving ("Object already exists")
            var copy = CreateInstance<XRSkeletonPose>();

            copy.leftHandPositions = _defaultPose.leftHandPositions;
            copy.leftHandRotations = _defaultPose.leftHandRotations;

            copy.rightHandPositions = _defaultPose.rightHandPositions;
            copy.rightHandRotations = _defaultPose.rightHandRotations;
            
            copy.leftSecondaryPositions = _defaultPose.leftSecondaryPositions;
            copy.leftSecondaryRotations = _defaultPose.leftSecondaryRotations;

            copy.rightSecondaryPositions = _defaultPose.rightSecondaryPositions;
            copy.rightSecondaryRotations = _defaultPose.rightSecondaryRotations;
            
            // _poser.selectedPose = copy;
            _poser.pose = copy;

            LoadPose(copy); // Load pose automatically for convenience
            
            // SaveMainPose();
            // SaveSecondaryPose();
            
            // Save and overwrite
            if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
            {
                // Folder doesn't exist, create new
                AssetDatabase.CreateFolder("Assets", "XRPoses");

                // Overwrite the pose with a default pose
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
            else
            {
                // Folder exists

                // Overwrite the pose with a default pose

                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
                // AssetDatabase.SaveAssets();
            }
        }
        
        // private void ResetToReferencePose()
        // {
        //     // Set pose to new pose data to avoid the need for reassignment after saving the file
        //     
        //     // Create copy of pose to stop error whilst saving ("Object already exists")
        //     var copy = CreateInstance<XRSkeletonPose>();
        //
        //     var referencePose = XRSkeletonPoserSettings.Instance.referencePose;
        //     
        //     copy.leftHandPositions = referencePose.leftHandPositions;
        //     copy.leftHandRotations = referencePose.leftHandRotations;
        //
        //     copy.rightHandPositions = referencePose.rightHandPositions;
        //     copy.rightHandRotations = referencePose.rightHandRotations;
        //     
        //     // _poser.selectedPose = copy;
        //
        //     LoadPose(copy); // Load pose automatically for convenience
        //     
        //     // Save and overwrite
        //     if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
        //     {
        //         // Folder doesn't exist, create new
        //         AssetDatabase.CreateFolder("Assets", "XRPoses");
        //
        //         // Overwrite the pose with a default pose
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //     }
        //     else
        //     {
        //         // Folder exists
        //
        //         // Overwrite the pose with a default pose
        //
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //         // AssetDatabase.SaveAssets();
        //     }
        // }

        // private XRSkeletonPose GetMainPose(XRSkeletonPose inputPose)
        // {
        //     // Get pose without saving
        //     
        //     inputPose.leftHandPositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
        //     inputPose.leftHandRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
        //
        //     inputPose.rightHandPositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
        //     inputPose.rightHandRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);
        //
        //     // Get input XRPose instance and return it but full from scene
        //     return inputPose;
        // }
        //
        // private XRSkeletonPose GetSecondaryPose(XRSkeletonPose inputPose)
        // {
        //     inputPose.leftSecondaryPositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
        //     inputPose.leftSecondaryRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
        //
        //     inputPose.rightSecondaryPositions =
        //         _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
        //     inputPose.rightSecondaryRotations =
        //         _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);
        //
        //     return inputPose;
        // }
        //
        // private XRSkeletonPose RemoveSecondaryPose(XRSkeletonPose inputPose)
        // {
        //     inputPose.leftSecondaryPositions = null;
        //     inputPose.leftSecondaryRotations = null;
        //
        //     inputPose.rightSecondaryPositions = null;
        //     inputPose.rightSecondaryRotations = null;
        //
        //     return inputPose;
        // }

        private void LoadPose(XRSkeletonPose loadedPose)
        {
            // var loadedPose = _poser.FetchPose();

            var leftHandObject = _propertyTempLeft.objectReferenceValue as GameObject;
            var rightHandObject = _propertyTempRight.objectReferenceValue as GameObject;
            
            var leftBonePositions = loadedPose.leftHandPositions;
            var leftBoneRotations = loadedPose.leftHandRotations;

            var rightBonePositions = loadedPose.rightHandPositions;
            var rightBoneRotations = loadedPose.rightHandRotations;

            if (leftHandObject == null) return;

            var leftTransforms = leftHandObject.GetComponentsInChildren<Transform>().ToArray();

            // Set left values to loaded pose
            for (int i = 0; i < leftBonePositions.Length; i++)
            {
                leftTransforms[i].localPosition = leftBonePositions[i];
            }

            for (int i = 0; i < leftBoneRotations.Length; i++)
            {
                leftTransforms[i].localRotation = leftBoneRotations[i];
            }

            if (rightHandObject == null) return;
            
            var rightTransforms = rightHandObject.GetComponentsInChildren<Transform>().ToArray();

            for (int i = 0; i < rightBonePositions.Length; i++)
            {
                rightTransforms[i].localPosition = rightBonePositions[i];
            }

            for (int i = 0; i < rightBoneRotations.Length; i++)
            {
                rightTransforms[i].localRotation = rightBoneRotations[i];
            }
            
        }

        private void LoadSecondaryPose(XRSkeletonPose loadedPose)
        {
            var leftHandObject = _propertyTempLeft.objectReferenceValue as GameObject;
            var rightHandObject = _propertyTempRight.objectReferenceValue as GameObject;
            
            var leftSecondaryPositions = loadedPose.leftSecondaryPositions;
            var leftSecondaryRotations = loadedPose.leftSecondaryRotations;

            var rightSecondaryPositions = loadedPose.rightSecondaryPositions;
            var rightSecondaryRotations = loadedPose.rightSecondaryRotations;

            if (leftHandObject == null) return;

            var leftTransforms = leftHandObject.GetComponentsInChildren<Transform>().ToArray();

            // Set left values to loaded pose
            for (int i = 0; i < leftSecondaryPositions.Length; i++)
            {
                leftTransforms[i].localPosition = leftSecondaryPositions[i];
            }

            for (int i = 0; i < leftSecondaryRotations.Length; i++)
            {
                leftTransforms[i].localRotation = leftSecondaryRotations[i];
            }

            if (rightHandObject == null) return;
            
            var rightTransforms = rightHandObject.GetComponentsInChildren<Transform>().ToArray();

            for (int i = 0; i < rightSecondaryPositions.Length; i++)
            {
                rightTransforms[i].localPosition = rightSecondaryPositions[i];
            }

            for (int i = 0; i < rightSecondaryRotations.Length; i++)
            {
                rightTransforms[i].localRotation = rightSecondaryRotations[i];
            }
        }
    }    
}