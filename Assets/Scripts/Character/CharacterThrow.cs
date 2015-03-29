using UnityEngine;
using System.Collections;

public class CharacterThrow : MonoBehaviour {

	[SerializeField]
	private ItemThrow[] throwItems;
	public ItemThrow[] ThrowItems {
		get { return throwItems; }
		set { throwItems = value; }
	}

	[SerializeField]
	private ItemPickup[] pickUpItems;
	public ItemPickup[] PickUpItems {
		get { return pickUpItems; }
		set { pickUpItems = value; }
	}

	//[SerializeField]
	//private float buffTime = 1.5f;

	private bool canShoot = false;
	public bool CanShoot {
		get { return canShoot; }
		set { canShoot = value; }
	}

	private bool canPickItem = true;
	public bool CanPickItem {
		get { return canPickItem; }
		set { canPickItem = value; }
	}

	//private bool canPickItem = true;
	//public bool CanPickItem {
	//    get {
	//        if (!canPickItem && heroEntity.CurItem != ItemType.None)
	//            StartCoroutine("SpawnPreviewsItem");

	//        return canPickItem; 
	//    }
	//    set { canPickItem = value; }
	//}

	//private ItemType prevItem;
	//private bool spawnOnce = true;

	private BaseCharacterEntity heroEntity;
	//private NetworkManager networkManager;

	private void Awake() {
		heroEntity = GetComponent<BaseCharacterEntity>();
		//networkManager = FindObjectOfType<NetworkManager>();
	}

	public void Throw() {
		if (canShoot) {
			networkView.RPC("SpawnItemThrow", RPCMode.All, (int)heroEntity.CurItem);
		}
	}

	//public void Throw() {
	//    if (canFire) {
	//        networkView.RPC("SpawnItemThrow", RPCMode.All, (int)heroEntity.CurItem);
	//        heroEntity.CurItem = ItemType.None;
	//        canFire = false;
	//        canPickItem = true;
	//    }
	//}

	//private void Start() {
	//    StartCoroutine("UpdateThrowItem");
	//}

	//private IEnumerator UpdateThrowItem() {
	//    while (enabled) {
	//        if (Input.GetKeyDown(KeyCode.E) && Network.player == networkManager.PlayerHeroEntity.Owner && 
	//            heroEntity.CurItem != ItemType.None && canFire) {

	//            networkView.RPC("SpawnItemThrow", RPCMode.All, (int)heroEntity.CurItem);
	//            heroEntity.CurItem = ItemType.None;
	//            canFire = false;
	//        }

	//        yield return null;
	//    }

	//    StopCoroutine("UpdateThrowItem");
	//}

	//private IEnumerator SpawnPreviewsItem() {
	//    if (spawnOnce) {
	//        prevItem = heroEntity.CurItem;
	//        canPickItem = true;
	//        yield return new WaitForSeconds(0.1f);
	//        canPickItem = false;
	//        networkView.RPC("SpawnItemPickup", RPCMode.All, (int)prevItem);

	//        StartCoroutine("ResetSpawn");
	//        spawnOnce = false;
	//    }
	//}

	//private IEnumerator ResetSpawn() {
	//    yield return new WaitForSeconds(buffTime);
	//    spawnOnce = true;
	//}

	//[RPC]
	//private void SetCanPickItem(bool canPick) {
	//    canPickItem = canPick;
	//}

	[RPC]
	private void SpawnItemThrow(int indx) {
		if (Network.isServer) {
			GameObject item = (GameObject)Network.Instantiate(throwItems[indx].gameObject, transform.position, Quaternion.identity, NetworkGroup.Item);
			item.GetComponent<ItemThrow>().MoveDir = new Vector2(transform.localScale.x, 0f);
			item.GetComponent<ItemThrow>().networkView.RPC("SetOwner", RPCMode.All, heroEntity.Owner);
		}

		heroEntity.CurItem = ItemType.None;
		canShoot = false;
		canPickItem = true;
	}

	//[RPC]
	//private void SpawnItemPickup(int indx) {
	//    if (Network.isServer) {
	//        Network.Instantiate(pickUpItems[indx].gameObject, transform.position, Quaternion.identity, NetworkGroup.Item);
	//    }
	//}
}
