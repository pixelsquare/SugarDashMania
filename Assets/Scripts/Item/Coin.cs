using UnityEngine;
using System.Collections;

public class Coin : BaseItemEntity {
	[SerializeField]
	private int score = 1000;

	private bool triggerOnce = true;

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();
	}

	protected override void CleanItem() {
		AddScore(score);
		base.CleanItem();
	}

	public void DestroyCoin() {
		Destroy(collider2D);
		anim.SetTrigger("Remove");
	}

	protected override void OnTriggerEnter2D(Collider2D col) {
		base.OnTriggerEnter2D(col);
		if (col.tag == "Player" && triggerOnce && !col.collider2D.isTrigger) {
			DestroyCoin();
			triggerOnce = false;
		}
	}
}
