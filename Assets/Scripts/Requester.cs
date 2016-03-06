using System;
using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;

public class Requester  {

		
	public static string getTestString() {
		
		WWW request = new WWW("http://localhost:8080/Game/Request?type=mapTerrain"); 
		
		while (! request.isDone) {} ;
		
		return request.text;
	}
	
	public static TerrainMap getMap() {

		WWW request = new WWW(Globals.REQUEST_URL + "type=mapTerrain"); 

		while (! request.isDone) {} ;
		
		Debug.Log("text = " + request.text);
		
		//string textResult = "[{\"Y-SCALE\":200,\"X-SCALE\":200 }]";
		string textResult = request.text;// + "END";
		
		string trimmedResult = fixResult(textResult) ;

		Debug.Log("substring = " + trimmedResult );

		var N = JSONNode.Parse(trimmedResult);
		var x = N["X-SCALE"].AsInt;
		
		Debug.Log(" x = " + x);

		string terrainStr = N["TERRAIN"];
		
		return new TerrainMap( N["X-SCALE"].AsInt, N["Y-SCALE"].AsInt, terrainStr);
	}

	//--------------------------------------------------------------------------------

	public static ServerObject[] getServerObjects() {

		WWW request = new WWW(Globals.REQUEST_URL + "type=status&playerId=" + Globals.playerId); 
		
		while (! request.isDone) {} ;

		JSONNode objs = JSONNode.Parse(request.text);
		JSONArray castleArray = objs["CASTLES"].AsArray;
		JSONArray armyArray = objs["ARMIES"].AsArray;

		ServerObject[] serverObjects = new ServerObject[ castleArray.Count + armyArray.Count ];

		int i=0;

		foreach (JSONNode castle in castleArray) {

			ServerObject serverObject = new Town(
				castle["NAME"],
				castle["ID"],
				castle["FARMLAND"].AsInt,
				castle["FARMERS"].AsInt,
				castle["FOOD"].AsInt,
				castle["STATE"],
				castle["X"].AsFloat * Globals.MAP2TERRAIN_MULT,
				castle["Y"].AsFloat * Globals.MAP2TERRAIN_MULT,
				castle["PLAYER"].AsInt);

			serverObjects[i] = serverObject;
			i++;
		}

		foreach (JSONNode army in armyArray) {
			
			ServerObject serverObject = new Army(
				army["NAME"],
				army["STATE"],
				army["MEN"].AsInt,
				army["ID"],
				army["X"].AsFloat * Globals.MAP2TERRAIN_MULT, 
				army["Y"].AsFloat * Globals.MAP2TERRAIN_MULT,
				army["PATH"],
				army["PLAYER"].AsInt
				);
			
			serverObjects[i] = serverObject;
			i++;
		}

		/*
		string result = "{ \"objectArray\" : " + request.text + "}";
		Debug.Log("text = " + result);

		JSONNode objects = JSONNode.Parse(result);


		JSONArray objArray = objects["objectArray"].AsArray;

		ServerObject[] serverObjects = new ServerObject[ objArray.Count ];

		int i=0;
		foreach (JSONNode obj in objArray) {
			string type = obj["TYPE"];
			Debug.Log ("object=" + obj["ID"] + " type =" + type);
			ServerObject serverObject;
			if (type == "CASTLE") {

				serverObject = new Town(
					obj["NAME"],
					obj["ID"],
					obj["FARMLAND"].AsInt,
					obj["FARMERS"].AsInt,
					obj["FOOD"].AsInt,
					obj["STATE"],
					obj["X"].AsFloat * Globals.MAP2TERRAIN_MULT,
					obj["Y"].AsFloat * Globals.MAP2TERRAIN_MULT,
					obj["PLAYER"].AsInt);
			}
			else {

				serverObject = new Army(
					obj["NAME"],
					obj["STATE"],
					obj["MEN"].AsInt,
					obj["ID"],
					obj["X"].AsFloat * Globals.MAP2TERRAIN_MULT, 
					obj["Y"].AsFloat * Globals.MAP2TERRAIN_MULT,
					obj["PATH"],
					obj["PLAYER"].AsInt
					);

				Debug.Log("STATE = " + obj["STATE"]);
			}
			serverObjects[i] = serverObject;
			i++;
		}*/
		return serverObjects;
	}


	/// <summary> 
	/// 
	/// </summary>
	/// <returns>The towns.</returns> =======================================================================

	public static Army[] getTownInfo(string castleId, GameObject castleGameObject) {

		WWW request = new WWW(Globals.REQUEST_URL + "type=castle&castleId=" + castleId); 
		
		while (! request.isDone) {} ;

		string result = "{ \"objectArray\" : " + request.text + "}";
		Debug.Log("text = " + result);
		
		JSONNode objects = JSONNode.Parse(result);
		JSONArray objArray = objects["objectArray"].AsArray;

		Army[] armyList = new Army[objArray.Count];
		int i=0;
		foreach (JSONNode obj in objArray) {
			armyList[i] = new Army(
				obj["NAME"],
				obj["STATE"],
				obj["MEN"].AsInt,
				obj["ID"],
				castleGameObject
				);
			i++;
		}


		return armyList;
	}

