# XR-SkeletonPoser
Skeleton poser tool for the Unity XR Interaction Toolkit
A skeleton poser is useful to author poses in the scene view easily, which can scale better than animations, where you would have to create animations for each object in the project.

[![License](https://img.shields.io/badge/license-MIT-yellow)](https://github.com/yellowyears/XR-SkeletonPoser/blob/master/LICENSE) [![Discord](https://img.shields.io/badge/discord-%20-blue)](https://discord.gg/Y6GpkRF)

![Preview](https://raw.githubusercontent.com/yellowyears/XR-SkeletonPoser/master/img/xr-skeletonposer-preview.gif)

## Installation
Check the releases tab on this repository and download the .unitypackage for the latest release. Make sure you have the XR Interaction Toolkit installed.

## Setup

[Video Tutorial](https://youtu.be/M7WA779XA0E)

### Pre-setup

To do some prep work for working with the skeleton poser you will need to add some components and create some gameobjects on your controllers.

1. Download the Unity package from the releases tab, or directly download the repository and take the "Scripts" folder out.
2. Setup your XR Rig, and replace the XRDirectInteractor with the XR_SkeletonPoseInteractor component.
3. Add your hand prefab (the same one you want to use the skeleton poser with!) as a child of the controller. This hand prefab should have an "AttachTransform" empty gameobject as the **last** child gameobject of the hand.
4. On your XR_SkeletonPoseInteractor component you need to assign the AttachTransform gameobject from the hand onto the interactor, then scroll down and you'll see the "HandObject" and "HandType" variables. You want to assign the HandObject variable as your hand prefab from earlier. Also make sure to set the HandType to be Left/Right depending on what hand it corresponds to.

### Object setup

1. Get a 3D object inside of your scene and add the XR_SkeletonPoser script to it. Also make sure to add an attachment point 
2. From here you can assign the hand preview prefabs and press the "Show Left Hand" and "Show Right Hand" buttons.
3. Morph the bones around your object and make sure your attachment point on the hand is in the correct position, basically where the object is on your pose.
4. Hit "Save Pose" and you will see a ScriptableObject gets saved under "Assets/XRPoses". This folder will contain all of your poses, but you don't have to worry about that other than assigning and loading poses.
5. Now that should be it all setup! Go into your game and pick up the object and your hand bones will morph to fit around the object, just as the pose you created is!

Finding this hard to understand? [Check out the video tutorial!](https://youtu.be/M7WA779XA0E)


# Troubleshooting

Send an [issue report](https://github.com/yellowyears/XR-SkeletonPoser/issues), contact me on [Discord](https://discord.gg/Y6GpkRF) 

## FAQ
**Question:** "Why do my hands change to the pose but my object doesn't get grabbed?"

**Answer:** This occurs when you don't assign an attach transform on your XRGrabInteractable.

**Q:** Where is the option to set the hand preview gameobjects?

**A:** As of update 1.1 this is inside of the SkeletonPoserSettings asset. You can find this in "Assets/XRPoses/Resources"

**Q:** Why are some buttons not available to click?

**A:** These are intentionally greyed out, generally when the option is not available under the current circumstances. For example, you cannot preview the left / right hand if it is not assigned inside of the SkeletonPoserSettings.
Therefore, to fix this you must assign the left and right preview gameobjects, and it will be available to press.

## Additional notes:

Want to take your hands to the next level? During my endeavours into VR physics I found [a method for physics based hands in VR](https://youtu.be/uG5aTsS5sNk)

The demo project uses the SteamVR gloves for demonstration, to clarify this is still intended for UnityXR Toolkit, but I just simply use the SteamVR gloves for demonstration.

# Thanks to..

All the folks who helped me get this up and running, and all the folks who use it. 