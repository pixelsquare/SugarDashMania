using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserInterface : BaseUIEntity {

	private Vector2 chatScrollPos;
	public Vector2 ChatScrollPos {
		get { return chatScrollPos; }
		set { chatScrollPos = value; }
	}

	private string outputString = string.Empty;
	public string OutputString {
		get { return outputString; }
		set { outputString = value; }
	}

	private string inputString = string.Empty;

	private Vector2 masterServerScrollPos;
	public Vector2 MasterServerScrollPos {
		get { return masterServerScrollPos; }
		set { masterServerScrollPos = value; }
	}

	private bool masterServerView = false;
	public bool MasterServerView {
		get { return masterServerView; }
		set { masterServerView = value; }
	}

	private bool showScoreboard = false;
	public bool ShowScoreboard {
		get { return showScoreboard; }
		set { showScoreboard = value; }
	}

	private bool paused = false;
	public bool Paused {
		get { return paused; }
		set { paused = value; }
	}

	private float lastRequest = 0f;
	public float LastRequest {
		get { return lastRequest; }
		set { lastRequest = value; }
	}

	private float refreshTimeout = 3f;
	public float RefreshTimeout {
		get { return refreshTimeout; }
		set { refreshTimeout = value; }
	}

	private Rect tmpRect;
	private GUIStyle tmpStyle;
	private int focusedWindowID = 0;

	private PlayerInformation myInfo;
	private PlayerInformation[] otherInfo = new PlayerInformation[3];

	private int myIndx = 0;
	private int[] otherIndx = new int[3];

	protected override void Awake() {
		guiSceneList.Add(new BaseUIEntity.GuiSceneHandler(OnIntro));
		guiSceneList.Add(new BaseUIEntity.GuiSceneHandler(OnMainMenu));
		guiSceneList.Add(new BaseUIEntity.GuiSceneHandler(OnFindingServer));
		guiSceneList.Add(new BaseUIEntity.GuiSceneHandler(OnRoomLobby));
		guiSceneList.Add(new BaseUIEntity.GuiSceneHandler(OnCharacterSelection));
		guiSceneList.Add(new BaseUIEntity.GuiSceneHandler(OnInGame));
		guiSceneList.Add(new BaseUIEntity.GuiSceneHandler(OnCredits));
		guiSceneList.Add(new BaseUIEntity.GuiSceneHandler(OnSettings));

		base.Awake();
	}

	protected override void Start() {
		StopCoroutine("UpdateUserInterface");
		StartCoroutine("UpdateUserInterface");
		base.Start();
	}

	private IEnumerator UpdateUserInterface() {
		while (true) {
			if (Scene == GameState.RoomLobby && !networkManager.Starting) {
				if (focusedWindowID == 2) {
					networkManager.networkView.RPC("ChangeTeam", RPCMode.AllBuffered, Network.player, (int)Team.Red);
				}
				else if (focusedWindowID == 3) {
					networkManager.networkView.RPC("ChangeTeam", RPCMode.AllBuffered, Network.player, (int)Team.Green);
				}
			}

			yield return null;
		}
	}

	private void OnIntro() {
		//base.GameStateSwitcher(GameState.Settings, GameState.MainMenu);
	}

	private void OnMainMenu() {
		//base.GameStateSwitcher(GameState.Intro, GameState.FindingServer);
	}

	private void OnFindingServer() {
		//base.GameStateSwitcher(GameState.MainMenu, GameState.RoomLobby);

		if (findingServerUi.EnableTextInput) {
			//tmpRect = default(Rect);
			//tmpRect = findingServerUi.NameInputWindow.WindowRect;
			GUI.Window(0, findingServerUi.NameInputWindow.WindowRect, new GUI.WindowFunction(NameInputWindow), string.Empty, findingServerUi.NameInputWindow.Skin.window);
		}

		if (masterServerView && !findingServerUi.EnableTextInput) {
			//tmpRect = default(Rect);
			//tmpRect = findingServerUi.ServerListWindow.WindowRect;
			GUI.Window(1, findingServerUi.ServerListWindow.WindowRect, new GUI.WindowFunction(ServerListWindow), string.Empty, findingServerUi.ServerListWindow.Skin.window);
			GUILayout.BeginArea(new Rect((Screen.width * 0.45f) - 100f, (Screen.height * 0.13f) - 100f, 200f, 200f), findingServerUi.AvailableServerTex);
			GUILayout.EndArea();
		}
	}

	private void OnRoomLobby() {
		//base.GameStateSwitcher(GameState.FindingServer, GameState.CharacterSelection);

		if (!networkManager.HasStarted) {
			GUILayout.BeginArea(new Rect((Screen.width * 0.35f) - 100f, (Screen.height * 0.15f) - 100f, 200f, 200f), roomLobbyUi.SelectTeamTex);
			GUILayout.EndArea();
			GUI.Window(2, roomLobbyUi.RedPlayerlist.WindowRect, new GUI.WindowFunction(RedPlayerListWindow), string.Empty, roomLobbyUi.RedPlayerlist.Skin.window);
			GUI.Window(3, roomLobbyUi.GreenPlayerlist.WindowRect, new GUI.WindowFunction(GreenPlayerListWindow), string.Empty, roomLobbyUi.GreenPlayerlist.Skin.window);
			GUI.Window(4, roomLobbyUi.PlayerInfo.WindowRect, new GUI.WindowFunction(PlayerInfoWindow), string.Empty, roomLobbyUi.PlayerInfo.Skin.window);
			GUI.Window(5, roomLobbyUi.Chat.WindowRect, new GUI.WindowFunction(ChatWindow), string.Empty, roomLobbyUi.Chat.Skin.window);
		}

	}

	private void OnCharacterSelection() {
		//base.GameStateSwitcher(GameState.RoomLobby, GameState.InGame);
	}

	private void OnInGame() {
		//base.GameStateSwitcher(GameState.CharacterSelection, GameState.Credits);

		//GUI.Window(6, inGameUi.PlayerPlacement[0].WindowRect, new GUI.WindowFunction(Player1Window), string.Empty, inGameUi.PlayerPlacement[0].Skin.window);
		//GUI.Window(7, inGameUi.PlayerPlacement[1].WindowRect, new GUI.WindowFunction(Player2Window), string.Empty, inGameUi.PlayerPlacement[1].Skin.window);
		//GUI.Window(8, inGameUi.PlayerPlacement[2].WindowRect, new GUI.WindowFunction(Player3Window), string.Empty, inGameUi.PlayerPlacement[2].Skin.window);
		//GUI.Window(9, inGameUi.PlayerPlacement[3].WindowRect, new GUI.WindowFunction(Player4Window), string.Empty, inGameUi.PlayerPlacement[3].Skin.window);

		//GUI.Window(10, inGameUi.Scoreboard.WindowRect, new GUI.WindowFunction(ScoreboardWindow), string.Empty, inGameUi.Scoreboard.Skin.window);

		if (networkManager.GameStart) {
			if (!networkManager.GameEnd && !paused && GUI.Button(new Rect(25f, 25f, 50f, 50f), string.Empty, inGameUi.Pause.Skin.button)) {
				networkManager.PlayerHeroEntity.MyCamera.GetComponent<GrayscaleEffect>().enabled = true;
				paused = true;
			}

			if (!networkManager.GameEnd && paused) {
				GUILayout.BeginArea(new Rect((Screen.width * 0.5f) - 75f, (Screen.height * 0.45f) - 75f, 150f, 150f), inGameUi.PauseTex);
				GUILayout.EndArea();

				GUI.Window(11, inGameUi.Pause.WindowRect, new GUI.WindowFunction(PauseWindow), string.Empty, inGameUi.Pause.Skin.window);

				if (Time.timeScale != 0f)
					Time.timeScale = 0f;
			}
			else {
				if (!networkManager.GameEnd || !networkManager.PlayerHeroEntity.HasEnded) {
					if (Time.timeScale != 1f)
						Time.timeScale = 1f;

					GUILayout.BeginArea(new Rect((Screen.width * 0.74f), (Screen.height * 0.055f), 180f, 50f), inGameUi.ScoreBG.box);
					GUILayout.EndArea();
					GUI.Label(new Rect((Screen.width * 0.72f), (-Screen.height * 0.045f), 200f, 200f), "Score: " + networkManager.Score, inGameUi.Score.Skin.label);

					foreach (PlayerInformation element in networkManager.PlayerInformationList) {
						if (element.PlayerID == Network.player) {
							myInfo = element;
							myIndx = networkManager.PlayerInformationList.IndexOf(myInfo) + 1;
							break;
						}
					}

					int windowIndx = 6;
					int otherPlayerCount = 1;
					foreach (PlayerInformation element in networkManager.PlayerInformationList) {
						if (element.PlayerID == Network.player) {
							GUI.Window(windowIndx, inGameUi.PlayerPlacement[0].WindowRect,
								new GUI.WindowFunction(Player1Window), string.Empty, inGameUi.PlayerPlacement[0].Skin.window);
						}
						else {
							if (otherPlayerCount == 1) {
								otherInfo[0] = element;
								otherIndx[0] = networkManager.PlayerInformationList.IndexOf(element) + 1;
								GUI.Window(windowIndx, inGameUi.PlayerPlacement[1].WindowRect,
									new GUI.WindowFunction(Player2Window), string.Empty, inGameUi.PlayerPlacement[1].Skin.window);
							}
							else if (otherPlayerCount == 2) {
								otherInfo[1] = element;
								otherIndx[1] = networkManager.PlayerInformationList.IndexOf(element) + 1;
								GUI.Window(windowIndx, inGameUi.PlayerPlacement[2].WindowRect,
									new GUI.WindowFunction(Player3Window), string.Empty, inGameUi.PlayerPlacement[2].Skin.window);
							}
							else if (otherPlayerCount == 3) {
								otherInfo[2] = element;
								otherIndx[2] = networkManager.PlayerInformationList.IndexOf(element) + 1;
								GUI.Window(windowIndx, inGameUi.PlayerPlacement[3].WindowRect,
									new GUI.WindowFunction(Player4Window), string.Empty, inGameUi.PlayerPlacement[3].Skin.window);
							}

							otherPlayerCount++;
						}

						windowIndx++;
					}
				}

				if (showScoreboard) {
					if (networkManager.GreenTeamWins) {
						inGameUi.Scoreboard.Skin.window.normal.background = inGameUi.GreenBG;
						inGameUi.Scoreboard.Skin.window.onNormal.background = inGameUi.GreenBG;
						inGameUi.Scoreboard.Skin.customStyles[1].normal.background = inGameUi.GreenTeamWins;
					}
					else if (networkManager.RedTeamWins) {
						inGameUi.Scoreboard.Skin.window.normal.background = inGameUi.RedBG;
						inGameUi.Scoreboard.Skin.window.onNormal.background = inGameUi.RedBG;
						inGameUi.Scoreboard.Skin.customStyles[1].normal.background = inGameUi.RedTeamWins;
					}

					GUILayout.BeginArea(new Rect((Screen.width * 0.5f) - (inGameUi.Scoreboard.WindowRect.width * 0.5f), (Screen.height * 0.15f) - 25f, inGameUi.Scoreboard.WindowRect.width, 50f), inGameUi.Scoreboard.Skin.customStyles[1]);
					GUILayout.EndArea();

					GUI.Window(10, inGameUi.Scoreboard.WindowRect, new GUI.WindowFunction(ScoreboardWindow), string.Empty, inGameUi.Scoreboard.Skin.window);
				}
			}
		}
	}

	private void OnCredits() {
		//base.GameStateSwitcher(GameState.InGame, GameState.Settings);
	}

	private void OnSettings() {
		//base.GameStateSwitcher(GameState.Credits, GameState.Intro);

		float min = settingsUi.Brightness.Min;
		float max = settingsUi.Brightness.Max;
		float num = settingsUi.Brightness.CurN;

		Rect position = default(Rect);
		position = settingsUi.Brightness.SliderRect;
		position.x = ((float)Screen.width - settingsUi.Brightness.SliderRect.width) * 0.5f;
		position.y *= (float)Screen.height;

		GUI.Label(new Rect(position.x, position.y - 15f, position.width, position.height), "Brightness [" + (int)num + "]", settingsUi.Brightness.Skin.label);
		num = GUI.HorizontalSlider(position, num, min, max, settingsUi.Brightness.Skin.horizontalSlider, settingsUi.Brightness.Skin.horizontalSliderThumb);
		
		base.ApplyBrightness(num, min, max);
		settingsUi.Brightness.CurN = num;

		// ---

		min = settingsUi.Volume.Min;
		max = settingsUi.Volume.Max;
		num = settingsUi.Volume.CurN;

		position = default(Rect);
		position = settingsUi.Volume.SliderRect;
		position.x = ((float)Screen.width - settingsUi.Volume.SliderRect.width) * 0.5f;
		position.y *= (float)Screen.height;

		GUI.Label(new Rect(position.x, position.y - 15f, position.width, position.height), "Volume [" + (int)num + "]", settingsUi.Volume.Skin.label);

		num = GUI.HorizontalSlider(position, num, min, max, settingsUi.Volume.Skin.horizontalSlider, settingsUi.Volume.Skin.horizontalSliderThumb);
		base.ApplyVolume(num, min, max);

		settingsUi.Volume.CurN = num;

		// ---

		GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.5f, 100f, 100f), "IP: ", settingsUi.IP.Skin.label);
		settingsUi.IpString = GUI.TextField(new Rect(Screen.width * 0.3f, Screen.height * 0.5f, 100f, 100f), settingsUi.IpString, 15, settingsUi.IP.Skin.textField);
		networkManager.IpAddress = settingsUi.IpString;

		// ---

		GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.6f, 150f, 100f), "Port: ", settingsUi.Port.Skin.label);
		settingsUi.PortString = GUI.TextField(new Rect(Screen.width * 0.3f, Screen.height * 0.6f, 100f, 100f), settingsUi.PortString, 10, settingsUi.Port.Skin.textField);
		networkManager.Port = settingsUi.PortString;

		// ---
		//GUILayout.BeginArea(new Rect(Screen.width * 0.2f, Screen.height * 0.7f, 150f, 100f));
		//GUILayout.Label("Full Screen", settingsUi.FullScreen.Skin.label);
		//GUILayout.EndArea();

		//GUILayout.BeginArea(new Rect(Screen.width * 0.35f, Screen.height * 0.7f, 40f, 40f), settingsUi.FullScreen.Skin.box);
		//Screen.fullScreen = settingsUi.IsFullScreen = GUI.Toggle(new Rect(3f, 3f, 50f, 50f), settingsUi.IsFullScreen, string.Empty, settingsUi.FullScreen.Skin.toggle);
		//GUILayout.EndArea();

		//if (!Screen.fullScreen && Screen.currentResolution.height != 1024 && Screen.currentResolution.height != 768)
		//    Screen.SetResolution(1024, 768, settingsUi.IsFullScreen);
	}

	private void NameInputWindow(int id) {
		focusedWindowID = id;

		GUI.BringWindowToFront(id);

		GUILayout.Label("Name", findingServerUi.NameInputWindow.Skin.label);
		GUI.SetNextControlName("Namefield");
		findingServerUi.NameString = GUILayout.TextField(findingServerUi.NameString, 7, findingServerUi.NameInputWindow.Skin.textField).Replace("\n", string.Empty);

		GUI.FocusControl("Namefield");

		if (Event.current.keyCode == KeyCode.Return && findingServerUi.NameString != string.Empty) {
			findingServerUi.EnableTextInput = false;
			findingServerUi.WelcomeNote.InitializeText("Welcome, " + findingServerUi.NameString);
			networkManager.PlayerName = findingServerUi.NameString;
			networkManager.GameName = findingServerUi.NameString;
			findingServerUi.ConnectionStats.gameObject.SetActive(true);
		}
	}

	private void PauseWindow(int id) {
		if (GUI.Button(new Rect((inGameUi.Pause.WindowRect.width * 0.35f) - 35f, (inGameUi.Pause.WindowRect.height * 0.51f) - 35f, 70f, 70f), 
			string.Empty, inGameUi.Pause.Skin.customStyles[0])) {
				Time.timeScale = 1f;

			if (masterServerView)
				MasterServer.UnregisterHost();

			Network.Disconnect();
		}

		if (GUI.Button(new Rect((inGameUi.Pause.WindowRect.width * 0.65f) - 35f, (inGameUi.Pause.WindowRect.height * 0.51f) - 35f, 70f, 70f),
			string.Empty, inGameUi.Pause.Skin.customStyles[1])) {
				networkManager.PlayerHeroEntity.MyCamera.GetComponent<GrayscaleEffect>().enabled = false;
				paused = false;
		}
	}

	private void ServerListWindow(int id) {
		focusedWindowID = id;

		HostData[] hostDataList = MasterServer.PollHostList();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		findingServerUi.ServerListWindow.Skin.button.fixedHeight = 0f;
		findingServerUi.ServerListWindow.Skin.button.alignment = TextAnchor.MiddleCenter;
		if (GUILayout.Button("Refresh", findingServerUi.ServerListWindow.Skin.button, GUILayout.Width(70f), GUILayout.Height(20f)) || Time.realtimeSinceStartup > lastRequest + refreshTimeout) {
			MasterServer.ClearHostList();
			MasterServer.RequestHostList("Sugar Dash Mania v1.0");
			lastRequest = Time.realtimeSinceStartup;
			Debug.Log(hostDataList.Length);
		}
		GUILayout.EndHorizontal();

		if (Time.realtimeSinceStartup > lastRequest + refreshTimeout) {
			MasterServer.RequestHostList(networkManager.GameType);
			lastRequest = Time.realtimeSinceStartup;
		}

		GUILayout.Space(5f);
		findingServerUi.ServerListWindow.Skin.button.fixedHeight = 50f;
		findingServerUi.ServerListWindow.Skin.button.alignment = TextAnchor.MiddleLeft;
		masterServerScrollPos = GUILayout.BeginScrollView(masterServerScrollPos, findingServerUi.ServerListWindow.Skin.scrollView);

		if (hostDataList.Length != 0) {
			foreach (HostData hostData in hostDataList) {
				//if (!hostData.useNat) {
				string text = hostData.gameName + " | " + hostData.connectedPlayers.ToString() + " / " + hostData.playerLimit + " | " + hostData.comment;
				if (GUILayout.Button(text, findingServerUi.ServerListWindow.Skin.button)) {
					Network.Connect(hostData);
				}
				//}
			}
		}
	
		GUILayout.EndScrollView();
	}

	private void RedPlayerListWindow(int id) {
		focusedWindowID = id;

		GUILayout.BeginHorizontal();
		GUILayout.Space(roomLobbyUi.RedPlayerlist.WindowRect.height * 0.1f);
		GUILayout.Label("Name", roomLobbyUi.RedPlayerlist.Skin.label);
		//GUILayout.Label("Status", roomLobbyUi.RedPlayerlist.Skin.label);
		GUILayout.EndHorizontal();

		foreach (PlayerInformation current in networkManager.RedTeamList) {
			string playerName = current.PlayerName;
			GUILayout.BeginHorizontal();
			GUILayout.Space(roomLobbyUi.RedPlayerlist.WindowRect.height * 0.1f);
			GUILayout.Label(playerName, roomLobbyUi.RedPlayerlist.Skin.label);
			GUILayout.EndHorizontal();

			GUILayout.Space(roomLobbyUi.RedPlayerlist.WindowRect.height * 0.04f);
			GUILayout.BeginHorizontal();
			tmpStyle = roomLobbyUi.RedPlayerlist.Skin.box;
			tmpStyle.normal.background = GetCharacterIcon(current.CharType);

			GUILayout.Space(roomLobbyUi.RedPlayerlist.WindowRect.width * 0.55f);
			GUILayout.Box(string.Empty, tmpStyle, GUILayout.Width(40f),	GUILayout.Height(10f));

			//if (current.IsReady) {
			//    roomLobbyUi.RedPlayerlist.Skin.box.normal.background = roomLobbyUi.ReadyIndicator;
			//}
			//else {
			//    roomLobbyUi.RedPlayerlist.Skin.box.normal.background = roomLobbyUi.NotReadyIndicator;
			//}

			//GUILayout.Space(roomLobbyUi.RedPlayerlist.WindowRect.width * 0.1f);
			//GUILayout.Box(string.Empty, roomLobbyUi.RedPlayerlist.Skin.box, GUILayout.Width(40f), GUILayout.Height(10f));

			GUILayout.EndHorizontal();
		}
	}

	private void GreenPlayerListWindow(int id) {
		focusedWindowID = id;

		GUILayout.BeginHorizontal();
		GUILayout.Space(roomLobbyUi.RedPlayerlist.WindowRect.height * 0.1f);
		GUILayout.Label("Name", roomLobbyUi.GreenPlayerlist.Skin.label);
		//GUILayout.Label("Status", roomLobbyUi.GreenPlayerlist.Skin.label);
		GUILayout.EndHorizontal();

		foreach (PlayerInformation current in networkManager.GreenTeamList) {
			string playerName = current.PlayerName;
			GUILayout.BeginHorizontal();
			GUILayout.Space(roomLobbyUi.RedPlayerlist.WindowRect.height * 0.1f);
			GUILayout.Label(playerName, roomLobbyUi.GreenPlayerlist.Skin.label);
			GUILayout.EndHorizontal();

			GUILayout.Space(roomLobbyUi.GreenPlayerlist.WindowRect.height * 0.04f);
			GUILayout.BeginHorizontal();
			tmpStyle = roomLobbyUi.GreenPlayerlist.Skin.box;
			roomLobbyUi.GreenPlayerlist.Skin.box.normal.background = GetCharacterIcon(current.CharType);

			GUILayout.Space(roomLobbyUi.GreenPlayerlist.WindowRect.width * 0.55f);
			GUILayout.Box(string.Empty, tmpStyle, GUILayout.Width(40f), GUILayout.Height(10f));

			//if (current.IsReady) {
			//    roomLobbyUi.GreenPlayerlist.Skin.box.normal.background = roomLobbyUi.ReadyIndicator;
			//}
			//else {
			//    roomLobbyUi.GreenPlayerlist.Skin.box.normal.background = roomLobbyUi.NotReadyIndicator;
			//}

			//GUILayout.Space(roomLobbyUi.GreenPlayerlist.WindowRect.width * 0.1f);
			//GUILayout.Box(string.Empty, roomLobbyUi.GreenPlayerlist.Skin.box, GUILayout.Width(40f), GUILayout.Height(10f));

			GUILayout.EndHorizontal();
		}
	}

	private void PlayerInfoWindow(int id) {
		focusedWindowID = id;

		tmpStyle = roomLobbyUi.PlayerInfo.Skin.window;
		tmpStyle.normal.background = GetCharacterBG(networkManager.CharType);
		tmpStyle.onNormal.background = GetCharacterBG(networkManager.CharType);

		tmpStyle = roomLobbyUi.PlayerInfo.Skin.box;
		tmpStyle.normal.background = GetCharacterIcon(networkManager.CharType);

		GUILayout.BeginArea(new Rect(50f, 60f, 135f, 135f), tmpStyle);
		GUILayout.EndArea();
	
		if (!networkManager.Starting) {

			GUILayout.BeginArea(new Rect(10f, 100f, 135f, 135f));
			if (GUILayout.Button(string.Empty, roomLobbyUi.PlayerInfo.Skin.customStyles[0])) {
				int num = (int)networkManager.CharType;
				if (num > 0) {
					num--;
				}
				else {
					num = 3;
				}

				networkManager.CharType = (CharacterType)num;
				networkManager.networkView.RPC("ChangeCharacterType", RPCMode.AllBuffered, Network.player, (int)networkManager.CharType);
			}
			GUILayout.EndArea();

			GUILayout.BeginArea(new Rect(180f, 100f, 135f, 135f));
			if (GUILayout.Button(string.Empty, roomLobbyUi.PlayerInfo.Skin.customStyles[1])) {
				int num2 = (int)networkManager.CharType;
				if (num2 < 3) {
					num2++;
				}
				else {
					num2 = 0;
				}
				networkManager.CharType = (CharacterType)num2;
				networkManager.networkView.RPC("ChangeCharacterType", RPCMode.AllBuffered, Network.player, (int)networkManager.CharType);
			}
			GUILayout.EndArea();
		}
	}

	private void ChatWindow(int id) {
		focusedWindowID = id;

		GUILayout.Label(string.Empty, roomLobbyUi.Chat.Skin.label);
		chatScrollPos = GUILayout.BeginScrollView(chatScrollPos, roomLobbyUi.Chat.Skin.scrollView);

		float oldFlash = GUI.skin.settings.cursorFlashSpeed;
		Color oldColor = GUI.skin.settings.cursorColor;
		GUI.skin.settings.cursorFlashSpeed = 0f;
		GUI.skin.settings.cursorColor = new Color(0f, 0f, 0f, 0f);
		GUILayout.TextArea(outputString, GUILayout.ExpandHeight(true));
		GUI.skin.settings.cursorFlashSpeed = oldFlash;
		GUI.skin.settings.cursorColor = oldColor;

		GUILayout.EndScrollView();

		GUILayout.Label(string.Empty, roomLobbyUi.Chat.Skin.label);
		inputString = GUILayout.TextField(inputString, 50);

		if (Event.current.Equals(Event.KeyboardEvent("None"))) {
			string currentTime = "[" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second + "] ";

			if (inputString != string.Empty) {
				networkManager.networkView.RPC("SendChatMessage", RPCMode.AllBuffered, currentTime + networkManager.PlayerName + ": " + inputString);
				inputString = string.Empty;
			}
		}

		GUILayout.BeginArea(new Rect((roomLobbyUi.Chat.WindowRect.width * 0.5f) - 65f, (roomLobbyUi.Chat.WindowRect.height * 0.08f) - 65f, 130f, 130f), roomLobbyUi.ChatTex);
		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect((roomLobbyUi.Chat.WindowRect.width * 0.5f) - 65f, (roomLobbyUi.Chat.WindowRect.height * 0.8f) - 65f, 130f, 130f), roomLobbyUi.LogTex);
		GUILayout.EndArea();
	}

	private void Player1Window(int id) {
		Texture2D tmpTex = inGameUi.Item[0].Skin.box.normal.background;
		if (tmpTex != GetItemIcon(myInfo.CurItem)) {
			tmpTex = GetItemIcon(myInfo.CurItem);
			inGameUi.Item[0].Skin.box.normal.background = tmpTex;
		}

		GUILayout.BeginArea(inGameUi.CharIconRect[0], inGameUi.PlayerPlacement[0].Skin.box);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.Item[0].WindowRect, inGameUi.Item[0].Skin.box);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.Place[0].WindowRect, inGameUi.Place[0].Skin.box);
		GUILayout.Label(myIndx.ToString(), inGameUi.Place[0].Skin.label);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.NameHolder[0].WindowRect, inGameUi.NameHolder[0].Skin.box);
		GUILayout.Label(myInfo.PlayerName, inGameUi.NameHolder[0].Skin.label);
		GUILayout.EndArea();
	}

	private void Player2Window(int id) {
		Texture2D tmpTex = inGameUi.Item[1].Skin.box.normal.background;
		if (tmpTex != GetItemIcon(otherInfo[0].CurItem)) {
			tmpTex = GetItemIcon(otherInfo[0].CurItem);
			inGameUi.Item[1].Skin.box.normal.background = tmpTex;
		}

		GUILayout.BeginArea(inGameUi.CharIconRect[1], inGameUi.PlayerPlacement[1].Skin.box);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.Item[1].WindowRect, inGameUi.Item[1].Skin.box);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.Place[1].WindowRect, inGameUi.Place[1].Skin.box);
		GUILayout.Label(otherIndx[0].ToString(), inGameUi.Place[1].Skin.label);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.NameHolder[1].WindowRect, inGameUi.NameHolder[1].Skin.box);
		GUILayout.Label(networkManager.PlayerInformationList[otherIndx[0] - 1].PlayerName, inGameUi.NameHolder[1].Skin.label);
		GUILayout.EndArea();
	}

	private void Player3Window(int id) {
		Texture2D tmpTex = inGameUi.Item[2].Skin.box.normal.background;
		if (tmpTex != GetItemIcon(otherInfo[1].CurItem)) {
			tmpTex = GetItemIcon(otherInfo[1].CurItem);
			inGameUi.Item[2].Skin.box.normal.background = tmpTex;
		}

		GUILayout.BeginArea(inGameUi.CharIconRect[2], inGameUi.PlayerPlacement[2].Skin.box);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.Item[2].WindowRect, inGameUi.Item[2].Skin.box);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.Place[2].WindowRect, inGameUi.Place[2].Skin.box);
		GUILayout.Label(otherIndx[1].ToString(), inGameUi.Place[2].Skin.label);
		GUILayout.EndArea();

		
		GUILayout.BeginArea(inGameUi.NameHolder[2].WindowRect, inGameUi.NameHolder[2].Skin.box);
		GUILayout.Label(networkManager.PlayerInformationList[otherIndx[1] - 1].PlayerName, inGameUi.NameHolder[2].Skin.label);
		GUILayout.EndArea();
	}

	private void Player4Window(int id) {
		Texture2D tmpTex = inGameUi.Item[3].Skin.box.normal.background;
		if (tmpTex != GetItemIcon(otherInfo[2].CurItem)) {
			tmpTex = GetItemIcon(otherInfo[2].CurItem);
			inGameUi.Item[3].Skin.box.normal.background = tmpTex;
		}

		GUILayout.BeginArea(inGameUi.CharIconRect[3], inGameUi.PlayerPlacement[3].Skin.box);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.Item[3].WindowRect, inGameUi.Item[3].Skin.box);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.Place[3].WindowRect, inGameUi.Place[3].Skin.box);
		GUILayout.Label(otherIndx[2].ToString(), inGameUi.Place[3].Skin.label);
		GUILayout.EndArea();

		GUILayout.BeginArea(inGameUi.NameHolder[3].WindowRect, inGameUi.NameHolder[3].Skin.box);
		GUILayout.Label(networkManager.PlayerInformationList[otherIndx[2] - 1].PlayerName, inGameUi.NameHolder[3].Skin.label);
		GUILayout.EndArea();
	}

	private void ScoreboardWindow(int id) {
		GUILayout.BeginArea(new Rect(0f, 10f, inGameUi.Scoreboard.WindowRect.width, 80f), inGameUi.Scoreboard.Skin.box);
		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect(0f, 20f, inGameUi.Scoreboard.WindowRect.width, 50f), inGameUi.Scoreboard.Skin.customStyles[0]);
		GUILayout.EndArea();

		# region Name
		GUILayout.BeginArea(new Rect((400f * 0.25f), 100f, 100f, 300f));
		GUILayout.Label("NAME", inGameUi.Scoreboard.Skin.label);

		int i;
		for (i = 0; i < networkManager.PlayerInformationList.Count; i++) {
			if (networkManager.PlayerInformationList[i].PlayerTeam == Team.Red)
				inGameUi.ScoreboardName.label.normal.textColor = inGameUi.RedNames;
			else if (networkManager.PlayerInformationList[i].PlayerTeam == Team.Green)
				inGameUi.ScoreboardName.label.normal.textColor = inGameUi.GreenNames;

			GUILayout.Space(5f);
			GUILayout.Label(networkManager.PlayerInformationList[i].PlayerName, inGameUi.ScoreboardName.label);
		}

		GUILayout.EndArea();
		# endregion

		# region Score
		GUILayout.BeginArea(new Rect((400f * 0.75f), 100f, 100f, 300f));
		GUILayout.Label("SCORE", inGameUi.Scoreboard.Skin.label);

		for (i = 0; i < networkManager.PlayerInformationList.Count; i++) {
			if (networkManager.PlayerInformationList[i].PlayerTeam == Team.Red)
				inGameUi.ScoreboardName.label.normal.textColor = inGameUi.RedNames;
			else if (networkManager.PlayerInformationList[i].PlayerTeam == Team.Green)
				inGameUi.ScoreboardName.label.normal.textColor = inGameUi.GreenNames;

			GUILayout.Space(5f);
			GUILayout.Label(string.Format("{0:#,###0.#}", networkManager.PlayerInformationList[i].Score), inGameUi.ScoreboardName.label);
		}

		GUILayout.EndArea();
		# endregion

		if (GUI.Button(new Rect((inGameUi.Scoreboard.WindowRect.width * 0.85f) - 35f, (inGameUi.Scoreboard.WindowRect.height * 0.85f) - 35f, 70f, 70f),
			string.Empty, inGameUi.Pause.Skin.customStyles[0])) {
			Time.timeScale = 1f;

			if (masterServerView)
				MasterServer.UnregisterHost();

			Network.Disconnect();
		}
	}

	private Texture2D GetCharacterIcon(CharacterType type) {
		Texture2D result = null;
		if (type == CharacterType.Bubbly)
			result = roomLobbyUi.BubblyIcon;
		else if (type == CharacterType.Coco)
			return roomLobbyUi.CocoIcon;
		else if (type == CharacterType.Pop)
			result = roomLobbyUi.PopIcon;
		else if (type == CharacterType.Rico)
			result = roomLobbyUi.RicoIcon;
		else if (type == CharacterType.None)
			result = null;

		return result;
	}

	private Texture2D GetCharacterBG(CharacterType type) {
		Texture2D result = null;
		if (type == CharacterType.Bubbly)
			result = roomLobbyUi.BubblyBG;
		else if (type == CharacterType.Coco)
			result = roomLobbyUi.CocoBG;
		else if (type == CharacterType.Pop)
			result = roomLobbyUi.PopBG;
		else if (type == CharacterType.Rico)
			result = roomLobbyUi.RicoBG;
		else if (type == CharacterType.None)
			result = roomLobbyUi.SelectCharBG;

		return result;
	}

	private Texture2D GetItemIcon(ItemType type) {
		Texture2D result = null;
		if (type == ItemType.None)
			return result;
		else if (type == ItemType.BlueGumball)
			result = inGameUi.ItemUi[0];
		else if (type == ItemType.BlueJellybean)
			result = inGameUi.ItemUi[1];
		else if (type == ItemType.GreenGumball)
			result = inGameUi.ItemUi[2];
		else if (type == ItemType.RedGumball)
			result = inGameUi.ItemUi[3];
		else if (type == ItemType.RedJellybean)
			result = inGameUi.ItemUi[4];
		else if (type == ItemType.YellowGumball)
			result = inGameUi.ItemUi[5];
		else if (type == ItemType.BlueHardCandyMiC)
			result = inGameUi.ItemUi[6];
		else if (type == ItemType.RedHardCandyMiC)
			result = inGameUi.ItemUi[7];

		return result;
	}

	public void ResetUIComponents() {
		outputString = string.Empty;
		inputString = string.Empty;

		//ResetBaseUIEntity();
	}
}