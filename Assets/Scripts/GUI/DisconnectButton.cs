using UnityEngine;
using System.Collections;

public class DisconnectButton : BaseButtonEntity {

	[SerializeField]
	private GameState goBackTo = GameState.MainMenu;

	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", goBackTo);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		baseUI.Scene = state;

		if (userInterface.MasterServerView)
			MasterServer.UnregisterHost();

		//networkManager.ResetRoomLobbyTimer();
		Network.Disconnect();
	}
}
