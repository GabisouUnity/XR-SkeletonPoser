using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(XR_SkeletonPoser))]
public class XR_SkeletonPoserEditor : Editor
{
    private XR_SkeletonPoser _poser = null;
    
    private SerializedProperty _propertyShowLeft = null;
    private SerializedProperty _propertyTempLeft = null;
    
    private GameObject _leftGameObject = null;

    private void OnEnable()
    {
        _poser = (XR_SkeletonPoser) target;

        _propertyShowLeft = serializedObject.FindProperty("showLeft");
        _propertyTempLeft = serializedObject.FindProperty("tempLeft");
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
            // Preview button
            
            if (!_propertyShowLeft.boolValue)
            {
                if (GUILayout.Button("Show Hand"))
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
                if (GUILayout.Button("Hide Hand"))
                {
                    _poser.DestroyLeftPreview(_propertyTempLeft.objectReferenceValue as GameObject);
                    _propertyShowLeft.boolValue = false;
                }
            }
        }
    }
}