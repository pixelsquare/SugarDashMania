using UnityEngine;
using System.Collections;

public class ConnectionStatus : MonoBehaviour {

	[SerializeField]
	private BaseSceneEntity scene;

	[SerializeField]
	private float waitingTime = 3f;

	//[SerializeField]
	//private TextMesh determiningStatus;

	private Animator anim;
	private bool animateOnce = true;

	private void Awake() {
		anim = GetComponent<Animator>();

		anim.enabled = true;
		animateOnce = true;

		GameUtility.ChangeSortingLayerRecursively(transform, LayerManager.SortingLayerUiFront, LayerManager.SortingLayerUiBack);

		//StartCoroutine("ConnectionStatusUpdate");
	}

	private void Start() {
		gameObject.SetActive(false);
	}

	private void OnEnable() {
		anim.enabled = true;
		animateOnce = true;

		StopCoroutine("ConnectionStatusUpdate");
		StartCoroutine("ConnectionStatusUpdate");
	}

	private void OnDisable() {
		StopCoroutine("ConnectionStatusUpdate");
	}

	private IEnumerator ConnectionStatusUpdate() {
		while (anim.enabled) {
			if (animateOnce) {
				while (!ConnectionTest.ConnectionPing.isDone)
					yield return null;

				yield return new WaitForSeconds(waitingTime);

				if (ConnectionTest.HasInternetConnection)
					anim.SetBool("Has Internet", true);
				else
					anim.SetBool("No Internet", true);
				animateOnce = false;
			}

			yield return null;
		}
	}

	private void DestroyAnimator() {
		anim.SetBool("Has Internet", false);
		anim.SetBool("No Internet", false);
		anim.enabled = false;
	}

	//private void Text1() {
	//    determiningStatus.text = "Reaching Network .";
	//}

	//private void Text2() {
	//    determiningStatus.text = "Reaching Network ..";
	//}

	//private void Text3() {
	//    determiningStatus.text = "Reaching Network ...";
	//}
}
