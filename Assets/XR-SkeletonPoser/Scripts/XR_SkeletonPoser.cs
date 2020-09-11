using System;
using UnityEngine;

public class XR_SkeletonPoser : MonoBehaviour
{

    // [SerializeField] private XR_SkeletonPose activePose;

    public GameObject leftHand = null;

    private bool _showLeft;
    
    // Functions used by XR_SkeletonPoserEditor.
    
    public GameObject ShowLeftPreview()
    {
        GameObject left = Instantiate(leftHand);
        return left;
    }
    
    public void DestroyLeftPreview(GameObject obj)
    {
        DestroyImmediate(obj);
    }
    
}
