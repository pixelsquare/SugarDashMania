using UnityEngine;
using System.Collections;

public class JellyBounce : BaseEnvironmentEntity {
	[SerializeField]
	private float upwardForce = 20000f;

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();
	}

	private void OnCollisionEnter2D(Collision2D col) {
		if (col.transform.tag == "Player" && (col.contacts[0].normal == -Vector2.up && col.contacts[0].normal != Vector2.right)) {
			Rigidbody2D rb2D = col.transform.GetComponent<Rigidbody2D>();

			if (Mathf.Sign(rb2D.velocity.y) > 0f)
				rb2D.GetComponent<Rigidbody2D>().AddForce(Vector2.up * upwardForce);

			//BaseCharacterEntity heroEntity = col.transform.GetComponent<BaseCharacterEntity>();

			//if (Mathf.Abs(heroEntity.rigidbody2D.velocity.y) > 0)
			//    heroEntity.rigidbody2D.AddForce(Vector3.up * upwardForce * 2000f);
		}
	}
}
