using UnityEngine;
using System.Collections;

public class PlatformMove : BaseEnvironmentEntity {
	[SerializeField]
	private Transform point1;

	[SerializeField]
	private Transform point2;

	[SerializeField]
	private Transform platform;

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private float offset = 0f;

	private Vector3 newPos;

	protected override void Awake() {
		duration = Mathf.Clamp(duration, 0f, Mathf.Infinity);
		offset = Mathf.Clamp(offset, 0f, duration * 2f);
		base.Awake();
	}

	protected override void Start() {
		newPos = transform.position;
		StartCoroutine("UpdatePlatform");
		base.Start();
	}

	protected override void RemoveItem() {
		base.RemoveItem();
	}

	private IEnumerator UpdatePlatform() {
		while (true) {
			platform.localPosition = Vector3.Lerp(point1.localPosition, point2.localPosition, Mathf.PingPong(offset + Time.time, duration) / duration);
			yield return null;
		}
	}

	protected virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			Vector3 tmpPos = transform.position;
			stream.Serialize(ref tmpPos);
		}
		else {
			stream.Serialize(ref newPos);
			transform.position = newPos;
		}
	}

	//private void OnDrawGizmos() {
	//    Gizmos.DrawLine(point1.position, point2.position);
	//}
}
