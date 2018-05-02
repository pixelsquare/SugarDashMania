using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class RandomItemSpawner : MonoBehaviour {
	[SerializeField]
	private ItemPickup[] items;
	int randIndx = 0;

	private void Awake() {
		GetComponent<NetworkView>().observed = this;
		randIndx = Random.Range(0, items.Length - 1);
	}

	private void Start() {
		if (Network.isServer) {
			GetComponent<NetworkView>().RPC("SpawnItemPickup", RPCMode.All, randIndx);
			Network.Destroy(gameObject);
		}
	}

	[RPC]
	private void SpawnItemPickup(int indx) {
		if (Network.isServer) {
			Network.Instantiate(items[indx], transform.position, Quaternion.identity, NetworkGroup.Item);
		}
	}
}
