using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]

public class ArmyScriptMain : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void DeleteThisUpdate () {

		if(Input.GetButtonDown("Fire1"))
		{
			print("Mouse Down");
			
			Vector3 origin = Camera.main.transform.position;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit; // = new RaycastHit();
			//var layerMask = 1 << 8; // check help page "layerMask", here we only cast ray on layer 8 (prefab layer)
			
			// first we check if we hit previously added prefab
			if (Physics.Raycast(origin, ray.direction, out hit, Mathf.Infinity)) //, LayerMask.NameToLayer("Armies")))
			{
				print ("Its a hit!!");
				//clone = hit.transform; // grab the object that we hit, so we can move it around
				
			} else { // we didnt hit prefab, check if are will hit something else
				
				//layerMask = ~layerMask; // invert layermask, so we dont hit layer 8 (which is for prefabs)
				//if (Physics.Raycast (ray, hit, Mathf.Infinity, layerMask))
				//	clone = Instantiate(ObjectToPlace, hit.point, Quaternion.identity);
				print ("You missed layer = " + LayerMask.NameToLayer("Armies"));
			}
			
		}
		
		if(Input.GetButtonUp("Fire1")) // button released
		{
			print("Mouse up");
		}
	}
}
