using System;
using UnityEngine;

public class XR_SkeletonPoser : MonoBehaviour
{

    // [SerializeField] private XR_SkeletonPose activePose;

    public GameObject leftHand = null;
    
    public bool leftToggled = false;
    
    // Functions used by XR_SkeletonPoserEditor
    
    public GameObject ShowLeftPreview()
    {
        return Instantiate(leftHand);
    }
    
    public GameObject DestroyLeftPreview(GameObject obj)
    {
        DestroyImmediate(obj);
        return null;
    }
    
}
