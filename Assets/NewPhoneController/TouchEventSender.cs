using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TouchEventSender : MonoBehaviour
{
    /// <summary>
    /// In this script we are sending all the touch events to the remoted computer
    /// Here we first read the input from every frame, and then send the informations back from 
    /// </summary>
    /// 
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            List<SimpleTouch> touches = new List<SimpleTouch>();
            for (int i = 0; i < Input.touchCount; i++)
            {
                SimpleTouch newTouch = new SimpleTouch();
                newTouch.position = Input.GetTouch(i).position;
                newTouch.phase = Input.GetTouch(i).phase;
                newTouch.radius = Input.GetTouch(i).radius;
                newTouch.radiusVarience = Input.GetTouch(i).radiusVariance;
                touches.Add(newTouch);
            }
            string touchString = JsonConvert.SerializeObject(touches);
            UDPSender.Instance.SendUDPMessage(touchString);
        }
    }
}

public class SimpleTouch
{
    public Vector2 position { get; set; }
    public TouchPhase phase { get; set; }
    public float radius { get; set; }
    public float radiusVarience { get; set; }
}


