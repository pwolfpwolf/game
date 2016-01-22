using UnityEngine;
using System.Collections;

/**
 * parses a string from the server into the a 2 dimensitional array
 */

public class TerrainMap  {

	public int x_scale;
	public int y_scale;
	
	public int[,] terrain;
	
	public TerrainMap(int x_scale, int y_scale, string terrainString) {
		
		this.x_scale = x_scale;
		this.y_scale = y_scale;
		
		terrain = new int[x_scale,y_scale];
		
		for (int x=0; x < x_scale; x++) {
			for (int y=0; y < y_scale; y++) {
				terrain[x,y] = terrainString[x + (y * y_scale)] - 48;
			}
		}
	}
}

