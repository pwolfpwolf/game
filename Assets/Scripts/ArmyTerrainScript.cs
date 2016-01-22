using UnityEngine;
using UnityEditor;

public class ArmyTerrainScript : MonoBehaviour {

	public GameObject armyGO;

	// Use this for initialization
	void Start () {

		GameObject go = PrefabUtility.InstantiatePrefab(armyGO) as GameObject; 
		go.transform.localPosition = new Vector3(3,0,3);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
