using UnityEngine;
using System.Collections;

public class ParallaxBG : MonoBehaviour {
	[SerializeField]
	private ParallaxBG partner;

	[SerializeField]
	private float speed;

	[SerializeField]
	private bool playerInputBase;
	public bool PlayerInputBase {
		get { return playerInputBase; }
	}

	[SerializeField]
	private bool spectateMode = false;
	public bool SpectateMode {
		get { return spectateMode; }
		set { spectateMode = value; }
	}

	[SerializeField]
	private Vector2 moveDirection = Vector2.right;

	//[SerializeField]
	private Camera cam;
	public Camera Cam {
		get { return cam; }
		set { cam = value; }
	}

	private Rigidbody2D playerRB;
	public Rigidbody2D PlayerRB {
		get { return playerRB; }
		set { playerRB = value; }
	}

	private NetworkManager networkManager;

	private void Awake() {
		networkManager = FindObjectOfType<NetworkManager>();
	}

	private void FixedUpdate() {
		if (cam != null) {

			if (playerInputBase) {
				if (playerRB != null) {
					if (playerRB.velocity.normalized.x != 0f)
						moveDirection.x = -playerRB.velocity.normalized.x;
					else
						moveDirection = Vector2.zero;
				}
			}
			else {
				if (spectateMode && networkManager.PlayerHeroEntity.HasEnded && !networkManager.GameEnd) {
					if (Input.GetMouseButton(0) && Input.GetAxis("Mouse X") != 0f)
						moveDirection.x = -Input.GetAxis("Mouse X");
					else if (Input.GetAxis("Horizontal") != 0f)
						moveDirection.x = -Input.GetAxis("Horizontal");
				}
			}

			transform.Translate(moveDirection * speed * Time.deltaTime);

			Vector2 objectCorner = transform.position;
			Vector2 multiplier = new Vector2();

			Vector2 objectSize = new Vector2();
			objectSize = GetComponent<SpriteRenderer>().bounds.size;
			//objectSize.Scale(transform.localScale);

			if (moveDirection.x != 0f) {
				multiplier.x = moveDirection.x / Mathf.Abs(moveDirection.x);
				objectCorner.x += multiplier.x * objectSize.x / -2f;
			}

			Vector3 viewportCoord = cam.WorldToViewportPoint(objectCorner);

			Vector2 newPos = transform.position;
			Vector2 partnerSize = new Vector2();
			partnerSize = GetComponent<SpriteRenderer>().bounds.size;
			partnerSize.x = partnerSize.x - 0.2f;
			//partnerSize.Scale(transform.localScale);

			if (multiplier.x != 0f) {
				if (multiplier.x > 0f && viewportCoord.x > 1f)
					newPos.x = partner.transform.position.x - (partnerSize.x + objectSize.x) / 2f;
				else if (multiplier.x < 0f && viewportCoord.x < 0f)
					newPos.x = partner.transform.position.x + (partnerSize.x + objectSize.x) / 2f;
			}

			transform.position = newPos;
		}
	}
}
