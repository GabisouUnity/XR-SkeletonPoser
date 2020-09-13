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
            
            if (GUILayout.Button("Save Pose"))
            {
                XR_SkeletonPose newPose = CreateInstance<XR_SkeletonPose>();

                if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
                {
                    // Folder doesn't exist, create new
                    AssetDatabase.CreateFolder("Assets", "XRPoses");
                    AssetDatabase.CreateAsset(newPose, "Assets/XRPoses/" + _poser.gameObject.name + ".asset");
                }
                else
                {
                    // Folder exists
                    AssetDatabase.CreateAsset(newPose, "Assets/XRPoses/" + _poser.gameObject.name + ".asset");
                }
            }
            
            // Reset Pose

            if (GUILayout.Button("Reset Pose"))
            {
                
            }
            
        }
    }
}