using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class UnitsWindowScript : MonoBehaviour, IGameWindow
{
    public static UnitsWindowScript instance { get;  set; }

    private static CanvasGroup unitsWindowCanvasGroup;
    private Army army;
    public ScrollRect scrollView;
    private ArmyNameButton[] buttonArray;
    public ArmyNameButton armyNameButton;
    public Image contentImage;


    void Start ()
    {
        instance = this;
        unitsWindowCanvasGroup = GetComponent<CanvasGroup>();
        MakeInvisable();
    }

    public void UpdateState() {
    }

    public static void ShowWindow(Army army) {
        instance.showWindow(army);
    }

    private void showWindow(Army army_) {
        print("show units window");
        army = army_;
        unitsWindowCanvasGroup.alpha = 1f;

        List<Unit> unitList = Requester.getUnits(army.Id);

        Vector2 size = contentImage.rectTransform.sizeDelta;
        float buttonHeight = 30; //todo: make it not hard-coded

        // size the content for the number of units
        contentImage.rectTransform.sizeDelta = new Vector2(size.x, buttonHeight * unitList.Count);

        buttonArray = new ArmyNameButton[unitList.Count];
        int i = 0;
        foreach (Unit unit in unitList) {
            ArmyNameButton anb = GameObject.Instantiate(armyNameButton);

            anb.unit = unit;
            anb.GetComponentInChildren<Text>().text = unit.unitType;
            anb.transform.SetParent(contentImage.transform);
            buttonArray[i++] = anb;
            anb.parentWindow = this;
        }
        scrollView.verticalNormalizedPosition = 1;
    }

    public static void MakeInvisable() {

        unitsWindowCanvasGroup.alpha = 0f;
    }

    public void ClickClose() {
        MakeInvisable();
    }
}

