using UnityEngine;
using System.Collections;

public class CharacterEffects : MonoBehaviour {
	private bool slowed = false;
	public bool Slowed {
		get { return slowed; }
		set { slowed = value; }
	}

	private bool sliding = false;
	public bool Sliding {
		get { return sliding; }
		set { sliding = value; }
	}

	private bool invulnerable = false;
	public bool Invulnerable {
		get { return invulnerable; }
		set { invulnerable = value; }
	}

	private bool speedIncreased = false;
	public bool SpeedIncreased {
		get { return speedIncreased; }
		set { speedIncreased = value; }
	}

	private bool jumpIncreased = false;
	public bool JumpIncreased {
		get { return jumpIncreased; }
		set { jumpIncreased = value; }
	}

	private bool pushed = false;
	public bool Pushed {
		get { return pushed; }
		set { pushed = value; }
	}

	private bool pulled = false;
	public bool Pulled {
		get { return pulled; }
		set { pulled = value; }
	}

	private BaseCharacterEntity heroEntity;
	//private NetworkManager networkManager;

	private float origSpeedCap;
	private float origJumpCap;
	private float origGravityModifer;

	private float origDrag;
	//private float origGravityScale;

	private void Awake() {
		heroEntity = GetComponent<BaseCharacterEntity>();
		//networkManager = FindObjectOfType<NetworkManager>();
	}

	private void Start() {
		origSpeedCap = heroEntity.SpeedCap;
		origJumpCap = heroEntity.JumpCap;
		origGravityModifer = heroEntity.CharPhysics.GravityModifier;

		origDrag = heroEntity.rigidbody2D.drag;
		//origGravityScale = heroEntity.rigidbody2D.gravityScale;
	}

	public void ApplySlow(Slow slow) {
		if (!Slowed && slow.Enable) {
			StopCoroutine("SlowRoutine");
			StartCoroutine("SlowRoutine", slow);
		}
	}

	private IEnumerator SlowRoutine(Slow slow) {
		Slowed = true;
		float slowDuration = slow.Duration;
		float slowModifier = origSpeedCap * slow.SlowFactor;
		float lerp = 0f;

		while (slowDuration > 0f) {
			slowDuration -= Time.deltaTime;

			if (lerp < 1f) {
				lerp = Mathf.Repeat(Time.time, 1.1f);
				heroEntity.SpeedCap = Mathf.Lerp(origSpeedCap, slowModifier, lerp);
				heroEntity.Anim.speed = Mathf.Lerp(1f, 0.3f, lerp);
			}

			yield return null;
		}

		Slowed = false;

		lerp = 0f;
		while (heroEntity.SpeedCap < origSpeedCap) {
			slowDuration -= Time.deltaTime;

			if (lerp < 1f) {
				lerp = Mathf.Repeat(Time.time, 1.1f);
				heroEntity.SpeedCap = Mathf.Lerp(slowModifier, origSpeedCap, lerp);
				heroEntity.Anim.speed = Mathf.Lerp(0.3f, 1f, lerp);
			}

			yield return null;
		}
	}

	public void ApplySliding(Sliding slide) {
		if (!Sliding && slide.Enable) {
			StopCoroutine("SlideRoutine");
			StartCoroutine("SlideRoutine", slide);
		}
	}

	private IEnumerator SlideRoutine(Sliding slide) {
		Sliding = true;
		float slideDuration = slide.Duration;
		float gravityMod = origGravityModifer * slide.SlideFactor;
		//float lerp = 0f;

		heroEntity.CharPhysics.GravityModifier = gravityMod;
		//heroEntity.rigidbody2D.gravityScale = 0f;
		heroEntity.rigidbody2D.drag = 0f;
		heroEntity.Anim.speed = 5f;
		while (slideDuration > 0f) {
			slideDuration -= Time.deltaTime;

			//if (lerp < 1f) {
			//    lerp = Mathf.Repeat(Time.time, 1.1f);
			//    heroEntity.CharPhysics.GravityModifier = Mathf.Lerp(origGravityModifer, gravityMod, lerp);
			//    heroEntity.Anim.speed = Mathf.Lerp(1f, 5f, lerp);
			//}

			yield return null;
		}

		heroEntity.CharPhysics.GravityModifier = origGravityModifer;
		//heroEntity.rigidbody2D.gravityScale = origGravityScale;
		heroEntity.rigidbody2D.drag = origDrag;
		heroEntity.Anim.speed = 1f;
		Sliding = false;

		//lerp = 0f;
		//while (heroEntity.CharPhysics.GravityModifier < origGravityModifer) {
		//    slideDuration -= Time.deltaTime;

		//    //if (lerp < 1f) {
		//    //    lerp = Mathf.Repeat(Time.time, 1.1f);
		//    //    heroEntity.CharPhysics.GravityModifier = Mathf.Lerp(gravityMod, origGravityModifer, lerp);
		//    //    heroEntity.Anim.speed = Mathf.Lerp(5f, 1f, lerp);
		//    //}

		//    yield return null;
		//}
	}

