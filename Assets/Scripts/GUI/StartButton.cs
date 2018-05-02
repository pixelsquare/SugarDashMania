using UnityEngine;
using System.Collections;

public class StartButton : BaseButtonEntity {

	private int playerCharacters = 0;

	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.InGame);
		base.Clicked(clicked);
	}

	protected override void OnEnable() {
		//StartCoroutine("UpdateStartButton");
		base.OnEnable();
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		//baseUI.Scene = state;
		networkManager.IsReady = !networkManager.IsReady;
		networkManager.GetComponent<NetworkView>().RPC("SetReady", RPCMode.AllBuffered, Network.player, networkManager.IsReady);

		playerCharacters = 0;
		foreach (PlayerInformation element in networkManager.PlayerInformationList) {
			if (element.CharType != CharacterType.Bubbly)
				playerCharacters++;
		}

		yield return new WaitForSeconds(0.2f);
		//if (playerReadyCounter < networkManager.PlayerInformationList.Count)
		//    networkManager.networkView.RPC("SendChatMessage", RPCMode.All, "[SERVER] Force Start Initiated!");

		foreach (PlayerInformation element in networkManager.PlayerInformationList) {
			if(element.CharType != CharacterType.Bubbly)
				networkManager.GetComponent<NetworkView>().RPC("SendChatMessage", RPCMode.All, "[SERVER] Game could not be started!");
		}

		if (playerCharacters != 0)
			yield break;

		networkManager.GetComponent<NetworkView>().RPC("SetAllPlayersReady", RPCMode.AllBuffered);

		networkManager.StartCoroutine(networkManager.StartRoomLobbyTimer());
	}

	//private IEnumerator UpdateStartButton() {
	//    while (true) {

	//        playerCharacters = 0;
	//        foreach (PlayerInformation element in networkManager.PlayerInformationList) {
	//            if (element.CharType != CharacterType.Bubbly)
	//                playerCharacters++;
	//        }

	//        yield return null;
	//    }
	//}
}
