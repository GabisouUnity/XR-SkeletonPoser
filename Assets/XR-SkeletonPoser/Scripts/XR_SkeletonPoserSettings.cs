using UnityEditor;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XR_SkeletonPoserSettings : ScriptableObject
    {

        private static XR_SkeletonPoserSettings _instance;

        public static XR_SkeletonPoserSettings Instance
        {
            get
            {
                LoadInstance();

                return _instance;
            }
        }

        private static void LoadInstance()
        {
            if (_instance != null) return;
            
            _instance = Resources.Load<XR_SkeletonPoserSettings>("SkeletonPoserSettings");
            
            if (_instance != null) return;
            
            _instance = CreateInstance<XR_SkeletonPoserSettings>();
                    
            if(!AssetDatabase.IsValidFolder("Assets/XRPoses/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/XRPoses", "Resources");
            }
                    
            AssetDatabase.CreateAsset(_instance, "Assets/XRPoses/Resources/SkeletonPoserSettings.asset");
            AssetDatabase.SaveAssets();
            
            SetDefaultValues();
        }

        private static void SetDefaultValues()
        {
            _instance.showLeftHandColour = Color.white;
            _instance.hideLeftHandColour = Color.white;

            _instance.showRightHandColour = Color.white;
            _instance.hideRightHandColour = Color.white;
            
            _instance.loadPoseColour = new Color32(160, 255, 66, 40);
            _instance.savePoseColour = new Color32(160, 255, 66, 40);
            
            _instance.resetPoseColour = new Color32(255, 101, 101, 96);
            _instance.resetToReferencePoseColour = new Color32(255, 101, 101, 96);
            
            AssetDatabase.SaveAssets();
        }
        
        // Variables

        [Space]
        
        [Tooltip("Left hand model to be spawned in as a left preview. Should be the same as your in game left hand")]
        public GameObject leftHand;
        
        [Tooltip("Left hand model to be spawned in as a left preview. Should be the same as your in game left hand")] 
        public GameObject rightHand;

        [Space] 
        
        public XR_SkeletonPose referencePose;

        [Space]
        
        public Font guiFont;

        public Color showLeftHandColour;
        public Color showRightHandColour;
        public Color hideLeftHandColour;
        public Color hideRightHandColour;
        
        public Color loadPoseColour;
        public Color savePoseColour;
        public Color resetPoseColour;
        public Color resetToReferencePoseColour;

    }    
}
