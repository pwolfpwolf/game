using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArmyDetailWindow : MonoBehaviour
{
    public Text armyNameText;
    public Image armyWindowImage;
    private CanvasGroup armyDetailCanvasGroup;
    private Army army;

    public Button moveOrderButton;
    public Button moveButton;
    public Button cancelPlotButton;
    public Button unitsButton;
    public Button stopButton;

    public static ArmyDetailWindow instance { get;  set; }

    void Start ()
    {
        instance = this;

        armyDetailCanvasGroup = GetComponent<CanvasGroup>();

        MakeInvisable();
    }
	
    // Update is called once per frame
    void Update ()
    {
	
    }

    public void showWindow(Army army) {

        this.army = army;
        armyDetailCanvasGroup.alpha = 0.5f;
        armyNameText.text = army.Name;

        EnableDisableButtons();
    }

    public void EnableDisableButtons() {

        if (MasterController.guiMode == MasterController.GUIMode.PLOTTING) {
            UIUtil.DisableButton(moveButton); 
            UIUtil.EnableButton(cancelPlotButton);
        }
        else {
            UIUtil.EnableButton(moveButton);
            UIUtil.DisableButton(cancelPlotButton);
        }

        if (army.getPlotPath()!= null && army.getPlotPath().Count > 0) {
            UIUtil.EnableButton(stopButton);
        }
        else {
            UIUtil.DisableButton(stopButton);
        }

        if (MasterController.GetPendingPlot().Count > 0) {
            UIUtil.EnableButton(cancelPlotButton);
            UIUtil.EnableButton(moveOrderButton);
        }
        else {
            UIUtil.DisableButton(cancelPlotButton);
            UIUtil.DisableButton(moveOrderButton);
        }

        UIUtil.EnableButton(unitsButton);

    }

    public static void MakeInvisable() {
        
        instance.armyDetailCanvasGroup.alpha = 0f;
        //armyWindowImage.transform.DetachChildren();
    }

    public void ClickClose() {
        MasterController.SetModeNormal();
        MasterController.instance.ClickCancelMove();
        MakeInvisable();
    }
}

