using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(XR_SkeletonPoser))]
public class XR_SkeletonPoserEditor : Editor
{
    
    private XR_SkeletonPoser _poser;
    
    private GameObject tempLeft = null;
    private GameObject tempRight = null;

    private bool showLeftIsPressed;
    private bool hideLeftIsPressed;

    private bool showRightIsPressed;
    private bool hideRightIsPressed;

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        _poser = (XR_SkeletonPoser) target;
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        DestroyImmediate(tempLeft);
        DestroyImmediate(tempRight);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        DrawPoseEditor();
    }

    private void DrawPoseEditor()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("Cannot modify pose while in play mode.");
        }
        else
        {
            // Edit mode is active
            
            // Preview button
            
            EditorGUILayout.LabelField("Do not unselect the gameobject, it will destroy the hand! This isn't intended behaviour, fix soon™!");
            
            EditorGUILayout.BeginHorizontal("box");
            
            showLeftIsPressed = GUILayout.Button("Show Left", "box");
            if(showLeftIsPressed && tempLeft == null)
            {
                tempLeft = _poser.ShowLeftPreview();
            }
            else if (showLeftIsPressed && tempLeft != null)
            {
                return;
            }
            
            hideLeftIsPressed = GUILayout.Button("Destroy Left", "box");
            if(hideLeftIsPressed && tempLeft != null)
            {
                tempLeft = _poser.DestroyLeftPreview(tempLeft);
            }
            else if (hideLeftIsPressed && tempLeft == null)
            {
                return;
            }

            EditorGUILayout.Space(20);

            showRightIsPressed = GUILayout.Button("Show Right", "box");
            if(showRightIsPressed && tempRight == null)
            {
                tempRight = _poser.ShowLeftPreview();
            }
            else if (showRightIsPressed && tempRight != null)
            {
                return;
            }
            
            hideRightIsPressed = GUILayout.Button("Destroy Right", "box");
            if(hideRightIsPressed && tempRight != null)
            {
                tempRight = _poser.DestroyRightPreview(tempRight);
            }
            else if (hideRightIsPressed && tempRight == null)
            {
                return;
            }

            
            EditorGUILayout.EndHorizontal();

            // Reset Pose button

            // Save pose button

            // Load pose button

        }
    }

}
