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
        private SerializedProperty _propertyIgnoredBones = null;
        
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

            _propertyActivePose = serializedObject.FindProperty("currentPose");
            _propertyIgnoredBones = serializedObject.FindProperty("ignoredBones");

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

                EditorGUILayout.BeginHorizontal();

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
                
                // Preview Right Button
                
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

                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.PropertyField(_propertyActivePose);
                
                // Load pose button

                // Grey it out if hands aren't active
                EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false);

                if (GUILayout.Button("Load Pose"))
                {
                    XR_SkeletonPose loadedPose = _poser.GetLoadedPose();
                    
                    var leftBonePositions = loadedPose.leftBonePositions;
                    var leftBoneRotations = loadedPose.leftBoneRotations;

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
                    
                    // Declare right values
                    
                    if (_rightGameObject != null)
                    {
                        // Inverse right values
                        
                        var rightTransforms = _rightGameObject.GetComponentsInChildren<Transform>().ToArray();
                        Debug.Log("Create left bone array");
                        
                        var rightBonePositions = loadedPose.rightBonePositions;
                        var rightBoneRotations = loadedPose.rightBoneRotations;
                        Debug.Log("Set positions");

                        for (int i = 0; i < rightBonePositions.Length; i++)
                        {
                            rightTransforms[i].localPosition = rightBonePositions[i];
                            Debug.Log("Start position loop");
                        }
                        Debug.Log("End position loop");

                        for (int i = 0; i < rightBoneRotations.Length; i++)
                        {
                            rightTransforms[i].localRotation = rightBoneRotations[i];
                            Debug.Log("Start rotation loop");
                        }
                        Debug.Log("End rotation loop");
                    }

                }
                
                // Grey it out if hands aren't active
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                
                // Create new instance of XR_SkeletonPose, this is the one that is edited
                XR_SkeletonPose newPose = CreateInstance<XR_SkeletonPose>();
                
                // Grey it out if hands aren't active
                EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false);
                
                // Save pose button
                
                if (GUILayout.Button("Save Pose"))
                {

                    if (_propertyTempLeft.objectReferenceValue != null)
                    {
                        newPose.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
                        newPose.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
                    }

                    if (_propertyTempRight.objectReferenceValue != null)
                    {
                        newPose.rightBonePositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
                        newPose.rightBoneRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);
                    }
                    
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
                
                // Reset Pose

                if (GUILayout.Button("Reset Pose"))
                {
                    // Make sure we warn the user before they reset their pose
                    if (EditorUtility.DisplayDialog("Reset Pose?", "Are you sure you want to do this? You will lose your pose on this object!", "Yes", "No"))
                    {
                        // They are sure, reset pose
                        
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
                }
                
                EditorGUI.EndDisabledGroup();
                
            }
        }
    }    
}

