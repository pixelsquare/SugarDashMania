using UnityEngine;
using System.Collections;

public class RoomLobby : BaseSceneEntity {

	protected override void OnEnable() {
		if (Network.isServer)
			windowManager[WindowIndx].Buttons[0].gameObject.SetActive(true);
		if (Network.isClient)
			windowManager[WindowIndx].Buttons[0].gameObject.SetActive(false);
		StopCoroutine("UpdateRoomLobby");
		StartCoroutine("UpdateRoomLobby");
		base.OnEnable();
	}

	private IEnumerator UpdateRoomLobby() {
		yield return new WaitForSeconds(0.2f);
		foreach (BaseButtonEntity element in windowManager[WindowIndx].Buttons) {
			if (!element.Clickable)
				element.Clickable = true;
		}

		while (true) {

			/**
			 *	When timer reaches less than 1 disable all the buttons in the scene
			 **/

			if (networkManager.Starting) {
				foreach (BaseButtonEntity element in windowManager[WindowIndx].Buttons) {
					if (element.Clickable)
						element.Clickable = false;
				}
			}

			//if (Network.isServer && windowManager[WindowIndx].Buttons[2].gameObject.activeSelf) {
			//    windowManager[WindowIndx].Buttons[2].gameObject.SetActive(false);
			//    windowManager[WindowIndx].Buttons[1].gameObject.SetActive(true);
			//}

			//if (Network.isClient && windowManager[WindowIndx].Buttons[1].gameObject.activeSelf) {
			//    windowManager[WindowIndx].Buttons[1].gameObject.SetActive(false);
			//    windowManager[WindowIndx].Buttons[2].gameObject.SetActive(true);
			//}

			//if (networkManager.IsReady && windowManager[WindowIndx].Buttons[0].Clickable && windowManager[WindowIndx].Buttons[1].Clickable && windowManager[WindowIndx].Buttons[2].Clickable) {
			//    windowManager[WindowIndx].Buttons[0].Clickable = false;
			//    windowManager[WindowIndx].Buttons[1].Clickable = false;
			//    windowManager[WindowIndx].Buttons[2].Clickable = false;
			//}

			//if (!networkManager.IsReady && !windowManager[WindowIndx].Buttons[0].Clickable && windowManager[WindowIndx].Buttons[1].Clickable && windowManager[WindowIndx].Buttons[2].Clickable) {
			//    windowManager[WindowIndx].Buttons[0].Clickable = true;
			//    windowManager[WindowIndx].Buttons[1].Clickable = false;
			//    windowManager[WindowIndx].Buttons[2].Clickable = false;
			//}

			yield return null;
		}
	}
}
