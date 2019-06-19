using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TouchEventReceiver : Singleton<TouchEventReceiver>
{
    private int frameCount = 0;

    public struct TouchStroke
    {
        public SimpleTouch touch;
        public int frame;
    }
    
    [SerializeField]
    public List<TouchStroke> touchStrokes = new List<TouchStroke>();

    private void OnEnable()
    {
        UDPReceiver.OnReceivedMessage += OnReceivedTouch;
    }

    private void OnDisable()
    {
        UDPReceiver.OnReceivedMessage -= OnReceivedTouch;
    }
    private void Update()
    {
        lock(((ICollection)touchStrokes).SyncRoot)
        {
            for (int i = 0; i < touchStrokes.Count; i++)
            {
                if (touchStrokes[i].frame == frameCount)
                {
                    touchStrokes.RemoveAt(i);
                    i--;
                }
            }
            frameCount++;
        }
        if (GetTouches().Count > 0)
        {
            List<SimpleTouch> frameTouches = GetTouches();
            Debug.Log("Finger Nums: " + frameTouches.Count);
            foreach(SimpleTouch touch in frameTouches)
            {
                Debug.Log(string.Format("pos: {0} , phase: {1}, id: {2}, SendFrame: {3}", touch.position, touch.phase, touch.fingerID, touch.sendFrame));
            }
        }
        
    }

    public void OnReceivedTouch(string message)
    {
        if (message.StartsWith("[{\"position\""))
        {
            lock (((ICollection)touchStrokes).SyncRoot)
            {
                List<SimpleTouch> touchesObj = JsonConvert.DeserializeObject<List<SimpleTouch>>(message);
                foreach (SimpleTouch newTouch in touchesObj)
                {
                        touchStrokes.Add(new TouchStroke
                        {
                            touch = newTouch,
                            frame = frameCount + 1
                        });
                }
            }           
        }
    }

    public List<SimpleTouch> GetTouches()
    {
        
        List<SimpleTouch> touches = new List<SimpleTouch>();
        List<int> fingerIDList = new List<int>();
        lock (((ICollection)touchStrokes).SyncRoot)
        {
            for (int i = 0; i < touchStrokes.Count; i++)
            {
                if (touchStrokes[i].frame == frameCount)
                {
                    if (!fingerIDList.Contains(touchStrokes[i].touch.fingerID))
                    {
                        touches.Add(touchStrokes[i].touch);
                        fingerIDList.Add(touchStrokes[i].touch.fingerID);
                    }
                    else
                    {
                        // if in the same frame we have same finger of begin, stationary, move and end, we ensure that
                        // if no end, only deal the begin
                        // if with end, only deal with end
                        int previousIndex = touches.FindIndex(s => s.fingerID.Equals(touchStrokes[i].touch.fingerID));
                        if (touches[previousIndex].phase == TouchPhase.Began)
                        {
                            if (touchStrokes[i].touch.phase == TouchPhase.Ended)
                            {
                                touches[previousIndex] = touchStrokes[i].touch;
                            }
                        }
                        if (touches[previousIndex].phase != TouchPhase.Began && touches[previousIndex].phase != TouchPhase.Ended)
                        {
                            if (touchStrokes[i].touch.phase == TouchPhase.Ended || touchStrokes[i].touch.phase == TouchPhase.Began)
                            {
                                touches[previousIndex] = touchStrokes[i].touch;
                            }
                        }
                        if (touches[previousIndex].phase == TouchPhase.Ended)
                        {
                            continue;
                        }
                    }

                }   
            }
        }
        return touches;
    }
}
