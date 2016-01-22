using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusUpdate  : MonoBehaviour {

	private static Dictionary<string, Army> fieldArmyList = 
		new Dictionary<string, Army>();

	public static void update() {

		// server call.

		ServerObject[] sobjArray = Requester.getServerObjects();

		// compare to current list

		print("checking array of size=" + sobjArray.Length);

		foreach (ServerObject sobj in sobjArray) {

			if (sobj is Army) {
				print("Found an Army");
				Army army = (Army) sobj;
				Army existingArmy;
				bool existsAlready = fieldArmyList.TryGetValue(sobj.ObjectId, out existingArmy);
				if (existsAlready) {

					updateExisting(existingArmy, army);
				}
				else {  // add new army
					print("Creating the new army");
					fieldArmyList.Add(sobj.ObjectId, army);
					GameObject go = TerrainScript.getInstance().createArmy(army);
					army.GameObject = go;
				}
			}

		}
		
		

		// remove stuff


	}

	private static void updateExisting(Army existingArmy, Army newArmy) {

		newArmy.GameObject = existingArmy.GameObject;

		//fieldArmyList.Remove(newArmy.Id);
		//fieldArmyList.Add(newArmy.Id, newArmy);

		if (newArmy.X != existingArmy.X ||
		    newArmy.Y != existingArmy.Y) {

			existingArmy.GameObject.transform.position = new Vector3(newArmy.X, 0, newArmy.Y);

			// face where he is going
			if (existingArmy.getPlotPath() != null && existingArmy.getPlotPath().Count > 0) {
				existingArmy.GameObject.transform.rotation.SetLookRotation( existingArmy.getPlotPath()[0]);
			}

			//newArmy.lerp(startingPoint);
		}

		existingArmy.updateWith(newArmy);

	}

	public static Army getArmyFromGO(GameObject go) {
		foreach (Army army in fieldArmyList.Values) {
			if (army.GameObject == go) {
				return army;
			}
		}

		throw new System.Exception("Can't find army from GameObject");
	}
}
