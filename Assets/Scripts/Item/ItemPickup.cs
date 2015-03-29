using UnityEngine;
using System.Collections;

public class ItemPickup : BaseItemEntity {

	private Transform hit;

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();
	}

	protected override void OnTriggerEnter2D(Collider2D col) {
		base.OnTriggerEnter2D(col);
		if (hitHeroThrowItem != null && hitHeroEntity.CurItem == ItemType.None) {
			if (col.tag == "Player" && hitHeroThrowItem.CanPickItem) {
				hit = col.transform;
				Destroy(collider2D);
				StartCoroutine("PlaySoundAndRemove");
			}
		}
	}

	protected override void CleanItem() {
		if (Network.isServer && hit != null)
			networkView.RPC("PlayerCollided", RPCMode.All, hit.networkView.viewID);
		base.CleanItem();
	}

	[RPC]
	private void PlayerCollided(NetworkViewID id) {
		GameObject player = NetworkView.Find(id).gameObject;
		BaseCharacterEntity playerEntity = player.GetComponent<BaseCharacterEntity>();
		playerEntity.CurItem = Item;
		
		CharacterThrow playerThrowItem = player.GetComponent<CharacterThrow>();
		playerThrowItem.CanShoot = true;
		playerThrowItem.CanPickItem = false;
	}
}
