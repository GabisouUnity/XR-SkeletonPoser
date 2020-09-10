using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(XR_SkeletonPoser))]
public class XR_SkeletonPoserEditor : Editor
{
    private GameObject _tempLeft;
    private bool _toggleLeftHand;

    private SerializedObject _targetObject;
    private SerializedProperty _propertyLeftToggled = null;
    private SerializedProperty _propertyTempLeft = null;

    private void OnEnable()
    {
        _targetObject = serializedObject;
        _propertyLeftToggled = _targetObject.FindProperty("leftToggled");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawPoseEditor();
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
            
            _targetObject.Update();
            
            _toggleLeftHand = EditorGUILayout.Toggle("Toggle Left", _toggleLeftHand);
            if (_toggleLeftHand)
            {
                if (!_propertyLeftToggled.boolValue) _tempLeft = _targetObject.ShowLeftPreview();
            
                _tempLeft.transform.parent = _targetObject.targetObject;
                _tempLeft.transform.localPosition = Vector3.zero;
                _tempLeft.transform.localRotation = Quaternion.identity;
            
                _propertyLeftToggled.boolValue = true;
            }
            else
            {
                if (_propertyLeftToggled.boolValue) _targetObject.DestroyLeftPreview(_tempLeft);
                _propertyLeftToggled.boolValue = false;
            }
        }
    }
}