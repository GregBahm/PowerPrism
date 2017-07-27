using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelectionScript))]
public class TransformationScript : MonoBehaviour 
{
    private SelectionScript selection;

    private Transform lastTarget;
    public Transform Target;
    
    public Transform ControlA;
    public Transform ControlB;

    private ControlMode lastMode;
    public ControlMode Mode;

    private Transform controlAverage;
    private float initialHandDist;

    private void Start()
    {
        selection = GetComponent<SelectionScript>();
        GameObject controlAverage = new GameObject("Control Average");
        this.controlAverage = controlAverage.transform;
    }

    private void Update()
    {
        Transform newTarget = GetTarget();
        if (lastTarget != newTarget && selection.CurrentSelection != null)
        {
            selection.CurrentSelection.SetParent(newTarget, true);
        }
        UpdateHandAverage();
        lastTarget = newTarget;
        lastMode = Mode;
    }

    private Transform GetTarget()
    {
        switch (Mode)
        {
            case ControlMode.None:
                return null;
            case ControlMode.ControlA:
                return ControlA;
            case ControlMode.ControlB:
                return ControlB;
            case ControlMode.Both:
            default:
                return controlAverage;
        }
    }

    private void UpdateHandAverage()
    {
        controlAverage.position = Vector3.Lerp(ControlA.position, ControlB.position, .5f);
        controlAverage.rotation = Quaternion.Lerp(ControlA.rotation, ControlB.rotation, .5f);
        controlAverage.LookAt(ControlA.position, controlAverage.up);
        UpdateScaling();
    }

    private void UpdateScaling()
    {
        if (Mode != ControlMode.Both)
        {
            controlAverage.localScale = Vector3.one;
        }
        else
        {
            float handDist = (ControlA.position - ControlB.position).magnitude;
            if (lastMode != ControlMode.Both)
            {
                initialHandDist = handDist;
            }
            else
            {
                float newScale = handDist / initialHandDist;
                controlAverage.localScale = new Vector3(newScale, newScale, newScale);
            }
        }
    }

    public enum ControlMode
    {
        None,
        ControlA,
        ControlB,
        Both
    }
}
