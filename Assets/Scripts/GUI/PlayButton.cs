using UnityEngine;
using System.Collections;

public class PlayButton : BaseButtonEntity {

	public override void Clicked(bool clicked) {
		if(!clicked)
			StartCoroutine("AnimationRoutine", GameState.FindingServer);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		baseUI.Scene = state;
	}
}
