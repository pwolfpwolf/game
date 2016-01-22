using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectCache : MonoBehaviour {

	private static Dictionary<string, Army> fieldArmyList = 
		new Dictionary<string, Army>();

	private static List<Town> townList = null;
	private static int turnNumber = 0;

	//----------------------------------------------------------------------------------------

	public static void update() {

		turnNumber++;

		// server call.

		ServerObject[] sobjArray = Requester.getServerObjects();

		// compare to current list
	
		foreach (ServerObject sobj in sobjArray) {

			if (sobj is Army) {
				Army army = (Army) sobj;
				Army existingArmy;
				bool existsAlready = fieldArmyList.TryGetValue(sobj.ObjectId, out existingArmy);
				if (existsAlready) {

					updateExisting(existingArmy, army);
				}
				else {  // add new army
					fieldArmyList.Add(sobj.ObjectId, army);
					GameObject go = TerrainScript.getInstance().createArmy(army);
					army.GameObject = go;
					army.TurnNumber = turnNumber;
				}
			}
		}

		// remove stuff if they no longer exist

		removeArmies();

		// the first time only, populate towns
		if (townList == null) {
			townList = new List<Town>();
			foreach (ServerObject sobj in sobjArray) {
				if (sobj is Town) {
					townList.Add((Town) sobj);
				}
			}
		}
	}

	//----------------------------------------------------------------------------------------

	private static void updateExisting(Army existingArmy, Army newArmy) {

		//newArmy.GameObject = existingArmy.GameObject;

		//fieldArmyList.Remove(newArmy.Id);
		//fieldArmyList.Add(newArmy.Id, newArmy);

		if (newArmy.X != existingArmy.X ||
		    newArmy.Y != existingArmy.Y) {

			existingArmy.GameObject.transform.position = new Vector3(newArmy.X, 0, newArmy.Y);

			//newArmy.lerp(startingPoint);

			// look where your going
			List<Vector3> path = existingArmy.getPlotPath();
			if (path != null && path.Count > 0) {
				existingArmy.GameObject.transform.LookAt(path[0]);
			}
			//else {
			//	//existingArmy.GameObject.transform.LookAt(  Vector3(newArmy.X, 0, newArmy.Y) );
			//}
		}

		existingArmy.updateWith(newArmy);
		existingArmy.TurnNumber = turnNumber;
	}

	//----------------------------------------------------------------------------------------

	public static Army getArmyFromGO(GameObject go) {
		foreach (Army army in fieldArmyList.Values) {
			if (army.GameObject == go) {
				return army;
			}
		}

		throw new System.Exception("Can't find army from GameObject");
	}

	//----------------------------------------------------------------------------------------
	
	public static Town getTownFromGO(GameObject go) {

		foreach (Town town in townList) {
			if (town.GameObject == go) {
				return town;
			}
		}
		
		throw new System.Exception("Can't find town from GameObject");
	}

	//----------------------------------------------------------------------------------------

	public static List<Town> getTowns() {
		return townList;
	}

	//----------------------------------------------------------------------------------------

	public static ServerObject getServerObjectFromGO(GameObject go) {

		GameObject findObject = null;
		if (go.transform.parent != null) {
			findObject = go.transform.parent.gameObject;
		}
		else {
			findObject = go;
		}

		foreach (Town town in townList) {

			if (town.GameObject.GetInstanceID() == findObject.GetInstanceID()) {
				return town;
			}
		}

		foreach (Army army in fieldArmyList.Values) {
			if (army.GameObject == findObject) 
				return army;
		}

		throw new System.Exception("Can't find ServerObject from GameObject");
	}

	//----------------------------------------------------------------------------------------

	public static List<Army> getMyArmies() {
		List<Army> armyList = new List<Army>();
		foreach (Army army in fieldArmyList.Values) {
			if (army.PlayerId == Globals.playerId) {
				armyList.Add(army);
			}
		}
		return armyList;
	}

	//----------------------------------------------------------------------------------------

	public static List<Army> getOtherArmies() {
		List<Army> armyList = new List<Army>();
		foreach (Army army in fieldArmyList.Values) {
			if (army.PlayerId != Globals.playerId) {
				armyList.Add(army);
			}
		}
		return armyList;
	}

	//----------------------------------------------------------------------------------------

	private static void removeArmies() {

		List<Army> toBeRemoved = new List<Army>();

		foreach (Army army in fieldArmyList.Values) {
			if (army.TurnNumber < turnNumber) {
				toBeRemoved.Add(army);
			}
		}

		foreach (Army army in toBeRemoved) {
			GUIScript.getInstance().disappearArmy(army);
			Destroy(army.GameObject);
			fieldArmyList.Remove(army.Id);
		}
	}


}
