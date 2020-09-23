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
                           CopyToRight(newPose);
                           
                           // _poser.activePose = newPose;
                           
                           // LoadPose(); // Load pose for convenience 
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

        private void CopyToRight(XR_SkeletonPose basePose)
        {
            // Copy left to right
            
            var rightBonePositions = basePose.leftBonePositions;
            var rightBoneRotations = basePose.leftBoneRotations;

            var currentPose = CreateInstance<XR_SkeletonPose>();
            currentPose = GetPose(currentPose);
            
            if (_propertyTempRight.objectReferenceValue as GameObject == null) return;
            Debug.Log("Start flip " + _propertyTempRight.objectReferenceValue);
            
            var rightTransforms = _poser.rightHand.GetComponentsInChildren<Transform>().ToArray();

            Debug.Log("Get right transforms" + rightTransforms);
            
            for (int i = 0; i < rightBonePositions.Length; i++)
            {
                rightTransforms[i].position = rightBonePositions[i];
            }

            for (int i = 0; i < rightBoneRotations.Length; i++)
            {
                rightTransforms[i].rotation = rightBoneRotations[i];
            }
        }

        // private void CopyToLeft()
        // {
        //     // Copy right pose data to left
        // }

        private XR_SkeletonPose GetPose(XR_SkeletonPose newPose)
        {
            // Todo: Only overwrite the data from the active hand(s). Although it might not be possible?

            newPose.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
            newPose.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);

            newPose.rightBonePositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
            newPose.rightBoneRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);

            return newPose;
        }
        
        private void SavePose(XR_SkeletonPose newPose)
        {
            // Set pose to new pose data to avoid the need for reassignment after saving
            _poser.activePose = newPose;

            if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
            {
                // Folder doesn't exist, create new
                AssetDatabase.CreateFolder("Assets", "XRPoses");
                AssetDatabase.CreateAsset(newPose, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
            else
            {
                // Folder exists
                AssetDatabase.CreateAsset(newPose, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
        }

        private void ResetPose(XR_SkeletonPose pose)
        {
            // Set pose to new pose data to avoid the need for reassignment after saving the file
            pose = _defaultPose;
            _poser.activePose = pose;

            LoadPose(); // Load pose automatically for convenience
            
            // Save and overwrite
            if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
            {
                // Folder doesn't exist, create new
                AssetDatabase.CreateFolder("Assets", "XRPoses");

                // Overwrite the pose with a default pose
                AssetDatabase.CreateAsset(_defaultPose, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
            else
            {
                // Folder exists

                // Overwrite the pose with a default pose

                AssetDatabase.CreateAsset(_defaultPose, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
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
            
            else if (_rightGameObject != null)
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