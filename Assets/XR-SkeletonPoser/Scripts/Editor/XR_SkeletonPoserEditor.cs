using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(XR_SkeletonPoser))]
public class XR_SkeletonPoserEditor : Editor
{
    private XR_SkeletonPoser _poser = null;

    private SerializedProperty _propertyActivePose = null;
    
    private SerializedProperty _propertyShowLeft = null;
    private SerializedProperty _propertyTempLeft = null;

    private SerializedProperty _propertyShowRight = null;
    private SerializedProperty _propertyTempRight = null;

    private void OnEnable()
    {
        _poser = (XR_SkeletonPoser) target;

        _propertyActivePose = serializedObject.FindProperty("currentPose");

        _propertyShowLeft = serializedObject.FindProperty("showLeft");
        _propertyTempLeft = serializedObject.FindProperty("tempLeft");

        _propertyShowRight = serializedObject.FindProperty("showRight");
        _propertyTempRight = serializedObject.FindProperty("tempRight");
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
            
            // Preview Right Button
            
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

            EditorGUILayout.EndHorizontal();
            
            // Save pose button
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(_propertyActivePose);
            
            EditorGUILayout.Space();
            
            // Grey it out if hands aren't active
            EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false);

            XR_SkeletonPose newPose = CreateInstance<XR_SkeletonPose>();
            
            if (GUILayout.Button("Save Pose"))
            {
                // Create new instance of XR_SkeletonPose
                
                // Set newPose bonepos and bonerot to the tempLeft's modified poses
                
                newPose.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
                newPose.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);

                // Inverse previous values for the right

                // Set bone rots to left bone rots so we have something to inverse
                newPose.rightBoneRotations = newPose.leftBoneRotations;

                for (var i = 0; i < newPose.leftBoneRotations.Length; i++)
                {
                    newPose.rightBoneRotations[i] = _poser.InverseBoneRotations(newPose.leftBoneRotations[i]);
                }

                // Set bone pos to left bone pos so we have something to inverse
                newPose.rightBonePositions = newPose.leftBonePositions;
                
                for (int i = 0; i < newPose.leftBonePositions.Length; i++)
                {
                    newPose.rightBonePositions[i] = _poser.InverseBonePositions(newPose.leftBonePositions[i]);
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
                    
                    newPose.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
                    newPose.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
                    
                    // Set right bones to be the same as left bones
                    // Note: There might be a better way to do this, but it stops a null ref and it's being set to be reset anyways
                    newPose.rightBonePositions = newPose.leftBonePositions;
                    newPose.rightBoneRotations = newPose.leftBoneRotations;

                    // Reset leftBonePos
                    for (var i = 0; i < newPose.leftBonePositions.Length; i++)
                    {
                        newPose.leftBonePositions[i] = Vector3.zero;
                    }
                    // Reset leftBoneRot
                    for (var i = 0; i < newPose.leftBoneRotations.Length; i++)
                    {
                        newPose.leftBoneRotations[i] = Quaternion.identity;
                    }
                    // Reset rightBonePos
                    for (int i = 0; i < newPose.rightBonePositions.Length; i++)
                    {
                        newPose.rightBonePositions[i] = Vector3.zero;
                    }
                    // Reset rightBoneRot
                    for (int i = 0; i < newPose.rightBoneRotations.Length; i++)
                    {
                        newPose.rightBoneRotations[i] = Quaternion.identity;
                    }
                }
            }
            
            EditorGUI.EndDisabledGroup();
            
        }
    }
}