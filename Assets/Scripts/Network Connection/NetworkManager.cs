using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharacterType {
	Bubbly, Pop, Rico, Coco, None
}

public enum Team {
	Red, Green, None
}

public class NetworkManager : MonoBehaviour {

	public const int MAX_CLIENT_COUNT = 3;

	[SerializeField]
	private Vector3 spawnPos = new Vector3(-10f, 5f, 0f);

	[SerializeField]
	private Vector3 cameraInitialPos = new Vector3(612f, 8f, -10f);

	[SerializeField]
	private CharacterModels bubbly;

	[SerializeField]
	private GameObject level1;

	[SerializeField]
	private Camera defaultCamera;
	public Camera DefaultCamera {
		get { return defaultCamera; }
	}

	//[SerializeField]
	//private Material rendererMaterial;

	[SerializeField]
	private GameStartTimer gameTimer;

	[SerializeField]
	private LevelScreen levelScreen;

	[SerializeField]
	private GameObject loadingIndicator;

	[SerializeField]
	private int[] rankScore = new int[4];

	private string gameType = "Sugar Dash Mania";
	public string GameType {
		get { return gameType; }
	}

	private string gameName = string.Empty;
	public string GameName {
		get { return gameName; }
		set { gameName = value; }
	}

	private string gameDescription = "JOIN MY GAME!";
	public string GameDescription {
		get { return gameDescription; }
		set { gameDescription = value; }
	}

	private string ipAddress = "127.0.0.1";
	public string IpAddress {
		get { return ipAddress; }
		set { ipAddress = value; }
	}

	private string port = "25003";
	public string Port {
		get { return port; }
		set { port = value; }
	}

	private string playerName = string.Empty;
	public string PlayerName {
		get { return playerName; }
		set { playerName = value; }
	}

	private GameObject playerHero;
	public GameObject PlayerHero {
		get { return playerHero; }
		set { playerHero = value; }
	}

	private BaseCharacterEntity playerHeroEntity;
	public BaseCharacterEntity PlayerHeroEntity {
		get { return playerHeroEntity; }
		set { playerHeroEntity = value; }
	}

	private PlayerInformation heroInformation;
	public PlayerInformation HeroInformation {
		get { return heroInformation; }
		set { heroInformation = value; }
	}

	private bool isReady = false;
	public bool IsReady {
		get { return isReady; }
		set { isReady = value; }
	}

	private CharacterType charType = CharacterType.None;
	public CharacterType CharType {
		get { return charType; }
		set { charType = value; }
	}

	private int score = 0;
	public int Score {
		get { return score; }
		set { score = value; }
	}

	private List<PlayerInformation> playerInformationList = new List<PlayerInformation>();
	public List<PlayerInformation> PlayerInformationList {
		get { return playerInformationList; }
		private set { playerInformationList = value; }

	}

	private List<PlayerInformation> redTeamList = new List<PlayerInformation>();
	public List<PlayerInformation> RedTeamList {
		get { return redTeamList; }
		set { redTeamList = value; }
	}

	private List<PlayerInformation> greenTeamList = new List<PlayerInformation>();
	public List<PlayerInformation> GreenTeamList {
		get { return greenTeamList; }
		set { greenTeamList = value; }
	}

	private float roomLobbyTimer = 3.1f;
	public float RoomLobbyTimer {
		get { return roomLobbyTimer; }
		set { roomLobbyTimer = value; }
	}

	// Room lobby Countdown
	private bool starting = false;
	public bool Starting {
		get { return starting; }
		set { starting = value; }
	}

	// Loading Screen
	private bool hasStarted = false;
	public bool HasStarted {
		get { return hasStarted; }
		set { hasStarted = value; }
	}

	// Start Running
	private bool gameStart = false;
	public bool GameStart {
		get { return gameStart; }
		set { gameStart = value; }
	}

	private bool gameEnd = false;
	public bool GameEnd {
		get { return gameEnd; }
		set { gameEnd = value; }
	}

	private int loadingScreenIndx = 0;
	public int LoadingScreenIndx {
		get { return loadingScreenIndx; }
		set { loadingScreenIndx = value; }
	}

	private bool redTeamWins = false;
	public bool RedTeamWins {
		get { return redTeamWins; }
		set { redTeamWins = value; }
	}

	private bool greenTeamWins = false;
	public bool GreenTeamWins {
		get { return greenTeamWins; }
		set { greenTeamWins = value; }
	}

	private int playersEnded = 0;
	public int PlayersEnded {
		get { return playersEnded; }
		set { playersEnded = value; }
	}

	private int loadedPlayers = 0;
	private BaseUIEntity baseUiEntity;
	private UserInterface userInterface;

