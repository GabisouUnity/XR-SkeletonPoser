using System.Linq;

using UnityEditor;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    [CustomEditor(typeof(XR_SkeletonPoser))]
    public class XR_SkeletonPoserEditor : Editor
    {
        private XR_SkeletonPoser _poser = null;
        
        private XR_SkeletonPose _defaultPose = null;

        private SerializedProperty _propertyActivePose = null;
        private SerializedProperty _propertyShowPoseEditor = null;
        
        private SerializedProperty _propertyShowLeft = null;
        private SerializedProperty _propertyTempLeft = null;
        // private GameObject _leftGameObject = null;

        private SerializedProperty _propertyShowRight = null;
        private SerializedProperty _propertyTempRight = null;
        // private GameObject _rightGameObject = null;

        private void OnEnable()
        {
            _poser = (XR_SkeletonPoser) target;

            _defaultPose = CreateInstance<XR_SkeletonPose>();
            GetDefaultPose();

            _propertyActivePose = serializedObject.FindProperty("activePose");
            _propertyShowPoseEditor = serializedObject.FindProperty("showPoseEditor");
            
            _propertyShowLeft = serializedObject.FindProperty("showLeft");
            _propertyTempLeft = serializedObject.FindProperty("tempLeft");

            _propertyShowRight = serializedObject.FindProperty("showRight");
            _propertyTempRight = serializedObject.FindProperty("tempRight");
        }
        
        private void GetDefaultPose()
        {
            // Get default values from the hand prefab.
            
            _defaultPose.leftBonePositions = _poser.GetBonePositions(XR_SkeletonPoserSettings.Instance.leftHand);
            _defaultPose.leftBoneRotations = _poser.GetBoneRotations(XR_SkeletonPoserSettings.Instance.leftHand);

            _defaultPose.rightBonePositions = _poser.GetBonePositions(XR_SkeletonPoserSettings.Instance.rightHand);
            _defaultPose.rightBoneRotations = _poser.GetBoneRotations(XR_SkeletonPoserSettings.Instance.rightHand);
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            DrawPoseEditor();
            
            serializedObject.ApplyModifiedProperties();
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

                // Create new instance of XR_SkeletonPose, this is the one that is edited
                var newPose = CreateInstance<XR_SkeletonPose>();

                // _propertyShowPoseEditor.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(_propertyShowPoseEditor.boolValue, "Show Pose Editor");

                // _propertyShowPoseEditor.boolValue = EditorGUILayout.Foldout(_propertyShowPoseEditor.boolValue, "Show Pose Editor");
                _propertyShowPoseEditor.boolValue =
                    IndentedFoldoutHeader(_propertyShowPoseEditor.boolValue, "Pose Editor");

                if (_propertyShowPoseEditor.boolValue)
                {
                   EditorGUILayout.BeginHorizontal(); EditorGUI.BeginDisabledGroup(XR_SkeletonPoserSettings.Instance.leftHand == null);
                   
                   if (!_propertyShowLeft.boolValue)
                   {
                       if (GUILayout.Button("Show Left Hand"))
                       {
                           var leftGameObject = _poser.ShowLeftPreview();

                           leftGameObject.transform.parent = _poser.transform;
                           leftGameObject.transform.localPosition = Vector3.zero;
                           leftGameObject.transform.localRotation = Quaternion.identity;
                           
                           _propertyShowLeft.boolValue = true;

                           _propertyTempLeft.objectReferenceValue = leftGameObject;
                       }
                   }
                   else
                   {
                       if (GUILayout.Button("Hide Left Hand"))
                       {
                           _poser.DestroyLeftPreview(_propertyTempLeft.objectReferenceValue as GameObject);
                           _propertyShowLeft.boolValue = false;
                       }
                   }
                   
                   EditorGUI.EndDisabledGroup();
                   
                   // Preview Right Button
                   
                   EditorGUI.BeginDisabledGroup(XR_SkeletonPoserSettings.Instance.rightHand == null);
                   
                   if (!_propertyShowRight.boolValue)
                   {
                       if (GUILayout.Button("Show Right Hand"))
                       {
                           var rightGameObject = _poser.ShowRightPreview();

                           rightGameObject.transform.parent = _poser.transform;
                           rightGameObject.transform.localPosition = Vector3.zero;
                           rightGameObject.transform.localRotation = Quaternion.identity;
                           
                           _propertyShowRight.boolValue = true;

                           _propertyTempRight.objectReferenceValue = rightGameObject;
                       }
                   }
                   else
                   {
                       if (GUILayout.Button("Hide Right Hand"))
                       {
                           _poser.DestroyRightPreview(_propertyTempRight.objectReferenceValue as GameObject);
                           _propertyShowRight.boolValue = false;
                       }
                   }

                   EditorGUI.EndDisabledGroup(); EditorGUILayout.EndHorizontal();
                   
                   // EditorGUILayout.BeginHorizontal();
                   //
                   // EditorGUI.BeginDisabledGroup(_propertyTempLeft.objectReferenceValue == null || _propertyTempRight.objectReferenceValue == null);
                   //
                   // if (GUILayout.Button("Copy Left Pose to right hand"))
                   // {
                   //     if (EditorUtility.DisplayDialog("Copy left pose to right hand?",
                   //         "Are you sure? This will overwrite your right hand's pose data", "Yes", "No"))
                   //     {
                   //         newPose = GetPose(newPose); // Get pose without saving
                   //         
                   //         CopyToRight(newPose, _poser.activePose);
                   //         
                   //         // SavePose(newPose);
                   //         // Debug.Log("Save pose");
                   //         
                   //         // _poser.activePose = newPose;
                   //         // Debug.Log("Set active pose to pose");
                   //         //
                   //         // LoadPose(); // Load pose for convenience 
                   //         // Debug.Log("Load pose");
                   //     }
                   // }
                   //
                   // if (GUILayout.Button("Copy Right Pose to left hand"))
                   // {
                   //     // newPose = CopyToOpposite();
                   //     // poser.activePose = newPose;
                   //     
                   //     LoadPose(); // Load pose for convenience 
                   // }
                   //
                   // EditorGUI.EndDisabledGroup();
                   //
                   // EditorGUILayout.EndHorizontal();
                   
                   EditorGUILayout.BeginHorizontal();
                   
                   // Create field in editor for active pose (also referred to as loaded pose)
                   EditorGUIUtility.labelWidth = 76;
                   EditorGUILayout.PropertyField(_propertyActivePose, new GUIContent("Active Pose"));

                   // Grey it out if hands aren't active and there is no loaded pose
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false || _poser.GetLoadedPose() == null);

                   if (GUILayout.Button("Load Pose", "button"))
                   {
                       LoadPose();
                   }
                   
                   EditorGUI.EndDisabledGroup(); EditorGUILayout.EndHorizontal();
                   
                   EditorGUILayout.Space();
                   
                   // Grey it out if hands aren't active
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false || _propertyShowRight.boolValue == false);
                   
                   if (GUILayout.Button("Save Pose", "button"))
                   {
                       SavePose(newPose);
                   }
                   
                   EditorGUI.EndDisabledGroup();

                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false || _poser.GetLoadedPose() == null);

                   EditorGUILayout.BeginHorizontal();
                   
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
                   
                   EditorGUI.EndDisabledGroup();
                   
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false || XR_SkeletonPoserSettings.Instance.referencePose == null);

                   if (GUILayout.Button("Reset To Reference Pose", "button"))
                   {
                       // Make sure we warn the user before they reset their pose
                       if (EditorUtility.DisplayDialog("Reset Pose?",
                           "Are you sure you want to do this? You will lose your pose on this object!", "Yes", "No"))
                       {
                           // They are sure, reset pose
                           ResetToReferencePose(); // Reset new pose instance to reference pose
                       }
                   }

                   EditorGUI.EndDisabledGroup();
                   EditorGUILayout.EndHorizontal();
                }
                
                // EditorGUILayout.EndFoldoutHeaderGroup();

                EditorGUILayout.EndVertical();
            }
        }

        bool IndentedFoldoutHeader(bool fold, string text, int indent = 1)
        {
            // Taken from the steamvr unity plugin code, just looks too good :p
            
            GUILayout.BeginHorizontal();
            var boldFoldoutStyle = new GUIStyle(EditorStyles.foldout) {fontStyle = FontStyle.Bold};
            GUILayout.Space(14f * indent);
            fold = EditorGUILayout.Foldout(fold, text, boldFoldoutStyle);
            GUILayout.EndHorizontal();
            return fold;
        }

        // private void CopyToRight(XR_SkeletonPose source, XR_SkeletonPose destination)
        // {
        //     destination.leftBonePositions = source.leftBonePositions;
        //     destination.leftBoneRotations = source.leftBoneRotations;
        //     
        //     // destination.rightBoneRotations = source.rightBoneRotations;
        //     // destination.rightBonePositions = source.rightBonePositions;
        //     
        //     for (int i = 0; i < destination.rightBoneRotations.Length; i++)
        //     {
        //         // destination.rightBoneRotations[i] = source.leftBoneRotations[i];
        //         // destination.rightBoneRotations[i] = _poser.MirrorBoneRotation(source.leftBoneRotations[i]);
        //         
        //         var reflectedRotation = new Quaternion(-source.rightBoneRotations[i].y, source.rightBoneRotations[i].y, source.rightBoneRotations[i].z, -source.rightBoneRotations[i].w);
        //
        //         destination.rightBoneRotations[i] = reflectedRotation;
        //
        //         // EditorUtility.DisplayProgressBar("Copying...", "Copying right hand pose", i / destination.leftBoneRotations.Length / 2f);
        //     }
        //     
        //     // Save it (if saved using SavePose() it overwrites the data for the assigned bone pos and rot)
        //
        //     var copy = CreateInstance<XR_SkeletonPose>();
        //
        //     copy.leftBonePositions = destination.leftBonePositions;
        //     copy.leftBoneRotations = destination.leftBoneRotations;
        //
        //     // copy.rightBonePositions = destination.rightBonePositions;
        //     copy.rightBoneRotations = destination.rightBoneRotations;
        //     
        //     if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
        //     {
        //         // Folder doesn't exist, create new
        //         AssetDatabase.CreateFolder("Assets", "XRPoses");
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //     }
        //     else
        //     {
        //         // Folder exists
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //     }
        //
        //     _poser.activePose = copy;
        //     
        //     // Load the pose onto the right hand only
        //     
        //     var rightHandObject = _propertyTempRight.objectReferenceValue as GameObject;
        //     
        //     if (rightHandObject == null) return;
        //     
        //     var rightTransforms = rightHandObject.GetComponentsInChildren<Transform>().ToArray();
        //
        //     // for (int i = 0; i < copy.rightBonePositions.Length; i++)
        //     // {
        //     //     rightTransforms[i].localPosition = copy.rightBonePositions[i];
        //     // }
        //     
        //     for (int i = 0; i < copy.rightBoneRotations.Length; i++)
        //     {
        //         rightTransforms[i].localRotation = copy.rightBoneRotations[i];
        //     }
        //     
        //     // EditorUtility.ClearProgressBar();
        // }

        // private void CopyToLeft()
        // {
        //     // Copy right pose data to left
        // }

        private void SavePose(XR_SkeletonPose inputPose)
        {
            // Todo: Only overwrite the data from the active hand(s). Although it might not be possible?

            // Create copy of pose to stop error whilst saving ("Object already exists")
            var copy = Instantiate(inputPose);
            
            copy.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
            copy.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);

            copy.rightBonePositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
            copy.rightBoneRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);

            // Set pose to new pose data to avoid the need for reassignment after saving
            _poser.activePose = copy;

            if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
            {
                // Folder doesn't exist, create new
                AssetDatabase.CreateFolder("Assets", "XRPoses");
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
            else
            {
                // Folder exists
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
        }

        private void ResetPose()
        {
            // Set pose to new pose data to avoid the need for reassignment after saving the file
            
            // Create copy of pose to stop error whilst saving ("Object already exists")
            var copy = CreateInstance<XR_SkeletonPose>();

            copy.leftBonePositions = _defaultPose.leftBonePositions;
            copy.leftBoneRotations = _defaultPose.leftBoneRotations;

            copy.rightBonePositions = _defaultPose.rightBonePositions;
            copy.rightBoneRotations = _defaultPose.rightBoneRotations;
            
            _poser.activePose = copy;

            LoadPose(); // Load pose automatically for convenience
            
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
        
        private void ResetToReferencePose()
        {
            // Set pose to new pose data to avoid the need for reassignment after saving the file
            
            // Create copy of pose to stop error whilst saving ("Object already exists")
            var copy = CreateInstance<XR_SkeletonPose>();

            var referencePose = XR_SkeletonPoserSettings.Instance.referencePose;
            
            copy.leftBonePositions = referencePose.leftBonePositions;
            copy.leftBoneRotations = referencePose.leftBoneRotations;

            copy.rightBonePositions = referencePose.rightBonePositions;
            copy.rightBoneRotations = referencePose.rightBoneRotations;
            
            _poser.activePose = copy;

            LoadPose(); // Load pose automatically for convenience
            
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

        // private XR_SkeletonPose GetPose(XR_SkeletonPose inputPose)
        // {
        //     // Get pose without saving
        //     
        //     inputPose.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
        //     inputPose.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
        //
        //     inputPose.rightBonePositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
        //     inputPose.rightBoneRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);
        //
        //     // Get input XRPose instance and return it but full from scene
        //     return inputPose;
        // }
        
        private void LoadPose()
        {
            var loadedPose = _poser.GetLoadedPose();

            var leftHandObject = _propertyTempLeft.objectReferenceValue as GameObject;
            var rightHandObject = _propertyTempRight.objectReferenceValue as GameObject;
            
            var leftBonePositions = loadedPose.leftBonePositions;
            var leftBoneRotations = loadedPose.leftBoneRotations;

            var rightBonePositions = loadedPose.rightBonePositions;
            var rightBoneRotations = loadedPose.rightBoneRotations;

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