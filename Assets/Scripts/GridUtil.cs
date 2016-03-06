using UnityEngine;
using System.Collections;

/**
 * This is a utility for mapping the position of game object on the plain,
 * to the backend coordinates
 * */

public class GridUtil  {

	private Plane groundPlane; // this needs to change to the uneven terrain at some point
	private static GridUtil instance;
	
	GridUtil() {
		groundPlane = new Plane(Vector3.up, Vector3.zero);
	}

	public Vector3 getMousePosition() {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		float hitDist = 0;
		groundPlane.Raycast(ray, out hitDist);
		Vector3 currClickPos = ray.GetPoint(hitDist);
		return currClickPos;
	}


	public static GridUtil getInstance()  {

		if (GridUtil.instance == null) {
			GridUtil.instance = new GridUtil();
		}

		return GridUtil.instance;
	}

}