	private void Awake() {
		baseUiEntity = FindObjectOfType<BaseUIEntity>();
		userInterface = FindObjectOfType<UserInterface>();

		levelScreen = FindObjectOfType<LevelScreen>();
		defaultCamera = levelScreen.transform.root.GetComponent<Camera>();
		loadingIndicator = levelScreen.LoadingIndicator;

		//foreach (SpriteRenderer element in Resources.FindObjectsOfTypeAll(typeof(SpriteRenderer))) {
		//    element.sharedMaterial = rendererMaterial;
		//}
	}

	private void Start() {
		DontDestroyOnLoad(this);
		DontDestroyOnLoad(defaultCamera.gameObject);
		levelScreen.LevelScreenControl(false);

		GameUtility.ChangeSortingLayerRecursively(loadingIndicator.transform, LayerManager.SortingLayerUiFront);
		loadingIndicator.SetActive(false);

		
	}

	private void OnApplicationExit() {
		Network.Disconnect();
		MasterServer.UnregisterHost();
	}

	private void OnLevelWasLoaded(int level) {
		if (level == 1) {
			/**
			 *	Update all that I'm loaded
			 **/
			GetComponent<NetworkView>().RPC("SetHasLoaded", RPCMode.All, Network.player, true);

			/**
			 *	Server Spawns the level and the characters
			 **/
			GetComponent<NetworkView>().RPC("InitializeLevel", RPCMode.All, Network.player);

			/**
			 *	Set local variables to ready to start
			 **/

			foreach (Camera element in FindObjectsOfType<Camera>()) {
				if (element != defaultCamera && element.tag != "Player")
					Destroy(element.gameObject);
			}

			///**
			// *	Enable Fade In Effect
			// *	the effect happens in UpdateLevelScreen RPC
			// **/
			//levelScreen.LevelScreenControl(true);
		}
	}

