﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBird : Actionable
{
	Rigidbody rb;

	public GameObject PoofSound;
	//public GameObject Poof;
	public float explosionForce = 300.0f;
	public float explosionRadius = 10.0f;
	public float upForce = 1.0f;
	public bool hasCollided = false;
	private bool hasExploded = false;
	private Vector3 explosionPosition;
	

	// Use this for initialization
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.maxAngularVelocity = Mathf.Infinity;
	}

	void OnBecameInvisible() {
		Destroy(gameObject);
	}

	void detonate()
	{
		Debug.Log("Detonation!");
		explosionPosition = transform.position; //center of the explosion originates from transform.position
		Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
		foreach (Collider hit in colliders)
		{
			Debug.Log("rigidbody " + hit.transform + " registered");
			Rigidbody r = hit.GetComponentInParent<Rigidbody>(); //rigidbody contained in parent of collider
			if (hit.name == "PigGraphics" || hit.name == "WoodPlankGraphics")
			{
				//only add explosion force to the pig and plank
				if (r != null)
				{
					Debug.Log("applied explosion to " + r.transform);
					r.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upForce, ForceMode.Impulse);
				}
			}
		}
		hasExploded = true;
	}

	public void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Bomb bird has collided with " + collision.collider.name);
		if(collision.collider.name == "WoodPlankGraphics"  || collision.collider.name == "PigGraphics")
		{
			Debug.Log("Bomb bird has met detonate conditions");
			if (!hasExploded)
			{
				Debug.Log("bomb bird entering detonation");
				detonate();
			}
		}
		
	}

	public override IEnumerable HandleFloorCollision(Collision collision)
	{
		if (!hasFired) {
			GameStatus.instance.SpawnNextBird();
			GameObject.Destroy(gameObject);
		} else {
			ContactPoint contact = collision.contacts[0];
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
			Vector3 pos = contact.point + Vector3.up;

			GameObject newPoofSound = GameObject.Instantiate(PoofSound, pos, rot);
			//GameObject newPoof = GameObject.Instantiate(Poof, pos, rot);
			
			yield return new WaitForSeconds(1);

			GameObject.Destroy(newPoofSound);
			//GameObject.Destroy(newPoof);
			GameObject.Destroy(gameObject);
		}
		
		yield return null;
	}

	public override IEnumerable HandlePlankCollision(Collision c) {
		GameObject newPoofSound = GameObject.Instantiate(PoofSound);
		Vector3 direction = (c.gameObject.transform.position - newPoofSound.transform.position).normalized;
		newPoofSound.transform.position += direction;

		yield return new WaitForSeconds(1);

		GameObject.Destroy(newPoofSound);
		//GameObject.Destroy(c.collider.gameObject);

		yield return null;
	}
}

