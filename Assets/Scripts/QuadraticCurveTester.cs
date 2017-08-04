using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadraticCurveTester : MonoBehaviour
{
    public Transform[] Points;
    public Transform Output;

    [Range(0, 10)]
    public float Param;

    private float BezierChannel(float a, float b, float c, float d, float by)
    {
        float ab = Mathf.Lerp(a, b, by);
        float bc = Mathf.Lerp(b, c, by);
        float cd = Mathf.Lerp(c, d, by);
        float abc = Mathf.Lerp(ab, bc, by);
        float bcd = Mathf.Lerp(bc, cd, by);
        return Mathf.Lerp(abc, bcd, by);
    }
    private float CubicStyle(float a, float b, float c, float by)
    {
        float ab = Mathf.Lerp(a, b, by);
        float bc = Mathf.Lerp(b, c, by);
        return Mathf.Lerp(ab, bc, by);
    }

    void Update ()
    {
        int AIndex = Mathf.Max(Mathf.FloorToInt(Param - 1), 0);
        int BIndex = Mathf.Max(Mathf.FloorToInt(Param), 0);
        int CIndex = Mathf.CeilToInt(Param) + ((Mathf.Abs(Param % 1) < Mathf.Epsilon) ? 1 : 0); // handles the case where the value is on a round number
        int DIndex = CIndex + 1;

        int first;
        int second;
        int third;

        float param = Param % 1;
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

        Vector3 firstPoint = Points[Mathf.Min(first, Points.Length - 1)].position;
        Vector3 secondPoint = Points[Mathf.Min(second, Points.Length - 1)].position;
        Vector3 thirdPoint = Points[Mathf.Min(third, Points.Length - 1)].position;

        Vector3 firstSecond = Vector3.Lerp(firstPoint, secondPoint, .5f);
        Vector3 secondThird = Vector3.Lerp(secondPoint, thirdPoint, .5f);

        float newX = CubicStyle(firstSecond.x, secondPoint.x, secondThird.x, param);
        float newY = CubicStyle(firstSecond.y, secondPoint.y, secondThird.y, param);
        float newZ = CubicStyle(firstSecond.z, secondPoint.z, secondThird.z, param);

        Output.position = new Vector3(newX, newY, newZ);
    }
}
