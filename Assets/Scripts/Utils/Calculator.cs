using UnityEngine;
using System.Collections;
using System;

public class Calculator 
{

	public static float CalcRotation(Vector3 currentPosition, Vector3 destPosition) {

		double slope = (currentPosition.y - destPosition.y) / (currentPosition.x - destPosition.x);

		return (float) Math.Atan(slope);

	}

}

