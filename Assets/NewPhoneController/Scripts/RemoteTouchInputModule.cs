using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.EventSystems
{
    [AddComponentMenu("Event/Touch Input Module")]
    public class RemoteTouchInputModule : PointerInputModule
    {
       
        //protected TouchInputModule()
        //{ }
        //Anything looks unnecessary will be commmented here

        private Vector2 m_LastMousePosition;
        private Vector2 m_MousePosition;

        private PointerEventData m_InputPointerEvent;

        //*[SerializeField]
        //*[FormerlySerializedAs("m_AllowActivationOnStandalone")]
        private bool m_ForceModuleActive;

        //*[Obsolete("allowActivationOnStandalone has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
        public bool allowActivationOnStandalone
        {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        public bool forceModuleActive
        {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }
        /*
        public override void UpdateModule()
        {
            if (!eventSystem.isFocused)
            {
                if (m_InputPointerEvent != null && m_InputPointerEvent.pointerDrag != null && m_InputPointerEvent.dragging)
                    ExecuteEvents.Execute(m_InputPointerEvent.pointerDrag, m_InputPointerEvent, ExecuteEvents.endDragHandler);

                m_InputPointerEvent = null;
            }

            m_LastMousePosition = m_MousePosition;
            m_MousePosition = input.mousePosition;
        }
        */
        /*
        public override bool IsModuleSupported()
        {
            return forceModuleActive || input.touchSupported;
        }
        */
        public override bool IsModuleSupported()
        {
            if (TouchEventReceiver.Instance != null)
            {
                return true;
            }
            return false;
        }

        public override bool ShouldActivateModule()
        {
            if (TouchEventReceiver.Instance != null)
            {
                return true;
            }
            return false;
            
            
        }
        /*
        public override bool ShouldActivateModule()
        {
            if (!base.ShouldActivateModule())
                return false;

            if (m_ForceModuleActive)
                return true;

            if (UseFakeInput())
            {
                bool wantsEnable = input.GetMouseButtonDown(0);

                wantsEnable |= (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0.0f;
                return wantsEnable;
            }

            if (input.touchCount > 0)
                return true;

            return false;
        }
        */
        /*
        private bool UseFakeInput()
        {
            return !input.touchSupported;
        }
        */

        
        public override void Process()
        {
            //if (UseFakeInput())
            //FakeTouches();
            // else
            //ProcessTouchEvents();
            ProcessRemoteTouchEvents();
        }

        /// <summary>
        /// For debugging touch-based devices using the mouse.
        /// </summary>
        private void FakeTouches()
        {
            var pointerData = GetMousePointerEventData(0);

            var leftPressData = pointerData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            // fake touches... on press clear delta
            if (leftPressData.PressedThisFrame())
                leftPressData.buttonData.delta = Vector2.zero;

            ProcessTouchPress(leftPressData.buttonData, leftPressData.PressedThisFrame(), leftPressData.ReleasedThisFrame());

            // only process move if we are pressed...
            if (input.GetMouseButton(0))
            {
                ProcessMove(leftPressData.buttonData);
                ProcessDrag(leftPressData.buttonData);
            }
        }

        /// <summary>
        /// Process all touch events.
        /// </summary>
        private void ProcessTouchEvents()
        {
            for (int i = 0; i < input.touchCount; ++i)
            {
                Touch touch = input.GetTouch(i);

                if (touch.type == TouchType.Indirect)
                    continue;

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(touch, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                    RemovePointerData(pointer);
            }
        }

        private void ProcessRemoteTouchEvents()
        {
            List<SimpleTouch> currentTouches = TouchEventReceiver.Instance.GetTouches();
            for (int i = 0; i < currentTouches.Count; i++)
            {
                SimpleTouch touch = currentTouches[i];

                bool released;
                bool pressed;
                var pointer = GetRemoteTouchPointerEventData(touch, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                    RemovePointerData(pointer);

            }
        }

        protected PointerEventData GetRemoteTouchPointerEventData(SimpleTouch input, out bool pressed, out bool released)
        {
            PointerEventData pointerData;
            var created = GetPointerData(input.fingerID, out pointerData, true);

            pointerData.Reset();

            pressed = created || (input.phase == TouchPhase.Began);
            released = (input.phase == TouchPhase.Canceled) || (input.phase == TouchPhase.Ended);

            if (created)
                pointerData.position = input.position;

            if (pressed)
                pointerData.delta = Vector2.zero;
            else
                pointerData.delta = input.position - pointerData.position;

            pointerData.position = input.position;

            pointerData.button = PointerEventData.InputButton.Left;

            if (input.phase == TouchPhase.Canceled)
            {
                pointerData.pointerCurrentRaycast = new RaycastResult();
            }
            else
            {
                eventSystem.RaycastAll(pointerData, m_RaycastResultCache);

                var raycast = FindFirstRaycast(m_RaycastResultCache);
                pointerData.pointerCurrentRaycast = raycast;
                m_RaycastResultCache.Clear();
            }
            return pointerData;
        }



        protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
        {
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (pressed)
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                if (pointerEvent.pointerEnter != currentOverGo)
                {
                    // send a pointer enter to the touched element if it isn't the one to select...
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    pointerEvent.pointerEnter = currentOverGo;
                }

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress)
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);

                m_InputPointerEvent = pointerEvent;
            }

            // PointerUp notification
            if (released)
            {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;

                m_InputPointerEvent = pointerEvent;
            }
        }

        public override void DeactivateModule()
        {
            base.DeactivateModule();
            ClearSelection();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            //sb.AppendLine(UseFakeInput() ? "Input: Faked" : "Input: Touch");
            /*
            if (UseFakeInput())
            {
                var pointerData = GetLastPointerEventData(kMouseLeftId);
                if (pointerData != null)
                    sb.AppendLine(pointerData.ToString());
            }
            else
            {*/
                foreach (var pointerEventData in m_PointerData)
                    sb.AppendLine(pointerEventData.ToString());
            //}
            return sb.ToString();
        }
    }
}
