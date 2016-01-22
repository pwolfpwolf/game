using UnityEngine;
using System.Collections;

public class ServerObject  {

	public enum ServerObjectType 
	{
		Castle,
		Army
	} 

	protected float x;
	protected float y;
	protected string id;
	private ServerObjectType type;
	private int playerId;
	private int turnNumber;
	private int instanceId;

	private GameObject gameObject;


	public ServerObject (float x, float y, string id, ServerObjectType type, int playerId)
	{
		this.x = x;
		this.y = y;
		this.id = id;
		this.type = type;
		this.playerId = playerId;
	}

	// this is used for a quick list of armies in a castle
	public ServerObject (string id) {
		this.id = id;
	}

	public GameObject GameObject {
		get {
				return gameObject;
		}
		set {
				gameObject = value;
		}
	}

	public int TurnNumber {
		get {
			return turnNumber;
		}
		set {
			turnNumber = value;
		}
	}
	
	public float X {
		get {
			return this.x;
		}
	}

	public float Y {
		get {
			return this.y;
		}
	}

	public string ObjectId {
		get {
			return this.id;
		}
	}

	public ServerObjectType Type {
		get {
			return this.type;
		}
	}		

	public int PlayerId {
		get {
			return this.playerId;
		}
	}

	public int InstanceId {
		get {
			return turnNumber;
		}
		set {
			turnNumber = value;
		}
	}
}
