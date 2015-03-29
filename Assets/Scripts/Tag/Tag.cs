using UnityEngine;
using System.Collections;

public class Tag : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer[] spriteRenderers;
	public SpriteRenderer[] SpriteRenderers {
		get { return spriteRenderers; }
	}

	[SerializeField]
	private AudioClip targetLockClip;

	[SerializeField]
	private AudioClip targetLostClip;

	private float duration = 3f;
	public float Duration {
		get { return duration; }
		set { duration = value; }
	}

	private bool outOfCameraBounds;
	private NetworkPlayer owner;

	private Animator anim;
	private AudioSource audioSource;
	private NetworkManager networkManager;

	private void Awake() {
		networkView.observed = this;
		anim = GetComponent<Animator>();
		networkManager = FindObjectOfType<NetworkManager>();
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
			audioSource = gameObject.AddComponent<AudioSource>();

		audioSource.playOnAwake = false;

		outOfCameraBounds = false;

		StartCoroutine("TagUpdate");
	}

	private void Start() {
		audioSource.clip = targetLockClip;
		if (owner == Network.player) {
			audioSource.Play();
		}
	}

	private void FixedUpdate() {
		Vector3 viewport = networkManager.PlayerHeroEntity.MyCamera.WorldToViewportPoint(transform.position);

		if (!outOfCameraBounds && viewport.x < 0f || viewport.x > 1f || viewport.y < 0f || viewport.y > 1f) {
			StopCoroutine("TagUpdate");
			networkView.RPC("RemoveTag", RPCMode.All);
			outOfCameraBounds = true;
		}
	}

	private IEnumerator TagUpdate() {
		while (duration > 0f) {
			duration -= Time.deltaTime;
			yield return null;
		}

		networkView.RPC("RemoveTag", RPCMode.All);
	}

	private IEnumerator TargetLost() {
		transform.parent = networkManager.PlayerHero.transform;
		transform.localPosition = Vector3.zero;
		audioSource.clip = targetLostClip;

		if (owner == Network.player) {
			audioSource.Play();
		}

		anim.enabled = false;
		SetColor(new Color(0f, 0f, 0f, 0f));

		yield return new WaitForSeconds(audioSource.clip.length);

		networkView.RPC("RemoveObj", RPCMode.All);
	}

	public void SetColor(Color col) {
		foreach (SpriteRenderer element in spriteRenderers) {
			element.color = col;
		}
	}

	[RPC]
	private void RemoveTag() {
		StartCoroutine("TargetLost");
	}

	[RPC]
	private void RemoveObj() {
		if (NetworkView.Find(networkView.viewID) != null) {
			if (Network.isServer) {
				Network.Destroy(networkView.viewID);
				Network.RemoveRPCsInGroup(NetworkGroup.PlayerTag);
			}
		}
	}

	[RPC]
	private void SetOwner(NetworkPlayer id) {
		owner = id;
	}
}
