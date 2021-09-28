using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace yellowyears.SkeletonPoser
{
    public class XRSkeletonPoseRayInteractor : XRRayInteractor
    {
        
        #region Editor storage

        [Space] [Tooltip("What hand is a child of the controller?")] public GameObject handObject;
        
        // public enum HandType { Left, Right } // TODO: Could possibly be accessed and set from the XRController?
        [Tooltip("What hand is attached to the XR_SkeletonPoseInteractor?")] public HandType handType;
        
        #endregion

        private XRSkeletonPose _defaultPose;
        private XRSkeletonPoser _poser = null;

        private bool _isSkeletonPoseInteractable = false;

        protected override void Awake()
        {
            base.Awake();
            
            // Cache default pose at runtime
            _defaultPose = XRSkeletonPoser.GetDefaultPose(handType, handObject);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            base.OnSelectEntered(selectEnterEventArgs);
            
            // Do not run the below code if the object isn't a skeleton poser, ie do not pose hand if not a poser interactable
            if (!selectEnterEventArgs.interactable.TryGetComponent(out _poser)) return;
            
            var pose = _poser.FetchPose();
                
            _poser.SetPose(pose, handObject, handType);
            _poser.SetOffset(selectTarget, handObject);
            
            _isSkeletonPoseInteractable = true;
        }

        protected override void OnSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            base.OnSelectExited(selectExitEventArgs);

            // Reset back to default bone pose on select exit if it was a poser interactable
            if(_isSkeletonPoseInteractable) _poser.SetDefaultPose(handType, handObject, _defaultPose);

            _isSkeletonPoseInteractable = false;
        }
    }
}