﻿using System.Linq;

using UnityEditor;
using UnityEngine;
using yellowyears.SkeletonPoser;

namespace yellowyears.SkeletonPoser
{
    [CustomEditor(typeof(XRSkeletonPoser))]
    public class XRSkeletonPoserEditor : Editor
    {
        private XRSkeletonPoser _poser = null;
        
        private XRSkeletonPose _defaultPose = null;

        private SerializedProperty _propertyMainPose = null;
        // private SerializedProperty _propertySecondaryPose = null;
        // private SerializedProperty _propertySelectedPose = null;
        // private SerializedProperty _propertyActivePoseEnum = null;
        // private SerializedProperty _propertyBlendBehaviour = null;
        // private SerializedProperty _propertyBlendWasCreated = null;
        // private SerializedProperty _propertyBlendInput = null;

        private SerializedProperty _propertyShowPoses = null;
        private SerializedProperty _propertyShowPoseEditor = null;
        // private SerializedProperty _propertyShowBlendEditor = null;
        private SerializedProperty _propertyScale = null;

        private bool _updateHands = false;
        
        private SerializedProperty _propertyShowLeft = null;
        private SerializedProperty _propertyTempLeft = null;

        private SerializedProperty _propertyShowRight = null;
        private SerializedProperty _propertyTempRight = null;
        
        private void OnEnable()
        {
            _poser = (XRSkeletonPoser) target;

            _defaultPose = CreateInstance<XRSkeletonPose>();
            GetDefaultPose();

            _propertyMainPose = serializedObject.FindProperty("mainPose");
            // _propertySecondaryPose = serializedObject.FindProperty("secondaryPose");
            // _propertySelectedPose = serializedObject.FindProperty("selectedPose");
            // _propertyActivePoseEnum = serializedObject.FindProperty("activePoseEnum");
            // _propertyBlendBehaviour = serializedObject.FindProperty("blendBehaviour");
            // _propertyBlendWasCreated = serializedObject.FindProperty("blendWasCreated");
            // _propertyBlendInput = serializedObject.FindProperty("blendInput");
            
            _propertyShowPoses = serializedObject.FindProperty("showPoses");
            _propertyShowPoseEditor = serializedObject.FindProperty("showPoseEditor");
            // _propertyShowBlendEditor = serializedObject.FindProperty("showBlendEditor");
            _propertyScale = serializedObject.FindProperty("scale");
            
            _propertyShowLeft = serializedObject.FindProperty("showLeft");
            _propertyTempLeft = serializedObject.FindProperty("tempLeft");

            _propertyShowRight = serializedObject.FindProperty("showRight");
            _propertyTempRight = serializedObject.FindProperty("tempRight");
        }
        
