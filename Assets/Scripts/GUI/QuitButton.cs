using UnityEngine;
using System.Collections;

public class QuitButton : BaseButtonEntity {

	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.Intro);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		Application.Quit();
	}
}
