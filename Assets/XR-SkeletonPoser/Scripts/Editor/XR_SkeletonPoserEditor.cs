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
        private GameObject _leftGameObject = null;

        private SerializedProperty _propertyShowRight = null;
        private SerializedProperty _propertyTempRight = null;
        private GameObject _rightGameObject = null;

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
            
            _defaultPose.leftBonePositions = _poser.GetBonePositions(_poser.leftHand);
            _defaultPose.leftBoneRotations = _poser.GetBoneRotations(_poser.leftHand);

            _defaultPose.rightBonePositions = _poser.GetBonePositions(_poser.rightHand);
            _defaultPose.rightBoneRotations = _poser.GetBoneRotations(_poser.rightHand);
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
                // Preview Left button
                
                // Create new instance of XR_SkeletonPose, this is the one that is edited
                var newPose = CreateInstance<XR_SkeletonPose>();

                _propertyShowPoseEditor.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(_propertyShowPoseEditor.boolValue, "Show Pose Editor");

                if (_propertyShowPoseEditor.boolValue)
                {
                   EditorGUILayout.BeginHorizontal(); EditorGUI.BeginDisabledGroup(_poser.leftHand == null);
                   
                   if (!_propertyShowLeft.boolValue)
                   {
                       if (GUILayout.Button("Show Left Hand"))
                       {
                           _leftGameObject = _poser.ShowLeftPreview();

                           _leftGameObject.transform.parent = _poser.transform;
                           _leftGameObject.transform.localPosition = Vector3.zero;
                           _leftGameObject.transform.localRotation = Quaternion.identity;
                           
                           _propertyShowLeft.boolValue = true;

                           _propertyTempLeft.objectReferenceValue = _leftGameObject;
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
                   
                   EditorGUI.BeginDisabledGroup(_poser.rightHand == null);
                   
                   if (!_propertyShowRight.boolValue)
                   {
                       if (GUILayout.Button("Show Right Hand"))
                       {
                           _rightGameObject = _poser.ShowRightPreview();

                           _rightGameObject.transform.parent = _poser.transform;
                           _rightGameObject.transform.localPosition = Vector3.zero;
                           _rightGameObject.transform.localRotation = Quaternion.identity;
                           
                           _propertyShowRight.boolValue = true;

                           _propertyTempRight.objectReferenceValue = _rightGameObject;
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
                   
                   EditorGUILayout.BeginHorizontal();
                   
                   EditorGUI.BeginDisabledGroup(_propertyTempLeft.objectReferenceValue == null || _propertyTempRight.objectReferenceValue == null);
                   
                   if (GUILayout.Button("Copy Left Pose to right hand"))
                   {
                       if (EditorUtility.DisplayDialog("Copy left pose to right hand?",
                           "Are you sure? This will overwrite your right hand's pose data", "Yes", "No"))
                       {
                           newPose = GetPose(newPose); // Get pose without saving
                           Debug.Log("Get pose");
                           
                           newPose = CopyToRight(newPose, _poser.activePose);
                           Debug.Log("Copy pose");
                           
                           SavePose(newPose);
                           Debug.Log("Save pose");
                           
                           // _poser.activePose = newPose;
                           // Debug.Log("Set active pose to pose");
                           
                           LoadPose(); // Load pose for convenience 
                           Debug.Log("Load pose");
                       }
                   }

                   if (GUILayout.Button("Copy Right Pose to left hand"))
                   {
                       // newPose = CopyToOpposite();
                       // poser.activePose = newPose;
                       
                       LoadPose(); // Load pose for convenience 
                   }
                   
                   EditorGUI.EndDisabledGroup();
                   
                   EditorGUILayout.EndHorizontal();
                   
                   EditorGUILayout.BeginHorizontal();
                   
                   // Create field in editor for active pose (also referred to as loaded pose)
                   EditorGUILayout.PropertyField(_propertyActivePose);

                   // Grey it out if hands aren't active and there is no loaded pose
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false || _poser.GetLoadedPose() == null);

                   if (GUILayout.Button("Load Pose"))
                   {
                       LoadPose();
                   }
                   
                   EditorGUI.EndDisabledGroup(); EditorGUILayout.EndHorizontal();
                   
                   EditorGUILayout.Space();
                   
                   // Grey it out if hands aren't active
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false || _propertyShowRight.boolValue == false);
                   
                   if (GUILayout.Button("Save Pose"))
                   {
                       SavePose(newPose);
                   }
                   
                   EditorGUI.EndDisabledGroup();

                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false || _poser.GetLoadedPose() == null);
                   
                   if (GUILayout.Button("Reset Pose"))
                   {
                       // Make sure we warn the user before they reset their pose
                       if (EditorUtility.DisplayDialog("Reset Pose?",
                           "Are you sure you want to do this? You will lose your pose on this object!", "Yes", "No"))
                       {
                           // They are sure, reset pose
                           ResetPose(newPose); // Reset new pose instance to default pose
                       }
                   }
                   
                   EditorGUI.EndDisabledGroup();
                }
                
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        private XR_SkeletonPose CopyToRight(XR_SkeletonPose source, XR_SkeletonPose destination)
        {
            // Not sure what is going wrong here but copying the pose nulls out the fields and then saves them??
            
            destination.leftBonePositions = source.leftBonePositions;
            destination.leftBoneRotations = source.leftBoneRotations;

            Debug.Log("Set dest left to source left");
            
            source.rightBonePositions = source.leftBonePositions;
            source.rightBoneRotations = source.leftBoneRotations;

            for (int i = 0; i < destination.rightBoneRotations.Length; i++)
            {
                destination.rightBoneRotations[i] = source.rightBoneRotations[i];
                Debug.Log("Set dest right bone rot to source right rot");
            }
            
            for (int i = 0; i < destination.rightBonePositions.Length; i++)
            {
                destination.rightBonePositions[i] = source.rightBonePositions[i];
                Debug.Log("Set dest right bone pos to source right pos");
            }

            return destination;
        }

        // private void CopyToLeft()
        // {
        //     // Copy right pose data to left
        // }

        private void SavePose(XR_SkeletonPose savedPose)
        {
            // Todo: Only overwrite the data from the active hand(s). Although it might not be possible?

            // Create copy of pose to stop error whilst saving ("Object already exists")
            var copy = ScriptableObject.Instantiate(savedPose);
            
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

        private void ResetPose(XR_SkeletonPose pose)
        {
            // Set pose to new pose data to avoid the need for reassignment after saving the file
            
            // Create copy of pose to stop error whilst saving ("Object already exists")
            var copy = ScriptableObject.Instantiate(pose);

            copy = _defaultPose;
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

        /// <summary>
        /// Get pose without saving
        /// </summary>
        private XR_SkeletonPose GetPose(XR_SkeletonPose inputPose)
        {
            inputPose.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
            inputPose.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);

            inputPose.rightBonePositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
            inputPose.rightBoneRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);

            // Get input XRPose instance and return it but full from scene
            return inputPose;
        }
        
        private void LoadPose()
        {
            var loadedPose = _poser.GetLoadedPose();

            var leftBonePositions = loadedPose.leftBonePositions;
            var leftBoneRotations = loadedPose.leftBoneRotations;

            var rightBonePositions = loadedPose.rightBonePositions;
            var rightBoneRotations = loadedPose.rightBoneRotations;

            if (_leftGameObject != null)
            {
                var leftTransforms = _leftGameObject.GetComponentsInChildren<Transform>().ToArray();

                // Set left values to loaded pose
                for (int i = 0; i < leftBonePositions.Length; i++)
                {
                    leftTransforms[i].localPosition = leftBonePositions[i];
                }

                for (int i = 0; i < leftBoneRotations.Length; i++)
                {
                    leftTransforms[i].localRotation = leftBoneRotations[i];
                }
            }
            
            if (_rightGameObject != null)
            {
                var rightTransforms = _rightGameObject.GetComponentsInChildren<Transform>().ToArray();

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
}