using UnityEngine;
using System.Collections;

public class JoinButton : BaseButtonEntity {

	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.RoomLobby);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		//baseUI.Scene = state;

		Network.Connect(networkManager.IpAddress, int.Parse(networkManager.Port));
	}
}
