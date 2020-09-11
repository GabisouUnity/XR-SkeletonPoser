using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(XR_SkeletonPoser))]
public class XR_SkeletonPoserEditor : Editor
{
    private XR_SkeletonPoser _poser = null;
    
    // private SerializedProperty _propertyShowLeft = null;
    private GameObject _leftGameObject = null;
    private bool _leftIsShown;

    private void OnEnable()
    {
        _poser = (XR_SkeletonPoser) target;

        // _propertyShowLeft = serializedObject.FindProperty("_showLeft");
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
            
            // _toggleLeftHand = EditorGUILayout.Toggle("Toggle Left", _toggleLeftHand);
            // if (_toggleLeftHand)
            // {
            //     if (!_showLeft) tempLeft = _poser.ShowLeftPreview();
            //
            //     tempLeft.transform.parent = _poser.gameObject.transform;
            //     tempLeft.transform.localPosition = Vector3.zero;
            //     tempLeft.transform.localRotation = Quaternion.identity;
            //
            //     _showLeft = true;
            // }
            // else
            // {
            //     if (_showLeft) _poser.DestroyLeftPreview(tempLeft);
            //     _showLeft = false;
            // }

            // if (_propertyShowLeft.boolValue)
            // {
                if (GUILayout.Button("Show Hand"))
                {
                    if (!_leftIsShown)
                    {
                        _leftGameObject = _poser.ShowLeftPreview();
                        _leftIsShown = true;
                    }
                }
                if (GUILayout.Button("Hide Hand"))
                {
                    if (_leftIsShown)
                    {
                        _poser.DestroyLeftPreview(_leftGameObject);
                        _leftIsShown = false;
                    }
                }
            // }
        }
    }
}