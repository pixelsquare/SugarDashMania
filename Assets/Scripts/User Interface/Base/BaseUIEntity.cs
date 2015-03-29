using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState {
	Intro,
	MainMenu,
	FindingServer,
	RoomLobby,
	CharacterSelection,
	InGame,
	Credits,
	Settings
}

public class BaseUIEntity : MonoBehaviour {
	[SerializeField]
	protected GameState curGameState = GameState.Intro;

	[SerializeField]
	protected List<BaseSceneEntity> gameSceneList = new List<BaseSceneEntity>();

	[SerializeField]
	protected SettingsUI settingsUi;
	public SettingsUI SettingsUi {
		get { return settingsUi; }
		set { settingsUi = value; }
	}

	[SerializeField]
	protected FindingServerUI findingServerUi;
	public FindingServerUI FindingServerUi {
		get { return findingServerUi; }
		set { findingServerUi = value; }
	}

	[SerializeField]
	protected RoomLobbyUI roomLobbyUi;
	public RoomLobbyUI RoomLobbyUi {
		get { return roomLobbyUi; }
		set { roomLobbyUi = value; }
	}

	[SerializeField]
	protected CharacterSelectionUI characterSelectionUi;
	public CharacterSelectionUI CharacterSelectionUi {
		get { return characterSelectionUi; }
		set { characterSelectionUi = value; }
	}

	[SerializeField]
	protected InGameUI inGameUi;
	public InGameUI InGameUi {
		get { return inGameUi; }
		set { inGameUi = value; }
	}

	protected delegate void GuiSceneHandler();
	protected List<GuiSceneHandler> guiSceneList = new List<GuiSceneHandler>();

	protected BaseSceneEntity curScene;
	protected NetworkManager networkManager;

	protected virtual void Awake() {
		networkManager = FindObjectOfType<NetworkManager>();

		settingsUi.Init();
		findingServerUi.Init();
		roomLobbyUi.Init();
		characterSelectionUi.Init();
		inGameUi.Init();
		ConnectionTest.InitConnectionTest();

		ApplyBrightness(settingsUi.Brightness.CurN, settingsUi.Brightness.Min, settingsUi.Brightness.Max);
		ApplyVolume(settingsUi.Volume.CurN, settingsUi.Volume.Min, settingsUi.Volume.Max);
	}

	protected virtual void Start() {
		DontDestroyOnLoad(this);
		//if (FindObjectsOfType(GetType()).Length > 1)
		//    Destroy(gameObject);

		DisableAllScenes();
		Scene = curGameState;
	}

	private void OnGUI() {
		if (guiSceneList.Count > 0) {
			guiSceneList[(int)curGameState]();
		}
	}

	public GameState Scene {
		get { return curGameState; }
		set { 
			curGameState = value;
			DisableAllScenes();
			curScene = gameSceneList[(int)curGameState];
			curScene.EnableScene();
			curScene.gameObject.SetActive(true);
		}
	}

	private void DisableAllScenes() {
		int i;
		for (i = 0; i < gameSceneList.Count; i++) {
			if (gameSceneList[i].gameObject.activeInHierarchy && gameSceneList[i] != null)
				gameSceneList[i].DisableScene();
		}
	}

	protected void GameStateSwitcher(GameState prev, GameState next) {
		GUILayout.BeginArea(new Rect((Screen.width * 0.5f) - 100f, 0f, Screen.width, 50f));

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(curGameState.ToString());
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect((Screen.width * 0.5f) - 100f, 20f, Screen.width, 50f));

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		if (GUILayout.Button("<<"))
			Scene = prev;

		if (GUILayout.Button(">>"))
			Scene = next;

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	protected void ApplyBrightness(float n, float min, float max) {
		// Clamps the value of Ambient to 0.4f to 1.0f
		// instead of 0 to 1.0f
		float inverseLerp = Mathf.InverseLerp(min, max, n);
		float minOffset = 0.4f;

		Color tmpAmbient = new Color();
		tmpAmbient.r = inverseLerp + (minOffset - inverseLerp * minOffset);
		tmpAmbient.g = inverseLerp + (minOffset - inverseLerp * minOffset);
		tmpAmbient.b = inverseLerp + (minOffset - inverseLerp * minOffset);

		RenderSettings.ambientLight = tmpAmbient;
	}

