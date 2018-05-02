using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(BoxCollider2D))]
public class Spawner : MonoBehaviour {

	[SerializeField]
	private GameObject obj;

	[SerializeField]
	private float spawnRate = 1f;

	[SerializeField]
	private NetworkGroups networkGroup;

	[SerializeField]
	private bool spawnInsideBox = false;

	[SerializeField]
	private GameObject holder;

	private BoxCollider2D boxCol;

	private void Awake() {
		GetComponent<NetworkView>().observed = this;
		//if (!networkView.isMine)
		//    Destroy(gameObject);

		holder.transform.position = Vector3.zero;
		holder.transform.name = transform.name + " Holder";
		holder.transform.parent = null;

		boxCol = GetComponent<BoxCollider2D>();

		if (Network.isServer)
			StartCoroutine("UpdateSpawner");
	}

	private IEnumerator UpdateSpawner() {
		float tmpRate = spawnRate;
		Vector3 randomPos = transform.position;
		if (spawnInsideBox)
			randomPos.x = Random.Range(transform.position.x + -(boxCol.size.x * 0.5f), transform.position.x + (boxCol.size.x * 0.5f));
		else
			randomPos = transform.position;

		while (tmpRate > 0f) {
			tmpRate -= Time.deltaTime;
			yield return null;
		}

		//networkView.RPC("SpawnObject", RPCMode.All, randomPos);
		GameObject tmpObj = (GameObject)Network.Instantiate(obj, randomPos, Quaternion.identity, (int)networkGroup);
		tmpObj.transform.parent = holder.transform;

		GetComponent<NetworkView>().RPC("UpdateSpawnerStorage", RPCMode.Others, tmpObj.GetComponent<NetworkView>().viewID, holder.GetComponent<NetworkView>().viewID);
		StopCoroutine("UpdateSpawner");
		StartCoroutine("UpdateSpawner");
	}

	[RPC]
	private void UpdateSpawnerStorage(NetworkViewID obj, NetworkViewID holder) {
		Transform tmpObj = NetworkView.Find(obj).transform;
		Transform tmpHolder = NetworkView.Find(holder).transform;

		tmpObj.parent = tmpHolder;
	}

	//[RPC]
	//private void SpawnObject(Vector3 pos) {
	//    if (Network.isServer) {
	//        GameObject tmpObj = (GameObject)Network.Instantiate(obj, pos, Quaternion.identity, (int)networkGroup);
	//        tmpObj.transform.parent = holder.transform;
	//    }
	//}
}
