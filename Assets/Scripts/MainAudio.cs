using UnityEngine;
using System.Collections;

public class MainAudio : MonoBehaviour {

	[SerializeField]
	private AudioClip mainClip;

	[SerializeField]
	private AudioClip inGameClip;

	private BaseUIEntity baseUI;
	private GameState gameState;
	private AudioSource audioSource;
	private bool invokeOnce = false;

	private void Awake() {
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
			Destroy(gameObject);

		audioSource = GetComponent<AudioSource>();
	}

	private void Start() {
		baseUI = FindObjectOfType<BaseUIEntity>();
		gameState = GameState.InGame;

		if (audioSource == null)
			audioSource = gameObject.AddComponent<AudioSource>();

		audioSource.loop = true;
	}

	private void Update() {

		if (gameState != baseUI.Scene)
			invokeOnce = true;

		if (invokeOnce) {
			gameState = baseUI.Scene;
			invokeOnce = false;

			if (gameState == GameState.InGame) {
				if (audioSource.clip != inGameClip) {
					audioSource.clip = inGameClip;
					audioSource.Play();
				}
			}
			else {
				if (audioSource.clip != mainClip) {
					audioSource.clip = mainClip;
					audioSource.Play();
				}
			}
		}
	}
}
