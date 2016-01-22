using UnityEngine;
using System.Collections.Generic;

public class Army : ServerObject {

	private string armyName;
	private string state;
	private int numberOfMen;
	private string path;
	private List<Vector3> plotPath;
	private GameObject castleGameObject;
	private int playerId;
	private float rotation = 90;

	// this is for just listing armies in a castle
	public Army (string name, string state, int numberOfMen, string id, 
	            GameObject castleGameObject) : 
		base(id)
	{
		this.armyName = name;
		this.state = state;
		this.numberOfMen = numberOfMen;
		this.castleGameObject = castleGameObject;
	}

	public Army (string name, string state, int numberOfMen, string id, float x, float y, 
	             string path, int playerId) :
		base (x, y, id, ServerObject.ServerObjectType.Army, playerId)
	{
		this.armyName = name;
		this.state = state;
		this.numberOfMen = numberOfMen;
		this.path = path;
	}

	public float GetRotation(Vector3 currentPosition) {

		getPlotPath();

		if (this.plotPath != null && this.plotPath.Count > 0) {
			rotation = Calculator.CalcRotation(currentPosition, plotPath[0]);
		}

		return rotation;
	}
		
	public string Name {
		get {
			return this.armyName;
		}
		set {
			armyName = value;
		}
	}

	public string State {
		get {
			return this.state;
		}
		set {
			state = value;
		}
	}

	public int NumberOfMen {
		get {
			return this.numberOfMen;
		}
		set {
			numberOfMen = value;
		}
	}	

	public string Id {
		get {
			return this.id;
		}
		set {
			this.id = value;
		}
	}

	public GameObject CastleGameObject {
		get {
			return this.castleGameObject;
		}
	}
	
	public List<Vector3> getPlotPath() {

		if (plotPath == null && path != null) {
			this.plotPath = new List<Vector3>();
			string[] points = path.Split(':');
			foreach (string point in points) {
				string[] xy = point.Split(',');
				this.plotPath.Add(new Vector3(float.Parse(xy[0]) * Globals.MAP2TERRAIN_MULT, 
				                              0, 
				                              float.Parse(xy[1]) * Globals.MAP2TERRAIN_MULT));
			}
		}

		return this.plotPath;
	}

	// and army might be in a castle, so you can't just get the location from the 
	// GameObject

	public Vector3 getPosition() {
		if (this.GameObject != null) 
			return GameObject.transform.position;
		else 
			return this.castleGameObject.transform.position;

	}

	public bool isMoving() {
		if (this.path == null)
			return false;
		else
			return true;
	}

	public void updateWith(Army updatedArmy) {
		this.armyName = updatedArmy.armyName;
		this.state = updatedArmy.state;
		this.numberOfMen = updatedArmy.numberOfMen;
		this.path = updatedArmy.path;
		this.plotPath = null;
	}

}
	