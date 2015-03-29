using UnityEngine;
using System.Collections;

public enum Layer {
	Player,
	Ally,
	Enemy,
	NPC,
	Ground,
	Slippery,
	Foreground,
	SlideEdge,
	Platform
}

public enum SortingLayer {
	Background,
	Foreground,
	Platform,
	CharacterBack,
	CharacterMid,
	CharacterFront,
	UIBack,
	UIMid,
	UIFront
}

public class LayerManager : MonoBehaviour {

	private static LayerManager layerManager;

	//[SerializeField]
	private string layerPlayer = "Player";
	public static int LayerPlayer {
		get { return LayerMask.NameToLayer(layerManager.layerPlayer); }
	}

	//[SerializeField]
	private string layerAlly = "Ally";
	public static int LayerAlly {
		get { return LayerMask.NameToLayer(layerManager.layerAlly); }
	}

	//[SerializeField]
	private string layerEnemy = "Enemy";
	public static int LayerEnemy {
		get { return LayerMask.NameToLayer(layerManager.layerEnemy); }
	}

	//[SerializeField]
	private string layerNPC = "NPC";
	public static int LayerNPC {
		get { return LayerMask.NameToLayer(layerManager.layerNPC); }
	}

	//[SerializeField]
	private string layerGround = "Ground";
	public static int LayerGround {
		get { return LayerMask.NameToLayer(layerManager.layerGround); }
	}

	private string layerSlippery = "Slippery";
	public static int LayerSlippery {
		get { return LayerMask.NameToLayer(layerManager.layerSlippery); }
	}

	private string layerParallaxBG = "Parallax Background";
	public static int LayerParallaxBG {
		get { return LayerMask.NameToLayer(layerManager.layerParallaxBG); }
	}

	private string layerStaticBG = "Static Background";
	public static int LayerStaticBG {
		get { return LayerMask.NameToLayer(layerManager.layerStaticBG); }
	}

	//[SerializeField]
	private string layerForeground = "Foreground";
	public static int LayerForeground {
		get { return LayerMask.NameToLayer(layerManager.layerForeground); }
	}

	//[SerializeField]
	private string layerSlideEdge = "Slide Edge";
	public static int LayerSlideEdge {
		get { return LayerMask.NameToLayer(layerManager.layerSlideEdge); }
	}

	private string layerPlatform = "Platform";
	public static int LayerPlatform {
		get { return LayerMask.NameToLayer(layerManager.layerPlatform); }
	}

	//[SerializeField]
	private string sortingLayerBackground = "Background";
	public static string SortingLayerBackground {
		get { return layerManager.sortingLayerBackground; }
	}

	//[SerializeField]
	private string sortingLayerForeground = "Foreground";
	public static string SortingLayerForeground {
		get { return layerManager.sortingLayerForeground; }
	}

	//[SerializeField]
	private string sortingLayerPlatform = "Platform";
	public static string SortingLayerPlatform {
		get { return layerManager.sortingLayerPlatform; }
	}

	//[SerializeField]
	private string sortingLayerCharacterBack = "Character Back";
	public static string SortingLayerCharacterBack {
		get { return layerManager.sortingLayerCharacterBack; }
	}

	//[SerializeField]
	private string sortingLayerCharacterMid = "Character Mid";
	public static string SortingLayerCharacterMid {
		get { return layerManager.sortingLayerCharacterMid; }
	}

	//[SerializeField]
	private string sortingLayerCharacterFront = "Character Front";
	public static string SortingLayerCharacterFront {
		get { return layerManager.sortingLayerCharacterFront; }
	}

	//[SerializeField]
	private string sortingLayerUiBack = "UI Back";
	public static string SortingLayerUiBack {
		get { return layerManager.sortingLayerUiBack; }
	}

	//[SerializeField]
	private string sortingLayerUiMid = "UI Mid";
	public static string SortingLayerUiMid {
		get { return layerManager.sortingLayerUiMid; }
	}

	//[SerializeField]
	private string sortingLayerUiFront = "UI Front";
	public static string SortingLayerUiFront {
		get { return layerManager.sortingLayerUiFront; }
	}


	public static int GetLayer(Layer layer) {
		int layerInt = (int)layer;

		if (layer == Layer.Player)
			layerInt = LayerPlayer;
		else if (layer == Layer.Ally)
			layerInt = LayerAlly;
		else if (layer == Layer.Enemy)
			layerInt = LayerEnemy;
		else if (layer == Layer.NPC)
			layerInt = LayerNPC;
		else if (layer == Layer.Ground)
			layerInt = LayerGround;
		else if (layer == Layer.Slippery)
			layerInt = LayerSlippery;
		else if (layer == Layer.Foreground)
			layerInt = LayerForeground;
		else if (layer == Layer.SlideEdge)
			layerInt = LayerSlideEdge;
		else if (layer == Layer.Platform)
			layerInt = LayerPlatform;

		return layerInt;
	}

	public enum SortingLayer {
		Background,
		Foreground,
		Platform,
		CharacterBack,
		CharacterMid,
		CharacterFront,
		UIBack,
		UIMid,
		UIFront
	}

	public static string GetSortingLayer(SortingLayer layer) {
		string layerString = string.Empty;

		if (layer == SortingLayer.Background)
			layerString = SortingLayerBackground;
		else if (layer == SortingLayer.Foreground)
			layerString = SortingLayerForeground;
		else if (layer == SortingLayer.Platform)
			layerString = SortingLayerPlatform;
		else if (layer == SortingLayer.CharacterBack)
			layerString = SortingLayerCharacterBack;
		else if (layer == SortingLayer.CharacterMid)
			layerString = SortingLayerCharacterMid;
		else if (layer == SortingLayer.CharacterFront)
			layerString = SortingLayerCharacterFront;
		else if (layer == SortingLayer.UIBack)
			layerString = SortingLayerUiBack;
		else if (layer == SortingLayer.UIMid)
			layerString = SortingLayerUiMid;
		else if (layer == SortingLayer.UIFront)
			layerString = SortingLayerUiFront;

		return layerString;
	}

	//[SerializeField]
	//private string layerIndicator = "Indicator";
	//public static int LayerIndicator {
	//    get { return LayerMask.NameToLayer(layerManager.layerIndicator); }
	//}

	//[SerializeField]
	//private string layerEnd = "End";
	//public static int LayerEnd {
	//    get { return LayerMask.NameToLayer(layerManager.layerEnd); }
	//}

	//[SerializeField]
	//private string layerUI = "UI";
	//public static int LayerUI {
	//    get { return LayerMask.NameToLayer(layerManager.layerUI); }
	//}

	//[SerializeField]
	//private string layerCheetahBounds = "CheetahBounds";
	//public static int LayerCheetahBounds {
	//    get { return LayerMask.NameToLayer(layerManager.layerCheetahBounds); }
	//}

	//[SerializeField]
	//private string layerCheetah = "Cheetah";
	//public static int LayerCheetah {
	//    get { return LayerMask.NameToLayer(layerManager.layerCheetah); }
	//}

	//[SerializeField]
	//private string layerCandyCanes = "CandyCane";
	//public static int LayerCandyCanes {
	//    get { return LayerMask.NameToLayer(layerManager.layerCandyCanes); }
	//}

	//public static int player;
	//public static int ally;
	//public static int enemy;
	//public static int npc;
	//public static int ground;
	//public static int indicator;

	private void Awake() {
		layerManager = this;
	}

	private void Start() {
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
			Destroy(gameObject);
	}
}
