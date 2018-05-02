using UnityEngine;
using System.Collections;

public class RandomItemCrate : BaseItemEntity {
	[SerializeField]
	private ItemPickup[] randomItemDrop;

	private int randItemIndx = 0;

	protected override void Awake() {
		randItemIndx = Random.Range(0, randomItemDrop.Length);
		base.Awake();
	}

	protected override void Start() {
		base.Start();
	}

	protected override void CleanItem() {
		base.CleanItem();

		if (Network.isClient)
			GetComponent<NetworkView>().RPC("SpawnItem", RPCMode.All);

		if (Network.isServer && randomItemDrop[randItemIndx] != null)
			Network.Instantiate(randomItemDrop[randItemIndx].gameObject, transform.position, Quaternion.identity
				, NetworkGroup.Item);
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
	private void SpawnItem() {
		if (Network.isServer && randomItemDrop[randItemIndx] != null) {
			Network.Instantiate(randomItemDrop[randItemIndx].gameObject, transform.position, Quaternion.identity
				, NetworkGroup.Item);
		}
	}
}
