using UnityEngine;

public class RemoteControlledObjectExample : MonoBehaviour {

    public Quaternion offset = Quaternion.identity;
    
	void Update () {
	    if (AGRemoteController.RemoteControllerReceiver.Instance.GetKeyDown('R'))
        {
			offset = AGRemoteController.RemoteControllerReceiver.Instance.Orientation * Quaternion.Euler(-90,0,0);
        }		
		else if (AGRemoteController.RemoteControllerReceiver.Instance.GetKeyDown('A')) {
			this.transform.localScale *= 1.1f;
		} else if (AGRemoteController.RemoteControllerReceiver.Instance.GetKeyDown('B')) {
			this.transform.localScale *= 0.9f;
		}
		transform.localRotation = AGRemoteController.RemoteControllerReceiver.Instance.Orientation * Quaternion.Inverse(offset);
	}
}