	public void ApplyInvulnerability(Invulnerability invul) {
		if (!Invulnerable && invul.Enable) {
			StopCoroutine("InvulnerabilityRoutine");
			StartCoroutine("InvulnerabilityRoutine", invul);
		}
	}

	private IEnumerator InvulnerabilityRoutine(Invulnerability invul) {
		Invulnerable = true;
		float tmpDuration = invul.Duration;

		while (tmpDuration > 0f) {
			tmpDuration -= Time.deltaTime;
			yield return null;
		}

		Invulnerable = false;
	}

	public void ApplySpeedIncrease(SpeedIncrease spdInc) {
		if (!SpeedIncreased && spdInc.Enable) {
			StopCoroutine("SpeedIncreaseRoutine");
			StartCoroutine("SpeedIncreaseRoutine", spdInc);
		}
	}

	private IEnumerator SpeedIncreaseRoutine(SpeedIncrease spdInc) {
		SpeedIncreased = true;
		float tmpDuration = spdInc.Duration;
		float totalSpeedCap = origSpeedCap + spdInc.Increase;
		heroEntity.SpeedCap = totalSpeedCap;

		while (tmpDuration > 0f) {
			tmpDuration -= Time.deltaTime;
			yield return null;
		}

		heroEntity.SpeedCap = origSpeedCap;
		SpeedIncreased = false;
	}

	public void ApplyJumpIncrease(JumpIncrease jmpInc) {
		if (!JumpIncreased && jmpInc.Enable) {
			StopCoroutine("JumpIncreaseRoutine");
			StartCoroutine("JumpIncreaseRoutine", jmpInc);
		}
	}

	private IEnumerator JumpIncreaseRoutine(JumpIncrease jmpInc) {
		JumpIncreased = true;
		float tmpDuration = jmpInc.Duration;
		float totalJumpCap = origJumpCap + jmpInc.Increase;
		heroEntity.JumpCap = totalJumpCap;

		while (tmpDuration > 0f) {
			tmpDuration -= Time.deltaTime;
			yield return null;
		}

		heroEntity.JumpCap = origJumpCap;
		JumpIncreased = false;
	}

	public void ApplyPush(Push push) {
		if (!Pushed && push.Enable) {
			StopCoroutine("PushRoutine");
			StartCoroutine("PushRoutine", push);
		}
	}

	private IEnumerator PushRoutine(Push push) {
		Pushed = true;
		heroEntity.CharPhysics.GravityModifier = 0f;
		rigidbody2D.AddForceAtPosition(push.PushVector * push.Force, transform.position);
		yield return new WaitForSeconds(0.1f);
		heroEntity.CharPhysics.GravityModifier = origGravityModifer;
		Pushed = false;
	}

	public void ApplyPull(Pull pull) {
		if (!Pulled && pull.Enable) {
			StopCoroutine("PullRoutine");
			StartCoroutine("PullRoutine", pull);
		}
	}

	private IEnumerator PullRoutine(Pull pull) {
		Pulled = true;
		heroEntity.CharPhysics.GravityModifier = 0f;
		rigidbody2D.AddForceAtPosition(new Vector2(-pull.PullVector.x, pull.PullVector.y) * pull.Force, transform.position);
		yield return new WaitForSeconds(0.1f);
		heroEntity.CharPhysics.GravityModifier = origGravityModifer;
		Pulled = false;
	}

