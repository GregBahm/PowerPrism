using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveControls : MonoBehaviour
{
    public SteamVR_TrackedObject LeftHand;
    public SteamVR_TrackedObject RightHand;

    public SceneManagerScript ManagerScript;
    public TransformationScript TransformScript;
    public SelectionScript SelectScript;
    public TimelineControlScript TimelineScript;

	void Update ()
    {
        if(!LeftHand.isValid || !RightHand.isValid)
        {
            Debug.Log("Waiting for controllers.");
            return;
        }
        SteamVR_Controller.Device leftHandDevice = SteamVR_Controller.Input((int)LeftHand.index);
        SteamVR_Controller.Device rightHandDevice = SteamVR_Controller.Input((int)RightHand.index);

        SelectScript.Selecting = leftHandDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
        TimelineScript.ShowTimelineControl = rightHandDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
        TransformScript.Mode = GetMode(leftHandDevice, rightHandDevice);

        if(leftHandDevice.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            ManagerScript.ToggleRecording();
        }

        if(rightHandDevice.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            ManagerScript.Save();
        }

    }

    private TransformationScript.ControlMode GetMode(SteamVR_Controller.Device leftHandDevice, SteamVR_Controller.Device rightHandDevice)
    {
        bool leftGrip = leftHandDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip);
        bool rightGrip = rightHandDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip);
        if(leftGrip && rightGrip)
        {
            return TransformationScript.ControlMode.Both;
        }
        if(leftGrip)
        {
            return TransformationScript.ControlMode.ControlA;
        }
        if(rightGrip)
        {
            return TransformationScript.ControlMode.ControlB;
        }
        return TransformationScript.ControlMode.None;
    }
}
