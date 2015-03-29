using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	[SerializeField]
	private Transform target;
	public Transform Target {
		get { return target; }
		set { target = value; }
	}

	[SerializeField]
	private Vector3 targetScreenPos = new Vector3(0.5f, 0.5f, 0.0f);

	[SerializeField]
	private float damping = 0.3f;

	[SerializeField]
	private float zOffset = -10f;

	private Vector3 velocity;
	public Vector3 Velocity {
		get { return velocity; }
	}

	private Camera cam;

	private void Start() {
		//transform.name = target.name + "'s Camera";
		cam = GetComponent<Camera>();
	}

	private void LateUpdate() {
		if (target && cam != null) {
			Vector3 delta = target.position - cam.ViewportToWorldPoint(targetScreenPos);
			Vector3 destination = transform.position + delta;
			destination.z = zOffset;

			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, damping);
		}
	}
}
