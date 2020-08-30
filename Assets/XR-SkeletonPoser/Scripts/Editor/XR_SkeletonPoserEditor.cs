using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(XR_SkeletonPoser))]
public class XR_SkeletonPoserEditor : Editor
{
    
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
            
            // Reset Pose button
            
            // Save pose button
            
            // Load pose button
            
        }
    }
    
}
