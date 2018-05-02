using UnityEngine;
using System.Collections;

public class ItemThrow : BaseItemEntity {

	[SerializeField]
	private float speed = 5f;

	[SerializeField]
	private float torque = 50f;

	[SerializeField]
	private float duration = 3f;

	[SerializeField]
	private Layer targetLayer;

	[SerializeField]
	private Slow slow;

	[SerializeField]
	private Invulnerability invulnerable;

	[SerializeField]
	private SpeedIncrease speedIncrease;

	[SerializeField]
	private JumpIncrease jumpIncrease;

	[SerializeField]
	private Push push;

	[SerializeField]
	private Pull pull;

	
	private Vector3 newVel;

	private Vector2 moveDir;
	public Vector2 MoveDir {
		get { return moveDir; }
		set { moveDir = value; }
	}

	protected override void Awake() {
		base.Awake();
	}

	protected override void OnTriggerEnter2D(Collider2D col) {
		base.OnTriggerEnter2D(col);
		if (col.tag == "Player" && hitHeroEntity.Owner != owner) {
			hitHeroEntity = col.GetComponent<BaseCharacterEntity>();
			hitHeroEffects = col.GetComponent<CharacterEffects>();
			hitHeroThrowItem = col.GetComponent<CharacterThrow>();
			DestroyItem();
		}
	}

	protected override void Start() {
		base.Start();
		newVel = GetComponent<Rigidbody2D>().velocity;
		StartCoroutine("UpdateItem");
	}

	protected override void CleanItem() {
		GetComponent<NetworkView>().RPC("ApplyEffects", RPCMode.All);
		base.CleanItem();
	}

	public void DestroyItem() {
		anim.SetTrigger("Remove");
	}

	private IEnumerator UpdateItem() {
		float tmpDuration = duration;
		GetComponent<Rigidbody2D>().velocity = new Vector2(moveDir.x * speed, 0f);
		GetComponent<Rigidbody2D>().AddTorque(torque);

		while (tmpDuration > 0f) {
			tmpDuration -= Time.deltaTime;
			yield return null;
		}

		showIndicator = false;
		DestroyItem();
		StopCoroutine("UpdateItem");
	}

	protected override void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			Vector3 tmpVel = GetComponent<Rigidbody2D>().velocity;
			stream.Serialize(ref tmpVel);
		}
		else {
			stream.Serialize(ref newVel);
			GetComponent<Rigidbody2D>().velocity = newVel;
		}
	}

	[RPC]
	private void ApplyEffects() {
		if (Network.isServer) {
			hitHeroEffects.ApplySlow(slow);
			hitHeroEffects.ApplyInvulnerability(invulnerable);
			hitHeroEffects.ApplySpeedIncrease(speedIncrease);
			hitHeroEffects.ApplyJumpIncrease(jumpIncrease);
			hitHeroEffects.ApplyPush(push);
			hitHeroEffects.ApplyPull(pull);
		}
	}
}
