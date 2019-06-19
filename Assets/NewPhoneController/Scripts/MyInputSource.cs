using TouchScript.InputSources;
using TouchScript.Pointers;
using UnityEngine;
using TouchScript.Utils;
using System.Collections.Generic;

public class MyInputSource : InputSource
{

    // Device touch area width in meters
    public float Width = 1080;
    // Device touch area height in meters
    public float Height = 1920;

    // Pool to reuse TouchPointers
    private ObjectPool<TouchPointer> touchPool;
    // Map from device touch id to TouchScript pointer
    private Dictionary<int, TouchPointer> deviceIdToTouch = new Dictionary<int, TouchPointer>(10);

    public MyInputSource()
    {
        touchPool = new ObjectPool<TouchPointer>(10, () => new TouchPointer(this), null, resetPointer);
    }

    // Cancels the pointer from the system and optionally returns it at the same position
    public override bool CancelPointer(Pointer pointer, bool shouldReturn)
    {
        base.CancelPointer(pointer, shouldReturn);

        lock (this)
        {
            int id = -1;
            foreach (var touchPoint in deviceIdToTouch)
            {
                if (touchPoint.Value.Id == pointer.Id)
                {
                    id = touchPoint.Key;
                    break;
                }
            }
            if (id > -1)
            {
                cancelPointer(pointer);
                if (shouldReturn)
                    deviceIdToTouch[id] = internalReturnTouch(pointer as TouchPointer);
                else
                    deviceIdToTouch.Remove(id);
                return true;
            }
            return false;
        }
    }

    // Called by TouchScript when the pointer is no longer needed
    public override void INTERNAL_DiscardPointer(Pointer pointer)
    {
        touchPool.Release(pointer as TouchPointer);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // Subscribe to DeviceProxy events
        var device = UDPTouchProxy.Instance;
        if (device != null)
        {
            device.PointerPressed += pointerPressedHandler;
            device.PointerMoved += pointerMovedHandler;
            device.PointerReleased += pointerReleasedHandler;
        }
    }

    protected override void OnDisable()
    {
        // Unsubscribe from DeviceProxy events
        var device = UDPTouchProxy.Instance;
        if (device != null)
        {
            device.PointerPressed -= pointerPressedHandler;
            device.PointerMoved -= pointerMovedHandler;
            device.PointerReleased -= pointerReleasedHandler;
        }

        base.OnDisable();
    }

    // Resets pointer before putting it in the pool
    private void resetPointer(Pointer p)
    {
        p.INTERNAL_Reset();
    }

    // Adds a pointer into the system
    private TouchPointer internalAddTouch(Vector2 position)
    {
        // Get a pointer from the pool
        var pointer = touchPool.Get();
        // Set its (remapped) position
        pointer.Position = remapCoordinates(position);
        // Set its buttons as "button one pressed this frame"
        pointer.Buttons |= Pointer.PointerButtonState.FirstButtonDown | Pointer.PointerButtonState.FirstButtonPressed;
        // Add the pointer to the system
        addPointer(pointer);
        // Press the pointer
        pressPointer(pointer);
        return pointer;
    }

    // Returns a copy of a cancelled pointer
    private TouchPointer internalReturnTouch(TouchPointer pointer)
    {
        // Get a pointer from the pool
        var newPointer = touchPool.Get();
        // Copy data from the old pointer
        newPointer.CopyFrom(pointer);
        // Set its buttons as "button one pressed this frame"
        pointer.Buttons |= Pointer.PointerButtonState.FirstButtonDown | Pointer.PointerButtonState.FirstButtonPressed;
        // Set flags
        newPointer.Flags |= Pointer.FLAG_RETURNED;
        // Add the pointer to the system
        addPointer(newPointer);
        // Press the pointer
        pressPointer(newPointer);
        return newPointer;
    }

    // Removes a pointer from the system
    private TouchPointer internalRemoveTouch(int id)
    {
        TouchPointer pointer;
        // Check if we have a pointer with such id
        if (!deviceIdToTouch.TryGetValue(id, out pointer)) return null;

        releasePointer(pointer);
        removePointer(pointer);
        return pointer;
    }

    private void pointerPressedHandler(object sender, DeviceProxyEventArgs e)
    {
        Debug.LogFormat("Pointer {0} added at {1}.", e.Id, e.Position);
        lock (this)
        {
            deviceIdToTouch.Add(e.Id, internalAddTouch(new Vector2(e.Position.x / Width * Screen.width, e.Position.y / Height * Screen.height)));
        }
    }

    private void pointerMovedHandler(object sender, DeviceProxyEventArgs e)
    {
        Debug.LogFormat("Pointer {0} moved to {1}.", e.Id, e.Position);
        lock (this)
        {
            TouchPointer touch;
            if (!deviceIdToTouch.TryGetValue(e.Id, out touch)) return;

            // Update to new position
            touch.Position = remapCoordinates(new Vector2(e.Position.x / Width * Screen.width, e.Position.y / Height * Screen.height));
            updatePointer(touch);
        }
    }

    private void pointerReleasedHandler(object sender, DeviceProxyEventArgs e)
    {
        Debug.LogFormat("Pointer {0} removed.", e.Id);
        lock (this)
        {
            internalRemoveTouch(e.Id);
            deviceIdToTouch.Remove(e.Id);
        }
    }
}