	// ------------------------------------------------------------------------------------------------------

	public static void savePlot(string armyId, List<Vector3> plotList, string castleId) {

		int i = 0;
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		foreach (Vector3 v3 in plotList) {

			sb.Append(
				convertUnityToServer(v3.x) + ":" + 
				convertUnityToServer(v3.z));

			if (i < (plotList.Count - 1)) {
				sb.Append(",");
			}
			i++;
		}
		
		new WWW(Globals.REQUEST_URL + "type=move&id=" + armyId + "&path=" + 
		                      sb.ToString() + "&castleId=" + castleId); 

	}

	// ------------------------------------------------------------------------------------------------------

	public static void cancelMove(string id) {

		new WWW(Globals.REQUEST_URL + "type=cancelMove&id=" + id); 
	}

	// ------------------------------------------------------------------------------------------------------

	public static void readNote(int noteNumber) {
		
		new WWW(Globals.REQUEST_URL + "type=readNote&noteNumber=" + noteNumber + 
		        "&playerId=" + Globals.playerId); 
	}

	// ------------------------------------------------------------------------------------------------------
	
	public static void deleteNote(int noteNumber) {
		
		new WWW(Globals.REQUEST_URL + "type=deleteNote&noteNumber=" + noteNumber + 
		        "&playerId=" + Globals.playerId); 
	}

	// ------------------------------------------------------------------------------------------------------
	// check to see if a plotted move is legal, ie: not accross water etc..

	public static string checkMove(Army army, List<Vector3> plotList, Vector3 destination) {

		Vector3 start;
		if (plotList.Count > 0) {
			start = plotList[ plotList.Count - 1 ];
		}
		else {
			if (army.CastleGameObject != null) {
				start = army.CastleGameObject.transform.position;
			}
			else {
				start = army.GameObject.transform.position;
			}
		}

		string requestStr = Globals.REQUEST_URL + 
			"type=checkMove&startX=" + convertUnityToServer(start.x) + 
				"&startY=" + convertUnityToServer(start.z) +
				"&endX=" + convertUnityToServer(destination.x) + 
				"&endY=" + convertUnityToServer(destination.z) +
				"&id=" + army.Id;

		Debug.Log("request=" + requestStr);
				
		WWW request = new WWW(requestStr);

		while (! request.isDone) {} ;

		Debug.Log("result=" + request.text);

		JSONNode node = JSON.Parse(request.text);

		string status = node["STATUS"];

		if (status == "success") {
			return null;
		}	
		else {
			Debug.Log("Failure message: " + status);
			return node["MESSAGE"];
		}
	}

	// ------------------------------------------------------------------------------------------------------

	public static List<Notification> getNotifications() {

		WWW request = new WWW(Globals.REQUEST_URL + "type=notes&playerId=" + Globals.playerId); 
		
		while (! request.isDone) {} ;
		
		string result = "{ \"objectArray\" : " + request.text + "}";
		Debug.Log("text = " + result);
		
		JSONNode objects = JSONNode.Parse(result);
		JSONArray objArray = objects["objectArray"].AsArray;

		List<Notification> noteList = new List<Notification>();

		foreach (JSONNode obj in objArray) {
			Notification notification = new Notification(
				obj["SUBJECT"],
				obj["AGO"],
				obj["TIME"].AsInt,
				obj["NOTENUMBER"].AsInt,
				obj["READ"].AsBool,
				obj["TEXT"]
				);
			noteList.Add (notification);
		}

		return noteList;
	}
	
	// ------------------------------------------------------------------------------------------------------

	private static string fixResult(string result) {
		return result.Substring(1, result.Length -3) ;
	}

	private static float convertUnityToServer(float f) {
		return f / Globals.MAP2TERRAIN_MULT;
	}

    //-----------------------------------------------------------------------------------------

    public static List<Unit> getUnits(string armyId) {

        WWW request = new WWW(Globals.REQUEST_URL + "type=armyUnits&id=" + armyId +
            "&playerId=" + Globals.playerId); 

        while (! request.isDone) {} ;

        List<Unit> unitList = new List<Unit>();

        JSONNode units = JSONNode.Parse(request.text);

        Debug.Log("result=" + request.text);

        JSONArray unitsArray = units["UNITS"].AsArray;
        foreach (JSONNode obj in unitsArray) {
            Debug.Log("MEN=" + obj["MEN"]);

            Unit unit = new Unit(
                obj["UNITID"],
                obj["UNITTYPE"],
                Int32.Parse(obj["MEN"]),
                obj["EXPERIENCE"] );

            unitList.Add(unit);
        }

        return unitList;
    }

}
