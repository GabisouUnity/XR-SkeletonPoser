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
            
            // Load settings, from a folder in Assets/XRPoses called "Resources"
            _instance = (XR_SkeletonPoserSettings)AssetDatabase.LoadAssetAtPath("Assets/XRPoses/Resources", typeof(XR_SkeletonPoserSettings));
        
            if (_instance != null) return;
                
            // If it still is null, then it does not exist
            
            _instance = CreateInstance<XR_SkeletonPoserSettings>();
            
            if(!AssetDatabase.IsValidFolder("Assets/XRPoses/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/XRPoses", "Resources");
            }
            
            AssetDatabase.CreateAsset(_instance, "Assets/XRPoses/Resources/SkeletonPoserSettings.asset");
            AssetDatabase.SaveAssets();
        }
        
        // Variables

        public GameObject leftHand;
        public GameObject rightHand;

    }    
}
