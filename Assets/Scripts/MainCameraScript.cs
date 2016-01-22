using UnityEngine;
using System.Collections;




/**
 * This is used for zooming in and out of Castles 
 */




public class MainCameraScript : MonoBehaviour {

	private static int ZOOM_Y_DELTA = 5;
	private static int ZOOM_Z_DELTA = -30;

	private static MainCameraScript instance;
	Vector3 lookFromLocation;
	Vector3 lookAtLocation;

	static bool zooming = false;
	static float increment;

	// Use this for initialization
	void Start () {

		instance = this;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (zooming) {
			Camera.main.transform.position =

				Vector3.Slerp(instance.transform.position,
				              lookFromLocation,
				              increment);
			
			if (increment <1)
				increment +=0.01f;

			if (increment >= 1) {
				zooming = false;
				instance.transform.LookAt(lookAtLocation);
			}
			
		}
	
	}

	public static void lookAt(Vector3 location) {
		//instance.transform.LookAt(location);
		zooming = true;
		increment = 0.0f;
		instance.lookFromLocation = new Vector3(location.x, 
		                                      location.y + ZOOM_Y_DELTA, 
		                                      location.z + ZOOM_Z_DELTA);
		instance.lookAtLocation = location;

	}
	
}
