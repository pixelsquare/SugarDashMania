using UnityEngine;
using System.Collections;

public class BaseCharacterEntity : MonoBehaviour {
	[SerializeField]
	private float moveForce = 5000f;
	public float MoveForce {
		get { return moveForce; }
		set { moveForce = value; }
	}

	[SerializeField]
	private float speedCap = 8f;
	public float SpeedCap {
		get { return speedCap; }
		set { speedCap = value; }
	}

	[SerializeField]
	private float jumpForce = 4000f;
	public float JumpForce {
		get { return jumpForce; }
		set { jumpForce = value; }
	}

	[SerializeField]
	private float jumpCap = 40f;
	public float JumpCap {
		get { return jumpCap; }
		set { jumpCap = value; }
	}

	[SerializeField]
	private Transform groundCheck;

	[SerializeField]
	private Transform ceilingCheck;

	[SerializeField]
	private Transform frontFace;
	public Transform FrontFace {
		get { return frontFace; }
		set { frontFace = value; }
	}

	[SerializeField]
	private Transform frontFaceTop;

	[SerializeField]
	private Transform frontFaceBot;

	[SerializeField]
	private SpriteRenderer arrow;
	public SpriteRenderer Arrow {
		get { return arrow; }
		set { arrow = value; }
	}

	[SerializeField]
	private SpriteRenderer mainTexture;
	public SpriteRenderer MainTexture {
		get { return mainTexture; }
		set { mainTexture = value; }
	}

	[SerializeField]
	private SpriteRenderer silhouette;
	public SpriteRenderer Silhouette {
		get { return silhouette; }
		set { silhouette = value; }
	}

	[SerializeField]
	private Sprite defaultTexture;
	public Sprite DefaultTexture {
		get { return defaultTexture; }
		set { defaultTexture = value; }
	}

	[SerializeField]
	private Sprite starTexture;
	public Sprite StarTexture {
		get { return starTexture; }
		set { starTexture = value; }
	}

	[SerializeField]
	private TextMesh nameplate;
	public TextMesh Nameplate {
		get { return nameplate; }
		set { nameplate = value; }
	}

	[SerializeField]
	private ParticleSystem starParticle;
	public ParticleSystem StarParticle {
		get { return starParticle; }
		set { starParticle = value; }
	}

	[SerializeField]
	private Camera myCamera;
	public Camera MyCamera {
		get { return myCamera; }
		set { myCamera = value; }
	}

	[SerializeField]
	private CavityOverlay overlay;
	public CavityOverlay Overlay {
		get { return overlay; }
		set { overlay = value; }
	}

	[SerializeField]
	private GameEndTimer endTimer;
	public GameEndTimer EndTimer {
		get { return endTimer; }
		set { endTimer = value; }
	}

	[SerializeField]
	private Tracker allyTracker;
	public Tracker AllyTracker {
		get { return allyTracker; }
		set { allyTracker = value; }
	}

	[SerializeField]
	private Tracker enemy1Tracker;
	public Tracker Enemy1Tracker {
		get { return enemy1Tracker; }
		set { enemy1Tracker = value; }
	}

	[SerializeField]
	private Tracker enemy2Tracker;
	public Tracker Enemy2Tracker {
		get { return enemy2Tracker; }
		set { enemy2Tracker = value; }
	}

	[SerializeField]
	private CharacterPhysics charPhysics;
	public CharacterPhysics CharPhysics {
		get { return charPhysics; }
		set { charPhysics = value; }
	}

	private NetworkPlayer owner;
	public NetworkPlayer Owner {
		get { return owner; }
		set {
			Debug.Log("Setting Owner to " + value);
			owner = value;
		}
	}

	private ItemType curItem = ItemType.None;
	public ItemType CurItem {
		get { return curItem; }
		set { 
			curItem = value;
			networkManager.GetComponent<NetworkView>().RPC("SetCurrentItem", RPCMode.All, owner, (int)curItem);
		}
	}

	private Team myTeam = Team.Red;
	public Team MyTeam {
		get { return myTeam; }
		set { myTeam = value; }
	}

	private bool dead = false;
	public bool Dead {
		get { return dead; }
		set { dead = value; }
	}

	private Animator anim;
	public Animator Anim {
		get { return anim; }
		set { anim = value; }
	}

	private CharacterThrow heroThrow;
	public CharacterThrow HeroThrow {
		get { return heroThrow; }
		set { heroThrow = value; }
	}

	private CharacterEffects heroEffects;
	public CharacterEffects HeroEffects {
		get { return heroEffects; }
		set { heroEffects = value; }
	}

