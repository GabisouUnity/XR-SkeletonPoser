using UnityEditor;
using UnityEngine;

namespace yellowyears.SkeletonPoser
{
    public class XR_SkeletonPoserSettings : ScriptableObject
    {

        private static XR_SkeletonPoserSettings _instance;

        public static XR_SkeletonPoserSettings instance
        {
            get
            {
                LoadInstance();

                return instance;
            }
        }

        private static void LoadInstance()
        {
            if (_instance != null) return;
            
            // Load settings, from a folder in Assets/XRPoses called "Resources"
            _instance = Resources.Load<XR_SkeletonPoserSettings>("XRPoses");

            if (_instance != null) return;
                
            // If it still is null, then it does not exist

            _instance = CreateInstance<XR_SkeletonPoserSettings>();
                    
            AssetDatabase.CreateAsset(_instance, "Assets/XRPoses/Resources/SkeletonPoserSettings.asset");
            AssetDatabase.SaveAssets();
        }
        
    }    
}
