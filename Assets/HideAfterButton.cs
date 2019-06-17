using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAfterButton : MonoBehaviour
{
    public GameObject SettingCanvas;
    public void HideCanvas()
    {
        SettingCanvas.SetActive(false);
    }
    public void ShowCanvas()
    {
        SettingCanvas.SetActive(true);
    }
}
