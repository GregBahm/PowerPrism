using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineControlScript : MonoBehaviour 
{
    public float CurrentTime;
    public float MaxTime;

    public bool Play;
    
	void Update () 
    {
		if(Play)
        {
            CurrentTime += Time.deltaTime;
            if(CurrentTime > MaxTime)
            {
                MaxTime = CurrentTime;
            }
        }
	}
}
