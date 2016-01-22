using UnityEngine;
using System.Collections;

public class KeepScript : MonoBehaviour {

	public GameObject keep;

	// Use this for initialization
	void Start () {
		GameObject obj = (GameObject) Instantiate(keep, new Vector3(0, 1, 0), Quaternion.identity);
		obj.transform.Rotate(new Vector3(-90, 0, 0));
	}

}
