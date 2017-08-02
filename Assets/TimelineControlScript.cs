using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineControlScript : MonoBehaviour
{
    private float NormalizedTime
    {
        get
        {
            if (MaxTime < float.Epsilon)
            {
                return 0;
            }
            return CurrentTime / MaxTime;
        }
    }

    public float CurrentTime;

    public float MaxTime;
    public Transform Needlehead;

    public bool Play;
    public bool ShowTimelineControl;

    private float _timeUiScale;

    public GameObject MainTimelineControl;
    public Transform CurrentTimeUi;
    public Transform TimeStartUi;
    public Transform TimeEndUi;
    public Transform TimeContainerUi;
    public Transform TimeConnectorUi;

    void Start()
    {
        _timeUiScale = (TimeStartUi.position - TimeEndUi.position).magnitude;
    }
    void Update()
    {
        if (Play)
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime > MaxTime)
            {
                MaxTime = CurrentTime;
            }
        }
        UpdateTimeline();
    }
    private void DisplayTimeUi()
    {
        MainTimelineControl.SetActive(true);
        MainTimelineControl.transform.position = Needlehead.position;
        MainTimelineControl.transform.rotation = Needlehead.rotation;
        CurrentTimeUi.localPosition = Vector3.zero;
        float startOffset = _timeUiScale * NormalizedTime;
        float endOffset = _timeUiScale * (1 - NormalizedTime);
        TimeStartUi.localPosition = new Vector3(-startOffset, 0, 0);
        TimeEndUi.localPosition = new Vector3(endOffset, 0, 0);
        TimeContainerUi.position = (TimeStartUi.position + TimeEndUi.position) / 2;
    }

    private void UpdateTimeline()
    {
        if (ShowTimelineControl && !MainTimelineControl.activeSelf)
        {
            DisplayTimeUi();
        }
        if (!ShowTimelineControl && MainTimelineControl.activeSelf)
        {
            MainTimelineControl.SetActive(false);
        }
        if (ShowTimelineControl)
        {
            float l2 = (TimeStartUi.position - TimeEndUi.position).sqrMagnitude;
            float theDot = Vector3.Dot(Needlehead.position - TimeStartUi.position, TimeEndUi.position - TimeStartUi.position);
            CurrentTimeUi.position = Vector3.Lerp(TimeStartUi.position, TimeEndUi.position, NormalizedTime);
            CurrentTime = Mathf.Max(0, Mathf.Min(1, theDot / l2)) * MaxTime;

            float connectorLength = (CurrentTimeUi.position - Needlehead.position).magnitude / 2;
            TimeConnectorUi.localScale = new Vector3(TimeConnectorUi.localScale.x, TimeConnectorUi.localScale.y, connectorLength);
            TimeConnectorUi.position = (CurrentTimeUi.position + Needlehead.position) / 2;
            TimeConnectorUi.LookAt(CurrentTimeUi);
        }
    }
}
