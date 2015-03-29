using UnityEngine;
using System.Collections;

public class Nougat : BaseItemEntity {
	[SerializeField]
	private int score = 300;

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

	public void DestroyNougat() {
		Destroy(collider2D);
		anim.SetTrigger("Activate");
	}

	protected override void OnTriggerEnter2D(Collider2D col) {
		base.OnTriggerEnter2D(col);
		if (col.tag == "Player" && !col.collider2D.isTrigger) {
			DestroyNougat();
		}
	}
}



//public class Nougat : BaseEnvironmentEntity {

//    protected override void Awake() {
//        base.Awake();
//    }

//    protected override void Start() {
//        base.Start();
//    }

//    protected override void RemoveItem() {
//        if (Network.isServer) {
//            AddScore(300);
//        }
//        //PushObjects(5f, 10000f);
//        base.RemoveItem();
//    }

//    private void OnTriggerEnter2D(Collider2D col) {
//        if (col.tag == "Player" && !col.isTrigger) {
//            hitId = col.GetComponent<BaseCharacterEntity>().Owner;
//            networkView.RPC("RunAnimation", RPCMode.All);
//        }
//    }

//    private void CallIndicator() {
//        // If nougat is triggerd by client machine
//        // call an RPC to everyone, to locate for the server
//        // to spawn the indicator
//        if (Network.isClient)
//            networkView.RPC("SpawnIndicator", RPCMode.All, Network.player);

//        // If the nougat is triggered by server machine
//        // Instantiate immediately
//        if (Network.isServer && indicator != null)
//            Network.Instantiate(indicator, transform.position, Quaternion.identity, NetworkGroup.Indicator);
//    }

//    [RPC]
//    private void SpawnIndicator(NetworkPlayer id) {
//        if (Network.isServer && Network.player == id && indicator != null) {
//            Network.Instantiate(indicator, transform.position, Quaternion.identity, NetworkGroup.Indicator);
//        }
//    }

//    [RPC]
//    private void RunAnimation() {
//        anim.SetTrigger("Activate");
//    }
//}
