using UnityEngine;
using System.Collections;

public class GameEndTimer : MonoBehaviour {
	[SerializeField]
	private TextMesh countMesh;

	[SerializeField]
	private TextMesh endMesh;

	[SerializeField]
	private SpriteRenderer hover;

	private NetworkManager networkManager;

	private float maxTime = 10f;
	public float MaxTime {
		get { return maxTime; }
		set { maxTime = value; }
	}

	private float curTime = 0f;
	public float CurTime {
		get { return curTime; }
		set { curTime = value; }
	}

	private void Awake() {
		networkManager = FindObjectOfType<NetworkManager>();
	}

	private void Start() {
		GameUtility.ChangeSortingLayerRecursively(countMesh.transform, LayerManager.SortingLayerCharacterBack);
		GameUtility.ChangeSortingLayerRecursively(endMesh.transform, LayerManager.SortingLayerCharacterBack);

		countMesh.GetComponent<Renderer>().sortingOrder = 1;
		endMesh.GetComponent<Renderer>().sortingOrder = 1;

		curTime = maxTime;
		countMesh.GetComponent<Renderer>().enabled = false;
		endMesh.GetComponent<Renderer>().enabled = false;
		hover.GetComponent<Renderer>().enabled = false;
	}

	public void EnableTimer() {
		if (Network.player == networkManager.PlayerHeroEntity.Owner) {
			curTime = maxTime;
			countMesh.GetComponent<Renderer>().enabled = true;
			endMesh.GetComponent<Renderer>().enabled = true;
			hover.GetComponent<Renderer>().enabled = true;
			StopCoroutine("UpdateTimer");
			StartCoroutine("UpdateTimer");
		}
	}

	private IEnumerator UpdateTimer() {
		while (curTime > 0f) {
			curTime -= Time.deltaTime;
			countMesh.text = curTime.ToString("F1");
			yield return null;
		}

		networkManager.GetComponent<NetworkView>().RPC("EndPlayer", RPCMode.All, networkManager.PlayerHeroEntity.Owner);
		networkManager.GetComponent<NetworkView>().RPC("SetGameEnd", RPCMode.All, true);
	}
}
