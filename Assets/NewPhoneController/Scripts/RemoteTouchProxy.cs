using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System;

/*
public class DeviceProxy 
    {

        public static DeviceProxy Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DeviceProxy();
                }
                return instance;
            }
        }

        public event EventHandler<DeviceProxyEventArgs> PointerPressed;
        public event EventHandler<DeviceProxyEventArgs> PointerMoved;
        public event EventHandler<DeviceProxyEventArgs> PointerReleased;

        private static DeviceProxy instance;

        private Timer updateTimer, createTimer;
        private Dictionary<int, Vector2> pointers = new Dictionary<int, Vector2>(MAX_POINTERS);
        private int pointerId = 0;

        public DeviceProxy()
        {
            rnd = new System.Random();
            createTimer = new Timer(CREATE_INTERVAL);
            createTimer.Start();
            createTimer.Elapsed += (sender, e) =>
            {
                addPointer();
            };

            updateTimer = new Timer(UPDATE_INTERVAL);
            updateTimer.Start();
            updateTimer.Elapsed += (sender, e) =>
            {
                var keys = new List<int>(pointers.Keys);
                foreach (var id in keys)
                {
                    var pos = pointers[id];
                    pos += new Vector2((float)(rnd.NextDouble()) - 0.5f, (float)(rnd.NextDouble()) - 0.5f).normalized * speed;
                    pointers[id] = pos;
                    if (PointerMoved != null) PointerMoved(this, new DeviceProxyEventArgs() { Id = id, Position = pos });
                }
            };
        }

        private void addPointer()
        {
            var toRemove = pointerId - MAX_POINTERS;
            if (toRemove >= 0)
            {
                pointers.Remove(toRemove);
                if (PointerReleased != null) PointerReleased(this, new DeviceProxyEventArgs() { Id = toRemove });
            }

            var pos = new Vector2((float)(WIDTH_IN_M * rnd.NextDouble()), (float)(HEIGHT_IN_M * rnd.NextDouble()));
            pointers.Add(pointerId, pos);
            if (PointerPressed != null) PointerPressed(this, new DeviceProxyEventArgs() { Id = pointerId, Position = pos });
            pointerId++;
        }
    }

    public class DeviceProxyEventArgs : EventArgs
    {
        public int Id;
        public Vector2 Position;
    }
*/