using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class IPAddressManager : Singleton<IPAddressManager>
{
    /// <summary>
    /// This scripts doing the following:
    /// 1. UpdateTargetIPAddress from UIs or other commands
    /// 2. UpdateUIListIfNeeded
    /// </summary>

    [SerializeField]
    public string targetIPAddress;
    [SerializeField]
    public int port = 6666;//default 6666
    public string selfIPAddress;
    // belows are elements from UIs
    public TMP_Dropdown ipAddressDropDown;
    public TMP_InputField ipAddressInputField;
    public TMP_Text selfIPAddressArea;

    #region IPAddressRelatedMethods

    public void UpdateTargetIPAddressFromDropDown(int targetNum)
    {
        if (ipAddressDropDown != null)
        {
            targetIPAddress = ipAddressDropDown.options[targetNum].text;
        }
        if (ipAddressDropDown.options[targetNum].text.StartsWith("Custom"))
        {
            if (ipAddressInputField != null && ipAddressInputField.text.Length > 0)
            {
                targetIPAddress = ipAddressInputField.text;
            }
            else
            {
                Debug.Log("Invalid IPAddress, waiting for the text from the input field");
            }
        }
    }

    public void UpdateTargetIPAddressFromInput(string targetString)
    {
        if (ipAddressDropDown != null)
        {
            if (ipAddressDropDown.options[ipAddressDropDown.value].text.StartsWith("Custom"))
            {
                if (ipAddressInputField != null && targetString.Length > 0)
                {
                    targetIPAddress = targetString;
                }
                else
                {
                    Debug.Log("Invalid IPAddress, waiting for the text from the input field");
                }
            }
        }
    }

    public void UpdateTargetIPAddress(string targetString)
    {
        targetIPAddress = targetString;
    }

    public string ReadSelfIPAddress()
    {
        IPHostEntry host;
        string localIP = "?";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily.ToString() == "InterNetwork")
            {
                localIP = ip.ToString();
                Debug.Log(localIP);
            }
        }
        return localIP;
    }
    #endregion

    #region UnityBehavior

    private void Awake()
    {
        selfIPAddress = ReadSelfIPAddress();
        if (selfIPAddressArea != null)
        {          
            selfIPAddressArea.text = selfIPAddress;
        }
        if (ipAddressDropDown != null)
        {
            targetIPAddress = ipAddressDropDown.options[0].text;
        }
    }

   

    #endregion




}