        private void GetDefaultPose()
        {
            // Get default values from the hand prefab.
            
            _defaultPose.leftBonePositions = _poser.GetBonePositions(XRSkeletonPoserSettings.Instance.leftHand);
            _defaultPose.leftBoneRotations = _poser.GetBoneRotations(XRSkeletonPoserSettings.Instance.leftHand);

            _defaultPose.rightBonePositions = _poser.GetBonePositions(XRSkeletonPoserSettings.Instance.rightHand);
            _defaultPose.rightBoneRotations = _poser.GetBoneRotations(XRSkeletonPoserSettings.Instance.rightHand);
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            DrawAdditionalPoses();
            
            DrawPoseEditor();
            
            // DrawBlendEditor();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAdditionalPoses()
        {
            if (Application.isPlaying) return;
            
            EditorGUILayout.BeginVertical("box");
            
            _propertyShowPoses.boolValue =
                IndentedFoldoutHeader(_propertyShowPoses.boolValue, "Pose Manager");

            if (_propertyShowPoses.boolValue)
            {

                EditorGUILayout.BeginHorizontal();
                // EditorGUI.BeginDisabledGroup(_propertySelectedPose.name != _propertyMainPose.name);

                if (GUILayout.Button(_propertyMainPose.name + " (MAIN)"))
                {
                    // _propertySelectedPose = _propertyMainPose;
                    LoadPose();
                }
                
                // EditorGUI.EndDisabledGroup();
                // EditorGUI.BeginDisabledGroup(_propertySelectedPose.name != _propertySecondaryPose.name);

                // if(GUILayout.Button(_propertySecondaryPose.name + " (Secondary)"))
                // {
                //     _propertySelectedPose = _propertySecondaryPose;
                //     LoadPose();
                // }
                
                // EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
                
                // EditorGUILayout.LabelField(_propertySelectedPose.name);
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawPoseEditor()
        {
            if (Application.isPlaying) // Check if in playmode and skip if not.
            {
                EditorGUILayout.LabelField("Cannot modify pose while in play mode.");
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
        
                var poserSettings = XRSkeletonPoserSettings.Instance;
                
                if (poserSettings.guiFont != null)
                {
                    GUI.skin.font = poserSettings.guiFont;
                }
        
                // Create new instance of XRSkeletonPose, this is the one that is edited
                var newPose = CreateInstance<XRSkeletonPose>();
        
                // _propertyShowPoseEditor.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(_propertyShowPoseEditor.boolValue, "Show Pose Editor");
                // _propertyShowPoseEditor.boolValue = EditorGUILayout.Foldout(_propertyShowPoseEditor.boolValue, "Show Pose Editor");
                
                _propertyShowPoseEditor.boolValue =
                    IndentedFoldoutHeader(_propertyShowPoseEditor.boolValue, "Pose Editor");
        
                if (_propertyShowPoseEditor.boolValue)
                {
                   EditorGUILayout.BeginHorizontal(); EditorGUI.BeginDisabledGroup(XRSkeletonPoserSettings.Instance.leftHand == null);
                   
                   if (!_propertyShowLeft.boolValue)
                   {
                       GUI.backgroundColor = poserSettings.showLeftHandColour;
                       
                       if (GUILayout.Button("Show Left Hand"))
                       {
                           var leftGameObject = _poser.ShowLeftPreview();
        
                           leftGameObject.transform.parent = null;
                           leftGameObject.transform.localScale = Vector3.one * _propertyScale.floatValue;
                           leftGameObject.transform.parent = _poser.transform;
                           leftGameObject.transform.localPosition = Vector3.zero;
                           leftGameObject.transform.localRotation = Quaternion.identity;
                           
                           _propertyShowLeft.boolValue = true;
        
                           _propertyTempLeft.objectReferenceValue = leftGameObject;
                       }
        
                   }
                   else
                   {
                       GUI.backgroundColor = poserSettings.hideLeftHandColour;
                       
                       if (GUILayout.Button("Hide Left Hand"))
                       {
                           _poser.DestroyLeftPreview(_propertyTempLeft.objectReferenceValue as GameObject);
                           _propertyShowLeft.boolValue = false;
                       }
                   }
                   
                   EditorGUI.EndDisabledGroup();
                   
                   // Preview Right Button
                   
                   EditorGUI.BeginDisabledGroup(XRSkeletonPoserSettings.Instance.rightHand == null);
                   
                   if (!_propertyShowRight.boolValue)
                   {
                       GUI.backgroundColor = poserSettings.showRightHandColour;
                       
                       if (GUILayout.Button("Show Right Hand"))
                       {
                           var rightGameObject = _poser.ShowRightPreview();
        
                           rightGameObject.transform.parent = null;
                           rightGameObject.transform.localScale = Vector3.one * _propertyScale.floatValue;
                           rightGameObject.transform.parent = _poser.transform;
                           rightGameObject.transform.localPosition = Vector3.zero;
                           rightGameObject.transform.localRotation = Quaternion.identity;
                           
                           _propertyShowRight.boolValue = true;
        
                           _propertyTempRight.objectReferenceValue = rightGameObject;
                       }
                   }
                   else
                   {
                       GUI.backgroundColor = poserSettings.hideRightHandColour;
                       
                       if (GUILayout.Button("Hide Right Hand"))
                       {
                           _poser.DestroyRightPreview(_propertyTempRight.objectReferenceValue as GameObject);
                           _propertyShowRight.boolValue = false;
                       }
                   }
        
                   EditorGUI.EndDisabledGroup(); EditorGUILayout.EndHorizontal();
                   
                   EditorGUILayout.BeginHorizontal();
                   
                   // Grey it out if hands aren't active and there is no loaded pose
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false);
        
                   // rgba(160, 255, 66, 0.4)
                   // GUI.backgroundColor = new Color32(160, 255, 66, 100);
                   // GUI.backgroundColor = Color.green;
        
                   GUI.backgroundColor = poserSettings.loadPoseColour;
                   
                   if (GUILayout.Button("Load Pose", "button"))
                   {
                       LoadPose();
                   }
                   
                   EditorGUI.EndDisabledGroup();
                   
                   // Grey it out if hands aren't active
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false || _propertyShowRight.boolValue == false);
        
                   GUI.backgroundColor = poserSettings.savePoseColour;
                   
                   if (GUILayout.Button("Save Pose", "button"))
                   {
                       SavePose(newPose);
                   }
                   
                   EditorGUILayout.EndHorizontal();
                   
                   EditorGUI.EndDisabledGroup();
        
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false);
        
                   EditorGUILayout.BeginHorizontal();
                   
                   GUI.backgroundColor = poserSettings.resetPoseColour;
                   
                   if (GUILayout.Button("Reset Pose", "button"))
                   {
                       // Make sure we warn the user before they reset their pose
                       if (EditorUtility.DisplayDialog("Reset Pose?",
                           "Are you sure you want to do this? You will lose your pose on this object!", "Yes", "No"))
                       {
                           // They are sure, reset pose
                           ResetPose(); // Reset new pose instance to default pose
                       }
                   }
                   
                   EditorGUI.EndDisabledGroup();
                   
                   EditorGUI.BeginDisabledGroup(_propertyShowLeft.boolValue == false && _propertyShowRight.boolValue == false || XRSkeletonPoserSettings.Instance.referencePose == null);
        
                   GUI.backgroundColor = poserSettings.resetToReferencePoseColour;
                   
                   if (GUILayout.Button("Reset To Reference Pose", "button"))
                   {
                       // Make sure we warn the user before they reset their pose
                       if (EditorUtility.DisplayDialog("Reset Pose?",
                           "Are you sure you want to do this? You will lose your pose on this object!", "Yes", "No"))
                       {
                           // They are sure, reset pose
                           ResetToReferencePose(); // Reset new pose instance to reference pose
                       }
                   }
        
                   EditorGUI.EndDisabledGroup();
                   EditorGUILayout.EndHorizontal();
                }
        
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                GUI.backgroundColor = Color.white;
                EditorGUIUtility.labelWidth = 60;
                _propertyScale.floatValue = EditorGUILayout.FloatField("Scale", _propertyScale.floatValue);
                if (_propertyScale.floatValue <= 0) _propertyScale.floatValue = 1;
                EditorGUIUtility.labelWidth = 0;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
        
                var leftHand = _propertyTempLeft.objectReferenceValue as GameObject;
                var rightHand = _propertyTempRight.objectReferenceValue as GameObject;
                
                if (EditorGUI.EndChangeCheck())
                {
                    // Value has changed, update hands
                    _updateHands = true;
        
                    if (leftHand != null) UpdateHandScale(leftHand, _propertyScale.floatValue);
                    if (rightHand != null) UpdateHandScale(rightHand, _propertyScale.floatValue);
                }
                
                EditorGUILayout.EndFoldoutHeaderGroup();
        
                EditorGUILayout.EndVertical();
            }
        }