	private void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		Debug.Log(info);
	}

	private void OnFailedToConnect(NetworkConnectionError info) {
		Debug.Log(info);
	}

	private void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (userInterface.Scene == GameState.InGame) {
			Destroy(gameObject);
			Destroy(defaultCamera.gameObject);
			Destroy(baseUiEntity.gameObject);
		}
		
		ResetNetworkComponents();

		if (Application.loadedLevel == 1) {
			Application.LoadLevel("Main Menu");
			//level1.SetActive(false);
		}

		baseUiEntity.Scene = GameState.FindingServer;
	}

	private void OnPlayerDisconnected(NetworkPlayer player) {
		GetComponent<NetworkView>().RPC("RemovePlayerToList", RPCMode.AllBuffered, player);
	}

	private void OnServerInitialized() {
		PlayerInformation serverInfo = new PlayerInformation();
		serverInfo.PlayerName = baseUiEntity.FindingServerUi.NameString;
		serverInfo.PlayerID = Network.player;
		RedTeamList.Add(serverInfo);
		PlayerInformationList.Add(serverInfo);

		baseUiEntity.Scene = GameState.RoomLobby;

		GetComponent<NetworkView>().RPC("SendChatMessage", RPCMode.AllBuffered, serverInfo.PlayerName + " has connected.");
	}

	private void OnConnectedToServer() {
		PlayerInformation clientInfo = new PlayerInformation();
		clientInfo.PlayerName = baseUiEntity.FindingServerUi.NameString;
		clientInfo.PlayerID = Network.player;
		GetComponent<NetworkView>().RPC("AddPlayerToList", RPCMode.All, clientInfo.PlayerID, clientInfo.PlayerName);

		baseUiEntity.Scene = GameState.RoomLobby;
		GetComponent<NetworkView>().RPC("SendChatMessage", RPCMode.AllBuffered, clientInfo.PlayerName + " has connected.");
	}

	public void ResetNetworkComponents() {
		gameType = "Sugar Dash Mania";
		gameName = string.Empty;
		gameDescription = "JOIN MY GAME!";
		ipAddress = "127.0.0.1";
		port = "25003";
		playerName = string.Empty;
		playerHero = null;
		playerHeroEntity = null;
		heroInformation = null;
		isReady = false;
		charType = CharacterType.None;
		score = 0;
		playerInformationList = new List<PlayerInformation>();
		redTeamList = new List<PlayerInformation>();
		greenTeamList = new List<PlayerInformation>();
		roomLobbyTimer = 3.1f;
		starting = false;
		hasStarted = false;
		gameStart = false;
		gameEnd = false;
		loadingScreenIndx = 0;
		redTeamWins = false;
		greenTeamWins = false;
		loadedPlayers = 0;
		playersEnded = 0;
		Time.timeScale = 1f;

		userInterface.ResetUIComponents();
	}

	public void MoveCameraToPlayer() {
		defaultCamera.GetComponent<CameraFollow>().Target = playerHero.transform;
		StartCoroutine("ReleaseMainCamera");
	}

	private IEnumerator ReleaseMainCamera() {
		yield return new WaitForSeconds(0.2f);
		while (defaultCamera.velocity.magnitude > 0.2f) {
			yield return null;
		}

		//if (Network.isServer) {
		//    //cameraPanning = true;
		//    //networkView.RPC("SetCameraPan", RPCMode.All, cameraPanning);
		//    //gameStart = true;
		//    //networkView.RPC("SetGameStart", RPCMode.All, gameStart);
		//}

		//networkView.RPC("SetGameStart", RPCMode.All, gameStart);
		//defaultCamera.GetComponent<CameraFollow>().enabled = false;
		defaultCamera.enabled = false;
		defaultCamera.orthographicSize = 5f;
		playerHeroEntity.MyCamera.enabled = true;
		gameTimer.transform.parent = playerHeroEntity.MyCamera.transform;

		if (Network.isServer) {
			GetComponent<NetworkView>().RPC("ShowTimer", RPCMode.All);
		}

		StopCoroutine("ReleaseMainCamera");
	}

	//private IEnumerator RespawnPlayer() {
	//    yield return new WaitForSeconds(heroInformation.RespawnTime);
	//    networkView.RPC("SetPlayerDead", RPCMode.All, playerHeroEntity.Owner, false);
	//    playerHero.transform.position = heroInformation.RespawnPoint;
	//}

	/**
	 *	After counting in the room lobby call StartGame RPC
	 *	to load the scene in server first before clients
	 **/

	public IEnumerator StartRoomLobbyTimer() {
		if (!starting) {
			starting = true;
			GetComponent<NetworkView>().RPC("SetStarting", RPCMode.All, starting);
		}
		InvokeRepeating("PrintRoomLobbyTimer", 0.1f, 1.01f);
		roomLobbyTimer++;

		while (roomLobbyTimer > 1f) {
			roomLobbyTimer -= Time.deltaTime;
			yield return null;
		}

		CancelInvoke("PrintRoomLobbyTimer");
		GetComponent<NetworkView>().RPC("StartGame", RPCMode.All);
	}

	public void ResetRoomLobbyTimer() {
		StopAllCoroutines();
		CancelInvoke("PrintRoomLobbyTimer");
		roomLobbyTimer = 5.1f;
	}

	/**
	 *	Invoke this function to call RPC to print the room lobby time
	 **/

	private void PrintRoomLobbyTimer() {
		GetComponent<NetworkView>().RPC("SendChatMessage", RPCMode.AllBuffered, "Game Starting in " + (int)roomLobbyTimer + " ...");
		GetComponent<NetworkView>().RPC("UpdateRoomLobbyTimer", RPCMode.Others, roomLobbyTimer);
	}

	/**
	 *	Called on the server to continously to check when players are loaded
	 **/

	private IEnumerator CheckLoadedPlayers() {
		while (loadedPlayers < Network.connections.Length + 1) {

			foreach (PlayerInformation element in PlayerInformationList) {
				if (element.HasLoaded)
					loadedPlayers++;
			}

			yield return null;
		}

		yield return new WaitForSeconds(3f);
		GetComponent<NetworkView>().RPC("UpdateLevelScreen", RPCMode.All);
		StopCoroutine("CheckLoadedPlayers");
	}

	/**
	 *	Network components
	 **/

	[RPC]
	private void AddPlayerToList(NetworkPlayer id, string name) {
		if (Network.isServer) {
			PlayerInformation tmpInfo = new PlayerInformation();
			tmpInfo.PlayerName = name;
			tmpInfo.PlayerID = id;

			int redCount = 0; int greenCount = 0;
			foreach (PlayerInformation element in PlayerInformationList) {
				if (element.PlayerTeam == Team.Red)
					redCount++;

				if (element.PlayerTeam == Team.Green)
					greenCount++;
			}

			if (redCount < 2) {
				tmpInfo.PlayerTeam = Team.Red;
				RedTeamList.Add(tmpInfo);
			}

			else if (greenCount < 2) {
				tmpInfo.PlayerTeam = Team.Green;
				GreenTeamList.Add(tmpInfo);
			}

			PlayerInformationList.Add(tmpInfo);

			foreach (PlayerInformation element in PlayerInformationList)
				GetComponent<NetworkView>().RPC("UpdateClientPlayerList", RPCMode.Others, element.PlayerID, element.PlayerName, (int)element.CharType);
		}
	}

	[RPC]
	private void UpdateClientPlayerList(NetworkPlayer id, string name, int characterType) {
		PlayerInformation tmpInfo = new PlayerInformation();
		tmpInfo.PlayerName = name;
		tmpInfo.PlayerID = id;
		tmpInfo.CharType = (CharacterType)characterType;

		int redCount = 0; int greenCount = 0;
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == id && element.PlayerName == name)
				return;

			if (element.PlayerTeam == Team.Red)
				redCount++;

			if (element.PlayerTeam == Team.Green)
				greenCount++;
		}

		if (redCount < 2) {
			tmpInfo.PlayerTeam = Team.Red;
			RedTeamList.Add(tmpInfo);
		}

		else if (greenCount < 2) {
			tmpInfo.PlayerTeam = Team.Green;
			GreenTeamList.Add(tmpInfo);
		}

		PlayerInformationList.Add(tmpInfo);
	}

	[RPC]
	private void RemovePlayerToList(NetworkPlayer player) {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == player) {
				Network.DestroyPlayerObjects(element.PlayerID);
				Network.RemoveRPCs(element.Hero.GetComponent<NetworkView>().viewID);
				Network.RemoveRPCsInGroup(NetworkGroup.Player);
				Network.Destroy(element.Hero);
				PlayerInformationList.Remove(element);

				if (element.PlayerTeam == Team.Red)
					RedTeamList.Remove(element);
				else if (element.PlayerTeam == Team.Green)
					GreenTeamList.Remove(element);

				if (Network.isServer)
					GetComponent<NetworkView>().RPC("SendChatMessage", RPCMode.AllBuffered, element.PlayerName + " has left the game.");

				break;
			}
		}
	}

	[RPC]
	private void StartGame() {
		if (Network.isServer) {
			loadingIndicator.SetActive(true);
			Application.LoadLevel("Level 1");
			baseUiEntity.Scene = GameState.InGame;
			hasStarted = true;
			GetComponent<NetworkView>().RPC("SetHasStarted", RPCMode.All, hasStarted);
			defaultCamera.backgroundColor = Color.black;
			defaultCamera.transform.position = cameraInitialPos;
			defaultCamera.orthographicSize = 12f;

			loadingScreenIndx = Random.Range(0, levelScreen.LoadingScreens.Length);
			GetComponent<NetworkView>().RPC("SetLoadingScreenIndx", RPCMode.All, loadingScreenIndx);

			/**
			 *	Enable Fade In Effect
			 *	the effect happens in UpdateLevelScreen RPC
			 **/
			levelScreen.LevelScreenControl(true);
			//levelScreen.EnableLoadingScreen(loadingScreenIndx);

			/**
			 *	Check on the server side all connected players
			 *	using an Ienumerator
			 **/
			StartCoroutine("CheckLoadedPlayers");
			GetComponent<NetworkView>().RPC("UpdateClientStartGame", RPCMode.Others);
		}
	}

	[RPC]
	private void UpdateClientStartGame() {
		loadingIndicator.SetActive(true);
		Application.LoadLevel("Level 1");
		baseUiEntity.Scene = GameState.InGame;
		//hasStarted = false;
		defaultCamera.backgroundColor = Color.black;
		defaultCamera.transform.position = cameraInitialPos;
		defaultCamera.orthographicSize = 12f;

		/**
		 *	Enable Fade In Effect
		 *	the effect happens in UpdateLevelScreen RPC
		 **/
		levelScreen.LevelScreenControl(true);
	}

	[RPC]
	private void InitializeLevel(NetworkPlayer player) {
		if (Network.isServer) {

			if (Network.player == player)
				Network.Instantiate(level1, Vector3.zero, Quaternion.identity, NetworkGroup.Level1);

			int characterIndx = 0;
			foreach (PlayerInformation element in PlayerInformationList) {
				if (element.PlayerID == player && element.Hero == null) {
					element.Hero = Network.Instantiate(bubbly.Model[characterIndx], spawnPos, Quaternion.identity, NetworkGroup.Player) as GameObject;
					element.Hero.name = "Character " + element.CharType.ToString() + " (" + element.PlayerName + ")";

					element.HeroEntity = GameUtility.SearchBaseCharacterEntity(element.Hero.transform);
					element.HeroEntity.Nameplate.text = element.PlayerName;
					element.HeroEntity.name = element.CharType.ToString() + " (" + element.PlayerName + ")";

					element.HeroEntity.GetComponent<NetworkView>().RPC("Initialize", RPCMode.All, element.PlayerID, (int)element.PlayerTeam);

					if (element.PlayerID == Network.player) {
						playerHero = element.HeroEntity.gameObject;
						playerHeroEntity = element.HeroEntity;
						heroInformation = element;
					}
				}

				characterIndx++;
			}

			if (playerHero != null) {
				GameUtility.ChangeLayerRecursively(playerHero.transform.root.transform, LayerManager.LayerPlayer);
			}

			foreach (ParallaxBG element in FindObjectsOfType<ParallaxBG>()) {
				if (element.gameObject.layer == LayerManager.LayerParallaxBG) {
					element.Cam = playerHeroEntity.MyCamera;

					if (element.PlayerInputBase) {
						element.PlayerRB = playerHeroEntity.GetComponent<Rigidbody2D>();
					}
				}
			}

			GetComponent<NetworkView>().RPC("UpdateSpawnedPlayers", RPCMode.Others);
		}
	}

	[RPC]
	private void UpdateSpawnedPlayers() {
		BaseCharacterEntity[] players = FindObjectsOfType<BaseCharacterEntity>();

		int i;
		for (i = 0; i < PlayerInformationList.Count; i++) {
			int j;
			for (j = 0; j < players.Length; j++) {

				if (PlayerInformationList[i].PlayerID == players[j].Owner) {
					PlayerInformationList[i].Hero = players[j].transform.root.gameObject;
					PlayerInformationList[i].Hero.name = "Characater " + PlayerInformationList[i].CharType.ToString() + " (" + PlayerInformationList[i].PlayerName + ")";

					PlayerInformationList[i].HeroEntity = GameUtility.SearchBaseCharacterEntity(PlayerInformationList[i].Hero.transform);
					PlayerInformationList[i].HeroEntity.Nameplate.text = PlayerInformationList[i].PlayerName;
					PlayerInformationList[i].HeroEntity.name = PlayerInformationList[i].CharType.ToString() + " (" + PlayerInformationList[i].PlayerName + ")";

					if (PlayerInformationList[i].PlayerID == Network.player) {
						playerHero = PlayerInformationList[i].HeroEntity.gameObject;
						playerHeroEntity = playerInformationList[i].HeroEntity;
						heroInformation = PlayerInformationList[i];
					}
				}
			}
		}

		if (playerHero != null) {
			GameUtility.ChangeLayerRecursively(playerHero.transform.root.transform, LayerManager.LayerPlayer);
		}

		foreach (ParallaxBG element in FindObjectsOfType<ParallaxBG>()) {
			if (element.gameObject.layer == LayerManager.LayerParallaxBG) {
				element.Cam = playerHeroEntity.MyCamera;

				if (element.PlayerInputBase) {
					element.PlayerRB = playerHeroEntity.GetComponent<Rigidbody2D>();
				}
			}
		}

		GetComponent<NetworkView>().RPC("ChangePlayerLayer", RPCMode.All);
	}

	[RPC]
	private void ChangePlayerLayer() {
		if (Network.isServer) {
			int enemyTrackerCount = 0;
			foreach (PlayerInformation element in PlayerInformationList) {
				if ((int)playerHeroEntity.MyTeam == (int)element.PlayerTeam && element != heroInformation) {
					GameUtility.ChangeLayerRecursively(element.Hero.transform, LayerManager.LayerAlly);
					PlayerHeroEntity.AllyTracker.Target = element.HeroEntity.transform;
					PlayerHeroEntity.AllyTracker.SetIcon(element.PlayerName, (int)element.CharType);
				}
				else if ((int)playerHeroEntity.MyTeam != (int)element.PlayerTeam) {
					GameUtility.ChangeLayerRecursively(element.HeroEntity.transform, LayerManager.LayerEnemy);

					if (enemyTrackerCount == 0) {
						PlayerHeroEntity.Enemy1Tracker.Target = element.HeroEntity.transform;
						PlayerHeroEntity.Enemy1Tracker.SetIcon(element.PlayerName, (int)element.CharType);
						enemyTrackerCount++;
					}
					else if (enemyTrackerCount == 1) {
						PlayerHeroEntity.Enemy2Tracker.Target = element.HeroEntity.transform;
						PlayerHeroEntity.Enemy2Tracker.SetIcon(element.PlayerName, (int)element.CharType);
						enemyTrackerCount++;
					}
				}
			}

			GameUtility.ChangeLayerRecursively(playerHero.transform.root.transform, LayerManager.LayerPlayer);

			GetComponent<NetworkView>().RPC("UpdateChangePlayerLayer", RPCMode.Others);
		}
	}

	[RPC]
	private void UpdateChangePlayerLayer() {
		int enemyTrackerCount = 0;
		foreach (PlayerInformation element in PlayerInformationList) {
			if ((int)playerHeroEntity.MyTeam == (int)element.PlayerTeam && element != heroInformation) {
				GameUtility.ChangeLayerRecursively(element.Hero.transform, LayerManager.LayerAlly);
				PlayerHeroEntity.AllyTracker.Target = element.HeroEntity.transform;
				PlayerHeroEntity.AllyTracker.SetIcon(element.PlayerName, (int)element.CharType);
			}
			else if ((int)playerHeroEntity.MyTeam != (int)element.PlayerTeam) {
				GameUtility.ChangeLayerRecursively(element.Hero.transform, LayerManager.LayerEnemy);

				if (enemyTrackerCount == 0) {
					PlayerHeroEntity.Enemy1Tracker.Target = element.HeroEntity.transform;
					PlayerHeroEntity.Enemy1Tracker.SetIcon(element.PlayerName, (int)element.CharType);
					enemyTrackerCount++;
				}
				else if (enemyTrackerCount == 1) {
					PlayerHeroEntity.Enemy2Tracker.Target = element.HeroEntity.transform;
					PlayerHeroEntity.Enemy2Tracker.SetIcon(element.PlayerName, (int)element.CharType);
					enemyTrackerCount++;
				}
			}
		}

		GameUtility.ChangeLayerRecursively(playerHero.transform.root.transform, LayerManager.LayerPlayer);
	}

	/**
	 *	Player Information set parameters
	 **/

	[RPC]
	private void SetStarting(bool start) {
		starting = start;
	}

	[RPC]
	private void SetHasStarted(bool start) {
		HasStarted = start;
	}

	[RPC]
	private void SetGameStart(bool start) {
		gameStart = start;
	}

	[RPC]
	private void SetGameEnd(bool end) {
		gameEnd = end;
	}

	[RPC]
	private void SetWinnerTeam(bool greenTeam, bool redTeam) {
		greenTeamWins = greenTeam;
		redTeamWins = redTeam;
		userInterface.ShowScoreboard = true;
		DefaultCamera.GetComponent<CameraFollow>().enabled = false;
		DefaultCamera.GetComponent<CameraMove>().enabled = false;
	}

	[RPC]
	private void SetPlayersEnded(int end) {
		playersEnded = end;
	}

	[RPC]
	private void ShowTimer() {
		gameTimer.EnableTimer(true);
	}

	[RPC]
	private void SetLoadingScreenIndx(int indx) {
		loadingScreenIndx = indx;
	}

	//[RPC]
	//private void SetPlayerDead(NetworkPlayer id, bool dead) {
	//    foreach (PlayerInformation element in PlayerInformationList) {
	//        if (element.PlayerID == id) {
	//            element.Dead = dead;

	//            if (Network.player == id)
	//                StartCoroutine("RespawnPlayer");
	//            break;
	//        }
	//    }
	//}

	//[RPC]
	//private void SetPlayerRespawn(NetworkPlayer id, Vector3 point) {
	//    foreach (PlayerInformation element in PlayerInformationList) {
	//        if (element.PlayerID == id) {
	//            element.RespawnPoint = point;
	//            break;
	//        }
	//    }
	//}

	[RPC]
	private void SetReady(NetworkPlayer id, bool ready) {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == id) {
				element.IsReady = ready;
				break;
			}
		}
	}

	[RPC]
	private void SetCurrentItem(NetworkPlayer id, int item) {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == id) {
				element.CurItem = (ItemType)item;
				break;
			}
		}
	}

	[RPC]
	private void SetHasLoaded(NetworkPlayer id, bool loaded) {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == id) {
				element.HasLoaded = loaded;
				break;
			}
		}
	}

	[RPC]
	private void ChangeCharacterType(NetworkPlayer id, int characterType) {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == id) {
				element.CharType = (CharacterType)characterType;
				break;
			}
		}
	}

	[RPC]
	private void ChangeTeam(NetworkPlayer id, int team) {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == id && element.PlayerTeam != (Team)team) {
				if (element.PlayerTeam == Team.Red && GreenTeamList.Count < 2) {
					RedTeamList.Remove(element);
					GreenTeamList.Add(element);
					element.PlayerTeam = (Team)team;
				}
				else if (element.PlayerTeam == Team.Green && RedTeamList.Count < 2) {
					GreenTeamList.Remove(element);
					RedTeamList.Add(element);
					element.PlayerTeam = (Team)team;
				}

				break;
			}
		}
	}

	[RPC]
	private void SendChatMessage(string message) {
		userInterface.OutputString += message + "\n";

		Vector2 chatScrollPos = userInterface.ChatScrollPos;
		chatScrollPos.y = Mathf.Infinity;
		userInterface.ChatScrollPos = chatScrollPos;
	}

	[RPC]
	private void UpdateRoomLobbyTimer(float time) {
		roomLobbyTimer = time;
	}

	[RPC]
	private void UpdateLevelScreen() {
		levelScreen.StartGame();
	}

	[RPC]
	private void SetAllPlayersReady() {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (!element.IsReady)
				element.IsReady = true;

			if (element.PlayerID == Network.player)
				isReady = true;
		}
	}

	[RPC]
	private void AddScore(NetworkPlayer id, int score) {
		if (Network.isServer) {
			int playerScore = 0;
			foreach (PlayerInformation element in PlayerInformationList) {
				if (element.PlayerID == id) {
					element.Score += score;
					playerScore = element.Score;

					if (Network.player == id)
						Score = element.Score;

					break;
				}
			}

			PlayerInformationList.Sort(SortPlayerListByScore);
			GetComponent<NetworkView>().RPC("UpdateScore", RPCMode.Others, id, playerScore);
		}
	}

	[RPC]
	private void UpdateScore(NetworkPlayer id, int score) {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == id) {
				element.Score = score;

				if (Network.player == id)
					Score = element.Score;

				break;
			}
		}

		PlayerInformationList.Sort(SortPlayerListByScore);
	}

	[RPC]
	private void EndPlayer(NetworkPlayer id) {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == id) {
				element.HasEnded = true;

				GetComponent<NetworkView>().RPC("RemovePlayerEnded", RPCMode.All, id);

				break;
			}
		}

		if (Network.isServer && GetComponent<NetworkView>().isMine)
			GetComponent<NetworkView>().RPC("RankBonus", RPCMode.All, id);
	}

	[RPC]
	private void RankBonus(NetworkPlayer id) {
		if (Network.isServer) {

			// Check if the player is in the list
			foreach (PlayerInformation element in playerInformationList) {
				if (element.PlayerID == id && element.HeroEntity.Owner == id) {
					playersEnded++;
					break;
				}
			}
			
			if (playersEnded == 1)
				GetComponent<NetworkView>().RPC("AddScore", RPCMode.All, id, rankScore[0]);
			else if (playersEnded == 2)
				GetComponent<NetworkView>().RPC("AddScore", RPCMode.All, id, rankScore[1]);
			else if (playersEnded == 3)
				GetComponent<NetworkView>().RPC("AddScore", RPCMode.All, id, rankScore[2]);
			else if (playersEnded == 4)
				GetComponent<NetworkView>().RPC("AddScore", RPCMode.All, id, rankScore[3]);

			//networkView.RPC("SetPlayersEnded", RPCMode.Others, playersEnded);
		}
	}

	[RPC]
	private void RemovePlayerEnded(NetworkPlayer id) {
		if (playerHeroEntity.Owner == id) {
			DefaultCamera.enabled = true;
			DefaultCamera.GetComponent<CameraMove>().enabled = true;
			DefaultCamera.GetComponent<CameraFollow>().enabled = false;
			DefaultCamera.orthographicSize = 12f;
			DefaultCamera.cullingMask = ~(1 <<LayerManager.LayerParallaxBG);
			PlayerHeroEntity.MyCamera.enabled = false;

			foreach (ParallaxBG element in FindObjectsOfType<ParallaxBG>()) {
				if (element.gameObject.layer == LayerManager.LayerStaticBG)
					element.Cam = DefaultCamera;

				if (element.gameObject.layer == LayerManager.LayerParallaxBG)
					element.Cam = null;
			}
		}

		if (Network.isServer) {
			foreach (PlayerInformation element in PlayerInformationList) {
				if (element.PlayerID == id) {
					element.Hero.SetActive(false);
					element.HeroEntity.transform.position = Vector3.zero;
					Destroy(element.HeroEntity.GetComponent<Rigidbody2D>());
					//Network.RemoveRPCs(element.Hero.networkView.viewID);
					//Network.RemoveRPCsInGroup(NetworkGroup.Player);
					//Network.DestroyPlayerObjects(element.PlayerID);
					//Network.Destroy(element.Hero);
					//Network.DestroyPlayerObjects(element.PlayerID);
					//Network.RemoveRPCsInGroup(NetworkGroup.Player);
					break;
				}
			}

			GetComponent<NetworkView>().RPC("UpdatePlayerRemove", RPCMode.Others, id);
			GetComponent<NetworkView>().RPC("UpdateEndedPlayers", RPCMode.All);
		}
	}

	[RPC]
	private void UpdatePlayerRemove(NetworkPlayer id) {
		foreach (PlayerInformation element in PlayerInformationList) {
			if (element.PlayerID == id) {
				element.Hero.SetActive(false);
				element.HeroEntity.transform.position = Vector3.zero;
				Destroy(element.HeroEntity.GetComponent<Rigidbody2D>());
				//Network.RemoveRPCs(element.Hero.networkView.viewID);
				//Network.RemoveRPCsInGroup(NetworkGroup.Player);
				//Network.DestroyPlayerObjects(element.PlayerID);
				//Network.Destroy(element.Hero);
				//Network.DestroyPlayerObjects(element.PlayerID);
				//Network.RemoveRPCsInGroup(NetworkGroup.Player);
				break;
			}
		}
	}

	[RPC]
	private void UpdateEndedPlayers() {
		int redTeamEndedCount = 0;
		int greenTeamEndedCount = 0;

		int redTeamScore = 0;
		int greenTeamScore = 0;

		foreach (PlayerInformation element in redTeamList) {
			if (element.HasEnded) {
				redTeamEndedCount++;
				redTeamScore += element.Score;
			}
		}

		foreach (PlayerInformation element in greenTeamList) {
			if (element.HasEnded) {
				greenTeamEndedCount++;
				greenTeamScore += element.Score;
			}
		}

		if (redTeamEndedCount == redTeamList.Count && redTeamList.Count != 0) {
			foreach (PlayerInformation element in greenTeamList) {
				if (element.Hero != null && element.HeroEntity.Owner == Network.player) {
					PlayerHeroEntity.EndTimer.EnableTimer();
					break;
				}
			}
		}

		if (greenTeamEndedCount == greenTeamList.Count && greenTeamList.Count != 0) {
			foreach (PlayerInformation element in redTeamList) {
				if (element.Hero != null && element.PlayerID == Network.player) {
					PlayerHeroEntity.EndTimer.EnableTimer();
					break;
				}
			}
		}

		int totalPlayers = greenTeamEndedCount + redTeamEndedCount;
		if (totalPlayers == (Network.connections.Length + 1)) {
			if (redTeamScore > greenTeamScore)
				redTeamWins = true;
			else if (greenTeamScore > redTeamScore)
				greenTeamWins = true;

			GetComponent<NetworkView>().RPC("SetWinnerTeam", RPCMode.All, greenTeamWins, redTeamWins);
			GetComponent<NetworkView>().RPC("SetGameEnd", RPCMode.All, true);
		}
	}

	private int SortPlayerListByScore(PlayerInformation a, PlayerInformation b) {
		if (a.Score < b.Score) return 1;
		if (a.Score > b.Score) return -1;
		return 0;
	}
}

