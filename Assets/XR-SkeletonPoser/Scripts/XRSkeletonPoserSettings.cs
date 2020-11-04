using UnityEditor;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XRSkeletonPoserSettings : ScriptableObject
    {

        private static XRSkeletonPoserSettings _instance;

        public static XRSkeletonPoserSettings Instance
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
            
            _instance = Resources.Load<XRSkeletonPoserSettings>("SkeletonPoserSettings");
            
            if (_instance != null) return;
            
            _instance = CreateInstance<XRSkeletonPoserSettings>();
                    
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
            _instance.defaultExpandPreview = true;
            // _instance.lerpFingersOnSelect = true;

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
        
        public XRSkeletonPose referencePose;

        [Tooltip("Expand all children gameobjects recursively when the show hand button is pressed")]
        public bool defaultExpandPreview;
        
        [Space]
        
        [Tooltip("Lerp fingers on select for a smooth transition between the free pose to the object")]
        public bool lerpFingersOnSelect;
        
        public float fingerLerpTime = 3f;
        
        [Space]
        
        public Font guiFont;

        [Space]
        public Color showLeftHandColour;
        [Space]
        public Color hideLeftHandColour;
        [Space]
        public Color showRightHandColour;
        [Space]
        public Color hideRightHandColour;
        
        [Space]
        
        public Color loadPoseColour;
        public Color savePoseColour;
        public Color resetPoseColour;
        public Color resetToReferencePoseColour;

    }    
}
