using UnityEngine;
using System.Collections;

public class Pond : BaseEnvironmentEntity {
	[SerializeField]
	private Slow slow;

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();
	}

	protected override void RemoveItem() {
		base.RemoveItem();
	}

	private void OnCollisionEnter2D(Collision2D col) {
		if (col.transform.tag == "Player") {
			CharacterEffects heroEffects = col.transform.GetComponent<CharacterEffects>();
			heroEffects.ApplySlow(slow);
		}
	}
}
