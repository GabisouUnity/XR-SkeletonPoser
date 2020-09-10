using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(XR_SkeletonPoser))]
public class XR_SkeletonPoserEditor : Editor
{
    private XR_SkeletonPoser _poser = null;
    
    private GameObject _tempLeft;
    private bool _toggleLeftHand;

    private SerializedObject _targetObject = null;
    private SerializedProperty _propertyLeftToggled = null;
    // private SerializedProperty _propertyTempLeft = null;

    private void OnEnable()
    {
        _targetObject = serializedObject;
        
        GameObject poserGameObject = _targetObject.targetObject as GameObject;
        // Returns null, bug
        _poser = poserGameObject.GetComponent<XR_SkeletonPoser>();

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
                if (!_propertyLeftToggled.boolValue) _tempLeft = _poser.ShowLeftPreview();
            
                _tempLeft.transform.parent = _poser.gameObject.transform;
                _tempLeft.transform.localPosition = Vector3.zero;
                _tempLeft.transform.localRotation = Quaternion.identity;
            
                _propertyLeftToggled.boolValue = true;
            }
            else
            {
                if (_propertyLeftToggled.boolValue) _poser.DestroyLeftPreview(_tempLeft);
                _propertyLeftToggled.boolValue = false;
            }
        }
    }
}