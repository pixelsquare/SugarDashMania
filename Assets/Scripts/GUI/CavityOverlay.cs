using UnityEngine;
using System.Collections;

public class CavityOverlay : MonoBehaviour {
	[SerializeField]
	private Transform overlayBase;

	[SerializeField]
	private Transform overlayMain;

	[SerializeField]
	private float cur = 0f;

	[SerializeField]
	private float max = 120f;

	private float scaleModifierH = 1f;
	private Vector3 overlayScaleH;
	private Vector3 offsetH;

	private SpriteRenderer myRenderer;
	//private SpriteRenderer baseRenderer;
	//private SpriteRenderer mainRenderer;

	private Color origColor;
	//private Color origMainColor;

	private void Awake() {
		myRenderer = GetComponent<SpriteRenderer>();
		//baseRenderer = overlayBase.GetComponent<SpriteRenderer>();
		//mainRenderer = overlayMain.GetComponent<SpriteRenderer>();

		origColor = myRenderer.color;
		//origMainColor = mainRenderer.color;
	}

	private void Start() {
		if (overlayBase == null) {
			if (overlayMain) {
				overlayBase.position = overlayMain.position;
			}

			overlayBase.renderer.enabled = false;
			overlayBase.parent = transform;
		}

		if (overlayMain != null) {
			scaleModifierH = GameUtility.GetWorldScale(overlayMain).x;
			overlayMain.parent = overlayBase;
			overlayScaleH = overlayMain.localScale;
			offsetH = overlayMain.localPosition;
		}

		StartCoroutine("UpdateOV");
		UpdateOverlay();
	}

	public void AddMeter(int num) {
		cur += num;
		UpdateOverlay();
	}

	//private void Update() {
	//    if (Input.GetKeyDown(KeyCode.B)) {
	//        cur--;
	//        UpdateOverlay();
	//    }

	//    if (Input.GetKeyDown(KeyCode.N)) {
	//        cur++;
	//        UpdateOverlay();
	//    }
	//}

	private void UpdateOverlay() {
		if (overlayMain) {
			cur = Mathf.Clamp(cur, 0f, max);
		}

		if (overlayMain) {
			Vector2 scale = new Vector2(cur / max * overlayScaleH.x, overlayScaleH.y);
			overlayMain.localScale = scale;
		}
	}

	private IEnumerator UpdateOV() {
		if (overlayMain == null)
			yield break;

		while (true) {
			if (overlayMain || overlayBase) {
				Vector2 dirRIght = overlayBase.TransformDirection(-Vector2.right);

				overlayMain.localPosition = offsetH;
				float dist = ((max - cur) / max) * scaleModifierH * 0.75f;
				myRenderer.color = Color.Lerp(Color.red, origColor, dist);
				overlayMain.Translate(dirRIght * dist);
			}

			yield return null;
		}
	}

	public bool isMax() {
		return cur >= max;
	}

	public void Reset() {
		cur = 0f;
		myRenderer.color = origColor;
		UpdateOverlay();
	}
}
