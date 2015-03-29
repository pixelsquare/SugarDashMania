using UnityEngine;
using System.Collections;

public class IceFloor : BaseEnvironmentEntity {
	[SerializeField]
	private Sliding slide;

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
			heroEffects.ApplySliding(slide);
		}
	}
}
