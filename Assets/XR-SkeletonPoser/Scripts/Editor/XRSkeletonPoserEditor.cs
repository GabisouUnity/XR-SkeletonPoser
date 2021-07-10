using System.Linq;
using UnityEditor;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    [CustomEditor(typeof(XRSkeletonPoser))]
    public class XRSkeletonPoserEditor : Editor
    {
        private XRSkeletonPoser _poser = null;
        
        private XRSkeletonPose _defaultPose = null;

        private SerializedProperty _propertyPose = null;

        private SerializedProperty _propertyShowPoses = null;
        private SerializedProperty _propertyShowPoseEditor = null;
        
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
            
            _propertyShowPoses = serializedObject.FindProperty("showPoses");
            _propertyShowPoseEditor = serializedObject.FindProperty("showPoseEditor");
            
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
            
            DrawPoseManager();
            
            DrawPoseEditor();
            
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

            Handles.color = _poserSettings.boneGizmoColour;
            
            foreach (var bone in bones)
            {
                if (!IsValidBone(bone)) continue;
                
                if (Handles.Button(bone.position, bone.rotation, _poserSettings.boneGizmoSize, _poserSettings.boneGizmoPickSize, Handles.SphereHandleCap))
                {
                    Selection.activeGameObject = bone.gameObject;
                }
            }
        }

        private bool IsValidBone(Transform bone)
        {
            if (bone == null) return false;

            if (SceneVisibilityManager.instance.IsPickingDisabled(bone.gameObject)) return false;
            if (SceneVisibilityManager.instance.IsHidden(bone.gameObject)) return false;
            if (bone.gameObject.activeInHierarchy == false) return false;
            
            var ignoredBones = _poserSettings.ignoredBoneKeywords;
            
            var lowerIgnoredBones = ignoredBones.Select(s => s.ToLowerInvariant()).ToArray();

            foreach (var ignoredBone in lowerIgnoredBones)
            {
                var boneLower = bone.name.ToLower();
                
                if (boneLower.Contains(ignoredBone)) return false;
            }

            return true;
        }

        private bool IsValidMainPose(XRSkeletonPose pose)
        {
            return pose.leftHandPositions != null && pose.leftHandRotations != null && pose.rightHandPositions != null && pose.rightHandRotations != null;
        }
        
        private void DrawPoseManager()
        {
            if (Application.isPlaying) return;
            
            EditorGUILayout.BeginVertical("box");
            
            _propertyShowPoses.boolValue =
                IndentedFoldoutHeader(_propertyShowPoses.boolValue, "Pose Manager");

            if (_propertyShowPoses.boolValue)
            {

                EditorGUILayout.PropertyField(_propertyPose);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(!_propertyShowLeft.boolValue || !_propertyShowRight.boolValue || _poser.pose == null || !IsValidMainPose(_poser.pose));

                // TODO: disable buttons if there is no saved data

                GUI.backgroundColor = _poserSettings.loadPoseColour;
                
                if (GUILayout.Button(_propertyPose.name + " (MAIN)"))
                {
                    LoadPose(_propertyPose.objectReferenceValue as XRSkeletonPose);
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
                   
                   if (GUILayout.Button("Save Pose", "button"))
                   {
                       SavePose();
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
                   
                   EditorGUILayout.EndHorizontal();
                }
        
                EditorGUILayout.EndFoldoutHeaderGroup();
        
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
        
        private void SavePose()
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
            
            _poser.pose = copy;

            LoadPose(copy); // Load pose automatically for convenience
            
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
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
        }

        private void LoadPose(XRSkeletonPose loadedPose)
        {
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

    }    
}