using UnityEngine;
using System.Collections;

public class TaffyBomb : BaseEnvironmentEntity {

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Player") {
			anim.SetTrigger("Explode");
		}
	}
}