	protected void ApplyVolume(float n, float min, float max) {
		float inverseLerp = Mathf.InverseLerp(min, max, n);
		AudioListener.volume = inverseLerp;
	}

	protected Rect ConvertRectViewport(Rect rect) {
		Rect tmpRect = rect;
		tmpRect.x = (Screen.width * rect.x) - (rect.width * rect.x);	//(Screen.width * tmpRect.x) - (tmpRect.width * tmpRect.x);
		tmpRect.y = (Screen.height * rect.y) - (rect.height * rect.y);	//(Screen.height * tmpRect.y) - (tmpRect.height * tmpRect.y);
		return tmpRect;
	}

	//protected void ResetBaseUIEntity() {
	//    foreach (BaseSceneEntity element in gameSceneList) {
	//        element.ResetSceneEntity();
	//    }
	//}
}

[System.Serializable]
public class SettingsUI {
	[SerializeField]
	private Slider brightness;
	public Slider Brightness {
		get { return brightness; }
		set { brightness = value; }
	}

	[SerializeField]
	private Slider volume;
	public Slider Volume {
		get { return volume; }
		set { volume = value; }
	}

	private bool enableIpInput = false;
	public bool EnableIpInput {
		get { return enableIpInput; }
		set { enableIpInput = value; }
	}

	[SerializeField]
	private GUIProperty ip;
	public GUIProperty IP {
		get { return ip; }
		set { ip = value; }
	}

	private string ipString = "127.0.0.1";
	public string IpString {
		get { return ipString; }
		set { ipString = value; }
	}

	private bool enablePortInput = false;
	public bool EnablePortInput {
		get { return enablePortInput; }
		set { enablePortInput = value; }
	}

	[SerializeField]
	private GUIProperty port;
	public GUIProperty Port {
		get { return port; }
		set { port = value; }
	}

	private string portString = "25003";
	public string PortString {
		get { return portString; }
		set { portString = value; }
	}

	//[SerializeField]
	//private bool isFullscreen = false;
	//public bool IsFullScreen {
	//    get { return isFullscreen; }
	//    set { isFullscreen = value; }
	//}

	//[SerializeField]
	//private GUIProperty fullScreen;
	//public GUIProperty FullScreen {
	//    get { return fullScreen; }
	//    set { fullScreen = value; }
	//}

	public void Init() {
		ip.Init();
		port.Init();
	}
}

[System.Serializable]
public class FindingServerUI {
	private bool enableTextInput = true;
	public bool EnableTextInput {
		get { return enableTextInput; }
		set { 
			enableTextInput = value;

			if (enableTextInput)
				NameString = "";
		}
	}

	[SerializeField]
	private GUIProperty nameInputWindow;
	public GUIProperty NameInputWindow {
		get { return nameInputWindow; }
		set { nameInputWindow = value; }
	}

	[SerializeField]
	private TextAreaButton welcomeNote;
	public TextAreaButton WelcomeNote {
		get { return welcomeNote; }
		set { welcomeNote = value; }
	}

	private string nameString = "";
	public string NameString {
		get { return nameString; }
		set { nameString = value; }
	}

	[SerializeField]
	private GUIProperty serverListWindow;
	public GUIProperty ServerListWindow {
		get { return serverListWindow; }
		set { serverListWindow = value; }
	}

	[SerializeField]
	private Texture2D availableServerTex;
	public Texture2D AvailableServerTex {
		get { return availableServerTex; }
		set { availableServerTex = value; }
	}

	[SerializeField]
	private ConnectionStatus connectionStatus;
	public ConnectionStatus ConnectionStats {
		get { return connectionStatus; }
		set { connectionStatus = value; }
	}

	public void Init() {
		serverListWindow.Init();
		nameInputWindow.Init();
	}
}

[System.Serializable]
public class RoomLobbyUI {
	[SerializeField]
	private GUIProperty redPlayerlist;
	public GUIProperty RedPlayerlist {
		get { return redPlayerlist; }
		set { redPlayerlist = value; }
	}

	[SerializeField]
	private GUIProperty greenPlayerlist;
	public GUIProperty GreenPlayerlist {
		get { return greenPlayerlist; }
		set { greenPlayerlist = value; }
	}

