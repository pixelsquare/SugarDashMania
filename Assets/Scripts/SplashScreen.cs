using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {
	[SerializeField]
	private BaseUIEntity baseUiEntity;

	private Animator anim;

	private void Awake() {
		anim = GetComponent<Animator>();
	}

	private IEnumerator Start() {
		yield return new WaitForSeconds(3f);
		anim.enabled = false;
		baseUiEntity.Scene = GameState.MainMenu;
	}
}
