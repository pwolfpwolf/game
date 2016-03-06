using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MasterController : MonoBehaviour {
	
    public LineRenderer lineRenderer;

	public enum GUIMode { NORMAL, PLOTTING};
    public static GUIMode guiMode { get;  set; }

    private static Army currentArmy { get;  set; }
    private static List<Vector3> pendingPlot = new List<Vector3>();

    private static Town currentTown;

    public static MasterController instance { get; set; }

	// Use this for initialization
	void Start () {
        guiMode = GUIMode.NORMAL;
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {

        if(guiMode == GUIMode.PLOTTING) {  // ------ Plotting--------------

            // ---  RIGHT MOUSE CLICK ------------------

            if (Input.GetMouseButtonDown(1)) {

                ServerObject serverObject = getClickOnObject();
                if (serverObject != null) {

                    // if clicked on a friendly castle
                    if (serverObject.Type == ServerObject.ServerObjectType.Castle &&
                        serverObject.PlayerId == Globals.playerId) {

                        Prompter.enterFriendlyCastle(serverObject, currentArmy);
                    }
                    if (serverObject.Type == ServerObject.ServerObjectType.Army &&
                        serverObject.PlayerId == Globals.playerId) {

                        Prompter.mergeArmies(currentArmy);
                    }
                }

                Vector3 clickLocation = GridUtil.getInstance().getMousePosition();

                print ("click location = " + clickLocation );
              
                string problem = Requester.checkMove(currentArmy, pendingPlot, clickLocation);
                if (problem == null) {
                    pendingPlot.Add(clickLocation);

                    RenderPath.instance.renderPlotPath(currentArmy, pendingPlot, false);
                    ArmyDetailWindow.instance.EnableDisableButtons();
                }
                else {
                    //guiMessage(problem);
                    print("Error=" + problem);
                }

                print("plot list size = " + pendingPlot.Count);
            }
        } 
        else {  // ------- NOT Plotting ---------------------------

            if (Input.GetMouseButtonDown(0)) {  // -- if left mouse click

                ServerObject serverObject = getClickOnObject();
                if (serverObject != null) {

                    // if clicked on a friendly castle
                    if (serverObject.Type == ServerObject.ServerObjectType.Castle &&
                        serverObject.PlayerId == Globals.playerId) {

                        clickedOnTown(serverObject);
                    }
                }
            }
        }
	}

    private void clickedOnTown(ServerObject serverObject) {
        currentTown = (Town) serverObject;
        MainCameraScript.lookAt(currentTown.GameObject.transform.position);

        Army[] armyArray = Requester.getTownInfo(currentTown.Id, currentTown.GameObject );

        CastleWindowScript.GetInstance().Show (currentTown.Name, armyArray);
        ArmyDetailWindow.MakeInvisable();
    }

    public static void undoLastPlot() {
        if (pendingPlot.Count > 1) {
            pendingPlot.RemoveAt(pendingPlot.Count-1);
        }

        RenderPath.instance.renderPlotPath(currentArmy, pendingPlot, false);
    }


    private ServerObject getClickOnObject() {
        ServerObject serverObject = null;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast (ray, out hit)) {
            Transform trans = hit.transform;
            Debug.Log("Clicked on=" + trans.gameObject.name);

            serverObject = ObjectCache.getServerObjectFromGO( trans.gameObject );
        }
        return serverObject;
    }

    public static void startPlotting(Army army) {
        guiMode = GUIMode.PLOTTING;
        currentArmy = army;
        pendingPlot.Clear();
    }



    public void GiveMoveOrder() {

        Requester.savePlot(currentArmy.Id, pendingPlot, "");
        pendingPlot.Clear();
        RenderPath.instance.Clear();
        ArmyDetailWindow.MakeInvisable();
    }

    public void GiveMoveOrder(string destArmyOrCastle) {

        print("MasterController GiveMoveOder() save plot");
        Requester.savePlot(currentArmy.Id, pendingPlot, destArmyOrCastle);
        print("MasterController GiveMoveOder() done save plot");

        pendingPlot.Clear();
        RenderPath.instance.Clear();
        ArmyDetailWindow.MakeInvisable();
    }

    public static void ClickedOnArmy(Transform trans) {
        RenderPath.instance.Clear();
        currentArmy = ObjectCache.getArmyFromGO(trans.gameObject);

        if (currentArmy.PlayerId == Globals.playerId) {
            if (currentArmy.getPlotPath() != null) {
                
                RenderPath.instance.renderPlotPath(currentArmy, currentArmy.getPlotPath(), true);
                print("Need to move this over");
            }
        }
        ArmyDetailWindow.instance.showWindow(currentArmy);

        MainCameraScript.lookAt(trans.position);
    }

    public  void ClickMoveButton() {
        guiMode = GUIMode.PLOTTING;
        ArmyDetailWindow.instance.EnableDisableButtons();
    }

    public void ClickCancelMove() {
        RenderPath.instance.Clear();
        pendingPlot.Clear();
        guiMode = GUIMode.NORMAL;
        ArmyDetailWindow.MakeInvisable();
    }

    public void ClickStop() {
        Requester.cancelMove(currentArmy.Id);
        RenderPath.instance.Clear();
        currentArmy.getPlotPath().Clear();
        ArmyDetailWindow.instance.EnableDisableButtons();
        guiMode = GUIMode.NORMAL;
        ArmyDetailWindow.MakeInvisable();
    }

    public static List<Vector3> GetPendingPlot() {
        return pendingPlot;
    }
	
    public static void SetModeNormal() {
        guiMode = GUIMode.NORMAL;
    }

    public void ClickUnitsButton() {

        UnitsWindowScript.ShowWindow(currentArmy);
    }
}
