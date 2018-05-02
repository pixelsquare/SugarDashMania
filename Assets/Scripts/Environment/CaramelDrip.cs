using UnityEngine;
using System.Collections;

public class CaramelDrip : BaseEnvironmentEntity {
	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Pond") {
			anim.SetTrigger("Dissolve");
			GetComponent<Rigidbody2D>().gravityScale = 0f;
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			//if (Mathf.Abs(rigidbody2D.velocity.y) > 0)
			//    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, Mathf.Sign(rigidbody2D.velocity.y) * 0.01f);
		}
	}
}
