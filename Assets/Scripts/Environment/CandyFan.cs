using UnityEngine;
using System.Collections;

public class CandyFan : BaseEnvironmentEntity {

	[SerializeField]
	private Transform windDirection;

	[SerializeField]
	private float pushForce = 10000f;

	[SerializeField]
	private float pullForceFactor = 0.2f;

	private Collider2D hit;
	private Rigidbody2D rigidBody2DHit;

	private GameObject objHit;

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		StartCoroutine("UpdateFan");
		base.Start();
	}

	private IEnumerator UpdateFan() {
		while (true) {
			hit = Physics2D.Linecast(transform.position, windDirection.position, 1 << LayerManager.LayerPlayer | 1 << LayerManager.LayerAlly | 1 << LayerManager.LayerEnemy).collider;

			if (hit != null) {
				objHit = hit.gameObject;
				objHit.GetComponent<BaseCharacterEntity>().CharPhysics.GravityModifier = 1f;
				Vector3 direction = windDirection.position - transform.position;
				direction.Normalize();
				rigidBody2DHit = hit.attachedRigidbody;

				if (rigidBody2DHit != null) {
					rigidBody2DHit.AddForce((direction * pushForce) + (-Vector3.up * pushForce * pullForceFactor));
				}
			}
			else {
				if (hit != objHit) {
					objHit.GetComponent<BaseCharacterEntity>().CharPhysics.GravityModifier = 10f;
					objHit = null;
				}
			}
			
			yield return null;
		}
	}
}