[System.Serializable]
public class PlayerInformation {
	private string playerName = string.Empty;
	public string PlayerName {
		get { return playerName; }
		set { playerName = value; }
	}

	private CharacterType charType = CharacterType.None;
	public CharacterType CharType {
		get { return charType; }
		set { charType = value; }
	}

	private ItemType curItem = ItemType.None;
	public ItemType CurItem {
		get { return curItem; }
		set { curItem = value; }
	}

	private Team playerTeam;
	public Team PlayerTeam {
		get { return playerTeam; }
		set { playerTeam = value; }
	}

	private bool isReady = false;
	public bool IsReady {
		get { return isReady; }
		set { isReady = value; }
	}

	private bool hasLoaded = false;
	public bool HasLoaded {
		get { return hasLoaded; }
		set { hasLoaded = value; }
	}

	private NetworkPlayer playerID;
	public NetworkPlayer PlayerID {
		get { return playerID; }
		set { playerID = value; }
	}

	private GameObject hero;
	public GameObject Hero {
		get { return hero; }
		set { hero = value; }
	}

	private BaseCharacterEntity heroEntity = null;
	public BaseCharacterEntity HeroEntity {
		get { return heroEntity; }
		set { heroEntity = value; }

	}

	private int score = 0;
	public int Score {
		get { return score; }
		set { score = value; }
	}

	private bool hasEnded = false;
	public bool HasEnded {
		get { return hasEnded; }
		set { hasEnded = value; }
	}

	//private bool dead = false;
	//public bool Dead {
	//    get { return dead; }
	//    set { dead = value; }
	//}

	//private Vector3 respawnPoint;
	//public Vector3 RespawnPoint {
	//    get { return respawnPoint; }
	//    set { respawnPoint = value; }
	//}

	//private float respawnTime = 1f;
	//public float RespawnTime {
	//    get { return respawnTime; }
	//    set { respawnTime = value; }
	//}
}

[System.Serializable]
public class CharacterModels {
	[SerializeField]
	private GameObject[] model;
	public GameObject[] Model {
		get { return model; }
	}
}