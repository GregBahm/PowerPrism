using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class LiveObjectScript : MonoBehaviour 
{
    private List<Keyframe> keyframes;

    private int lastRecordedFrame;

    private Material mat;
    public Color Color = Color.white;

    private void Start()
    {
        keyframes = new List<Keyframe>
        {
            new Keyframe(transform, Color)
        };
        mat = gameObject.GetComponent<Renderer>().material;
    }

    public void Record(int keyframeIndex, bool wasRecordingLastFrame)
    {
        if (keyframeIndex > (keyframes.Count - 1))
        {
            FillInKeyframes(keyframeIndex);
        }
        Keyframe newFrame = new Keyframe(transform, Color);
        if ((keyframeIndex > (lastRecordedFrame + 1)) && wasRecordingLastFrame)
        {
            for (int i = lastRecordedFrame + 1; i < keyframeIndex; i++)
            {
                keyframes[i] = newFrame; //TOOD Make this an interpolation between the last keyframe and this keyframe;
            }
        }
        keyframes[keyframeIndex] = newFrame;
        lastRecordedFrame = keyframeIndex;
    }

    public void UpdateFramePlayback(float rawTime, int keyframeIndex, float timeBetweenKeyframes)
    {
        Keyframe target;
        if (keyframeIndex > (keyframes.Count - 1))
        {
            target = keyframes[keyframes.Count - 1];
        }
        else if (keyframeIndex > (keyframes.Count - 2))
        {
            target = keyframes[keyframeIndex];
        }
        else
        {
            target = GetInterpolatedKeyframeLinear(rawTime, keyframeIndex, timeBetweenKeyframes);
        }
        ApplyKeyframeToObject(target);
    }
    private Keyframe GetInterpolatedKeyframeLinear(float time, int keyframeIndex, float timebetweenKeyframes)
    {
        float fractionTime = time / timebetweenKeyframes;
        int lowKeyframeIndex = Mathf.FloorToInt(fractionTime);
        int highKeyframeIndex = Mathf.CeilToInt(fractionTime);
        Keyframe lowKeyframe = keyframes[lowKeyframeIndex];
        Keyframe highKeyframe = keyframes[highKeyframeIndex];
        float subFrameParam = fractionTime % 1;
        return Keyframe.Lerp(lowKeyframe, highKeyframe, subFrameParam);
    }

    public XmlElement ToXml(XmlDocument document)
    {
        XmlElement ret = document.CreateElement("LiveObject");
        foreach (Keyframe keyframe in keyframes)
        {
            XmlElement toXml = keyframe.ToXml(document);
            ret.AppendChild(toXml);
        }
        XmlAttribute nameElement = document.CreateAttribute("Name");
        nameElement.InnerText = gameObject.name; //TODO: Use guids or something so names don't have to be unique
        ret.Attributes.Append(nameElement);
        return ret;
    }

    public void Load(XmlElement data)
    {
        List<Keyframe> newKeyframes = new List<Keyframe>();
        foreach (XmlElement element in data.ChildNodes)
        {
            Keyframe newKeyframe = Keyframe.FromXml(element);
            newKeyframes.Add(newKeyframe);
        }
        keyframes = newKeyframes;
    }

    private void ApplyKeyframeToObject(Keyframe key)
    {
        transform.position = key.Position;
        transform.rotation = key.Rotation;
        transform.localScale = key.Scale;
        mat.SetColor("_Color", key.Color);
    }

    private void FillInKeyframes(int keyframeIndex)
    {
        Keyframe lastFrame = keyframes[keyframes.Count - 1];
        for (int i = keyframes.Count; i < keyframeIndex + 1; i++)
        {
            keyframes.Add(lastFrame);
        }
    }
}
