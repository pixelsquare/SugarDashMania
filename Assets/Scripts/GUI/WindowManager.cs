using UnityEngine;
using System.Collections;

public class WindowManager : MonoBehaviour {

	[SerializeField]
	private KeyCode keyboardPositive = KeyCode.UpArrow;
	public KeyCode KeyboardPositive {
		get { return keyboardPositive; }
	}

	[SerializeField]
	private KeyCode keyboardNegative = KeyCode.DownArrow;
	public KeyCode KeyboardNegative {
		get { return keyboardNegative; }
	}

	[SerializeField]
	private KeyCode keyboardEnter = KeyCode.Space;
	public KeyCode KeyboardEnter {
		get { return keyboardEnter; }
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
	private BaseButtonEntity[] buttons;
	public BaseButtonEntity[] Buttons {
		get { return buttons; }
	}

	private Animator anim;
	private float nextHover = 0f;
	private int hoveredIndx = -1;

	private int prevHorizIndx = 0;
	private int buttonIndx = 0;
	public int ButtonIndx {
		get { return Mathf.Clamp(buttonIndx, 0, Buttons.Length - 1); }
		set {
			if (!Buttons[buttonIndx].ButtonHover)
				Buttons[buttonIndx].ButtonHover = true;
			else
				buttonIndx = value;

			if (buttonIndx > Buttons.Length - 1) {
				buttonIndx = 0;
				Buttons[prevHorizIndx].ButtonHover = false;
			}
			else if (buttonIndx < 0) {
				buttonIndx = Buttons.Length - 1;
				Buttons[prevHorizIndx].ButtonHover = false;
			}

			if ((buttonIndx - prevHorizIndx) > 0)
				Buttons[buttonIndx - 1].ButtonHover = false;
			else if ((buttonIndx - prevHorizIndx) < 0)
				Buttons[buttonIndx + 1].ButtonHover = false;

			Buttons[buttonIndx].ButtonHover = true;
			prevHorizIndx = buttonIndx;
		}
	}

	private bool enable = false;
	public bool Enable {
		get { return enable; }
		set {
			enable = value;
			if (enable) {
				gameObject.SetActive(enable);
				//anim.SetBool("Selected", true);

				//StopCoroutine("WindowEnabled");
				//StartCoroutine("WindowEnabled");

				//nextHover = Random.Range(2f, 5f);
				//StopCoroutine("RandomHover");
				//StartCoroutine("RandomHover");
			}
			else {
				if (gameObject.activeInHierarchy) {
					anim.SetBool("Selected", false);

					StopCoroutine("WindowEnabled");
					StopCoroutine("RandomHover");
				}

				gameObject.SetActive(enable);
			}
		}
	}

	private BaseButtonEntity CurrentButton {
		get { return Buttons[ButtonIndx]; }
	}

	private void Awake() {
		anim = GetComponent<Animator>();
		Enable = false;
	}

	//public void ResetWindowButtons() {
	//    if (Buttons.Length > 0) {
	//        int i = 0;
	//        while (i < Buttons.Length) {
	//            Buttons[i].ResetButton();

	//            i++;
	//        }
	//    }
	//}

	private void OnEnable() {
		anim.SetBool("Selected", true);

		//StopCoroutine("WindowEnabled");
		//StartCoroutine("WindowEnabled");

		nextHover = Random.Range(2f, 5f);
		StopCoroutine("RandomHover");
		StartCoroutine("RandomHover");
	}

	private void OnDisable() {
		StopCoroutine("WindowEnabled");
	}

	//private IEnumerator WindowEnabled() {
	//    while (true && Enable) {
	//        if (Input.GetKeyDown(KeyboardPositive)) {
	//            ButtonIndx++;
	//        }

	//        if (Input.GetKeyDown(KeyboardNegative)) {
	//            ButtonIndx--;
	//        }

	//        if (Input.GetKeyDown(KeyboardEnter))
	//            Buttons[buttonIndx].Clicked(true);

	//        if (Input.GetKeyUp(keyboardEnter))
	//            Buttons[buttonIndx].Clicked(false);

	//        //SingleHover();

	//        yield return null;
	//    }
	//}

	private IEnumerator RandomHover() {
		while (nextHover > 0f && Enable) {
			nextHover -= Time.deltaTime;
			yield return null;
		}

		int activeButtonCounter = 0;
		if (Buttons.Length > 0) {
			int i;
			for (i = 0; i < Buttons.Length; i++) {
				if (Buttons[i].ButtonHover) {
					activeButtonCounter++;
				}
			}
		}

		hoveredIndx = Random.Range(0, Buttons.Length);
		if (!Buttons[hoveredIndx].ButtonHover && activeButtonCounter < 1) {
			Buttons[hoveredIndx].ButtonHover = true;

			yield return new WaitForSeconds(1f);

			Buttons[hoveredIndx].ButtonHover = false;
		}

		nextHover = Random.Range(2f, 5f);
		StartCoroutine("RandomHover");
	}

	private void SingleHover() {
		int activeButtonCounter = 0;

		if (Buttons.Length > 0) {
			int i;
			for (i = 0; i < Buttons.Length; i++) {
				if (Buttons[i].ButtonHover) {
					activeButtonCounter++;
				}

				if (activeButtonCounter < 2) {
					if (Buttons[i].ButtonHover && i != hoveredIndx) {
						buttonIndx = i;
					}
				}
				//if (!buttons[i].ButtonHover) {
				//    buttonIndx = i;
				//    break;
				//}
			}

			if (activeButtonCounter == 2) {
				CurrentButton.ButtonHover = false;
			}
		}
	}

	public void ResetWindowManager() {
		nextHover = 0;
		hoveredIndx = -1;

		prevHorizIndx = 0;
		buttonIndx = 0;

		anim = GetComponent<Animator>();
		Enable = false;

		foreach (BaseButtonEntity element in Buttons) {
			if (!element.gameObject.activeSelf)
				element.gameObject.SetActive(true);

			element.Clickable = true;
		}
	}
}