	[SerializeField]
	private GUIProperty playerInfo;
	public GUIProperty PlayerInfo {
		get { return playerInfo; }
		set { playerInfo = value; }
	}

	[SerializeField]
	private GUIProperty chat;
	public GUIProperty Chat {
		get { return chat; }
		set { chat = value; }
	}

	[SerializeField]
	private Texture2D readyIndicator;
	public Texture2D ReadyIndicator {
		get { return readyIndicator; }
		set { readyIndicator = value; }
	}

	[SerializeField]
	private Texture2D notReadyIndicator;
	public Texture2D NotReadyIndicator {
		get { return notReadyIndicator; }
		set { notReadyIndicator = value; }
	}

	[SerializeField]
	private Texture2D bubblyIcon;
	public Texture2D BubblyIcon {
		get { return bubblyIcon; }
		set { bubblyIcon = value; }
	}

	[SerializeField]
	private Texture2D ricoIcon;
	public Texture2D RicoIcon {
		get { return ricoIcon; }
		set { ricoIcon = value; }
	}

	[SerializeField]
	private Texture2D popIcon;
	public Texture2D PopIcon {
		get { return popIcon; }
		set { popIcon = value; }
	}

	[SerializeField]
	private Texture2D cocoIcon;
	public Texture2D CocoIcon {
		get { return cocoIcon; }
		set { cocoIcon = value; }
	}

	[SerializeField]
	private Texture2D bubblyBG;
	public Texture2D BubblyBG {
		get { return bubblyBG; }
		set { bubblyBG = value; }
	}

	[SerializeField]
	private Texture2D ricoBG;
	public Texture2D RicoBG {
		get { return ricoBG; }
		set { ricoBG = value; }
	}

	[SerializeField]
	private Texture2D popBG;
	public Texture2D PopBG {
		get { return popBG; }
		set { popBG = value; }
	}

	[SerializeField]
	private Texture2D cocoBG;
	public Texture2D CocoBG {
		get { return cocoBG; }
		set { cocoBG = value; }
	}

	[SerializeField]
	private Texture2D selectCharBG;
	public Texture2D SelectCharBG {
		get { return selectCharBG; }
		set { selectCharBG = value; }
	}

	[SerializeField]
	private Texture2D chatTex;
	public Texture2D ChatTex {
		get { return chatTex; }
		set { chatTex = value; }
	}

	[SerializeField]
	private Texture2D logTex;
	public Texture2D LogTex {
		get { return logTex; }
		set { logTex = value; }
	}

	[SerializeField]
	private Texture2D selectTeamTex;
	public Texture2D SelectTeamTex {
		get { return selectTeamTex; }
		set { selectTeamTex = value; }
	}

	[SerializeField]
	private Texture2D nameTex;
	public Texture2D NameTex {
		get { return nameTex; }
		set { nameTex = value; }
	}

	public void Init() {
		redPlayerlist.Init();
		greenPlayerlist.Init();
		playerInfo.Init();
		chat.Init();
		
	}
}

[System.Serializable]
public class CharacterSelectionUI {
	[SerializeField]
	private GUIProperty lore;
	public GUIProperty Lore {
		get { return lore; }
		set { lore = value; }
	}

	[SerializeField]
	private GUIProperty characterSelector;
	public GUIProperty CharacterSelector {
		get { return characterSelector; }
		set { characterSelector = value; }
	}

	public void Init() {
		lore.Init();
		characterSelector.Init();
	}
}

[System.Serializable]
public class InGameUI {
	[SerializeField]
	private GUIProperty pause;
	public GUIProperty Pause {
		get { return pause; }
		set { pause = value; }
	}

	[SerializeField]
	private GUIProperty[] playerPlacement;
	public GUIProperty[] PlayerPlacement {
		get { return playerPlacement; }
		set { playerPlacement = value; }
	}

	[SerializeField]
	private Rect[] charIconRect;
	public Rect[] CharIconRect {
		get { return charIconRect; }
		set { charIconRect = value; }
	}

	[SerializeField]
	private GUIProperty[] place;
	public GUIProperty[] Place {
		get { return place; }
		set { place = value; }
	}


	[SerializeField]
	private GUIProperty[] item;
	public GUIProperty[] Item {
		get { return item; }
		set { item = value; }
	}