	public void ResetSlide() {
		heroEntity.CharPhysics.GravityModifier = origGravityModifer;
		//heroEntity.rigidbody2D.gravityScale = origGravityScale;
		if (heroEntity.rigidbody2D != null)
			heroEntity.rigidbody2D.drag = origDrag;
		heroEntity.Anim.speed = 1f;
	}
}

[System.Serializable]
public class Slow {
	[SerializeField]
	private bool enable = false;
	public bool Enable {
		get { return enable; }
		set { enable = value; }
	}

	[SerializeField]
	private float duration = 2f;
	public float Duration {
		get { return duration; }
		set { duration = value; }
	}

	[SerializeField]
	private float slowFactor = 0.3f;
	public float SlowFactor {
		get { return slowFactor; }
		set { slowFactor = value; }
	}

	public Slow() { }
}

[System.Serializable]
public class Sliding {
	[SerializeField]
	private bool enable = false;
	public bool Enable {
		get { return enable; }
		set { enable = value; }
	}

	[SerializeField]
	private float duration = 1f;
	public float Duration {
		get { return duration; }
		set { duration = value; }
	}

	[SerializeField]
	private float slideFactor = 0.1f;
	public float SlideFactor {
		get { return slideFactor; }
		set { slideFactor = value; }
	}

	public Sliding() { }
}

[System.Serializable]
public class Invulnerability {
	[SerializeField]
	private bool enable = false;
	public bool Enable {
		get { return enable; }
		set { enable = value; }
	}

	[SerializeField]
	private float duration = 1f;
	public float Duration {
		get { return duration; }
		set { duration = value; }
	}

	public Invulnerability() { }
}

[System.Serializable]
public class SpeedIncrease {
	[SerializeField]
	private bool enable = false;
	public bool Enable {
		get { return enable; }
		set { enable = value; }
	}

	[SerializeField]
	private float duration = 1f;
	public float Duration {
		get { return duration; }
		set { duration = value; }
	}

	[SerializeField]
	private float increase = 5f;
	public float Increase {
		get { return increase; }
		set { increase = value; }
	}

	public SpeedIncrease() { } 
}

[System.Serializable]
public class JumpIncrease {
	[SerializeField]
	private bool enable = false;
	public bool Enable {
		get { return enable; }
		set { enable = value; }
	}

	[SerializeField]
	private float duration = 1f;
	public float Duration {
		get { return duration; }
		set { duration = value; }
	}

	[SerializeField]
	private float increase = 5f;
	public float Increase {
		get { return increase; }
		set { increase = value; }
	}

	public JumpIncrease() { }
}

[System.Serializable]
public class Push {
	[SerializeField]
	private bool enable = false;
	public bool Enable {
		get { return enable; }
		set { enable = value; }
	}

	[SerializeField]
	private float force = 500f;
	public float Force {
		get { return force; }
		set { force = value; }
	}

	[SerializeField]
	private float upwardForceFactor = 0.3f;
	public float UpwardForceFactor {
		get { return Mathf.Clamp(upwardForceFactor, 0f, 1f); }
		set { upwardForceFactor = value; }
	}

	[SerializeField]
	private Vector3 pushVector;
	public Vector3 PushVector {
		get {
			pushVector.Normalize();
			if (pushVector.y < upwardForceFactor)
				pushVector.y = upwardForceFactor;

			return pushVector; 
		}
		set { pushVector = value; }
	}

	public Push() { }
}

[System.Serializable]
public class Pull {
	[SerializeField]
	private bool enable = false;
	public bool Enable {
		get { return enable; }
		set { enable = value; }
	}

	[SerializeField]
	private float force = 500f;
	public float Force {
		get { return force; }
		set { force = value; }
	}

	[SerializeField]
	private float upwardForceFactor = 0.3f;
	public float UpwardForceFactor {
		get { return Mathf.Clamp(upwardForceFactor, 0f, 1f); }
		set { upwardForceFactor = value; }
	}

	[SerializeField]
	private Vector3 pullVector;
	public Vector3 PullVector {
		get {
			pullVector.Normalize();
			if (pullVector.y < upwardForceFactor)
				pullVector.y = upwardForceFactor;

			return pullVector;
		}
		set { pullVector = value; }
	}
	public Pull() { }
}