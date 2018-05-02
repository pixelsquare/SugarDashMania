using UnityEngine;
using System.Collections;

public class TextAreaButton : BaseButtonEntity {

	private TextMesh textAreaMesh;
	private BaseUIEntity baseUserInterface;

	private void Awake() {
		textAreaMesh = GetComponent<TextMesh>();
		baseUserInterface = FindObjectOfType<BaseUIEntity>();

		GetComponent<Renderer>().enabled = false;
	}

	public void InitializeText(string info) {
		GetComponent<Renderer>().enabled = true;
		textAreaMesh.text = info;
		GameUtility.ChangeSortingLayerRecursively(transform, LayerManager.SortingLayerUiFront, LayerManager.SortingLayerUiFront);

		BoxCollider2D boxCollider = gameObject.GetComponent<BoxCollider2D>();
		if (boxCollider == null) {
			boxCollider = gameObject.AddComponent<BoxCollider2D>();
		}
	}

	public override void Clicked(bool clicked) {
		if (!clicked)
			StartCoroutine("AnimationRoutine", GameState.Intro);
		base.Clicked(clicked);
	}

	protected override IEnumerator AnimationRoutine(GameState state) {
		yield return StartCoroutine(base.AnimationRoutine(state));
		baseUserInterface.FindingServerUi.EnableTextInput = true;
		Destroy(GetComponent<Collider2D>());
		GetComponent<Renderer>().enabled = false;
	}

	//private void Awake() {
	//    baseUserInterface = FindObjectOfType<BaseUIEntity>();
	//}

	//private void OnMouseDown() {
	//    baseUserInterface.FindingServerUi.EnableTextInput = true;
	//    baseUserInterface.FindingServerUi.OutputString = "";
	//    Destroy(collider2D);
	//    renderer.enabled = false;
	//}
}
