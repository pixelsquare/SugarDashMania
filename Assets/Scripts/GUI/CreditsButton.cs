using UnityEngine;
using System.Collections;

public class CreditsButton : BaseButtonEntity {

	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.Credits);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		baseUI.Scene = state;
	}
}
