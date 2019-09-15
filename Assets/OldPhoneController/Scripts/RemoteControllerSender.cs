using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Text;

namespace AGRemoteController
{

public class RemoteControllerSender : MonoBehaviour {

	private string ipAddress;
	private int port = 0;

	public InputField ipInputField;
	public InputField portInputField;

	public void UpdateIPAddress() {
		ipAddress = ipInputField.text;
		PlayerPrefs.SetString("ipAddress", ipAddress);
		PlayerPrefs.Save();
	}

	public void UpdatePort() {
		if (!int.TryParse(portInputField.text, out port)) {
			port = 0;
		}
		PlayerPrefs.SetInt("port", port);
		PlayerPrefs.Save();
	}

	public void ButtonTapped(Button button) {
		Send(string.Format("tap {0}", button.GetComponentInChildren<Text>().text));	
	}

	void Start() {
        Input.gyro.enabled = true;
		ipInputField.text = PlayerPrefs.GetString("ipAddress");
		UpdateIPAddress();

		portInputField.text = PlayerPrefs.GetInt("port", 0).ToString();
		UpdatePort();
	}

	void Update() {
		Quaternion q = Input.gyro.attitude;
		Send(string.Format("{0},{1},{2},{3}\n", q.x, q.y, q.z, q.w));
	}

	void OnDestroy() {
	}

	private void Send(string message) {
		if (ipAddress == null || ipAddress.Length == 0 || port == 0) {
			return;
		}

		Debug.Log(string.Format("Send to {0}:{1}. Message: {2}", ipAddress, port, message));

		byte[] data = Encoding.UTF8.GetBytes(message);

		UdpClient udpClient = new UdpClient();
		udpClient.Send(data, data.Length, ipAddress, port);
	}
}

}