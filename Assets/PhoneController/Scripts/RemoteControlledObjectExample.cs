using UnityEngine;

public class RemoteControlledObjectExample : MonoBehaviour {

    //public Quaternion offset = Quaternion.identity;
    public Vector3 offset = new Vector3(0, 0, 0);
	void Update () {
        Vector3 eulerReceived = AGRemoteController.RemoteControllerReceiver.Instance.Orientation.eulerAngles;
        eulerReceived -= offset;
        this.transform.localRotation = Quaternion.Euler(eulerReceived);
        //this.transform.localRotation = AGRemoteController.RemoteControllerReceiver.Instance.Orientation * offset;
        if (AGRemoteController.RemoteControllerReceiver.Instance.GetKeyDown('R'))
        {
            offset = AGRemoteController.RemoteControllerReceiver.Instance.Orientation.eulerAngles;
            offset.x -= 90; // forward recenter
            //offset.SetFromToRotation(AGRemoteController.RemoteControllerReceiver.Instance.Orientation * Vector3.forward, Vector3.forward);
            //Debug.LogWarning(offset);


        }		

		if (AGRemoteController.RemoteControllerReceiver.Instance.GetKeyDown('A')) {
			this.transform.localScale *= 1.1f;
		} else if (AGRemoteController.RemoteControllerReceiver.Instance.GetKeyDown('B')) {
			this.transform.localScale *= 0.9f;
		}
	}
}
