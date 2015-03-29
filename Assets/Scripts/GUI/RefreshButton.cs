using UnityEngine;
using System.Collections;

public class RefreshButton : BaseButtonEntity {
	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.RoomLobby);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		MasterServer.RequestHostList(networkManager.GameType);
		userInterface.LastRequest = Time.realtimeSinceStartup;
	}
}
