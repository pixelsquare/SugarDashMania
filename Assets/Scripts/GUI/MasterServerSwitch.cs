using UnityEngine;
using System.Collections;

public class MasterServerSwitch : BaseButtonEntity {

	[SerializeField]
	private TextMesh buttonText;

	protected override void OnEnable() {
		GameUtility.ChangeSortingLayerRecursively(buttonText.transform, LayerManager.SortingLayerUiFront);
		buttonText.renderer.sortingOrder = 1;
		base.OnEnable();
	}

	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.FindingServer);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		if (userInterface.MasterServerView)
			buttonText.text = "LAN View";
		else
			buttonText.text = "Master Server View";

		userInterface.MasterServerView = !userInterface.MasterServerView;
	}
}
