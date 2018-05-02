using UnityEngine;
using System.Collections;

public class Tracker : MonoBehaviour {

	[SerializeField]
	private Camera cam;
	public Camera Cam {
		get { return cam; }
		set { cam = value; }
	}

	[SerializeField]
	private Transform target;
	public Transform Target {
		get { return target; }
		set { target = value; }
	}

	[SerializeField]
	private TextMesh trackerName;

	[SerializeField]
	private SpriteRenderer[] icons;

	private int iconIndx = 0;
	private string targetName;
	private NetworkManager networkManager;

	private float distance = 0f;

	private void Awake() {
		networkManager = FindObjectOfType<NetworkManager>();
		GameUtility.ChangeSortingLayerRecursively(trackerName.transform, LayerManager.SortingLayerUiBack);

		int i;
		for (i = 0; i < icons.Length; i++) {
			icons[i].enabled = false;
		}
	}

	private void Start() {
		if (cam == null || target == null)
			gameObject.SetActive(false);
	}

	private void FixedUpdate() {
		if (target.GetComponent<BaseCharacterEntity>().HasEnded) {
			target = null;
		}

		if (target == null) {
			gameObject.SetActive(false);
		}

		EnableRenderer(false);
		//renderer.enabled = false;

		Vector3 v3Pos = cam.WorldToViewportPoint(target.position);

		if (v3Pos.z < cam.nearClipPlane)
			return;  // Object is behind the camera

		if (v3Pos.x >= 0.0f && v3Pos.x <= 1.0f && v3Pos.y >= 0.0f && v3Pos.y <= 1.0f)
			return; // Object center is visible

		EnableRenderer(true);
		//renderer.enabled = true;

		distance = (networkManager.PlayerHeroEntity.transform.position - target.transform.position).magnitude;
		trackerName.text = targetName + "\n(" + distance.ToString("F1") + "m)";

		v3Pos.x -= 0.5f;  // Translate to use center of viewport
		v3Pos.y -= 0.5f;
		v3Pos.z = 0;      // I think I can do this rather than do a 
		//   a full projection onto the plane

		float fAngle = Mathf.Atan2(v3Pos.x, v3Pos.y);
		transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fAngle * Mathf.Rad2Deg);

		v3Pos.x = 0.45f * Mathf.Sin(fAngle) + 0.5f;  // Place on ellipse touching 
		v3Pos.y = 0.45f * Mathf.Cos(fAngle) + 0.5f;  //   side of viewport
		v3Pos.z = cam.nearClipPlane + 0.01f;  // Looking from neg to pos Z;
		transform.position = cam.ViewportToWorldPoint(v3Pos);
	}

	private void EnableRenderer( bool enable) {
		GetComponent<Renderer>().enabled = enable;
		trackerName.GetComponent<Renderer>().enabled = enable;
		icons[iconIndx].enabled = enable;
	}

	public void SetIcon(string playerName, int indx) {
		gameObject.SetActive(true);
		targetName = playerName;
		iconIndx = indx;
		icons[indx].enabled = true;
	}
}
