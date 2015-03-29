using UnityEngine;
using System.Collections;

public class BaseButtonEntity : MonoBehaviour {

	[SerializeField]
	private AudioClip[] buttonClips;

	private AudioSource audioSource;

	private SpriteRenderer spriteRend;
	private Sprite mainTex;
	private Color mainColor;

	protected Animator anim;
	protected BaseUIEntity baseUI;
	protected UserInterface userInterface;
	protected NetworkManager networkManager;

	private bool clickable = true;
	public bool Clickable {
		get { return clickable; }
		set {
			clickable = value;

			if (clickable) {
				anim.enabled = true;
				Color tmpColor = spriteRend.color;
				tmpColor.a = 1f;
				spriteRend.color = tmpColor;
			}
			else {
				anim.enabled = false;
				Color tmpColor = spriteRend.color;
				tmpColor.a = 0.2f;
				spriteRend.color = tmpColor;
			}
		}
	}

	private bool buttonHover;
	public bool ButtonHover {
		get { return buttonHover; }
		set {
			buttonHover = value;
			if (anim != null)
				anim.SetBool("Hover", buttonHover);
		}
	}

	private bool buttonActive;
	public bool ButtonActive {
		get { return buttonActive; }
		set {
			buttonActive = value;
			if (anim != null)
				anim.SetBool("Active", buttonActive);
		}
	}

	private void Awake() {
		anim = GetComponent<Animator>();
		baseUI = FindObjectOfType<BaseUIEntity>();
		userInterface = FindObjectOfType<UserInterface>();
		networkManager = FindObjectOfType<NetworkManager>();
	}

	protected virtual void OnEnable() {
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
			audioSource = gameObject.AddComponent<AudioSource>();

		audioSource.playOnAwake = false;
		audioSource.loop = false;

		spriteRend = GameUtility.GetSpriteRendererRecursively(transform);
		if (spriteRend != null) {
			mainTex = spriteRend.sprite;
			mainColor = spriteRend.color;
		}		
	}

	public void ResetButton() {
		if (spriteRend != null) {
			spriteRend.sprite = mainTex;
			spriteRend.color = mainColor;
		}
		ButtonHover = false;
	}

	public virtual void Clicked(bool clicked) {
		if (clicked) {
			ButtonHover = false;
			ButtonActive = true;
		}
		else {
			ButtonHover = true;
			ButtonActive = false;
		}
	}

	protected virtual IEnumerator AnimationRoutine(GameState state) {
		if (buttonClips.Length > 0) {
			int randClipIndx = Random.Range(0, buttonClips.Length - 1);
			audioSource.clip = buttonClips[randClipIndx];
			audioSource.Play();
			yield return new WaitForSeconds(audioSource.clip.length);
		}
	}

	private void OnMouseEnter() {
		if (Clickable)
			ButtonHover = true;
	}

	private void OnMouseExit() {
		if (Clickable)
			ButtonHover = false;
	}

	private void OnMouseUp() {
		if (Clickable)
			Clicked(false);
	}

	private void OnMouseDown() {
		if (Clickable)
			Clicked(true);
	}
}