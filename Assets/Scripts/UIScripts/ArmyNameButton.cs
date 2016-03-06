using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArmyNameButton : MonoBehaviour {

	private Color normalColor;
	private Color pressedColor;
	private Button button;
	private bool buttonSelected = false;


    public Army army { get;  set; }
    public Unit unit { get;  set; }
    public IGameWindow parentWindow  { get;  set; }

	void Start () {
		button = GetComponent<Button>();
		normalColor = button.colors.normalColor;
		pressedColor = button.colors.pressedColor;

	}

	public void PressedButton() {

		Debug.Log("Pressed button");

		if (buttonSelected) {
			buttonSelected = false;
		    GetComponent<Image>().color = normalColor;
		}
		else {
			buttonSelected = true;
			GetComponent<Image>().color = pressedColor;
		}

        parentWindow.UpdateState();
	}

	public bool ButtonSelected
	{
		get
		{
			return buttonSelected;
		}
	}

}
