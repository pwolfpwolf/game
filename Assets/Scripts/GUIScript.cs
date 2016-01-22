using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIScript : MonoBehaviour {

	private enum GUIMode { TOP_LEVEL, TOWN_LIST, TOWN_DETAIL, PLOTTING, ARMY_DETAIL, ARMY_LIST };
	private enum PromptType { CURRENT_PROMPT, ATTACK_ARMY };

	public LineRenderer lineRenderer;

	private LineRenderer pendingLineRenderer;

	
	private GUIMode guiMode = GUIMode.TOP_LEVEL;
	
	private Town currentTown;
	private Army currentArmy;

	private bool[] toggleTxt;
	private Army[] armyArray;

	private List<Vector3> plotList = new List<Vector3>();
	private bool plotLineVisible = false;

	private bool promptEnterCastle = false;

	public Color c1 = Color.red;
	public Color c2 = new Color(1, 1, 1, 0);

	private Stack<GUIMode> modeStack = new Stack<GUIMode>();

	private static GUIScript instance;

	private bool messageOn = false;
	private string messageText;

	private bool showPrompt = false;
	private PromptType promptType;

	private ServerObject targetServerObject;

	void Start() {

		pendingLineRenderer = gameObject.AddComponent<LineRenderer>();
		pendingLineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		pendingLineRenderer.SetColors(c1, c2);

		//plotLineRenderer.SetPosition(0, new Vector3(0,1,0));
		//plotLineRenderer.SetPosition(0, new Vector3(500,1,500));

		instance = this;

	}

	public static GUIScript getInstance() {
		return instance;
	}

	void OnGUI () {

		playerSwitcher();
		
		if (this.guiMode == GUIMode.TOWN_LIST) {
			List<Town> townList = ObjectCache.getTowns();
			
			// Make a background box
			GUI.Box(new Rect(10,10,170,50 + ((townList.Count * 20)) + 20 ), "");
			
			int lastControlAt = 50;
			lastControlAt = createButtonsFromTowns(true, townList, lastControlAt);

			lastControlAt += 20;

			createButtonsFromTowns(false, townList, lastControlAt);

			armyArray = null;
		}
		else if (this.guiMode == GUIMode.ARMY_LIST) {

			List<Army> armyList = ObjectCache.getMyArmies();
			int lastControlAt = 50;
			lastControlAt = createButtonsFromArmies(armyList, lastControlAt);

			armyList = ObjectCache.getOtherArmies();
			lastControlAt = lastControlAt + 100;
			lastControlAt = createButtonsFromArmies(armyList, lastControlAt);
		}
		else if (this.guiMode == GUIMode.TOWN_DETAIL) {
						
			if (armyArray == null) {
				armyArray = Requester.getTownInfo(this.currentTown.Id, this.currentTown.GameObject );
				toggleTxt = new bool[armyArray.Length];
			}
			
			int lastControlAt = 15;
			//int i = 0;
			//foreach (Army army in armyArray) {
			//	lastControlAt = 100 + (i*20);
			//	toggleTxt[i] = GUI.Toggle(new Rect(10, lastControlAt, 100, 30), toggleTxt[i], 
			//	                          army.Name + "(" + army.NumberOfMen + ")");
			//	i++;
			//}
			
			lastControlAt = lastControlAt + 50;
			
			GUI.enabled = false;
			
			int numSelected = 0;
			for (int j=0; j<toggleTxt.Length; j++) {
				if (toggleTxt[j] == true) {
					numSelected++;
				}
			}
			if (numSelected == 1) {
				GUI.enabled = true;
			}
			if(GUI.Button(new Rect(15,lastControlAt,80,20), "Sally Forth")) {
				sallyForth();
			}
			if(GUI.Button(new Rect(115,lastControlAt,80,20), "Split")) {
			}
			if (numSelected > 1) 
				GUI.enabled = true;
			else
				GUI.enabled = false;
			
			if(GUI.Button(new Rect(215,lastControlAt,80,20), "Merge")) {
			}
		}
		else if (this.guiMode == GUIMode.PLOTTING) {

			int verticleStart = 60;

			GUI.Box(new Rect(10,verticleStart,170,100), "Right click to plot move");

			if (this.plotList.Count > 0) {
				GUI.enabled = true;
			} else {
				GUI.enabled = false;
			}
			if(GUI.Button(new Rect(10,verticleStart + 30,80,20), "Undo")) {
				this.plotList.RemoveAt( this.plotList.Count - 1);
				renderPlotList();
			}
			if(GUI.Button(new Rect(60, verticleStart + 60,80,20), "Give Orders")) {
				Requester.savePlot(currentArmy.Id, this.plotList, "");
				turnOffPlotLine();
				this.guiMode = GUIMode.TOWN_LIST;
			}

			GUI.enabled = true;
			if(GUI.Button(new Rect(100, verticleStart + 30,80,20), "Cancel")) {

				this.guiMode = GUIMode.TOWN_LIST;
				this.plotList.Clear();
				this.pendingLineRenderer.SetVertexCount(0);
				this.lineRenderer.SetVertexCount(0);
			}

			if (promptEnterCastle)
				guiPrompEnterCastle();
		}
		else if (this.guiMode == GUIMode.ARMY_DETAIL) {

			guiArmyDetail();

		}

		GUI.enabled = true;

		if(GUI.Button(new Rect(15,15,80,20), "Castles")) {
			this.guiMode = GUIMode.TOWN_LIST;
		}
		
		if(GUI.Button(new Rect(95,15,80,20), "Armies")) {
			this.guiMode = GUIMode.ARMY_LIST;
		}

		if (this.messageOn) {
			guiMessage();
		}

		if (this.showPrompt) {
			guiPrompt(PromptType.CURRENT_PROMPT);
		}
	}

	//-------------------------------------------------------------------------------------------------

	void Update()
	{
		if(this.guiMode == GUIMode.PLOTTING) {

			// ---  RIGHT MOUSE CLICK ------------------

			if (Input.GetMouseButtonDown(1)) {

				Vector3 clickLocation = GridUtil.getInstance().getMousePosition();

				print ("click location = " + clickLocation );

				string problem = Requester.checkMove(currentArmy, this.plotList, clickLocation);
				if (problem == null) {
					plotList.Add(clickLocation);

					renderPlotList();
				}
				else {
					guiMessage(problem);
				}
			}
		}

		if (Input.GetMouseButtonDown(0)) {

			checkIfClickedOnSomething();
		}

		// if right clicked on something while plotting
		if (Input.GetMouseButtonDown(1) && guiMode == GUIMode.PLOTTING) {
			ServerObject serverObject = getClickOnObject();
			if (serverObject != null) {
				if (serverObject is Town) {
					promptEnterCastle = true;
					currentTown = (Town) serverObject;
				}
				else if (serverObject is Army) {

					if (serverObject.PlayerId != Globals.playerId) {
						this.targetServerObject = serverObject;
						this.promptType = PromptType.ATTACK_ARMY;
						this.showPrompt = true;
					}
				}

			}
		}

		// check if the plot line needs adjusting
		if (this.plotLineVisible ) {

			renderPlotList();
		}
	}

	//-----------------------------------------------------------------------------------------------

	private void sallyForth() {

		currentArmy = getSelectedArmy();
		this.plotList.Clear();

		guiMode = GUIMode.PLOTTING;
	}

	//-----------------------------------------------------------------------------------------------

	private void renderPlotList() {

		this.plotLineVisible = true;

		bool somethingToDisplay = false;
	
		// current plot path
		if (currentArmy.getPlotPath() != null) {
			drawLines( currentArmy.getPosition(), currentArmy.getPlotPath(), this.lineRenderer);
			somethingToDisplay = true;
		}
		else {
			this.lineRenderer.SetVertexCount(0);
		}

		// pending plot path
		if (this.plotList.Count > 0) {

			Vector3 startingPoint;
			List<Vector3> currentPath = currentArmy.getPlotPath();
			if (currentPath != null) {
				startingPoint = currentPath[ currentPath.Count - 1 ];
			}
			else {
				startingPoint = currentArmy.getPosition();
			}

			drawLines( startingPoint, this.plotList, this.pendingLineRenderer);
			somethingToDisplay = true;
		}

		if ( !somethingToDisplay )
			this.plotLineVisible = false;
	}

	//-----------------------------------------------------------------------------------------------

	private void drawLines(Vector3 startingPoint, List<Vector3> linePoints, LineRenderer lineRenderer ) {

		if (linePoints.Count > 0) {
			lineRenderer.SetVertexCount(linePoints.Count + 1);

			lineRenderer.SetPosition(0, startingPoint);
			
			int position = 1; 
			foreach (Vector3 plot in linePoints) {
				lineRenderer.SetPosition(position, plot);
				position++;
			}
		}
		else {
			lineRenderer.SetVertexCount(0); // turn it off if nothing to display
		}
	}

	//-----------------------------------------------------------------------------------------------


	private void checkIfClickedOnSomething() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast (ray, out hit)) {
			Transform trans = hit.transform;
			Debug.Log("Clicked on=" + trans.gameObject.name);
			
			if (trans.gameObject.name == "Army") {
				clickedOnArmy(trans);
			}
			else {
				//GameObject parent = trans.parent.gameObject;

				if (trans.gameObject.name == "keep3") {
					clickedOnTown(trans);
				}
			}
		}
	}

	//---------------------------------------------------------------------------

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

	private void turnOffPlotLine() {
		this.pendingLineRenderer.SetVertexCount(0);
		this.plotList.Clear();
		this.lineRenderer.SetVertexCount(0);
		this.plotLineVisible = false;
	}

	private void clickedOnArmy(Transform trans) {
		currentArmy = ObjectCache.getArmyFromGO(trans.gameObject);
		guiMode = GUIMode.ARMY_DETAIL;

		if (currentArmy.PlayerId == Globals.playerId) {
			if (currentArmy.getPlotPath() != null) {
				renderPlotList();
			}
		}
		MainCameraScript.lookAt(trans.position);
	}

	// clicked on a town, so show the town window

	private void clickedOnTown(Transform trans) {
		this.currentTown = ObjectCache.getTownFromGO(trans.gameObject);
		guiMode = GUIMode.TOWN_DETAIL;
		MainCameraScript.lookAt(trans.position);

		armyArray = Requester.getTownInfo(this.currentTown.Id, this.currentTown.GameObject );

		CastleWindowScript.GetInstance().Show (this.currentTown.Name, armyArray);

	}
	
	private Army getSelectedArmy() {
		for (int i=0; i<this.toggleTxt.Length; i++) {
			if (this.toggleTxt[i]) {
				return this.armyArray[i];
			}
		}
		return null;
	}

	private int createButtonsFromArmies(List<Army> armyList, int lastControlAt) {
		foreach (Army army in armyList) {

			string armyName;
			if (army.PlayerId == Globals.playerId)
				armyName = army.Name;
			else
				armyName = "Enemy Army";

			if(GUI.Button(new Rect(20,lastControlAt,160,20), armyName)) {
				currentArmy = army;
				this.guiMode = GUIMode.ARMY_DETAIL;
				if (army.isMoving()) {
					this.plotList = army.getPlotPath();
					renderPlotList();
				}
				MainCameraScript.lookAt( army.GameObject.transform.localPosition);
			}
			lastControlAt = lastControlAt + 20;
		}
		return lastControlAt;
	}

	private int createButtonsFromTowns(bool mine, List<Town> townList, int lastControlAt) {
		foreach (Town town in townList) {
				if ( ( mine && town.PlayerId == Globals.playerId) ||
				     ( !mine && town.PlayerId != Globals.playerId ) ) {

				if(GUI.Button(new Rect(20,lastControlAt,160,20), town.Name)) {
					print ("You clicked town = " + town.Name);
					currentTown = town;	
					MainCameraScript.lookAt( town.GameObject.transform.localPosition );
					this.guiMode = GUIMode.TOWN_DETAIL;
				}
				lastControlAt = lastControlAt + 20;
			}
		}
		return lastControlAt;
	}

	private void playerSwitcher() {
		if (GUI.Button(new Rect(Screen.width-500,20,80,20), "Player " + Globals.playerId)) {
			if (Globals.playerId == 1) {
				Globals.playerId = 2;
			}
			else {
				Globals.playerId = 1;
			}
		}
	}

	//--------------------------------------------------------------------------------------------

	private void guiArmyDetail() {

		GUI.Box(new Rect(10,40,220,100), currentArmy.Name);
		
		GUI.Label(new Rect(15,70,80,20), "Men: " + currentArmy.NumberOfMen);
		GUI.Label(new Rect(15,90,180,20), "State: " + currentArmy.State);

		if (currentArmy.PlayerId == Globals.playerId) {

			if(GUI.Button(new Rect(15,110,80,20), "Move")) {
				
				this.guiMode = GUIMode.PLOTTING;
			}

			if ( currentArmy.State == "Moving" ) {

				if(GUI.Button(new Rect(100,110,125,20), "Cancel Move Orders")) {

					currentArmy.State = "Stopping";

					Requester.cancelMove(currentArmy.Id);

					this.guiMode = GUIMode.ARMY_DETAIL;
				}
			}
		}
	}

	//--------------------------------------------------------------------------------------------

	private void guiPrompEnterCastle() {

		int centerX = Screen.width / 2;
		int centerY = Screen.height / 2;

		int boxWidth = 200;
		int boxHeight = 80;

		GUI.Box(new Rect(centerX - (boxWidth/2), centerY - (boxHeight/2), boxWidth, boxHeight), 
		                 "Enter Castle?");

		if (GUI.Button(new Rect(centerX - 90, centerY-10, 80, 20), "Yes")) {
			Requester.savePlot(currentArmy.Id, this.plotList, currentTown.Id);
			turnOffPlotLine();
			this.guiMode = GUIMode.TOWN_DETAIL;
			this.promptEnterCastle = false;
		}
		if (GUI.Button(new Rect(centerX + 10, centerY-10, 80, 20), "No")) {
			this.plotList.RemoveAt( this.plotList.Count - 1);
			renderPlotList();
			this.promptEnterCastle = false;
		}
	}

	//------------------------------------------------------------------------------------------------
	// when an army disappears for some reason (like goes into a Castle), you 
	// want to make sure the GUI doesn't break.

	public void disappearArmy(Army army) {
		if (army == this.currentArmy &&
		    this.guiMode == GUIMode.ARMY_DETAIL) {

			this.turnOffPlotLine();
			this.guiMode = GUIMode.TOP_LEVEL;
		}
	}

	//----------------------------------------------------------------------------------------------------

	private void pushMode(GUIMode newMode) {

		this.modeStack.Push( this.guiMode );
		this.guiMode = newMode;
	}

	private void popMode() {
		if (this.modeStack.Count > 0) {
			this.guiMode = this.modeStack.Pop();
		}
	}

	//----------------------------------------------------------------------------------------------------

	private void guiMessage(string messageText = null) {

		int centerX = Screen.width / 2;
		int centerY = Screen.height - 200;
		
		int boxWidth = 200;
		int boxHeight = 80;

		if (messageText != null) {
			this.messageText = messageText;
			this.messageOn = true;
		}
		
		GUI.Box(new Rect(centerX - (boxWidth/2), centerY - (boxHeight/2), boxWidth, boxHeight), 
		        this.messageText);

		if (GUI.Button(new Rect(centerX - 40, centerY, 80, 20), "OK")) {
			this.messageOn = false;
		}
	}

	//----------------------------------------------------------------------------------------------------

	private void guiPrompt(PromptType promptType) {

		if (promptType != PromptType.CURRENT_PROMPT) {
			this.showPrompt = true;
			this.promptType = promptType;
		}

		int centerX = Screen.width / 2;
		int centerY = Screen.height - 200;
		
		int boxWidth = 200;
		int boxHeight = 80;

		GUI.Box(new Rect(centerX - (boxWidth/2), centerY - (boxHeight/2), boxWidth, boxHeight), 
		        "Attack Army?");
		
		if (GUI.Button(new Rect(centerX - 90, centerY-10, 80, 20), "Yes")) {

			switch ( this.promptType) {
			case (PromptType.ATTACK_ARMY) :
					Requester.savePlot(currentArmy.Id, this.plotList, this.targetServerObject.ObjectId);
					turnOffPlotLine();
					this.guiMode = GUIMode.TOWN_LIST;
					break;

			default: 
				throw new System.Exception("Unknown PromptType");
			}

		}
		if (GUI.Button(new Rect(centerX + 10, centerY-10, 80, 20), "No")) {
			this.showPrompt = false;
		}
	}
	
}