
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CastleWindowScript : MonoBehaviour, IGameWindow {

    public  static CastleWindowScript instance { get;  set; }

	public Text castleNameText;
	public Image backgroundImage;
	public ArmyNameButton armyNameButton;
	public ScrollRect scrollView;
	public Image contentImage;

	public Button sallyForthButton;
	public Button mergeButton;
	public Button splitButton;

	private ArmyNameButton[] buttonArray;
    private CanvasGroup castleContainer;

	void Start () {
		instance = this;

        castleContainer = GetComponent<CanvasGroup>();

        this.MakeInvisable();
	}

	public static CastleWindowScript GetInstance() {
		return instance;
	}

	public void MakeVisible() {
		this.gameObject.SetActive(true);
	}

    public void UpdateState() {
        UpdateButtonStates();
    }

	public void Show(string name,  Army[] armyArray) {
		//this.gameObject.SetActive(true);
		castleContainer.alpha = 0.5f;

		castleNameText.text = name;

		Vector2 size = contentImage.rectTransform.sizeDelta;
		float buttonHeight = 30; //todo: make it not hard-coded

		// size the content for the number of units
		contentImage.rectTransform.sizeDelta = new Vector2(size.x, buttonHeight * armyArray.Length);

		buttonArray = new ArmyNameButton[armyArray.Length];
		for (int i=0; i < armyArray.Length; i++) {
			ArmyNameButton anb = GameObject.Instantiate(armyNameButton);

            anb.army = armyArray[i];
			anb.GetComponentInChildren<Text>().text = armyArray[i].Name;
			anb.transform.SetParent(contentImage.transform);
			buttonArray[i] = anb;
		}
		scrollView.verticalNormalizedPosition = 1;
		this.UpdateButtonStates();
	}

	public void MakeInvisable() {
		
        instance.castleContainer.alpha = 0f;

		contentImage.transform.DetachChildren();
	}

	public void UpdateButtonStates() {

		int numberSelected = 0;

		foreach (ArmyNameButton armyNameButton in buttonArray) {
			if (armyNameButton.ButtonSelected) {
				numberSelected++;
			}

			if (numberSelected == 1) {
				sallyForthButton.enabled = true;
				sallyForthButton.gameObject.SetActive(true);
				splitButton.enabled = true;
				splitButton.gameObject.SetActive(true);
			}
			else {
				sallyForthButton.enabled = false;
				sallyForthButton.gameObject.SetActive(false);
				splitButton.enabled = false;
				splitButton.gameObject.SetActive(false);
			}

            if (numberSelected > 1) {
                mergeButton.enabled = true;
				mergeButton.gameObject.SetActive(true);
			}
			else {
				mergeButton.enabled = false;
				mergeButton.gameObject.SetActive(false);
			}
		}
	}

    /** 
     * This assumes there is only one army selected 
     **/

    private Army getSelectedArmy() {
        foreach (ArmyNameButton armyNameButton in buttonArray) {
            if (armyNameButton.ButtonSelected) {
                return armyNameButton.army;
            }
        }

        return null;
    }

    public void clickSallyForth() {
        
        Debug.Log("sally forth pressed");

        Army army = instance.getSelectedArmy();

        MasterController.startPlotting(army);

        ArmyDetailWindow.instance.showWindow(army);

        instance.MakeInvisable();
    }
}
