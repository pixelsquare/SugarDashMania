using UnityEngine;
using System.Collections;

public class CandyCaneHook : BaseEnvironmentEntity {

	[SerializeField]
	private Transform mainTexture;

	[SerializeField]
	private float pullHeight = 5f;

	[SerializeField]
	private float liftDuration = 2.5f;

	private bool isOpen;
	public bool IsOpen {
		get { return isOpen; }
	}

	private bool isClosed;
	public bool IsClosed {
		get { return isClosed; }
	}

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		isClosed = false;
		isOpen = true;
		base.Start();
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Player") {
			if (!isClosed && isOpen) {
				StopAllCoroutines();
				StartCoroutine("PullHook");
			}
		}
	}

	private IEnumerator PullHook() {
		isClosed = true;
		isOpen = false;

		float tmpTime = 0f;
		float tmpDuration = liftDuration;
		Vector3 tmpPos = mainTexture.localPosition;

		while (tmpDuration > 0f) {
			tmpDuration -= Time.deltaTime;
			tmpTime += Time.deltaTime;

			tmpPos = Vector3.Lerp(tmpPos, Vector3.down * pullHeight, tmpTime);
			mainTexture.localPosition = tmpPos;

			yield return null;
		}

		if (isClosed && !isOpen)
			StartCoroutine("ReleaseHook");
	}

	private IEnumerator ReleaseHook() {
		isClosed = false;
		isOpen = true;

		float tmpTime = 0f;
		float tmpDuration = liftDuration;
		Vector3 tmpPos = mainTexture.localPosition;

		while (tmpDuration > 0f) {
			tmpDuration -= Time.deltaTime;
			tmpTime += Time.deltaTime;

			tmpPos = Vector3.Lerp(tmpPos, Vector3.zero, tmpTime);
			mainTexture.localPosition = tmpPos;

			yield return null;
		}
	}
}