        // private void DrawBlendEditor()
        // {
        //     if (Application.isPlaying)
        //     {
        //         EditorGUILayout.LabelField("Cannot modify blends in playmode.");
        //     }
        //     else
        //     {
        //         EditorGUILayout.BeginVertical("box");
        //         
        //         _propertyShowBlendEditor.boolValue =
        //             IndentedFoldoutHeader(_propertyShowBlendEditor.boolValue, "Blend Editor");
        //
        //         if (_propertyShowBlendEditor.boolValue)
        //         {
        //             var blender = _propertyBlendBehaviour;
        //             var enabled = blender.FindPropertyRelative("enabled");
        //             var blendName = blender.FindPropertyRelative("blendName");
        //             var from = blender.FindPropertyRelative("from");
        //             var to = blender.FindPropertyRelative("to");
        //             
        //             from.objectReferenceValue = _propertyMainPose.objectReferenceValue as XRSkeletonPose;
        //             to.objectReferenceValue = _propertySecondaryPose.objectReferenceValue as XRSkeletonPose;
        //
        //             EditorGUI.BeginDisabledGroup(!_poser.FetchSecondaryPose() || !_poser.FetchMainPose());
        //             
        //             if (GUILayout.Button("Create Blend", "button"))
        //             {
        //                 if (!_propertyBlendWasCreated.boolValue)
        //                 {
        //                     // Create New
        //                     blendName.stringValue = "New Blend";
        //                     
        //                     from.objectReferenceValue = _propertyMainPose.objectReferenceValue as XRSkeletonPose;
        //                     to.objectReferenceValue = _propertySecondaryPose.objectReferenceValue as XRSkeletonPose;
        //                     
        //                     _propertyBlendWasCreated.boolValue = true;
        //                 }
        //                 else
        //                 {
        //                     EditorUtility.DisplayDialog("Error!",
        //                         "You already have a blend active! You cannot create a new one.", "ok");
        //                 }
        //             }
        //
        //             enabled.boolValue = IndentedFoldoutHeader(enabled.boolValue, blendName.stringValue);
        //
        //             if (enabled.boolValue)
        //             {
        //                 
        //                 EditorGUI.BeginChangeCheck();                        
        //                 EditorGUILayout.PropertyField(blendName);
        //                 if (EditorGUI.EndChangeCheck())
        //                 {
        //                     if (string.IsNullOrEmpty(blendName.stringValue)) blendName.stringValue = "New Blend";
        //                 }
        //
        //                 EditorGUILayout.Space();
        //
        //                 var mainPose = _propertyMainPose.objectReferenceValue as XRSkeletonPose;
        //                 var secondaryPose = _propertySecondaryPose.objectReferenceValue as XRSkeletonPose;
        //
        //                 if (!(mainPose is null)) EditorGUILayout.LabelField("Primary Pose: " + mainPose.name);
        //                 if (!(secondaryPose is null)) EditorGUILayout.LabelField("Secondary Pose: " + secondaryPose.name);
        //                 
        //                 // EditorGUI.BeginChangeCheck();
        //
        //                 EditorGUIUtility.labelWidth = 70;
        //                 EditorGUILayout.PropertyField(_propertyBlendInput);
        //
        //                 // if (EditorGUI.EndChangeCheck())
        //             }
        //         }
        //
        //         EditorGUILayout.EndVertical();
        //     }
        // }
        
