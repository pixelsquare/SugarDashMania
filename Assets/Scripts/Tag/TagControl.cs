using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TagTargetType { Ally, Enemy }

public class TagControl : MonoBehaviour {
	[SerializeField]
	private Tag tagIndicator;

	[SerializeField]
	private float radius = 5f;

	[SerializeField]
	private float duration = 5f;

	[SerializeField]
	private Color allyTagColor = Color.green;

	[SerializeField]
	private Color enemyTagColor = Color.red;

	private List<TagTargets> allyList = new List<TagTargets>();
	private List<TagTargets> enemyList = new List<TagTargets>();

	private Transform allyTarget;
	private Transform enemyTarget;

	private int allyTargetIndx = 0;
	private int enemyTargetIndx = 0;

	private GameObject allyTag;
	private GameObject enemyTag;

	private BaseCharacterEntity heroEntity;

	private void Awake() {
		heroEntity = GetComponent<BaseCharacterEntity>();
	}

	private void FixedUpdate() {
		if(allyTag == null)
			allyTarget = null;

		if (enemyTag == null)
			enemyTarget = null;

		if (Network.player == heroEntity.Owner) {
			if (Input.GetMouseButtonDown(0)) {
				allyTarget = FindTarget(TagTargetType.Ally);

				if (allyTarget != null) {
					GetComponent<NetworkView>().RPC("SpawnAllyTag", RPCMode.All, Network.player, allyTarget.position, allyTarget.GetComponent<NetworkView>().viewID, duration);
				}
			}

			if (Input.GetMouseButtonDown(1)) {
				enemyTarget = FindTarget(TagTargetType.Enemy);

				if (enemyTarget != null) {
					GetComponent<NetworkView>().RPC("SpawnEnemyTag", RPCMode.All, Network.player, enemyTarget.position, enemyTarget.GetComponent<NetworkView>().viewID, duration);
				}
			}
		}
	}

	private Transform FindTarget(TagTargetType type) {
		Transform target = null;


		if (type == TagTargetType.Ally) {
			Collider2D[] taggedObjects = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerManager.LayerAlly);
			allyList = new List<TagTargets>();

			int i;
			for (i = 0; i < taggedObjects.Length; i++) {
				if (taggedObjects[i] == taggedObjects[i].GetComponent<CircleCollider2D>())
					allyList.Add(new TagTargets(taggedObjects[i], Vector3.Distance(transform.position, taggedObjects[i].transform.position)));
			}

			allyList.Sort(Sort);

			if (allyTarget != null)
				allyTargetIndx++;
			else
				allyTargetIndx = 0;

			if (allyTargetIndx > allyList.Count - 1)
				allyTargetIndx = 0;

			if (allyList.Count > 0)
				target = allyList[allyTargetIndx].taggedObjects.transform;
		}

		if (type == TagTargetType.Enemy) {
			Collider2D[] taggedObjects = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerManager.LayerEnemy);
			enemyList = new List<TagTargets>();

			int i;
			for (i = 0; i < taggedObjects.Length; i++) {
				if (taggedObjects[i] == taggedObjects[i].GetComponent<CircleCollider2D>())
					enemyList.Add(new TagTargets(taggedObjects[i], Vector3.Distance(transform.position, taggedObjects[i].transform.position)));
			}

			enemyList.Sort(Sort);

			if (enemyTarget != null)
				enemyTargetIndx++;
			else
				enemyTargetIndx = 0;

			if (enemyTargetIndx > enemyList.Count - 1)
				enemyTargetIndx = 0;

			if (taggedObjects.Length > 0)
				target = enemyList[enemyTargetIndx].taggedObjects.transform;
		}

		return target;
	}

	private int Sort(TagTargets a, TagTargets b) {
		if (a.distance > b.distance) return 1;
		if (a.distance < b.distance) return -1;
		return 0;
	}

	[RPC]
	private void SpawnAllyTag(NetworkPlayer sender, Vector3 pos, NetworkViewID allyId, float duration) {
		if (Network.isServer) {
			if (allyTag == null)
				allyTag = Network.Instantiate(tagIndicator.gameObject, pos, Quaternion.identity, NetworkGroup.PlayerTag) as GameObject;

			allyTag.GetComponent<Tag>().SetColor(allyTagColor);
			allyTag.GetComponent<Tag>().Duration = duration;
			allyTag.GetComponent<Tag>().GetComponent<NetworkView>().RPC("SetOwner", RPCMode.All, sender);

			allyTarget = NetworkView.Find(allyId).gameObject.transform;
			if (allyTarget != null) {
				allyTag.transform.parent = allyTarget.transform;
				allyTag.transform.localPosition = Vector3.zero;
			}
			
			GetComponent<NetworkView>().RPC("UpdateSpawnAllyTag", RPCMode.Others, allyTag.GetComponent<NetworkView>().viewID, allyTarget.GetComponent<NetworkView>().viewID, duration);
		}
	}

	[RPC]
	private void UpdateSpawnAllyTag(NetworkViewID id, NetworkViewID allyId, float duration) {
		allyTag = NetworkView.Find(id).gameObject;

		allyTag.GetComponent<Tag>().SetColor(allyTagColor);
		allyTag.GetComponent<Tag>().Duration = duration;

		allyTarget = NetworkView.Find(allyId).gameObject.transform;
		if (allyTarget != null) {
			allyTag.transform.parent = allyTarget.transform;
			allyTag.transform.localPosition = Vector3.zero;
		}
	}

	[RPC]
	private void SpawnEnemyTag(NetworkPlayer sender, Vector3 pos, NetworkViewID enemyId, float duration) {
		if (Network.isServer) {
			if (enemyTag == null)
				enemyTag = Network.Instantiate(tagIndicator.gameObject, pos, Quaternion.identity, NetworkGroup.PlayerTag) as GameObject;

			enemyTag.GetComponent<Tag>().SetColor(enemyTagColor);
			enemyTag.GetComponent<Tag>().Duration = duration;
			enemyTag.GetComponent<Tag>().GetComponent<NetworkView>().RPC("SetOwner", RPCMode.All, sender);

			enemyTarget = NetworkView.Find(enemyId).gameObject.transform;
			if (enemyTarget != null) {
				enemyTag.transform.parent = enemyTarget.transform;
				enemyTag.transform.localPosition = Vector3.zero;
			}

			GetComponent<NetworkView>().RPC("UpdateSpawnEnemyTag", RPCMode.Others, enemyTag.GetComponent<NetworkView>().viewID, enemyTarget.GetComponent<NetworkView>().viewID, duration);
		}
	}

	[RPC]
	private void UpdateSpawnEnemyTag(NetworkViewID id, NetworkViewID enemyId, float duration) {
		enemyTag = NetworkView.Find(id).gameObject;

		enemyTag.GetComponent<Tag>().SetColor(enemyTagColor);
		enemyTag.GetComponent<Tag>().Duration = duration;

		enemyTarget = NetworkView.Find(enemyId).gameObject.transform;
		if (enemyTarget != null) {
			enemyTag.transform.parent = enemyTarget.transform;
			enemyTag.transform.localPosition = Vector3.zero;
		}
	}
}

[System.Serializable]
public class TagTargets {
	public TagTargets(Collider2D col, float dist) {
		taggedObjects = col;
		distance = dist;
	}

	public Collider2D taggedObjects;
	public float distance;
}