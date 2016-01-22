using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseResponses : MonoBehaviour, IScrollHandler  {

	public void OnScroll (PointerEventData eventData)
	{
		Debug.Log ("Scroll");


	}



}
