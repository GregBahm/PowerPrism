using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    public List<SceneSlideScript> SceneSlides;

    public SceneSlideScript CurrentSceneSlide;

    public Transform Tester;
    public Material TesterMat;

    private void OnRenderObject()
    {
        TesterMat.SetMatrix("_miniTransform", Tester.localToWorldMatrix);
    }
}

