﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class SceneManagerScript : MonoBehaviour
{
    public TimelineControlScript Timeline;
    public SelectionScript Selector;
    public List<LiveObjectScript> LiveObjects;

    private float lastTimeProcessed;
    public const float TimeBetweenKeyframes = 0.02f;

    private LiveObjectScript lastRecordedObject;
    public bool Recording;

    public bool DoTheLoad;

    internal void ToggleRecording()
    {
        Recording = !Recording;
        Timeline.Play = !Timeline.Play;
        if (Selector.CurrentSelection != null)
        {
            LiveObjectScript liveObject = Selector.CurrentSelection.GetComponent<LiveObjectScript>();
            Selector.Recording = !Selector.Recording;
        }
    }

    private void Update()
    {
        float time = Timeline.CurrentTime;
        if (Mathf.Abs(time - lastTimeProcessed) > TimeBetweenKeyframes)
        {
            int keyframeIndex = (int)(time / TimeBetweenKeyframes);
            foreach (LiveObjectScript liveObject in LiveObjects)
            {
                UpdateLiveObject(liveObject, time, keyframeIndex);
            }
            lastTimeProcessed = time;
        }
        lastRecordedObject = Recording ? Selector.CurrentSelection : null;

        if(DoTheLoad)
        {
            DoTheLoad = false;
            Load();
        }
    }

    private void UpdateLiveObject(LiveObjectScript liveObject, float rawTime, int keyframeIndex)
    {
        if (Recording && Selector.CurrentSelection == liveObject)
        {
            bool wasRecordingLastFrame = lastRecordedObject == liveObject;
            liveObject.Record(keyframeIndex, wasRecordingLastFrame);
        }
        else
        {
            liveObject.UpdateFramePlayback(rawTime, keyframeIndex, TimeBetweenKeyframes);
        }
    }

    private const string SaveFilePath = @"D:\PowerPrismRepo\SaveFile.xml";

    public void Save()
    {
        XmlDocument document = new XmlDocument();
        XmlElement root = document.CreateElement("Root");
        XmlAttribute maxTime = document.CreateAttribute("MaxTime");
        maxTime.InnerText = Timeline.MaxTime.ToString();
        root.Attributes.Append(maxTime);
        document.AppendChild(root);
        foreach (LiveObjectScript liveObject in LiveObjects)
        {
            XmlElement xml = liveObject.ToXml(document);
            root.AppendChild(xml);
        }
        document.Save(SaveFilePath);
    }

    public void Load()
    {
        XmlDocument document = new XmlDocument();
        document.Load(SaveFilePath);
        foreach (XmlElement objectData in document.DocumentElement.ChildNodes)
        {
            string name = objectData.Attributes.GetNamedItem("Name").InnerText;
            LiveObjectScript sceneAsset = LiveObjects.First(item => item.gameObject.name == name);
            sceneAsset.Load(objectData);
        }
        Timeline.MaxTime = Convert.ToSingle(document.DocumentElement.Attributes.GetNamedItem("MaxTime").InnerText);
    }
}