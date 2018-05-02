using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	[SerializeField]
	private Vector2 horizontalBounds = new Vector2(-59f, 105f);

	[SerializeField]
	private Vector2 verticalBounds = new Vector2(0f, 27f);

	[SerializeField]
	private TextMesh spectatingText;
	public TextMesh SpectatingText {
		get { return spectatingText; }
	}

	[SerializeField]
	private SpriteRenderer spectatingHover;

	[SerializeField]
	private float speed = 5f;

	private NetworkManager networkManager;

	private void Awake() {
		networkManager = FindObjectOfType<NetworkManager>();
		spectatingText.GetComponent<Renderer>().sortingLayerName = LayerManager.SortingLayerUiFront;
		spectatingText.GetComponent<Renderer>().sortingOrder = 1;
		enabled = false;
	}

	public void OnEnable() {
		spectatingText.GetComponent<Renderer>().enabled = true;
		spectatingHover.enabled = true;
	}

	public void OnDisable() {
		spectatingText.GetComponent<Renderer>().enabled = false;
		spectatingHover.enabled = false;
	}

	private void FixedUpdate() {

		if (networkManager.GameEnd)
			enabled = false;

		//spectatingText.renderer.enabled = transform.root.camera.enabled;

		if (Input.GetMouseButton(0)) {
			transform.Translate(Vector3.right * -Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f) * speed * 0.02f);
			Vector3 pos = transform.position;
			pos.x = Mathf.Clamp(pos.x, horizontalBounds.x, horizontalBounds.y);
			transform.position = pos;
		}

		if (Input.GetMouseButton(1)) {
			transform.Translate(Vector3.up * -Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f) * speed * 0.02f);
			Vector3 pos = transform.position;
			pos.y = Mathf.Clamp(pos.y, verticalBounds.x, verticalBounds.y);
			transform.position = pos;
		}

		if (Input.GetAxis("Horizontal") != 0f) {
			transform.Translate(Vector3.right * Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f) * speed * 0.02f);
			Vector3 pos = transform.position;
			pos.x = Mathf.Clamp(pos.x, horizontalBounds.x, horizontalBounds.y);
			transform.position = pos;
		}

		if (Input.GetAxis("Vertical") != 0f) {
			transform.Translate(Vector3.up * Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 1f) * speed * 0.02f);
			Vector3 pos = transform.position;
			pos.y = Mathf.Clamp(pos.y, verticalBounds.x, verticalBounds.y);
			transform.position = pos;
		}
	}
}
