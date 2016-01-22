

using UnityEngine;
using System.Collections;


public class Town : ServerObject {

	private string name;
	private int farmland;
	private int farmers;
	private int food;
	private string state;

	public Town (string name, string id, int farmland, int farmers, int food, string state, 
	             float x, float y, int playerId)
		: base(x, y, id, ServerObject.ServerObjectType.Castle, playerId)
	{
		this.name = name;
		this.id = id;
		this.farmland = farmland;
		this.farmers = farmers;
		this.food = food;
		this.state = state;
	}

	public string Name {
		get {
			return this.name;
		}
		set {
			name = value;
		}
	}

	public string Id {
		get {
			return this.id;
		}
		set {
			id = value;
		}
	}

	public int Farmland {
		get {
			return this.farmland;
		}
		set {
			farmland = value;
		}
	}

	public int Farmers {
		get {
			return this.farmers;
		}
		set {
			farmers = value;
		}
	}

	public int Food {
		get {
			return this.food;
		}
		set {
			food = value;
		}
	}

	public string State {
		get {
			return this.state;
		}
		set {
			state = value;
		}
	}


	
}
