using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenorator : MonoBehaviour {

	public int chunkSizeY = 16;
	public int chunkSizeX = 16;

	public float xSeed;
	public float ySeed;  

	public GameObject tile;

	// Use this for initialization
	void Start () {
		xSeed = Random.Range (-1000f, 1000f);
		ySeed = Random.Range (-1000f, 1000f);

		createChunk (new Vector2(0, 0));
	}

	public List<Vector2> chunkList;

	void createChunk (Vector2 pos) {
		for (int xPos = 0; xPos < chunkSizeX; xPos++) {  //xTiles
			for (int yPos = 0; yPos < chunkSizeY; yPos++) {  //yTiles
				float tileHeight = Mathf.RoundToInt(10 * Mathf.PerlinNoise(xSeed + ((float)xPos / 10f), ySeed + ((float)yPos / 10f)));  //calculate height
				Instantiate(tile, new Vector3((float)xPos + pos.x * chunkSizeX, tileHeight, (float)yPos + pos.y * chunkSizeY), Quaternion.identity);
			}
		}
	}

	void destroyChunk (Vector2 pos) {
		chunkList.Remove (pos);
	}
}
