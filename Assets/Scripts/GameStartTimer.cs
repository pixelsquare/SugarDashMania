using UnityEngine;
using System.Collections;

public class GameStartTimer : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer candy1;

	[SerializeField]
	private SpriteRenderer candy2;

	[SerializeField]
	private SpriteRenderer candy3;

	[SerializeField]
	private SpriteRenderer hover;

	[SerializeField]
	private AudioClip audioClip;

	private Animator anim;
	private NetworkManager networkManager;

	private AudioSource audioSource;

	private void Awake() {
		if (audioSource == null)
			audioSource = gameObject.AddComponent<AudioSource>();

		audioSource.playOnAwake = false;
	}

	private void Start() {
		//GameUtility.ChangeSortingLayerRecursively(candy1.transform, LayerManager.SortingLayerCharacterBack);
		//GameUtility.ChangeSortingLayerRecursively(candy2.transform, LayerManager.SortingLayerCharacterBack);
		//GameUtility.ChangeSortingLayerRecursively(candy3.transform, LayerManager.SortingLayerCharacterBack);
		//GameUtility.ChangeSortingLayerRecursively(hover.transform, LayerManager.SortingLayerCharacterBack);

		anim = GetComponent<Animator>();
		networkManager = FindObjectOfType<NetworkManager>();
		EnableTimer(false);
	}

	public void EnableTimer(bool activate) {
		if (anim.enabled != activate)
			anim.enabled = activate;

		if (candy1.enabled != activate)
			candy1.enabled = activate;

		if (candy2.enabled != activate)
			candy2.enabled = activate;

		if (candy3.enabled != activate)
			candy3.enabled = activate;

		if (hover.enabled != activate)
			hover.enabled = activate;
	}

	private void PlaySound() {
		audioSource.clip = audioClip;
		audioSource.Play();
	}

	private void StartGame() {
		if (Network.isServer) {
			networkManager.GameStart = true;
			networkManager.networkView.RPC("SetGameStart", RPCMode.All, networkManager.GameStart);
		}
	}

	private void DisableTimer() {
		EnableTimer(false);
	}
}
