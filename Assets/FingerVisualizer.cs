using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerVisualizer : MonoBehaviour
{
    // todo :
    // 1. in Update, read all the touch event data
    // 2. based on the touch event data, visualize fingers
    // 3. Visualize following attritubitons : 1. pos, 2: radius
    public List<RectTransform> fingers;
    public bool isLastFrameOnTouch;

    private void Awake()
    {
        fingers = new List<RectTransform>();
        foreach (Transform t in this.transform)
        {
            fingers.Add(t.GetComponent<RectTransform>());
            t.GetComponent<Image>().enabled = false;
        }
        isLastFrameOnTouch = false;
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.touchCount < 6)
        {
            Debug.Log("Have Touch");
            int touchCount = Input.touchCount;
            for (int i = 0; i < fingers.Count; i++)
            {
                if (i < touchCount)
                {
                    fingers[i].GetComponent<Image>().enabled = true;
                }
                else
                {
                    fingers[i].GetComponent<Image>().enabled = false;
                }
            }

            for (int i = 0; i < touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                fingers[i].anchoredPosition = t.position;
                fingers[i].sizeDelta = new Vector2(t.radius * 2, t.radius * 2);
            }
            isLastFrameOnTouch = true;
        }
        if (Input.touchCount == 0 && isLastFrameOnTouch)
        {
            foreach (RectTransform f in fingers)
            {
                f.GetComponent<Image>().enabled = false;
            }
            isLastFrameOnTouch = false;
        }
        
    }


}
