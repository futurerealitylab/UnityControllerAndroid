using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPSender : Singleton<UDPSender>
{
    /// <summary>
    /// This is the simple Singleton to hold the sending events.
    /// Here in the version 1 we only send via new client everytime, and destroy that accordingly
    /// </summary>
    
    public void SendUDPMessage(string message)
    {
        if (IPAddressManager.Instance != null)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            UdpClient udpClient = new UdpClient();
            udpClient.Send(data, data.Length, IPAddressManager.Instance.targetIPAddress, IPAddressManager.Instance.port);
            udpClient.Close();
        }
    }

    public void SendUDPMessage(string message, string ipAddress, int port)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        UdpClient udpClient = new UdpClient();
        udpClient.Send(data, data.Length, ipAddress, port);
        udpClient.Close();
    }

    public void SendTestMessage()
    {
        SendUDPMessage("Test");
    }


}
