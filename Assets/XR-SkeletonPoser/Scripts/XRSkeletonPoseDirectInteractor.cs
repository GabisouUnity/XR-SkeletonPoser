using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XRSkeletonPoseDirectInteractor : XRDirectInteractor
    {

        #region Editor storage

        [Space] [Tooltip("What hand is a child of the controller?")]
        public GameObject handObject;
        
        [Tooltip("What hand is attached to the XR_SkeletonPoseInteractor?")]
        public HandType handType;

        #endregion

        private XRSkeletonPose _defaultPose;
        private XRSkeletonPoser _poser;
        private XRController _inputController;

        private bool _isSkeletonPoseInteractable = false;

        protected override void Awake()
        {
            base.Awake();

            _inputController = GetComponent<XRController>();

            // Cache default pose at runtime
            _defaultPose = XRSkeletonPoser.GetDefaultPose(handType, handObject);
        }
        
        protected override void OnSelectEntered(XRBaseInteractable interactable)
        {
            base.OnSelectEntered(interactable);

            // Do not run the below code if the object isn't a skeleton poser, ie do not pose hand if not a poser interactable
            if (!interactable.TryGetComponent(out _poser)) return;

            var pose = _poser.FetchPose();
            
            _poser.SetPose(pose, handObject, handType);
            
            _poser.SetOffset(selectTarget, handObject);
            
            _isSkeletonPoseInteractable = true;
        }
        
        protected override void OnSelectExited(XRBaseInteractable interactable)
        {
            base.OnSelectExited(interactable);

            if (_isSkeletonPoseInteractable)
            {
                _poser.SetDefaultPose(handType, handObject, _defaultPose); // Reset back to default bone pose on select exit if it was a skeleton poser
            }
            
            _isSkeletonPoseInteractable = false;

            _poser = null;
        }
        
    }
}