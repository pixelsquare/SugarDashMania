using UnityEngine;
using System.Collections;

public class JawbreakerBoulder : BaseEnvironmentEntity {
	[SerializeField]
	private float torque = 500f;

	[SerializeField]
	private float forwardForce = 500f;

	[SerializeField]
	private float duration = 10f;

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		GetComponent<Rigidbody2D>().AddTorque(torque);
		GetComponent<Rigidbody2D>().AddForce(Vector3.right * -forwardForce);
		StartCoroutine("UpdateJawbreaker");
		base.Start();
	}

	private IEnumerator UpdateJawbreaker() {
		yield return new WaitForSeconds(duration);
		anim.SetTrigger("Collapse");
	}
}
