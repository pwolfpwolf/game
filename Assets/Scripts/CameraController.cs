using UnityEngine;
using System.Collections;

/**
 * This is used for paning, zomming  etc...
 */

public class CameraController : MonoBehaviour {

	private float zoomSpeed = 10000;
	
	private Plane groundPlane;
	private bool dragging;
	private Vector3 mouseDownPos;
	private Vector3 dragVector;

	void Start () {
		groundPlane = new Plane(new Vector3(0,1,0), new Vector3(0,0,0));
	}
	
	// Update is called once per frame
	void Update () {

		// arrow keys  ///////////////////////////////////////////////////////
		float xAxisValue = Input.GetAxis("Horizontal");
    	float zAxisValue = Input.GetAxis("Vertical");
		
		this.transform.Translate(new Vector3(xAxisValue * 5, 0.0f, zAxisValue * 5));


		///// click and drag ///////////////////////////////////////////////

		float hitDist;

		if (Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (dragging) {	
				groundPlane.Raycast(ray, out hitDist);
				Vector3 currClickPos = ray.GetPoint(hitDist);
				Camera.main.transform.position += mouseDownPos - currClickPos;
			} else if (Input.GetMouseButtonDown(0)) {
				dragging = true;			
				groundPlane.Raycast(ray, out hitDist);
				mouseDownPos = ray.GetPoint(hitDist);
			}

		} 
		else {
			dragging = false;
		}


		////////// Zooming /////////////////////

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0.0f)
		{
			//Camera.main.transform.position.y += scroll*zoomSpeed;
			//Camera.main.transform.position.z += scroll*zoomSpeed;
			float zoomAmount = scroll * zoomSpeed * Time.deltaTime;
			//float newX = this.transform.position.x;// + zoomAmount;
			//float newY = this.transform.position.y;// + zoomAmount;
			//float newZ = this.transform.position.z;// + zoomAmount;

			this.transform.Translate(new Vector3(0.0f, 0.0f, zoomAmount));
		}


	}
}
