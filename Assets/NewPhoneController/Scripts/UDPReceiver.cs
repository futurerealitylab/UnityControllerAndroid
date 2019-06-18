using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class UDPReceiver : Singleton<UDPReceiver>
{
    private UdpClient client;
    private bool isDestroyed;

    public delegate void ReceiveInfo(string message);
    public static event ReceiveInfo OnReceivedMessage;

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

        //Debug.Log("Received: " + str);

        if (OnReceivedMessage != null)
        {
            OnReceivedMessage(str);
        }

        if (!isDestroyed)
        {
            client.BeginReceive(new AsyncCallback(udpReceive), null);
        }
    }
}
