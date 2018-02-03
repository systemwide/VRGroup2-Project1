﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlatform : MonoBehaviour {

	public GameObject spawnPlatform;
    private Transform spawnLocation;

	// Use this for initialization
	void Start () {
		spawnLocation = spawnPlatform.transform.Find("SpawnPlatformGraphics/SpawnLocation").transform;
	}

	// Called when something hits the game platform
	void OnCollisionEnter(Collision collision) {
		StartCoroutine(handleCollision(collision));
	}

	// Determines what hit the floor, and then performs the appropriate action
	private IEnumerator handleCollision(Collision collision) {
		// Log what collided
		Debug.Log("A " + collision.transform.name + " hit the floor.");
		
		// Determine if the collider is an actionableobject
		Actionable actionableObj = collision.gameObject.GetComponent<Actionable>();
		if (actionableObj != null) {
			Debug.Log(collision.transform.name + " is actionable!");
			// Retrieve its collision handler and execute it
			IEnumerator collisionHandler = actionableObj.HandleCollision(collision, spawnLocation).GetEnumerator();
			while (collisionHandler.MoveNext()) {
				yield return collisionHandler.Current;
			}
		}

		yield return null;
	}
}
