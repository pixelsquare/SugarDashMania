using UnityEngine;
using System.Collections;

public class LevelScreen : MonoBehaviour {

	[SerializeField]
	private GameObject loadingIndicator;
	public GameObject LoadingIndicator {
		get { return loadingIndicator; }
	}

	[SerializeField]
	private Animator loadingPivotAnim;

	[SerializeField]
	private ParticleSystem loadingParticle;

	[SerializeField]
	private TextMesh loadingText;

	[SerializeField]
	private Sprite[] loadingScreens;
	public Sprite[] LoadingScreens {
		get { return loadingScreens; }
		set { loadingScreens = value; }
	}

	private SpriteRenderer spriteRend;
	private Animator anim;
	private NetworkManager networkManager;

	private void Awake() {
		spriteRend = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		networkManager = FindObjectOfType<NetworkManager>();
	}

	public void LevelScreenControl(bool enable) {
		if (spriteRend.enabled != enable) {
			spriteRend.sprite = loadingScreens[networkManager.LoadingScreenIndx];
			spriteRend.enabled = enable;
		}

		if (anim.enabled != enable)
			anim.enabled = enable;

		if (loadingParticle != null && loadingParticle.GetComponent<Renderer>().enabled != enable)
			loadingParticle.GetComponent<Renderer>().enabled = enable;

		if (loadingText != null && loadingText.GetComponent<Renderer>().enabled != enable)
			loadingText.GetComponent<Renderer>().enabled = enable;

		if (loadingPivotAnim != null && loadingPivotAnim.enabled != enable)
			loadingPivotAnim.enabled = enable;
	}

	public void StartGame() {
		anim.SetTrigger("Fade In");
	}

	private void DisableScreen() {
		LevelScreenControl(false);
		networkManager.MoveCameraToPlayer();
	}
}
