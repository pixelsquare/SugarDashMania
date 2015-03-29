using UnityEngine;
using System.Collections;

public class ReadyButton : BaseButtonEntity {
	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.FindingServer);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		//baseUI.Scene = state;
		networkManager.IsReady = !networkManager.IsReady;
		networkManager.networkView.RPC("SetReady", RPCMode.AllBuffered, Network.player, networkManager.IsReady);
	}
}
