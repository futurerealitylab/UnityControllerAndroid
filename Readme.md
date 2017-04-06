
# Sender

* Build SenderScene into iOS or Android
  * iOS Recommended. 
  * Didn't test on Android. Axis might be wrong.
  
# Receiver
* Add Prefabs/RemoteControllerReceiver to the scene.
* Get orientation: `AGRemoteController.RemoteControllerReceiver.Instance.Orientation`
* Get key event (currently with key 'A' or 'B'): `if (AGRemoteController.RemoteControllerReceiver.Instance.GetKeyDown('A'))`
* See ReceiverSceneExample
