using System;
using UnityEngine;

public class XR_SkeletonPoser : MonoBehaviour
{
    
    public GameObject leftHand = null;
    
    [HideInInspector]
    public bool showLeft;
    
    [HideInInspector]
    public GameObject tempLeft;

    // Functions used by XR_SkeletonPoserEditor.
    
    public GameObject ShowLeftPreview()
    {
        return Instantiate(leftHand);
    }
    
    public void DestroyLeftPreview(GameObject obj)
    {
        DestroyImmediate(obj);
    }
    
}
