using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	private NetworkManager networkManager;

	private void Awake() {
		networkManager = FindObjectOfType<NetworkManager>();
	}

	private void OnTriggerEnter2D(Collider2D col) {
		BaseCharacterEntity heroEntity = col.GetComponent<BaseCharacterEntity>();

		if (col.tag == "Player") {
			networkManager.GetComponent<NetworkView>().RPC("EndPlayer", RPCMode.All, heroEntity.Owner);
		}
	}
}
