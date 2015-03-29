using UnityEngine;
using System.Collections;

public enum NetworkGroups {
	Player					= 1,
	PlayerTag				= 2,
	CaramelDrip				= 3,
	Jawbreaker				= 4,
	Indicator				= 5,
	Level1					= 6,
	Item					= 7
}

public class NetworkGroup : MonoBehaviour {
	private static int player = 1;
	public static int Player {
		get { return player; }
	}

	private static int playerTag = 2;
	public static int PlayerTag {
		get { return playerTag; }
	}

	private static int caramelDrip = 3;
	public static int CaramelDrip {
		get { return caramelDrip; }
	}

	private static int jawbreaker = 4;
	public static int Jawbreaker {
		get { return jawbreaker; }
	}

	private static int indicator = 5;
	public static int Indicator {
		get { return indicator; }
	}

	private static int level1 = 6;
	public static int Level1 {
		get { return level1; }
	}

	private static int item = 7;
	public static int Item {
		get { return item; }
	}
}
