using UnityEngine;
using System.Collections;

public class FindingServer : BaseSceneEntity {

	protected override void OnEnable() {
		StopCoroutine("UpdateFindingServer");
		StartCoroutine("UpdateFindingServer");
		base.OnEnable();
	}

	private IEnumerator UpdateFindingServer() {
		while (true) {
			windowManager[WindowIndx].Enable = !baseUI.FindingServerUi.EnableTextInput;

			if (WindowIndx == 0 && userInterface.MasterServerView)
				WindowIndx = 1;
			else if (WindowIndx == 1 && !userInterface.MasterServerView)
				WindowIndx = 0;

			yield return null;
		}
	}
}
