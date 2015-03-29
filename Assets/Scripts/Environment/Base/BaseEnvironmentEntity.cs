using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class BaseEnvironmentEntity : MonoBehaviour {

	[SerializeField]
	protected NetworkGroups networkGroup;

	[SerializeField]
	protected GameObject indicator;

	protected NetworkPlayer hitId;
	protected Animator anim;
	protected NetworkManager networkManager;

	protected virtual void Awake() {
		networkView.observed = this;
		anim = GetComponent<Animator>();
		networkManager = FindObjectOfType<NetworkManager>();
	}

	protected virtual void Start() {
		GameUtility.ChangeSortingLayerRecursively(transform, LayerManager.SortingLayerCharacterBack);
	}

	protected virtual void RemoveItem() {
		if (Network.isServer) {
			Network.Destroy(networkView.viewID);

			if ((int)networkGroup > 0) {
				Network.RemoveRPCsInGroup((int)networkGroup);
			}
		}
	}

	private void Remove() {
		Network.Destroy(networkView.viewID);

		if ((int)networkGroup > 0) {
			Network.RemoveRPCsInGroup((int)networkGroup);
		}
	}

	private void DestroyCollider() {
		Destroy(collider2D);
	}

	protected void AddScore(int score) {
		if (Network.isServer) {
			networkManager.networkView.RPC("AddScore", RPCMode.All, hitId, score);
		}
	}

	//protected void PushObjects(float radius, float force) {
	//    //if (Network.isServer) {
	//        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerManager.LayerPlayer);

	//        foreach (Collider2D cols in colliders) {
	//            Rigidbody2D rb2D = cols.GetComponent<Rigidbody2D>();
	//            BaseCharacterEntity heroEntity = cols.GetComponent<BaseCharacterEntity>();

	//            if (heroEntity != null) {
	//                heroEntity.StopCoroutine("ResetPhysicsModifier");
	//                heroEntity.StartCoroutine("ResetPhysicsModifier", 0.1f);
	//                rb2D.drag = 0f;
	//                //heroEntity.SpeedCap = 30f;
	//                //heroEntity.JumpCap = 80f;
	//            }

	//            if (rb2D != null) {
	//                Vector2 oppositeForce = cols.transform.position - transform.position;
	//                oppositeForce.Normalize();
	//                rb2D.AddForce(oppositeForce * force);
	//            }
	//        }
	//    //}
	//}

	//protected void PullObjects(float radius, float force) {
	//    //if (Network.isServer) {
	//        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerManager.LayerPlayer);

	//        foreach (Collider2D cols in colliders) {
	//            Rigidbody2D rb2D = cols.GetComponent<Rigidbody2D>();
	//            BaseCharacterEntity heroEntity = cols.GetComponent<BaseCharacterEntity>();

	//            if (heroEntity != null) {
	//                heroEntity.StopCoroutine("ResetPhysicsModifier");
	//                heroEntity.StartCoroutine("ResetPhysicsModifier", 0.5f);
	//                rb2D.drag = 0f;
	//                //heroEntity.SpeedCap = 30f;
	//                //heroEntity.JumpCap = 80f;
	//            }

	//            if (rb2D != null) {
	//                Vector2 oppositeForce = transform.position - cols.transform.position;
	//                oppositeForce.Normalize();
	//                rb2D.AddForce(oppositeForce * force);
	//            }
	//        }
	//    //}
	//}
}
