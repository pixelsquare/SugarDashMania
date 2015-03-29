using UnityEngine;
using System.Collections;

public class BrowniePlatform : BaseEnvironmentEntity {

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();
	}

	private void OnCollisionEnter2D(Collision2D col) {
		if (col.transform.tag == "Player")
			anim.SetTrigger("Collapse");
	}
}