        private bool IndentedFoldoutHeader(bool fold, string text, int indent = 1)
        {
            // Taken from the steamvr unity plugin code, just looks too good :p
            
            GUILayout.BeginHorizontal();
            var boldFoldoutStyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
            GUILayout.Space(14f * indent);
            fold = EditorGUILayout.Foldout(fold, text, boldFoldoutStyle);
            GUILayout.EndHorizontal();
            return fold;
        }

        private void UpdateHandScale(GameObject obj, float scale)
        {
            if (_updateHands == false) return;
            
            obj.transform.localScale = Vector3.one * scale;

            _updateHands = false;
        }

        #region Copy To Right & Left

                // private void CopyToRight(XRSkeletonPose source, XRSkeletonPose destination)
        // {
        //     destination.leftBonePositions = source.leftBonePositions;
        //     destination.leftBoneRotations = source.leftBoneRotations;
        //     
        //     // destination.rightBoneRotations = source.rightBoneRotations;
        //     // destination.rightBonePositions = source.rightBonePositions;
        //     
        //     for (int i = 0; i < destination.rightBoneRotations.Length; i++)
        //     {
        //         // destination.rightBoneRotations[i] = source.leftBoneRotations[i];
        //         // destination.rightBoneRotations[i] = _poser.MirrorBoneRotation(source.leftBoneRotations[i]);
        //         
        //         var reflectedRotation = new Quaternion(-source.rightBoneRotations[i].y, source.rightBoneRotations[i].y, source.rightBoneRotations[i].z, -source.rightBoneRotations[i].w);
        //
        //         destination.rightBoneRotations[i] = reflectedRotation;
        //
        //         // EditorUtility.DisplayProgressBar("Copying...", "Copying right hand pose", i / destination.leftBoneRotations.Length / 2f);
        //     }
        //     
        //     // Save it (if saved using SavePose() it overwrites the data for the assigned bone pos and rot)
        //
        //     var copy = CreateInstance<XRSkeletonPose>();
        //
        //     copy.leftBonePositions = destination.leftBonePositions;
        //     copy.leftBoneRotations = destination.leftBoneRotations;
        //
        //     // copy.rightBonePositions = destination.rightBonePositions;
        //     copy.rightBoneRotations = destination.rightBoneRotations;
        //     
        //     if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
        //     {
        //         // Folder doesn't exist, create new
        //         AssetDatabase.CreateFolder("Assets", "XRPoses");
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //     }
        //     else
        //     {
        //         // Folder exists
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //     }
        //
        //     _poser.selectedPose = copy;
        //     
        //     // Load the pose onto the right hand only
        //     
        //     var rightHandObject = _propertyTempRight.objectReferenceValue as GameObject;
        //     
        //     if (rightHandObject == null) return;
        //     
        //     var rightTransforms = rightHandObject.GetComponentsInChildren<Transform>().ToArray();
        //
        //     // for (int i = 0; i < copy.rightBonePositions.Length; i++)
        //     // {
        //     //     rightTransforms[i].localPosition = copy.rightBonePositions[i];
        //     // }
        //     
        //     for (int i = 0; i < copy.rightBoneRotations.Length; i++)
        //     {
        //         rightTransforms[i].localRotation = copy.rightBoneRotations[i];
        //     }
        //     
        //     // EditorUtility.ClearProgressBar();
        // }

