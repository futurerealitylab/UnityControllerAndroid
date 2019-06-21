using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RefinedTouchProxy : Singleton<RefinedTouchProxy>
{
    public event EventHandler<DeviceProxyEventArgs> PointerPressed;
    public event EventHandler<DeviceProxyEventArgs> PointerMoved;
    public event EventHandler<DeviceProxyEventArgs> PointerReleased;

    private Dictionary<int, Vector2> pointers = new Dictionary<int, Vector2>();//id and pos



    private void Update()
    {
        if (TouchEventReceiver.Instance != null)
        {
            //Dealing with the touch event here
            List<SimpleTouch> currentFrameTouch = TouchEventReceiver.Instance.GetTouches();
            foreach (SimpleTouch t in currentFrameTouch)
            {
                dealWithSimpleTouch(t);
            }
        }
    }

    public void dealWithSimpleTouch(SimpleTouch touch)
    {
        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
        {
            if (!pointers.ContainsKey(touch.fingerID))
            {
                int pointerId = touch.fingerID;
                Vector2 pos = touch.position;
                pointers.Add(pointerId, pos);
                if (PointerPressed != null) PointerPressed(this, new DeviceProxyEventArgs() { Id = pointerId, Position = pos });
                //add this pointer
            }
            else
            {
                //This pointer is still pressed, don't need to do anything here
            }
        }
        if (touch.phase == TouchPhase.Moved)
        {
            // lost package for the first time adding, so add the pointer first
            if (!pointers.ContainsKey(touch.fingerID))
            {
                int pointerId = touch.fingerID;
                Vector2 pos = touch.position;
                pointers.Add(pointerId, pos);
                if (PointerPressed != null) PointerPressed(this, new DeviceProxyEventArgs() { Id = pointerId, Position = pos });
                //add this pointer
            }
            else
            {
                pointers[touch.fingerID] = touch.position;
                if (PointerMoved != null) PointerMoved(this, new DeviceProxyEventArgs() { Id = touch.fingerID, Position = touch.position });
            }
        }
        if (touch.phase == TouchPhase.Ended)
        {
            pointers.Remove(touch.fingerID);
            if (PointerReleased != null) PointerReleased(this, new DeviceProxyEventArgs() { Id = touch.fingerID });
        }
    }
}
