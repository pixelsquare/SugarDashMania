using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum JSInput {
	None,
	A, B, X, Y,
	LButton, RButton,
	Back, Start,
	LAnalogue, RAnalogue,
	LAxisX, LAxisY,
	LTrigger, RTrigger,
	RAxisX, RAxisY,
	DpadAxisX, DpadAxisY,
};

public class JoystickManager : MonoBehaviour {
	[SerializeField]
	private string a = "JS_A";
	[SerializeField]
	private string b = "JS_B";
	[SerializeField]
	private string x = "JS_X";
	[SerializeField]
	private string y = "JS_Y";

	[SerializeField]
	private string lButton = "JS_LButton";
	[SerializeField]
	private string rButton = "JS_RButton";

	[SerializeField]
	private string back = "JS_Back";
	[SerializeField]
	private string start = "JS_Start";

	[SerializeField]
	private string lAnalogue = "JS_LAnalogue";
	[SerializeField]
	private string rAnalogue = "JS_RAnalogue";

	[SerializeField]
	private string lAxisX = "JS_LAxis_X";
	[SerializeField]
	private string lAxisY = "JS_LAxis_Y";

	[SerializeField]
	private string lTrigger = "JS_LTrigger";
	[SerializeField]
	private string rTrigger = "JS_RTrigger";

	[SerializeField]
	private string rAxisX = "JS_RAxis_X";
	[SerializeField]
	private string rAxisY = "JS_RAxis_Y";

	[SerializeField]
	private string dpadAxisX = "JS_Dpad_X";
	[SerializeField]
	private string dpadAxisY = "JS_Dpad_Y";

	private List<JSButton> buttonPress = new List<JSButton>();

	private static JoystickManager joystickInput;

	private void Awake() {
		joystickInput = this;
		buttonPress.Add(new JSButton(JSInput.A, a));
		buttonPress.Add(new JSButton(JSInput.B, b));
		buttonPress.Add(new JSButton(JSInput.X, x));
		buttonPress.Add(new JSButton(JSInput.Y, y));
		buttonPress.Add(new JSButton(JSInput.LButton, lButton));
		buttonPress.Add(new JSButton(JSInput.RButton, rButton));
		buttonPress.Add(new JSButton(JSInput.Back, back));
		buttonPress.Add(new JSButton(JSInput.Start, start));
		buttonPress.Add(new JSButton(JSInput.LAnalogue, lAnalogue));
		buttonPress.Add(new JSButton(JSInput.RAnalogue, rAnalogue));
		buttonPress.Add(new JSButton(JSInput.LAxisX, lAxisX));
		buttonPress.Add(new JSButton(JSInput.LAxisY, lAxisY));
		buttonPress.Add(new JSButton(JSInput.LTrigger, lTrigger));
		buttonPress.Add(new JSButton(JSInput.RTrigger, rTrigger));
		buttonPress.Add(new JSButton(JSInput.RAxisX, rAxisX));
		buttonPress.Add(new JSButton(JSInput.RAxisY, rAxisY));
		buttonPress.Add(new JSButton(JSInput.DpadAxisX, dpadAxisX));
		buttonPress.Add(new JSButton(JSInput.DpadAxisY, dpadAxisY));
	}

	private void Start() {
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
			Destroy(gameObject);
	}

	public static bool GetButtonDown(JSInput input) {
		JSButton tmpJSButton = new JSButton();
		tmpJSButton = joystickInput.GetButtonInput(input);

		if (!tmpJSButton.isPressedDown) {
			if (Mathf.Ceil(Input.GetAxis(tmpJSButton.buttonString)) == 1 || Mathf.Floor(Input.GetAxis(tmpJSButton.buttonString)) == -1) {
				return tmpJSButton.isPressedDown = true;
			}
		}

		if (tmpJSButton.isPressedDown && Input.GetAxis(tmpJSButton.buttonString) == 0) {
			tmpJSButton.isPressedDown = false;
		}

		return false;
	}

	public static bool GetButtonUp(JSInput input) {
		JSButton tmpJSButton = new JSButton();
		tmpJSButton = joystickInput.GetButtonInput(input);

		if (!tmpJSButton.isPressedUp) {
			if (Mathf.Ceil(Input.GetAxis(tmpJSButton.buttonString)) == 1 || Mathf.Floor(Input.GetAxis(tmpJSButton.buttonString)) == -1) {
				tmpJSButton.isPressedUp = true;
			}
		}

		if (tmpJSButton.isPressedUp && Input.GetAxis(tmpJSButton.buttonString) == 0) {
			tmpJSButton.isPressedUp = false;
			return true;
		}

		return false;
	}

	public static float GetAxis(JSInput input) {
		JSButton tmpJSButton = new JSButton();
		tmpJSButton = joystickInput.GetButtonInput(input);
		return Input.GetAxis(tmpJSButton.buttonString);
	}

	public static float GetAxisRaw(JSInput input) {
		JSButton tmpJSButton = new JSButton();
		tmpJSButton = joystickInput.GetButtonInput(input);
		return Input.GetAxisRaw(tmpJSButton.buttonString);
	}

	public static int GetAxisRounded(JSInput input) {
		JSButton tmpJSButton = new JSButton();
		tmpJSButton = joystickInput.GetButtonInput(input);
		float result = Input.GetAxis(tmpJSButton.buttonString);

		if (result > 0)
			result = Mathf.Ceil(result);
		else if (result < 0)
			result = Mathf.Floor(result);

		return (int)result;
	}

	public static int GetAxisRawRounded(JSInput input) {
		JSButton tmpJSButton = new JSButton();
		tmpJSButton = joystickInput.GetButtonInput(input);
		float result = Input.GetAxisRaw(tmpJSButton.buttonString);

		if (result > 0)
			result = Mathf.Ceil(result);
		else if (result < 0)
			result = Mathf.Floor(result);

		return (int)result;
	}

	public JSButton GetButtonInput(JSInput input) {
		JSButton tmpJSButton = new JSButton();
		int i;
		for (i = 0; i < joystickInput.buttonPress.Count; i++) {
			if (joystickInput.buttonPress[i].button == input)
				tmpJSButton = joystickInput.buttonPress[i];
		}

		return tmpJSButton;
	}
}

[System.Serializable]
public class JSButton {

	public JSButton() { }

	public JSButton(JSInput input, string inputString) {
		button = input;
		buttonString = inputString;
		isPressedDown = false;
		isPressedUp = false;
	}

	[HideInInspector]
	public string buttonString;

	public JSInput button;
	public bool isPressedDown;
	public bool isPressedUp;
}