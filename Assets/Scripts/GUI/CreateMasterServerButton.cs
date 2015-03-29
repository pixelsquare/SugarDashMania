using UnityEngine;
using System.Collections;

public class CreateMasterServerButton : BaseButtonEntity {
	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.RoomLobby);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		//baseUI.Scene = state;

		bool useNat = !Network.HavePublicAddress();
		Network.InitializeServer(NetworkManager.MAX_CLIENT_COUNT, int.Parse(networkManager.Port), useNat);
		networkManager.GameName = networkManager.PlayerName;
		MasterServer.RegisterHost(networkManager.GameType, networkManager.GameName, networkManager.GameDescription);
	}
}
