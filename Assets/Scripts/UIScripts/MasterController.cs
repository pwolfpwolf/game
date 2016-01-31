using UnityEngine;
using System.Collections;

public class MasterController : MonoBehaviour {
	

	public enum GUIMode { TOP_LEVEL, TOWN_LIST, TOWN_DETAIL, PLOTTING, ARMY_DETAIL, ARMY_LIST };

    private GUIMode guiMode = GUIMode.TOP_LEVEL;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setMode(GUIMode guiMode) {

        this.guiMode = guiMode;
    }
	
}
