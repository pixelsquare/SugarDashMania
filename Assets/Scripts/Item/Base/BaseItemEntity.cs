using UnityEngine;
using System.Collections;

public enum ItemType {
	BlueGumball,
	BlueJellybean,
	GreenGumball,
	RedGumball,
	RedJellybean,
	YellowGumball,
	BlueHardCandyMiC,
	RedHardCandyMiC,
	None
}

[RequireComponent(typeof(NetworkView))]
public class BaseItemEntity : MonoBehaviour {
	[SerializeField]
	private ItemType item;
	public ItemType Item {
		get { return item; }
		set { item = value; }
	}

	[SerializeField]
	protected GameObject indicator;

	[SerializeField]
	protected AudioClip audioClip;

	protected bool showIndicator = true;
	public bool ShowIndicator {
		get { return showIndicator; }
		set { showIndicator = value; }
	}

	protected NetworkManager networkManager;
	protected Animator anim;
	protected UserInterface userInterface;

	protected BaseCharacterEntity hitHeroEntity;
	protected CharacterEffects hitHeroEffects;
	protected CharacterThrow hitHeroThrowItem;

	protected NetworkPlayer owner;

	protected AudioSource audioSource;

	private Vector3 newPos;

	protected virtual void Awake() {
		GetComponent<NetworkView>().observed = this;
		networkManager = FindObjectOfType<NetworkManager>();
		userInterface = FindObjectOfType<UserInterface>();
		anim = GetComponent<Animator>();

		if (audioSource == null)
			audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
	}

	protected virtual void Start() {
		newPos = transform.position;
	}

	protected virtual void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Player") {
			hitHeroEntity = col.GetComponent<BaseCharacterEntity>();
			hitHeroEffects = col.GetComponent<CharacterEffects>();
			hitHeroThrowItem = col.GetComponent<CharacterThrow>();
		}
	}

	protected virtual void CleanItem() {
		if (Network.isServer) {
			Network.Destroy(gameObject);
			Network.RemoveRPCs(gameObject.GetComponent<NetworkView>().viewID);
			if (showIndicator)
				CallIndicator();
		}
		//networkView.RPC("RemoveItem", RPCMode.All);
	}

	protected IEnumerator PlaySoundAndRemove() {
		if (audioSource != null && audioClip != null) {
			audioSource.clip = audioClip;
			audioSource.PlayOneShot(audioClip);
			yield return new WaitForSeconds(audioClip.length);
		}

		anim.SetTrigger("Remove");
	}

	private void RemoveItem() {
		CleanItem();
	}

	protected void AddScore(int score) {
		if (Network.isServer && owner == Network.player) {
			networkManager.GetComponent<NetworkView>().RPC("AddScore", RPCMode.All, hitHeroEntity.Owner, score);
		}
	}

	private void CallIndicator() {
		// If nougat is triggerd by client machine
		// call an RPC to everyone, to locate for the server
		// to spawn the indicator
		if (Network.isClient)
			GetComponent<NetworkView>().RPC("SpawnIndicator", RPCMode.All, Network.player);

		// If the nougat is triggered by server machine
		// Instantiate immediately
		if (Network.isServer && indicator != null)
			Network.Instantiate(indicator, transform.position, Quaternion.identity, NetworkGroup.Indicator);
	}

	[RPC]
	private void SetOwner(NetworkPlayer id) {
		owner = id;
	}

	[RPC]
	private void SpawnIndicator(NetworkPlayer id) {
		if (Network.isServer && Network.player == id && indicator != null) {
			Network.Instantiate(indicator, transform.position, Quaternion.identity, NetworkGroup.Indicator);
		}
	}

	protected virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			Vector3 tmpPos = transform.position;
			stream.Serialize(ref tmpPos);
		}
		else {
			stream.Serialize(ref newPos);
			transform.position = newPos;
		}
	}

	//[RPC]
	//private void RemoveItem() {
	//    if (Network.isServer) {
	//        Network.Destroy(gameObject);
	//    }
	//}

	//public void OnTriggerEnter2D(Collider2D col) {
	//    //if (col.tag == "Player") {
	//    //    BaseCharacterEntity heroEntity = col.GetComponent<BaseCharacterEntity>();
	//    //    heroEntity.CurItem = item;
	//    //    anim.SetTrigger("Remove");
	//    //}
	//}
}
