using UnityEngine;
using System.Collections;

public class CreateLanButton : BaseButtonEntity {

	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.RoomLobby);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		//baseUI.Scene = state;

		bool useNat = Network.HavePublicAddress();
		Network.InitializeServer(NetworkManager.MAX_CLIENT_COUNT, int.Parse(networkManager.Port), useNat);
	}
}
