using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScript : MonoBehaviour 
{
    private LiveObjectScript lastSelection;
    public LiveObjectScript CurrentSelection;
    public Transform SelectionWand;
    public float RayDistance = 10;

    public bool Selecting;

    public OutlineScript Outliner;

    public Color PreviewOutlineColor;
    public Color SelectedOutlineColor;
    public Color RecordingColor;

    public bool Recording;

    private GameObject wandRayVisiual;
    private Material wandRayMaterial;

    private void Start()
    {
        wandRayVisiual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wandRayMaterial = new Material(Shader.Find("Standard"));
        wandRayVisiual.GetComponent<Renderer>().material = wandRayMaterial;
        Destroy(wandRayVisiual.GetComponent<BoxCollider>());
        wandRayVisiual.transform.parent = SelectionWand;
    }

    private void Update()
    {
        Outliner.OutlineColor = GetOulinerColor();
        if (Recording)
        {
            return;
        }
        if (Selecting)
        {
            RaycastHit rayInfo;
            Ray ray = new Ray(SelectionWand.position, SelectionWand.forward);
            bool hit = Physics.Raycast(ray, out rayInfo, RayDistance);
            if (hit)
            {
                CurrentSelection = rayInfo.transform.gameObject.GetComponent<LiveObjectScript>();
            }
            else
            {
                CurrentSelection = null;
            }

            UpdateWandVisual(rayInfo, hit);

        }
        if (CurrentSelection != lastSelection)
        {
            if(CurrentSelection == null)
            {
                Outliner.ClearOutlineTarget();
            }
            else
            {
                Outliner.UpdateOutlineTarget(CurrentSelection.gameObject);
            }
        }
        wandRayVisiual.SetActive(Selecting);

        lastSelection = CurrentSelection;
        Outliner.DoBlit = CurrentSelection != null;

    }

    private Color GetOulinerColor()
    {
        if(Recording)
        {
            return RecordingColor;
        }
        return Selecting? PreviewOutlineColor : SelectedOutlineColor;
    }

    private void UpdateWandVisual(RaycastHit rayInfo, bool hit)
    {
        float length = RayDistance;
        if(hit)
        { 
            length = (rayInfo.point - SelectionWand.position).magnitude;
        }
        wandRayVisiual.transform.localPosition = new Vector3(0, 0, length / 2);
        wandRayVisiual.transform.localScale = new Vector3(.01f, .01f, length);
        wandRayMaterial.SetColor("_Color", hit ? PreviewOutlineColor : Color.white);
    }
}