	[SerializeField]
	private GUIProperty[] nameHolder;
	public GUIProperty[] NameHolder {
		get { return nameHolder; }
		set { nameHolder = value; }
	}

	[SerializeField]
	private GUISkin scoreBG;
	public GUISkin ScoreBG {
		get { return scoreBG; }
		set { scoreBG = value; }
	}

	[SerializeField]
	private Texture2D redTeamWins;
	public Texture2D RedTeamWins {
		get { return redTeamWins; }
		set { redTeamWins = value; }
	}

	[SerializeField]
	private Texture2D greenTeamWins;
	public Texture2D GreenTeamWins {
		get { return greenTeamWins; }
		set { greenTeamWins = value; }
	}

	[SerializeField]
	private Texture2D redBG;
	public Texture2D RedBG {
		get { return redBG; }
		set { redBG = value; }
	}

	[SerializeField]
	private Texture2D greenBG;
	public Texture2D GreenBG {
		get { return greenBG; }
		set { greenBG = value; }
	}

	[SerializeField]
	private Texture2D pauseTex;
	public Texture2D PauseTex {
		get { return pauseTex; }
		set { pauseTex = value; }
	}

	[SerializeField]
	private Color redNames = Color.red;
	public Color RedNames {
		get { return redNames; }
		set { redNames = value; }
	}

	[SerializeField]
	private Color greenNames = Color.green;
	public Color GreenNames {
		get { return greenNames; }
		set { greenNames = value; }
	}


	[SerializeField]
	private GUIProperty scoreboard;
	public GUIProperty Scoreboard {
		get { return scoreboard; }
		set { scoreboard = value; }
	}

	[SerializeField]
	private GUISkin scoreboardName;
	public GUISkin ScoreboardName {
		get { return scoreboardName; }
		set { scoreboardName = value; }
	}

	[SerializeField]
	private GUIProperty score;
	public GUIProperty Score {
		get { return score; }
		set { score = value; }
	}

	[SerializeField]
	private Texture2D[] itemUi;
	public Texture2D[] ItemUi {
		get { return itemUi; }
		set { itemUi = value; }
	}

	public void Init() {
		int i;
		for (i = 0; i < 4; i++) {
			playerPlacement[i].Init();
			place[i].Init();
			item[i].Init();
			nameHolder[i].Init();
		}

		scoreboard.Init();
		score.Init();
		pause.Init();
	}
}

[System.Serializable]
public class Slider {
	[SerializeField]
	private float min = 0f;
	public float Min {
		get { return min; }
		set { min = value; }
	}

	[SerializeField]
	private float max = 10f;
	public float Max {
		get { return max; }
		set { max = value; }
	}

	[SerializeField]
	private float curN;
	public float CurN {
		get { return curN; }
		set { curN = value; }
	}

	[SerializeField]
	private Rect sliderRect = new Rect(0f, 0.1f, 450f, 10f);
	public Rect SliderRect {
		get { return sliderRect; }
		set { sliderRect = value; }
	}

	[SerializeField]
	private GUISkin skin;
	public GUISkin Skin {
		get { return skin; }
		set { skin = value; }
	}
}

[System.Serializable]
public class GUIProperty {
	[SerializeField]
	private Rect windowRect;
	public Rect WindowRect {
		get { return windowRect; }
		set { windowRect = value; }
	}

	[SerializeField]
	private bool convertToViewport;
	public bool ConvertToViewport {
		get { return convertToViewport; }
		set { convertToViewport = value; }
	}

	[SerializeField]
	private GUISkin skin;
	public GUISkin Skin {
		get { return skin; }
		set { skin = value; }
	}

	public void Init() {
		if (convertToViewport)
			WindowRect = ConvertRectViewport(WindowRect);
	}

	public Rect ConvertRectViewport(Rect rect) {
		Rect tmpRect = rect;
		tmpRect.x = (Screen.width * rect.x) - (rect.width * rect.x);	//(Screen.width * tmpRect.x) - (tmpRect.width * tmpRect.x);
		tmpRect.y = (Screen.height * rect.y) - (rect.height * rect.y);	//(Screen.height * tmpRect.y) - (tmpRect.height * tmpRect.y);
		return tmpRect;
	}
}
