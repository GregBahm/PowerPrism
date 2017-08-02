using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class LiveObjectScript : MonoBehaviour 
{
    public TimelineControlScript Timeline;

    private List<Keyframe> keyframes;

    private int lastRecordedFrame;
    private bool wasRecordingLastFrame;

    private void Start()
    {
        keyframes = new List<Keyframe>
        {
            new Keyframe(transform)
        };
    }

    public void Record(int keyframeIndex, bool wasRecordingLastFrame)
    {
        if (keyframeIndex > (keyframes.Count - 1))
        {
            FillInKeyframes(keyframeIndex);
        }
        Keyframe newFrame = new Keyframe(transform);
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
        ApplyKeyframeToTransform(target);
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

    private void ApplyKeyframeToTransform(Keyframe key)
    {
        transform.position = key.Position;
        transform.rotation = key.Rotation;
        transform.localScale = key.Scale;
    }

    private void FillInKeyframes(int keyframeIndex)
    {
        Keyframe lastFrame = keyframes[keyframes.Count - 1];
        for (int i = keyframes.Count; i < keyframeIndex + 1; i++)
        {
            keyframes.Add(lastFrame);
        }
    }
    
    private struct Keyframe
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 Scale;

        public Keyframe(Transform from)
            :this(from.position, from.rotation, from.lossyScale)
        { }
        
        public Keyframe(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public static Keyframe Lerp(Keyframe from, Keyframe to, float by)
        {
            Vector3 newPos = Vector3.Lerp(from.Position, to.Position, by);
            Quaternion newRot = Quaternion.Lerp(from.Rotation, to.Rotation, by);
            Vector3 newScale = Vector3.Lerp(from.Scale, to.Scale, by);
            return new Keyframe(newPos, newRot, newScale);
        }

        internal XmlElement ToXml(XmlDocument document)
        {
            XmlElement ret = document.CreateElement("Keyframe");
            XmlElement posElement = VectorToXml(document, "Position", Position);
            XmlElement rotElement = QuaternionToXml(document, "Rotation", Rotation);
            XmlElement scaleElement = VectorToXml(document, "Scale", Scale);
            ret.AppendChild(posElement);
            ret.AppendChild(rotElement);
            ret.AppendChild(scaleElement);
            return ret;
        }

        private static XmlElement VectorToXml(XmlDocument document, string name, Vector3 vector)
        {
            XmlElement ret = document.CreateElement(name);
            AddAttribute(ret, document, "X", vector.x);
            AddAttribute(ret, document, "Y", vector.y);
            AddAttribute(ret, document, "Z", vector.z);
            return ret;
        }

        private static XmlElement QuaternionToXml(XmlDocument document, string name, Quaternion quaternion)
        {
            XmlElement ret = document.CreateElement(name);
            AddAttribute(ret, document, "X", quaternion.x);
            AddAttribute(ret, document, "Y", quaternion.y);
            AddAttribute(ret, document, "Z", quaternion.z);
            AddAttribute(ret, document, "W", quaternion.w);
            return ret;
        }

        private static void AddAttribute(XmlElement element, XmlDocument document, string name, float value)
        {
            XmlAttribute attribute = document.CreateAttribute(name);
            attribute.InnerText = value.ToString();
            element.Attributes.Append(attribute);
        }

        public static Keyframe FromXml(XmlElement element)
        {
            Vector3 pos = LoadVector(element.SelectSingleNode("Position"));
            Quaternion rot = LoadQuaternion(element.SelectSingleNode("Rotation"));
            Vector3 scale = LoadVector(element.SelectSingleNode("Scale"));
            return new Keyframe(pos, rot, scale);
        }

        private static Quaternion LoadQuaternion(XmlNode element)
        {
            float x = Convert.ToSingle(element.Attributes.GetNamedItem("X").InnerText);
            float y = Convert.ToSingle(element.Attributes.GetNamedItem("Y").InnerText);
            float z = Convert.ToSingle(element.Attributes.GetNamedItem("Z").InnerText);
            float w = Convert.ToSingle(element.Attributes.GetNamedItem("W").InnerText);
            return new Quaternion(x, y, z, w);
        }

        private static Vector3 LoadVector(XmlNode element)
        {
            float x = Convert.ToSingle(element.Attributes.GetNamedItem("X").InnerText);
            float y = Convert.ToSingle(element.Attributes.GetNamedItem("Y").InnerText);
            float z = Convert.ToSingle(element.Attributes.GetNamedItem("Z").InnerText);
            return new Vector3(x, y, z);
        }
    }
}
