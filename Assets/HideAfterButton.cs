using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAfterButton : MonoBehaviour
{
    public GameObject SettingCanvas;
    public GameObject fingerCanvas;
    public void HideCanvas()
    {
        SettingCanvas.SetActive(false);
        fingerCanvas.SetActive(true);
    }
    public void ShowCanvas()
    {
        SettingCanvas.SetActive(true);
        fingerCanvas.SetActive(false);
    }
}
