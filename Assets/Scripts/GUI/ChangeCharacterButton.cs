using UnityEngine;
using System.Collections;

public class ChangeCharacterButton : BaseButtonEntity {

	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.CharacterSelection);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		baseUI.Scene = state;
	}
}
