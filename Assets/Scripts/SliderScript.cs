using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour 
{
    public float CurrentValue;

    public float RangeBarWidth;
    public Transform NeedleHeadTarget;
    public Transform NeedleheadActual;
    public Transform RangeBar;
    public Transform Connector;

    private bool _wasShownLastFrame;
    public bool ShowSlider;

    void Update()
    {
        RangeBar.gameObject.SetActive(ShowSlider);
        NeedleHeadTarget.gameObject.SetActive(ShowSlider);
        Connector.gameObject.SetActive(ShowSlider);
        if (ShowSlider)
        {
            if(!_wasShownLastFrame)
            {
                EstablishSliderPlacement();
            }
            UpdateActiveSlider();
        }
        _wasShownLastFrame = ShowSlider;
    }

    private void UpdateActiveSlider()
    {
        Vector3 rangeBarStart = RangeBar.position - new Vector3(RangeBarWidth / 2, 0, 0);
        Vector3 rangeBarEnd = RangeBar.position - new Vector3(-RangeBarWidth / 2, 0, 0);

        float l2 = (rangeBarStart - rangeBarEnd).sqrMagnitude;
        float theDot = Vector3.Dot(NeedleHeadTarget.position - rangeBarStart, rangeBarEnd - rangeBarStart);
        NeedleheadActual.position = Vector3.Lerp(rangeBarStart, rangeBarEnd, CurrentValue);
        CurrentValue = Mathf.Max(0, Mathf.Min(1, theDot / l2));

        float connectorLength = (NeedleheadActual.position - NeedleHeadTarget.position).magnitude / 2;
        Connector.localScale = new Vector3(Connector.localScale.x, Connector.localScale.y, connectorLength);
        Connector.position = (NeedleheadActual.position + NeedleHeadTarget.position) / 2;
        Connector.LookAt(NeedleheadActual);
    }

    private void EstablishSliderPlacement()
    {
        RangeBar.transform.position = NeedleHeadTarget.position;
        RangeBar.transform.rotation = NeedleHeadTarget.rotation;
        NeedleheadActual.localPosition = Vector3.zero;
        float startOffset = RangeBarWidth * CurrentValue;
        float endOffset = RangeBarWidth * (1 - CurrentValue);
        RangeBar.position = new Vector3((startOffset + endOffset) / 2, 0, 0);
    }
}
