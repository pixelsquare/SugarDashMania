using UnityEngine;
using System.Collections;

public class PeperoMatchaGate : BaseEnvironmentEntity {
	[SerializeField]
	private Transform gate;

	[SerializeField]
	private float liftHeight = 5f;

	[SerializeField]
	private float liftDuration = 2.5f;
	
	private bool isOpen;
	private bool isClosed;

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		isClosed = true;
		isOpen = false;
		base.Start();
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Boulder") {
			isClosed = true;
			isOpen = false;

			if(isClosed && !isOpen) {
				StopAllCoroutines();
				StartCoroutine("OpenGate");
			}
		}
	}

	private IEnumerator OpenGate() {
		isClosed = false;
		isOpen = true;

		float tmpTime = 0f;
		float tmpDuration = liftDuration;
		Vector3 tmpPos = gate.localPosition;

		while (tmpDuration > 0f) {
			tmpDuration -= Time.deltaTime;
			tmpTime += Time.deltaTime;

			tmpPos = Vector3.Lerp(tmpPos, Vector3.up * liftHeight, tmpTime);
			gate.localPosition = tmpPos;

			yield return null;
		}

		if (!isClosed && isOpen)
			StartCoroutine("CloseGate");
	}

	private IEnumerator CloseGate() {
		isClosed = true;
		isOpen = false;

		float tmpTime = 0f;
		float tmpDuration = liftDuration;
		Vector3 tmpPos = gate.localPosition;

		while (tmpDuration > 0f) {
			tmpDuration -= Time.deltaTime;
			tmpTime += Time.deltaTime;

			tmpPos = Vector3.Lerp(tmpPos, Vector3.zero, tmpTime);
			gate.localPosition = tmpPos;

			yield return null;
		}
	}
}
