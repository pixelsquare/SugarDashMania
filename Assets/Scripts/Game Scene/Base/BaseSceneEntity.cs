using UnityEngine;
using System.Collections;

public class BaseSceneEntity : MonoBehaviour {

	[SerializeField]
	private KeyCode keyboardPositive = KeyCode.RightArrow;
	public KeyCode KeyboardPositive {
		get { return keyboardPositive; }
	}

	[SerializeField]
	private KeyCode keyboardNegative = KeyCode.LeftArrow;
	public KeyCode KeyboardNegative {
		get { return keyboardNegative; }
	}

	[SerializeField]
	private JSInput joystickAxis1;
	public JSInput JoystickAxis1 {
		get { return joystickAxis1; }
	}

	[SerializeField]
	private JSInput joystickAxis2;
	public JSInput JoystickAxis2 {
		get { return joystickAxis2; }
	}

	[SerializeField]
	protected WindowManager[] windowManager;
	public WindowManager[] WindowManagers {
		get { return windowManager; }
	}

	private int prevHorizIndx = 0;
	private int windowIndx = 0;
	public int WindowIndx {
		get { return windowIndx; }
		set {
			if (!windowManager[windowIndx].Enable)
				windowManager[windowIndx].Enable = true;
			else
				windowIndx = value;

			if (windowIndx > windowManager.Length - 1) {
				windowIndx = 0;
				windowManager[prevHorizIndx].Enable = false;
			}
			else if (windowIndx < 0) {
				windowIndx = windowManager.Length - 1;
				windowManager[prevHorizIndx].Enable = false;
			}

			if ((windowIndx - prevHorizIndx) > 0)
				windowManager[windowIndx - 1].Enable = false;
			else if ((windowIndx - prevHorizIndx) < 0)
				windowManager[windowIndx + 1].Enable = false;

			windowManager[windowIndx].Enable = true;
			prevHorizIndx = windowIndx;
		}
	}

	protected BaseUIEntity baseUI;
	protected UserInterface userInterface;
	protected NetworkManager networkManager;

	private void Awake() {
		DisableScene();

		baseUI = FindObjectOfType<BaseUIEntity>();
		userInterface = FindObjectOfType<UserInterface>();
		networkManager = FindObjectOfType<NetworkManager>();
	}

	private void Start() {
		//int i;
		//for (i = 0; i < windowManager.Length; i++)
		//    windowManager[i].Enable = true;

		if (baseUI.Scene != GameState.InGame && windowManager.Length > 0) {
			if (!windowManager[0].Enable)
				windowManager[0].Enable = true;
		}
	}

	protected virtual void OnEnable() {
		if (windowManager.Length > 0) {
			StopCoroutine("SceneControl");
			StartCoroutine("SceneControl");
		}
	}

	private void OnDisable() {
		StopCoroutine("SceneControl");
	}

	protected virtual IEnumerator SceneControl() {
		while (true) {
			//if (!windowManager[WindowIndx].Enable)
			//    windowManager[WindowIndx].Enable = true;

			if (Input.GetKeyDown(KeyboardPositive)) {
				WindowIndx++;
			}

			if (Input.GetKeyDown(KeyboardNegative)) {
				WindowIndx--;
			}

			//else {
			//    if (windowManager[WindowIndx].Enable)
			//        windowManager[WindowIndx].Enable = false;
			//}


			//yield return new WaitForSeconds(0.2f);
			//if (!windowManager[WindowIndx].Enable && (textField != null && !textField.Enable)) {
			//    yield return new WaitForSeconds(0.2f);
			//    windowManager[WindowIndx].Enable = true;
			//}

			yield return null;
		}
	}

	public void EnableScene() {
		if (!gameObject.activeInHierarchy) {
			gameObject.SetActive(true);
		}
	}

	public void DisableScene() {
		if (gameObject.activeSelf) {
			gameObject.SetActive(false);
		}
	}

	protected void ResetSceneEntity() {
		prevHorizIndx = 0;
		windowIndx = 0;

		baseUI = FindObjectOfType<BaseUIEntity>();
		userInterface = FindObjectOfType<UserInterface>();
		networkManager = FindObjectOfType<NetworkManager>();

		foreach (WindowManager element in windowManager) {
			element.ResetWindowManager();
		}
	}
}