using UnityEngine;

public class XR_SkeletonPoser : MonoBehaviour
{

    public XR_SkeletonPose currentPose;
    
    public GameObject leftHand = null;
    public GameObject rightHand = null;
    
    [HideInInspector] public bool showLeft;
    [HideInInspector] public GameObject tempLeft;
    
    [HideInInspector] public bool showRight;
    [HideInInspector] public GameObject tempRight;

    // Functions used by XR_SkeletonPoserEditor.
    
    public GameObject ShowLeftPreview()
    {
        return Instantiate(leftHand);
    }
    
    public void DestroyLeftPreview(GameObject obj)
    {
        DestroyImmediate(obj);
    }
    
    public GameObject ShowRightPreview()
    {
        return Instantiate(rightHand);
    }
    
    public void DestroyRightPreview(GameObject obj)
    {
        DestroyImmediate(obj);
    }

}