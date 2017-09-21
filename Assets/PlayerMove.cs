using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	public float moveSpeed = 2f;

	private Rigidbody playerRB;
	public bool isJumping = false;
	public float jumpHeight = 10f;

	public GameObject cam;
	public float mouseX;
	public float mouseY;

	private void Start() {
		playerRB = GetComponent<Rigidbody>();  //gets the player rigidbody.
	}
		
	private void Update() {
		//moves player forwards or backwards
		if(Input.GetKey(KeyCode.W)) {
			gameObject.transform.TransformDirection(Vector3.forward);
			gameObject.transform.Translate(new Vector3(0, 0, moveSpeed) * Time.deltaTime);
		}
		else if(Input.GetKey(KeyCode.S)) {
			gameObject.transform.TransformDirection(Vector3.forward);
			gameObject.transform.Translate(new Vector3(0, 0, -moveSpeed) * Time.deltaTime);
		}

		//jumping can be done while moving
		if(Input.GetKey(KeyCode.Space)) {
			if(isJumping == false) {
				isJumping = true;
				playerRB.velocity += new Vector3 (0, jumpHeight, 0);
			}
		}

		//check if touching the ground
		if (isJumping == true) {
			if (GetBlockHeightAtPos(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)) == Mathf.Round((gameObject.transform.position.y - 2.3f) * 1000) / 1000) {
				HitGround ();
			}
		}

		//lets player look around
		mouseX = Input.GetAxis("Mouse X"); 
		mouseY = -Input.GetAxis("Mouse Y");

		//body changes horizontal facing direction
		Vector3 bodyRotVector = new Vector3(0, mouseX, 0);
		gameObject.transform.Rotate(bodyRotVector);

		//camera changes vertical facing direction
		Vector3 camRotVector = new Vector3(mouseY, 0, 0); 
		cam.transform.Rotate(camRotVector);
	}

	//player has touched the ground.
	private void HitGround() {
		isJumping = false;
	}

	public float GetBlockHeightAtPos(Vector2 pos) {
		Vector3 playerPosition = GameObject.Find("Player").transform.position;

		return Mathf.Floor (TileGenerator.blockHeightMod * Mathf.PerlinNoise (
			TileGenerator.xSeed + playerPosition.x / TileGenerator.chunkSize, 
			TileGenerator.ySeed + playerPosition.z / TileGenerator.chunkSize
		));
	}
}