        // private void CopyToLeft()
        // {
        //     // Copy right pose data to left
        // }

        #endregion

        private void SavePose(XRSkeletonPose inputPose)
        {
            // Todo: Only overwrite the data from the active hand(s). Although it might not be possible?

            // Create copy of pose to stop error whilst saving ("Object already exists")
            var copy = Instantiate(inputPose);

            // var main = _poser.FetchMainPose();
            // var secondary = _poser.FetchSecondaryPose();
            
            copy.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
            copy.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);

            copy.rightBonePositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
            copy.rightBoneRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);

            // Set pose to new pose data to avoid the need for reassignment after saving
            // _poser.selectedPose = copy;

            if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
            {
                // Folder doesn't exist, create new
                AssetDatabase.CreateFolder("Assets", "XRPoses");
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
            else
            {
                // Folder exists
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
        }

        // private void SaveMainPose()
        // {
        //     var main = _poser.FetchMainPose();
        //     
        //     var copy = Instantiate(main);
        //     
        //     copy.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
        //     copy.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
        //
        //     copy.rightBonePositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
        //     copy.rightBoneRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);
        //
        //     // Don't overwrite secondary stuff
        //
        //     if (main.leftBlendPositions != null)
        //     {
        //         copy.leftBlendPositions = main.leftBlendPositions;
        //     }
        //
        //     if (main.leftBlendRotations != null)
        //     {
        //         copy.leftBlendRotations = main.leftBlendRotations;
        //     }
        //
        //     if (copy.rightBlendPositions != null)
        //     {
        //         copy.rightBlendPositions = main.rightBlendPositions;
        //     }
        //
        //     if (copy.rightBlendRotations != null)
        //     {
        //         copy.rightBlendRotations = main.rightBlendRotations;
        //     }
        //     
        //     // Set pose to new pose data to avoid the need for reassignment after saving
        //     _poser.selectedPose = copy;
        //
        //     if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
        //     {
        //         // Folder doesn't exist, create new
        //         AssetDatabase.CreateFolder("Assets", "XRPoses");
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //     }
        //     else
        //     {
        //         // Folder exists
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //     }
        // }
        //
        // private void SaveSecondaryPose()
        // {
        //     var secondary = _poser.FetchSecondaryPose();
        //     
        //     var copy = Instantiate(secondary);
        //     
        //     copy.leftBlendPositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
        //     copy.leftBlendRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
        //
        //     copy.rightBlendPositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
        //     copy.rightBlendRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);
        //
        //     // Don't overwrite main stuff
        //
        //     if (secondary.leftBonePositions != null)
        //     {
        //         copy.leftBonePositions = secondary.leftBonePositions;
        //     }
        //
        //     if (secondary.leftBoneRotations != null)
        //     {
        //         copy.leftBoneRotations = secondary.leftBoneRotations;
        //     }
        //
        //     if (copy.rightBonePositions != null)
        //     {
        //         copy.rightBonePositions = secondary.rightBonePositions;
        //     }
        //
        //     if (copy.rightBoneRotations != null)
        //     {
        //         copy.rightBoneRotations = secondary.rightBoneRotations;
        //     }
        //     
        //     // Set pose to new pose data to avoid the need for reassignment after saving
        //     _poser.selectedPose = copy;
        //
        //     if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
        //     {
        //         // Folder doesn't exist, create new
        //         AssetDatabase.CreateFolder("Assets", "XRPoses");
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //     }
        //     else
        //     {
        //         // Folder exists
        //         AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
        //     }
        // }

        private void ResetPose()
        {
            // Set pose to new pose data to avoid the need for reassignment after saving the file
            
            // Create copy of pose to stop error whilst saving ("Object already exists")
            var copy = CreateInstance<XRSkeletonPose>();

            copy.leftBonePositions = _defaultPose.leftBonePositions;
            copy.leftBoneRotations = _defaultPose.leftBoneRotations;

            copy.rightBonePositions = _defaultPose.rightBonePositions;
            copy.rightBoneRotations = _defaultPose.rightBoneRotations;
            
            // _poser.selectedPose = copy;

            LoadPose(); // Load pose automatically for convenience
            
            // Save and overwrite
            if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
            {
                // Folder doesn't exist, create new
                AssetDatabase.CreateFolder("Assets", "XRPoses");

                // Overwrite the pose with a default pose
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
            else
            {
                // Folder exists

                // Overwrite the pose with a default pose

                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
                // AssetDatabase.SaveAssets();
            }
        }
        
        private void ResetToReferencePose()
        {
            // Set pose to new pose data to avoid the need for reassignment after saving the file
            
            // Create copy of pose to stop error whilst saving ("Object already exists")
            var copy = CreateInstance<XRSkeletonPose>();

            var referencePose = XRSkeletonPoserSettings.Instance.referencePose;
            
            copy.leftBonePositions = referencePose.leftBonePositions;
            copy.leftBoneRotations = referencePose.leftBoneRotations;

            copy.rightBonePositions = referencePose.rightBonePositions;
            copy.rightBoneRotations = referencePose.rightBoneRotations;
            
            // _poser.selectedPose = copy;

            LoadPose(); // Load pose automatically for convenience
            
            // Save and overwrite
            if (!AssetDatabase.IsValidFolder("Assets/XRPoses"))
            {
                // Folder doesn't exist, create new
                AssetDatabase.CreateFolder("Assets", "XRPoses");

                // Overwrite the pose with a default pose
                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
            }
            else
            {
                // Folder exists

                // Overwrite the pose with a default pose

                AssetDatabase.CreateAsset(copy, $"Assets/XRPoses/{_poser.gameObject.name}.asset");
                // AssetDatabase.SaveAssets();
            }
        }

        private XRSkeletonPose GetMainPose(XRSkeletonPose inputPose)
        {
            // Get pose without saving
            
            inputPose.leftBonePositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
            inputPose.leftBoneRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);
        
            inputPose.rightBonePositions = _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
            inputPose.rightBoneRotations = _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);

            // Get input XRPose instance and return it but full from scene
            return inputPose;
        }

