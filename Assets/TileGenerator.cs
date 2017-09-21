using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

	public static int chunkSize = 16;

	public static int blockHeightMod = 8;

	public static float xSeed;
	public static float ySeed;

	public GameObject tile;

	private GameObject parent;

	private Vector3 playerPosition;

	public int chunkWidth = 3; //must be odd.

	// Use this for initialization

	void Start () {
		//Init random seeds.
		xSeed = Random.Range (-1000f, 1000f);
		ySeed = Random.Range (-1000f, 1000f);

		parent = GameObject.Find("ChunkHolder");

		//create the initial 9 chunks
		for (int x = 0; x <= chunkWidth - 1; x++) {
			for (int y = 0; y <= chunkWidth - 1; y++) {
				queCreateChunk (new Vector2 (x - ((chunkWidth - 1) / 2), y - ((chunkWidth - 1) / 2)));
			}
		}

		createAllChunks();

		//set pos marker
		homeChunk = new Vector2(0, 0);
	}


	public List<Vector2> chunkCreateQue;

	void queCreateChunk (Vector2 pos) {
		chunkCreateQue.Add(pos);
	}

	public List<Vector2> chunkList;
	//places the blocks in the chunk
	void createAllChunks () {
		while (chunkCreateQue.Count > 0) {
			Vector2 pos = chunkCreateQue[0];

			//creates the parent to this tile
			GameObject chunkParent = new GameObject ("chunk" + "x" + pos.x + "y" + pos.y);
			chunkParent.transform.SetParent(parent.transform);

			//create the tiles in the chunk
			for (float xPos = 0; xPos < chunkSize; xPos++) {  //xTiles
				for (float yPos = 0; yPos < chunkSize; yPos++) {  //yTiles
					//calculate height
					float tileHeight = Mathf.Floor(blockHeightMod * Mathf.PerlinNoise(
						xSeed + (pos.x) + (xPos / chunkSize), 
						ySeed + (pos.y) + (yPos / chunkSize)
					));

					Instantiate(tile, new Vector3(xPos + (pos.x * chunkSize), tileHeight, yPos + (pos.y * chunkSize)), Quaternion.identity, chunkParent.transform);
				}
			}
			chunkList.Add(pos);
			chunkCreateQue.Remove(chunkCreateQue[0]);
		}
	}


	public List<Vector2> chunkDestroyQue;

	//adds a chunk to the deletion que.
	void queDestroyChunk (Vector2 pos) {
		chunkDestroyQue.Add(pos);
	}

	//destroys all chunks and empties the que.
	void destroyAllChunks () {
		while (chunkDestroyQue.Count > 0) {
			Destroy (GameObject.Find("chunk" + "x" + chunkDestroyQue[0].x + "y" + chunkDestroyQue[0].y));
			chunkList.Remove (chunkDestroyQue[0]);
			chunkDestroyQue.Remove (chunkDestroyQue[0]);
		}
	}

	private Vector2 homeChunk;
	void Update () {
		//gets the position of the player rounded to 1 each tile
		playerPosition = GameObject.Find("Player").transform.position;
		playerPosition = new Vector3(
			Mathf.Floor(playerPosition.x / chunkSize), 
			Mathf.Floor(playerPosition.y / chunkSize), 
			Mathf.Floor(playerPosition.z / chunkSize)
		);

		//this checks if you move between chunks
		ChangeHomeChunk();

		//print (playerPosition);
	}

	//this checks if you move between chunks
	private void ChangeHomeChunk() {
		if (new Vector2(playerPosition.x, playerPosition.z) != homeChunk) {
			homeChunk = new Vector2 (playerPosition.x, playerPosition.z);

			foreach (Vector2 pos in chunkList) {
				//bothCases: chunk needs to be destroyed.
				if(Mathf.Abs(pos.x - playerPosition.x) > ((chunkWidth - 1) / 2)) { 
					queDestroyChunk(pos);
					float newChunkOffset = (pos.x - playerPosition.x) / 2;
					queCreateChunk(new Vector2(playerPosition.x - newChunkOffset, pos.y));  //position for chunk opposite the one destroyed. 
				}
				else if (Mathf.Abs(pos.y - playerPosition.z) > ((chunkWidth - 1) / 2)) {
					queDestroyChunk(pos);
					float newChunkOffset = (pos.y - playerPosition.z) / 2;
					queCreateChunk(new Vector2(pos.x, playerPosition.z - newChunkOffset));  //position for chunk opposite the one destroyed.
				}
			}
			destroyAllChunks();
			createAllChunks();
		}
	}
}
