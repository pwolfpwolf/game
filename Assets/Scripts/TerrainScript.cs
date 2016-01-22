using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TerrainScript : MonoBehaviour {
	
	private TerrainData tData;
	private float[,,] alphaData;
	
	private const int GRASS    = 0; //These numbers depend on the order in which
    private const int WATER    = 1; //the textures are loaded onto the terrain
	private const int FARM     = 2;
	
	public GameObject castleGO;
	public GameObject armyGO;

	public Camera mainCamera;

	private static TerrainScript instance;

	// Use this for initialization
	void Start () {

		instance = this;
	
		tData = Terrain.activeTerrain.terrainData;
		
		alphaData = tData.GetAlphamaps(0, 0, tData.alphamapWidth, tData.alphamapHeight);
		
		setGrass();

		ObjectCache.update();

		createTowns();

		StartCoroutine("pollServer");
	}

	public static TerrainScript getInstance() {
		return instance;
	}

	/// <summary>**************************************************************************
	///  
	/// </summary> ************************************************************************
	///
	void setGrass() {
		
		print ("Changing to water");
		print ("Terrain height = " + tData.alphamapHeight);
		print ("Terrain width = " + tData.alphamapHeight);
		print ("Getting the map");
		
		TerrainMap map = Requester.getMap();

		//map.terrain[3,3] = 1;
		
		for(int y=0; y<tData.alphamapHeight; y++) {
            for(int x = 0; x < tData.alphamapWidth; x++) {
				
				if (y < map.y_scale && x < map.x_scale) {
					if (map.terrain[x,y] == 0) {
						alphaData[y, x, WATER] = 1;
		                alphaData[y, x, GRASS] = 0;
						alphaData[y, x, FARM] = 0;
					}
					else {
						alphaData[y, x, WATER] = 0;
		                alphaData[y, x, GRASS] = 1;
						alphaData[y, x, FARM] = 0;
					}
				}
			}
		}
		
		tData.SetAlphamaps(0, 0, alphaData);
		
		print ("Done changing the map to water, grass etc.. ");
	}

	/// <summary>**************************************************************************
	///  
	/// </summary> ************************************************************************
	
	private IEnumerator pollServer()
	{

    	while(true)
    	{

        	yield return new WaitForSeconds(5.0f);  // wait some time
	
        	// do things 
			Debug.Log("Polling server");

			ObjectCache.update();
    	}
	}

	private void createTowns() {

		List<Town> townList = ObjectCache.getTowns();

		foreach (Town town in townList) {

			GameObject go = PrefabUtility.InstantiatePrefab(castleGO) as GameObject; 
			go.transform.localPosition = new Vector3(town.X, 1, town.Y);
			go.transform.Rotate(new Vector3(-90, 0, 0));
			town.GameObject = go;
			town.InstanceId = go.GetInstanceID();
		}

		//Instantiate(castle, new Vector3(0, 0, 0), Quaternion.identity);
		//GameObject go1  = PrefabUtility.InstantiatePrefab(castleGO) as GameObject; 
		//go1.transform.localPosition = new Vector3(0, 0, 0);

	}

	public GameObject createArmy(Army army) {
		GameObject go = PrefabUtility.InstantiatePrefab(armyGO) as GameObject; 
		army.GameObject = go;
		go.transform.localPosition = new Vector3(army.X, 0, army.Y);
		if (army.getPlotPath() != null && army.getPlotPath().Count > 0) {
			go.transform.rotation.SetLookRotation( army.getPlotPath()[0]);
		}
		return go;
	}


}