        private XRSkeletonPose GetSecondaryPose(XRSkeletonPose inputPose)
        {
            inputPose.leftBlendPositions = _poser.GetBonePositions(_propertyTempLeft.objectReferenceValue as GameObject);
            inputPose.leftBlendRotations = _poser.GetBoneRotations(_propertyTempLeft.objectReferenceValue as GameObject);

            inputPose.rightBlendPositions =
                _poser.GetBonePositions(_propertyTempRight.objectReferenceValue as GameObject);
            inputPose.rightBlendRotations =
                _poser.GetBoneRotations(_propertyTempRight.objectReferenceValue as GameObject);

            return inputPose;
        }

        private XRSkeletonPose RemoveSecondaryPose(XRSkeletonPose inputPose)
        {
            inputPose.leftBlendPositions = null;
            inputPose.leftBlendRotations = null;

            inputPose.rightBlendPositions = null;
            inputPose.rightBlendRotations = null;

            return inputPose;
        }

        private void LoadPose()
        {
            var loadedPose = _poser.FetchMainPose();

            var leftHandObject = _propertyTempLeft.objectReferenceValue as GameObject;
            var rightHandObject = _propertyTempRight.objectReferenceValue as GameObject;
            
            var leftBonePositions = loadedPose.leftBonePositions;
            var leftBoneRotations = loadedPose.leftBoneRotations;

            var rightBonePositions = loadedPose.rightBonePositions;
            var rightBoneRotations = loadedPose.rightBoneRotations;

            if (leftHandObject == null) return;

            var leftTransforms = leftHandObject.GetComponentsInChildren<Transform>().ToArray();

            // Set left values to loaded pose
            for (int i = 0; i < leftBonePositions.Length; i++)
            {
                leftTransforms[i].localPosition = leftBonePositions[i];
            }

            for (int i = 0; i < leftBoneRotations.Length; i++)
            {
                leftTransforms[i].localRotation = leftBoneRotations[i];
            }

            if (rightHandObject == null) return;
            
            var rightTransforms = rightHandObject.GetComponentsInChildren<Transform>().ToArray();

            for (int i = 0; i < rightBonePositions.Length; i++)
            {
                rightTransforms[i].localPosition = rightBonePositions[i];
            }

            for (int i = 0; i < rightBoneRotations.Length; i++)
            {
                rightTransforms[i].localRotation = rightBoneRotations[i];
            }
            
        }
    }    
}