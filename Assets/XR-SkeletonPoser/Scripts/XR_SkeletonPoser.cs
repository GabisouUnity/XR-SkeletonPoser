using UnityEngine;
using System.Linq;

public class XR_SkeletonPoser : MonoBehaviour
{

    public GameObject leftHand = null;
    public GameObject rightHand = null;

    [HideInInspector] public bool showLeft;
    [HideInInspector] public GameObject tempLeft;

    [HideInInspector] public XR_SkeletonPose currentPose;
    [HideInInspector] public bool showRight;
    [HideInInspector] public GameObject tempRight;

    private void Awake()
    {
        DestroyLeftPreview(tempLeft);
        DestroyRightPreview(tempRight);
    }

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

    public Vector3[] GetBonePositions(GameObject target)
    {
        return target.GetComponentsInChildren<Transform>().Select(x => x.position).ToArray();
    }

    public Quaternion[] GetBoneRotations(GameObject target)
    {
        return target.GetComponentsInChildren<Transform>().Select(x => x.rotation).ToArray();
    }

    public Quaternion InverseBoneRotations(Quaternion boneRot)
    {
        Quaternion normalQuat = boneRot;
        float mirrorY = -normalQuat.y;
        float mirrorZ = -normalQuat.z;
        
        return new Quaternion(boneRot.x, mirrorY, mirrorZ, boneRot.w);
    }

}