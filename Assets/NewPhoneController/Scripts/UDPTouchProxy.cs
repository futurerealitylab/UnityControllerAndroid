using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using Newtonsoft.Json;

public class UDPTouchProxy : Singleton<UDPTouchProxy>
{
    // reasons for a Behavior is to guarantee the Instance
    #region UDPReceiveInfos&UnityBehaviors
    private UdpClient client;
    private bool isDestroyed;
    public event EventHandler<DeviceProxyEventArgs> PointerPressed;
    public event EventHandler<DeviceProxyEventArgs> PointerMoved;
    public event EventHandler<DeviceProxyEventArgs> PointerReleased;

    private Dictionary<int, Vector2> pointers = new Dictionary<int, Vector2>();//id and pos

    private void Awake()
    {
        client = new UdpClient(6666);
        try
        {
            client.BeginReceive(new AsyncCallback(udpReceive), null);
            Debug.Log("UDP begin");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to start UDP receiver.");
        }
    }

    private void OnDestroy()
    {
        client.Close();
        isDestroyed = true;
    }

    private void udpReceive(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 6666);
        byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);

        string str = Encoding.ASCII.GetString(received);

        if (str.StartsWith("[{\"position\""))
        {
            List<SimpleTouch> touchesObj = JsonConvert.DeserializeObject<List<SimpleTouch>>(str);
            foreach (SimpleTouch touch in touchesObj)
            {
                dealWithSimpleTouch(touch);
            }

        }
        // here we need to deal with touch events
        if (!isDestroyed)
        {
            client.BeginReceive(new AsyncCallback(udpReceive), null);
        }
    }
    #endregion

    #region TouchScriptsRelated
   
   
    
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
    #endregion
}

public class DeviceProxyEventArgs : EventArgs
{
    public int Id;
    public Vector2 Position;
}
