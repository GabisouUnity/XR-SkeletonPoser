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
            
            _instance.useBoneGizmos = true;
            _instance.boneGizmoColour = Color.yellow;
            _instance.boneGizmoSize = 0.01f;
            _instance.boneGizmoPickSize = 0.01f;

            _instance.showLeftHandColour = Color.white;
            _instance.hideLeftHandColour = Color.white;

            _instance.showRightHandColour = Color.white;
            _instance.hideRightHandColour = Color.white;
            
            _instance.showBothHandsColour = Color.white;
            _instance.hideBothHandsColour = Color.white;
            
            _instance.loadPoseColour = new Color32(160, 255, 66, 40);
            _instance.savePoseColour = new Color32(160, 255, 66, 40);
            
            _instance.resetPoseColour = new Color32(255, 101, 101, 96);
            
            AssetDatabase.SaveAssets();
        }
        
        // Variables

        [Space]
        
        [Tooltip("Left hand model to be spawned in as a left preview. Should be the same as your in game left hand")]
        public GameObject leftHand;
        
        [Tooltip("Left hand model to be spawned in as a left preview. Should be the same as your in game left hand")] 
        public GameObject rightHand;

        [Space]
        
        public string poseNamePrefix;
        
        [Space]
        
        [Tooltip("Expand all children gameobjects recursively when the show hand button is pressed")]
        public bool defaultExpandPreview;

        [Space]
        
        public bool useBoneGizmos;
        public Color boneGizmoColour;
        public float boneGizmoSize;
        public float boneGizmoPickSize;
        
        public string[] ignoredBoneKeywords;
        
        [Space]
        
        public Font guiFont;

        [Space]
        
        public Color showLeftHandColour;
        public Color hideLeftHandColour;
        
        [Space]
        
        public Color showRightHandColour;
        public Color hideRightHandColour;
        
        [Space]
        
        public Color showBothHandsColour;
        public Color hideBothHandsColour;
        
        [Space]
        
        public Color loadPoseColour;
        public Color savePoseColour;
        public Color resetPoseColour;

    }    
}
