using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveObjectScript : MonoBehaviour 
{
    public TimelineControlScript Timeline;

    private List<Keyframe> keyframes;

    private float lastTimeProcessed;

    public bool Recording;
    public float TimeBetweenKeyframes;

    public bool TestRecord;
    public bool TestPlayback;

    private void Start()
    {
        keyframes = new List<Keyframe>();
        keyframes.Add(new Keyframe(transform));
    }

    void Update ()
    {
        if (TestRecord)
        {
            TestRecord = false;
            Timeline.CurrentTime = 0;
            Timeline.Play = true;
            Recording = true;
        }
        if (TestPlayback)
        {
            TestPlayback = false;
            Recording = false;
            Timeline.CurrentTime = 0;
            Timeline.Play = true;
        }

        float time = Timeline.CurrentTime;
        if(Mathf.Abs(time - lastTimeProcessed) > TimeBetweenKeyframes)
        {

            int keyframeIndex = (int)(time / TimeBetweenKeyframes);
            if (Recording)
            {
                UpdateFrameRecording(keyframeIndex);
            }
            else
            {
                UpdateFramePlayback(time, keyframeIndex);
            }
            lastTimeProcessed = time;
        }
	}

    private void UpdateFrameRecording(int keyframeIndex)
    {
        if (keyframeIndex > (keyframes.Count - 1))
        {
            FillInKeyframes(keyframeIndex);
        }
        keyframes[keyframeIndex] = new Keyframe(transform);
    }

    private void UpdateFramePlayback(float time, int keyframeIndex)
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
            target = GetInterpolatedKeyframe(time);
        }
        ApplyKeyframeToTransform(target);
    }
    private Keyframe GetInterpolatedKeyframe2(float time)
    {
        float index = time / TimeBetweenKeyframes;
        int lowKeyframeIndex = Mathf.FloorToInt(index);
        int highKeyframeIndex = Mathf.CeilToInt(index);
        Keyframe lowKeyframe = keyframes[lowKeyframeIndex];
        Keyframe highKeyframe = keyframes[highKeyframeIndex];
        float subFrameParam = index % 1;
        return Keyframe.Lerp(lowKeyframe, highKeyframe, subFrameParam);
    }

    private Keyframe GetInterpolatedKeyframe(float time)
    {
        time = time / TimeBetweenKeyframes;
        int AIndex = Mathf.Max(Mathf.FloorToInt(time - 1), 0);
        int BIndex = Mathf.Max(Mathf.FloorToInt(time), 0);
        int CIndex = Mathf.CeilToInt(time) + ((Mathf.Abs(time % 1) < Mathf.Epsilon) ? 1 : 0); // handles the case where the value is on a round number
        int DIndex = CIndex + 1;

        int first;
        int second;
        int third;

        float param = time % 1;
        if (param < 0.5f)
        {
            param = param + 0.5f;
            first = AIndex;
            second = BIndex;
            third = CIndex;
        }
        else
        {
            param = param - 0.5f;
            first = BIndex;
            second = CIndex;
            third = DIndex;
        }

        Keyframe firstPoint = keyframes[Mathf.Min(first, keyframes.Count - 1)];
        Keyframe secondPoint = keyframes[Mathf.Min(second, keyframes.Count - 1)];
        Keyframe thirdPoint = keyframes[Mathf.Min(third, keyframes.Count - 1)];

        Keyframe firstSecond = Keyframe.Lerp(firstPoint, secondPoint, .5f);
        Keyframe secondThird = Keyframe.Lerp(secondPoint, thirdPoint, .5f);

        return Keyframe.CubicLerp(firstSecond, secondPoint, secondThird, param);
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

        public static Keyframe CubicLerp(Keyframe a, Keyframe b, Keyframe c, float by)
        {
            Keyframe ab = Lerp(a, b, by);
            Keyframe bc = Lerp(b, c, by);
            return Lerp(ab, bc, by);
        }
    }
}
