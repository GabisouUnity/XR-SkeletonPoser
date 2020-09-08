using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(XR_SkeletonPoser))]
public class XR_SkeletonPoserEditor : Editor
{
    private XR_SkeletonPoser _poser;
    
    private GameObject _tempLeft;
    private bool _toggleLeftHand;
    private bool _setToggleLeftHand;

    private void OnEnable()
    {
        _poser = (XR_SkeletonPoser) target;
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
            
            // Destroy hands when in playmode
            DestroyImmediate(_tempLeft);
            // DestroyImmediate(_tempRight);
        }
        else
        {
            // Preview button

            // EditorGUILayout.BeginHorizontal("box");

            _toggleLeftHand = EditorGUILayout.Toggle("Toggle Left", _toggleLeftHand);
            if (_toggleLeftHand)
            {
                if (!_setToggleLeftHand) _tempLeft = _poser.ShowLeftPreview();
                _setToggleLeftHand = true;
            }
            else
            {
                if (_setToggleLeftHand) _poser.DestroyLeftPreview(_tempLeft);
                _setToggleLeftHand = false;
            }

            // EditorGUILayout.EndHorizontal();

            // Reset Pose button

            // Save pose button

            // Load pose button
        }
    }
}