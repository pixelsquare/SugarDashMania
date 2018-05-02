using UnityEngine;
using System.Collections;

public class CoinCrate : BaseItemEntity {

	[SerializeField]
	private GameObject coin;

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();
	}

	protected override void CleanItem() {
		base.CleanItem();

		if (Network.isClient)
			GetComponent<NetworkView>().RPC("SpawnCoin", RPCMode.All);

		if (Network.isServer && coin != null)
			Network.Instantiate(coin, transform.position, Quaternion.identity, NetworkGroup.Indicator);
	}

	protected override void OnTriggerEnter2D(Collider2D col) {
		base.OnTriggerEnter2D(col);
		if (col.tag == "Item") {
			anim.SetTrigger("Destroy Crate");
			col.GetComponent<ItemThrow>().DestroyItem();
			col.GetComponent<ItemThrow>().ShowIndicator = false;
		}
	}

	[RPC]
	private void SpawnCoin() {
		if (Network.isServer && coin != null) {
			Network.Instantiate(coin, transform.position, Quaternion.identity, NetworkGroup.Item);
		}
	}
}