	private bool canMove = true;
	public bool CanMove {
		get { return canMove; }
		set { canMove = value; }
	}

	private bool hasEnded = false;
	public bool HasEnded {
		get { return hasEnded; }
		set { hasEnded = value; }
	}

	private NetworkManager networkManager;
	private Lollipop lollipop;
	private int waypointIndx = 0;

	private float horizInput;
	private bool isFacingRight = true;
	private Transform parentTransform;

	public RaycastHit2D GroundHit { get; private set; }

	public bool Grounded { get; private set; }
	public bool HeadBump { get; private set; }
	public bool ObstructedSight { get; private set; }

	private int oldGroundID;
	private bool canJump = false;
	private bool didJump = false;

	private Vector3 respawnPoint;

	private void Awake() {
		GetComponent<NetworkView>().observed = this;
		Anim = GetComponent<Animator>();
		networkManager = FindObjectOfType<NetworkManager>();
		heroThrow = GetComponent<CharacterThrow>();
		heroEffects = GetComponent<CharacterEffects>();
		Grounded = true;
	}

	private void Start() {
		//transform.position = new Vector3(150f, 30f, 0f);
		GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
		nameplate.GetComponent<Renderer>().sortingLayerName = LayerManager.SortingLayerUiBack;
		nameplate.GetComponent<Renderer>().sortingOrder = 1;
		parentTransform = transform.root;
		GameUtility.SetRendererEnableRecursively(overlay.transform, false);

		GameUtility.ChangeSortingLayerRecursively(starParticle.transform, LayerManager.SortingLayerUiBack);
		starParticle.enableEmission = false;
		starParticle.Stop();

		if (myTeam == Team.Red) {
			arrow.color = Color.red;
			silhouette.color = Color.red;
		}
		if (myTeam == Team.Green) {
			arrow.color = Color.green;
			silhouette.color = Color.green;
		}

		if (Network.player != owner) {
			arrow.gameObject.SetActive(false);
			GameUtility.SetRendererEnableRecursively(overlay.transform, false);
			GameUtility.ChangeSortingLayerRecursively(transform, LayerManager.SortingLayerCharacterMid);
		}
	}

