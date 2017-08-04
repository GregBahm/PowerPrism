using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

public struct Keyframe
{
    public readonly Vector3 Position;
    public readonly Quaternion Rotation;
    public readonly Vector3 Scale;
    public readonly Color Color;

    public const string PositionKey = "Position";
    public const string RotationKey = "Rotation";
    public const string ScaleKey = "Scale";
    public const string ColorKey = "Color";
    public const string KeyframeKey = "Keyframe";

    public Keyframe(Transform from, Color color)
        : this(from.position, from.rotation, from.lossyScale, color)
    { }

    public Keyframe(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
        Color = color;
    }

    public static Keyframe Lerp(Keyframe from, Keyframe to, float by)
    {
        Vector3 newPos = Vector3.Lerp(from.Position, to.Position, by);
        Quaternion newRot = Quaternion.Lerp(from.Rotation, to.Rotation, by);
        Vector3 newScale = Vector3.Lerp(from.Scale, to.Scale, by);
        Color color = Color.Lerp(from.Color, to.Color, by);
        return new Keyframe(newPos, newRot, newScale, color);
    }

    public XmlElement ToXml(XmlDocument document)
    {
        XmlElement ret = document.CreateElement(KeyframeKey);
        XmlElement posElement = VectorToXml(document, PositionKey, Position);
        XmlElement rotElement = QuaternionToXml(document, RotationKey, Rotation);
        XmlElement scaleElement = VectorToXml(document, ScaleKey, Scale);
        ret.AppendChild(posElement);
        ret.AppendChild(rotElement);
        ret.AppendChild(scaleElement);
        return ret;
    }

    private static XmlElement ColorToXml(XmlDocument document, string name, Color color)
    {
        XmlElement ret = document.CreateElement(name);
        AddAttribute(ret, document, "R", color.r);
        AddAttribute(ret, document, "G", color.g);
        AddAttribute(ret, document, "B", color.b);
        AddAttribute(ret, document, "A", color.a);
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
        Vector3 pos = LoadVector(element.SelectSingleNode(PositionKey));
        Quaternion rot = LoadQuaternion(element.SelectSingleNode(RotationKey));
        Vector3 scale = LoadVector(element.SelectSingleNode(ScaleKey));
        Color color = LoadColor(element.SelectSingleNode(ColorKey));
        return new Keyframe(pos, rot, scale, color);
    }

    private static float LoadFloat(XmlNode element, string name)
    {
        return Convert.ToSingle(element.Attributes.GetNamedItem(name).InnerText);
    }

    private static Color LoadColor(XmlNode element)
    {
        float r = LoadFloat(element, "R");
        float g = LoadFloat(element, "G");
        float b = LoadFloat(element, "B");
        float a = LoadFloat(element, "A");
        return new Color(r, g, b, a);
    }

    private static Quaternion LoadQuaternion(XmlNode element)
    {
        float x = LoadFloat(element, "X");
        float y = LoadFloat(element, "Y");
        float z = LoadFloat(element, "Z");
        float w = LoadFloat(element, "W");
        return new Quaternion(x, y, z, w);
    }

    private static Vector3 LoadVector(XmlNode element)
    {
        float x = LoadFloat(element, "X");
        float y = LoadFloat(element, "Y");
        float z = LoadFloat(element, "Z");
        return new Vector3(x, y, z);
    }
}