	private void Update() {
		HeadBump = Physics2D.Linecast(transform.position, ceilingCheck.position, 1 << LayerManager.LayerGround);
		Grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerManager.LayerGround);
		GroundHit = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerManager.LayerGround | 1 << LayerManager.LayerSlideEdge);
		ObstructedSight = Physics2D.Linecast(transform.position, frontFace.position, 1 << LayerManager.LayerGround | 1 << LayerManager.LayerSlideEdge) |
						  Physics2D.Linecast(transform.position, frontFaceTop.position, 1 << LayerManager.LayerGround | 1 << LayerManager.LayerSlideEdge) |
						  Physics2D.Linecast(transform.position, frontFaceBot.position, 1 << LayerManager.LayerGround | 1 << LayerManager.LayerSlideEdge);
	}

	private void FixedUpdate() {
		if (gameObject.activeInHierarchy) {
			if (Network.player == owner && networkManager.GameStart && canMove) {
				if (!overlay.GetComponent<Renderer>().enabled)
					GameUtility.SetRendererEnableRecursively(overlay.transform, true);
				//if (Input.GetKeyDown(KeyCode.L)) {
				//    Pull push = new Pull();
				//    push.Force = 10000f;
				//    push.PullVector = Vector3.right;
				//    GetComponent<CharacterEffects>().ApplyPull(push);
				//}

				if (Input.GetKeyDown(KeyCode.E)) {
					heroThrow.Throw();
				}

				if (GetComponent<Rigidbody2D>() != null) {
					horizInput = Input.GetAxis("Horizontal") + JoystickManager.GetAxis(JSInput.LAxisX) + JoystickManager.GetAxis(JSInput.DpadAxisX);
					GetComponent<NetworkView>().RPC("SendUserInput", RPCMode.All, horizInput, (Vector3)transform.position, (Vector3)GetComponent<Rigidbody2D>().velocity);
				}


				if (Input.GetButton("Jump") && canJump && !HeadBump) {
					didJump = true;
				}

				//networkView.RPC("SendUserInput", RPCMode.All, horizInput, speedCap, isFacingRight, Grounded, 
				//                                transform.position, transform.localScale, (Vector3)rigidbody2D.velocity);
			}

			if (GroundHit.collider != null && GroundHit.collider.tag == "Slippery") {
				if (GetComponent<Rigidbody2D>() != null && horizInput == 0f && GetComponent<Rigidbody2D>().velocity != Vector2.zero)
					horizInput = Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x);

				Anim.SetFloat("Slip", Mathf.Abs(horizInput));
				Anim.SetFloat("Speed", Mathf.Abs(0f));
			}
			else if (GroundHit.collider != null && GroundHit.collider.tag != "Slippery") {
				Anim.SetFloat("Speed", Mathf.Abs(horizInput));
				Anim.SetFloat("Slip", Mathf.Abs(0f));
			}
			else
				Anim.SetFloat("Speed", Mathf.Abs(horizInput));

			if (!ObstructedSight) {
				if (GetComponent<Rigidbody2D>() != null && (horizInput * GetComponent<Rigidbody2D>().velocity.x) < speedCap)
					GetComponent<Rigidbody2D>().AddForce(Vector3.right * horizInput * moveForce);
			}

			if (GetComponent<Rigidbody2D>() != null && didJump) {
				anim.SetTrigger("Jump");
				if (transform.parent != parentTransform)
					transform.parent = parentTransform;

				GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
				didJump = false;
			}

			if (GroundHit.collider != null && GroundHit.transform.tag != "Slippery")
				heroEffects.ResetSlide();

			if (Grounded)
				canJump = true;

			if (GetComponent<Rigidbody2D>() != null && Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y) > jumpCap)
				canJump = false;

			if (GetComponent<Rigidbody2D>() != null && Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > speedCap)
				GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * speedCap, GetComponent<Rigidbody2D>().velocity.y);

			if (GetComponent<Rigidbody2D>() != null)
				GetComponent<Rigidbody2D>().AddForce(Physics2D.gravity * (charPhysics.GravityModifier - 1));


			if (horizInput < 0f && isFacingRight)
				Flip();

			if (horizInput > 0f && !isFacingRight)
				Flip();


			Vector3 desiredMove = transform.right * horizInput;

			if (GroundHit.collider != null && GroundHit.collider.tag != "Slippery") {
				if (desiredMove.magnitude > 0 || !Grounded)
					ChangeColliderMaterials(charPhysics.LowFrictionMat);
				else
					ChangeColliderMaterials(charPhysics.HighFrictionMat);
			}
		}
	}

	private void Flip() {
		isFacingRight = !isFacingRight;

		Vector3 scale = transform.localScale;
		scale.x *= -1f;
		transform.localScale = scale;

		scale = arrow.transform.localScale;
		scale.x *= -1f;
		arrow.transform.localScale = scale;

		scale = nameplate.transform.localScale;
		scale.x *= -1f;
		nameplate.transform.localScale = scale;
	}

	private void ChangeColliderMaterials(PhysicsMaterial2D mat) {
		foreach (Collider2D element in GetComponents<Collider2D>()) {
			if (element.enabled)
				element.enabled = false;

			if (element.sharedMaterial != mat)
				element.sharedMaterial = mat;

			if (!element.enabled)
				element.enabled = true;
		}
	}

	private void OnCollisionEnter2D(Collision2D col) {
		if (col.transform.tag == "MovingPlatform")
			transform.parent = col.transform;
		else
			transform.parent = parentTransform;

		if (col.transform.tag == "Slippery") {
			ChangeColliderMaterials(charPhysics.SlipperyFrictionMat);
		}

		if (col.transform.tag == "Lollipop" && Network.isServer) {
			lollipop = col.transform.GetComponent<Lollipop>();
			StopCoroutine("LollipopUpdate");
			StartCoroutine("LollipopUpdate");
		}

		if (col.transform.tag == "Coin") {
			col.transform.GetComponent<Coin>().DestroyCoin();
		}
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Player" && (col.gameObject.layer == LayerManager.LayerAlly
			|| col.gameObject.layer == LayerManager.LayerEnemy || col.gameObject.layer == LayerManager.LayerPlayer)) {

			Physics2D.IgnoreLayerCollision(gameObject.layer, col.gameObject.layer, true);
		}

		if (col.tag == "DeathTrigger")
			GetComponent<NetworkView>().RPC("PlayerRespawn", RPCMode.All);

		if (col.transform.tag == "Respawn" && respawnPoint != col.transform.position)
			GetComponent<NetworkView>().RPC("UpdateRespawnPoint", RPCMode.All, col.transform.position);

		if (col.transform.tag == "Finish") {
			if (!hasEnded) {
				if (Network.isServer) {
					networkManager.PlayersEnded++;
					GetComponent<NetworkView>().RPC("SetPlayersEnded", RPCMode.All, networkManager.PlayersEnded);
				}

				hasEnded = true;
				GetComponent<NetworkView>().RPC("UpdateHasEnded", RPCMode.All, hasEnded);
			}
			networkManager.GetComponent<NetworkView>().RPC("EndPlayer", RPCMode.All, Owner);
		}
	}

	private IEnumerator LollipopUpdate() {
		canMove = false;
		waypointIndx = 0;
		Vector3 vel = default(Vector3);

		float rotationTime = 0f;
		Vector3 origRotation = mainTexture.transform.eulerAngles;

		GetComponent<NetworkView>().RPC("InitLollipop", RPCMode.All);
		while (waypointIndx < lollipop.Waypoints.Length) {
			transform.position = Vector3.SmoothDamp(transform.position, lollipop.Waypoints[waypointIndx].position, ref vel, 0.2f);

			rotationTime += Time.deltaTime;
			mainTexture.transform.eulerAngles = Vector3.Lerp(origRotation, new Vector3(0f, 0f, 360f), Mathf.Repeat(rotationTime, 1f) / 1f);
			GetComponent<NetworkView>().RPC("SendUserStar", RPCMode.All, transform.position, mainTexture.transform.eulerAngles);

			if (Vector3.Distance(transform.position, lollipop.Waypoints[waypointIndx].position) < 1f)
				waypointIndx++;

			yield return null;
		}

		mainTexture.transform.eulerAngles = origRotation;
		GetComponent<NetworkView>().RPC("SendUserStar", RPCMode.All, transform.position, mainTexture.transform.eulerAngles);
		GetComponent<NetworkView>().RPC("DisableLollipop", RPCMode.All);
		//networkView.RPC("UpdateParallax", RPCMode.All, owner);

		canMove = true;
		StopCoroutine("LollipopUpdate");
	}

	private IEnumerator Respawn() {
		canMove = false;
		dead = true;

		GetComponent<NetworkView>().RPC("InitRespawn", RPCMode.All);

		//GameUtility.SetRendererEnableRecursively(transform, false);
		myCamera.GetComponent<CameraFollow>().Target = null;
		transform.position = respawnPoint;
		yield return new WaitForSeconds(0.5f);
		//GameUtility.SetRendererEnableRecursively(transform, true);
		myCamera.GetComponent<CameraFollow>().Target = transform;

		GetComponent<NetworkView>().RPC("DisableRespawn", RPCMode.All);

		dead = false;
		canMove = true;
		StopCoroutine("Respawn");
	}

	[RPC]
	private void UpdateHasEnded(bool ended) {
		hasEnded = ended;
	}

	[RPC]
	private void UpdateParallax(NetworkPlayer player) {
		if (Network.player == owner) {
			foreach (ParallaxBG element in FindObjectsOfType<ParallaxBG>()) {
				element.Cam = myCamera;

				if (element.PlayerInputBase) {
					element.PlayerRB = networkManager.PlayerHeroEntity.GetComponent<Rigidbody2D>();
				}
			}
		}
	}

	[RPC]
	private void InitRespawn() {
		canMove = false;
		dead = true;
		Destroy(GetComponent<Rigidbody2D>());

		anim.enabled = false;
		mainTexture.enabled = false;
		silhouette.enabled = false;
		arrow.enabled = false;
		nameplate.gameObject.SetActive(false);
	}

	[RPC]
	private void DisableRespawn() {
		dead = false;
		canMove = true;

		anim.enabled = true;
		mainTexture.enabled = true;
		silhouette.enabled = true;
		arrow.enabled = true;
		nameplate.gameObject.SetActive(true);

		gameObject.AddComponent<Rigidbody2D>();
		GetComponent<Rigidbody2D>().drag = 1;
		GetComponent<Rigidbody2D>().fixedAngle = true;
		GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;

		GetComponent<NetworkView>().RPC("UpdateParallax", RPCMode.All, owner);
	}

	[RPC]
	private void InitLollipop() {
		canMove = false;
		Destroy(GetComponent<Rigidbody2D>());

		starParticle.enableEmission = true;
		starParticle.Play();
		anim.enabled = false;
		mainTexture.sprite = starTexture;
		silhouette.sprite = starTexture;
	}

	[RPC]
	private void DisableLollipop() {
		canMove = true;

		starParticle.enableEmission = false;
		starParticle.Stop();
		anim.enabled = true;
		mainTexture.sprite = defaultTexture;
		silhouette.sprite = defaultTexture;

		gameObject.AddComponent<Rigidbody2D>();
		GetComponent<Rigidbody2D>().drag = 1;
		GetComponent<Rigidbody2D>().fixedAngle = true;
		GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;

		GetComponent<NetworkView>().RPC("UpdateParallax", RPCMode.All, owner);
	}

	[RPC]
	private void Initialize(NetworkPlayer id, int team) {
		owner = id;
		myTeam = (Team)team;
	}

	[RPC]
	private void UpdateRespawnPoint(Vector3 point) {
		respawnPoint = point;
	}

	[RPC]
	private void PlayerRespawn() {
		StopCoroutine("Respawn");
		StartCoroutine("Respawn");
	}

	[RPC]
	private void SendUserStar(Vector3 pos, Vector3 eulerAngle) {
		transform.position = pos;
		mainTexture.transform.eulerAngles = eulerAngle;
	}

	[RPC]
	private void SendUserInput(float horiz, Vector3 pos, Vector3 vel) {
		if (Network.isServer) {
			horizInput = horiz;
			transform.position = pos;
			GetComponent<Rigidbody2D>().velocity = vel;

			GetComponent<NetworkView>().RPC("UpdateUserInput", RPCMode.Others, horiz, pos, vel);
		}
	}

	[RPC]
	private void UpdateUserInput(float horiz, Vector3 pos, Vector3 vel) {
		if (Network.player != owner) {
			horizInput = horiz;
			transform.position = pos;
			GetComponent<Rigidbody2D>().velocity = vel;
		}
	}


	//[RPC]
	//private void SendUserInput(float horiz, float spdCap, bool facingRight, bool grounded, Vector3 pos, Vector3 scale, Vector3 vel) {
	//    if (Network.isServer) {
	//        horizInput = horiz;
	//        speedCap = spdCap;
	//        isFacingRight = facingRight;
	//        Grounded = grounded;

	//        Anim.SetFloat("Speed", Mathf.Abs(horizInput));

	//        Vector3 arrowScale = arrow.transform.localScale;
	//        arrowScale.x *= scale.x;
	//        arrow.transform.localScale = arrowScale;
	//        nameplate.transform.localScale = scale;

	//        Vector3 posVel = default(Vector3);
	//        Vector3 tmpVel = default(Vector3);

	//        if (transform.position != pos)
	//            transform.position = Vector3.SmoothDamp(transform.position, pos, ref posVel, 0.01f);
	//        if (transform.localScale != scale)
	//            transform.localScale = scale;
	//        if (rigidbody2D.velocity != (Vector2)vel)
	//            rigidbody2D.velocity = (Vector2)Vector3.SmoothDamp((Vector3)rigidbody2D.velocity, (Vector3)vel, ref tmpVel, 0.01f);

	//        networkView.RPC("UpdateUserInput", RPCMode.Others, horiz, spdCap, facingRight, grounded, pos, scale, vel);
	//    }
	//}

	//[RPC]
	//private void UpdateUserInput(float horiz, float spdCap, bool facingRight, bool grounded, Vector3 pos, Vector3 scale, Vector3 vel) {
	//    if (owner != Network.player) {
	//        horizInput = horiz;
	//        speedCap = spdCap;
	//        isFacingRight = facingRight;
	//        Grounded = grounded;

	//        Anim.SetFloat("Speed", Mathf.Abs(horizInput));

	//        Vector3 arrowScale = arrow.transform.localScale;
	//        arrowScale.x *= scale.x;
	//        arrow.transform.localScale = arrowScale;
	//        nameplate.transform.localScale = scale;

	//        Vector3 posVel = default(Vector3);
	//        Vector3 tmpVel = default(Vector3);

	//        if (transform.position != pos)
	//            transform.position = Vector3.SmoothDamp(transform.position, pos, ref posVel, 0.001f);
	//        if (transform.localScale != scale)
	//            transform.localScale = scale;
	//        if (rigidbody2D.velocity != (Vector2)vel)
	//            rigidbody2D.velocity = (Vector2)Vector3.SmoothDamp((Vector3)rigidbody2D.velocity, (Vector3)vel, ref tmpVel, 0.001f);
	//    }
	//}
}

[System.Serializable]
public class CharacterPhysics {
	[SerializeField]
	private float gravityModifier = 10f;
	public float GravityModifier {
		get { return gravityModifier; }
		set { gravityModifier = value; }
	}

	[SerializeField]
	private PhysicsMaterial2D lowFrictionMat;
	public PhysicsMaterial2D LowFrictionMat {
		get { return lowFrictionMat; }
	}

	[SerializeField]
	private PhysicsMaterial2D highFrictionMat;
	public PhysicsMaterial2D HighFrictionMat {
		get { return highFrictionMat; }
	}

	[SerializeField]
	private PhysicsMaterial2D slipperyFrictionMat;
	public PhysicsMaterial2D SlipperyFrictionMat {
		get { return slipperyFrictionMat; }
	}
}