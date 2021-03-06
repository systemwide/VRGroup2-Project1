﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawBallisticArc : MonoBehaviour
{

	// member attributes
	
	public float timeStep = 0.02f;  // default time step (In seconds I think)
	public float timeMaximum = 10.0f;
	public LayerMask layerMask = -1;
	public GameObject impactCircle;
	public GameObject go;
	private GameObject impactCircleInstance;
	private LineRenderer lr;

	
	// Use this for initialization
	void Start()
	{
		lr = GetComponent<LineRenderer>();
		
		
	}

	// use "LateUpdate to ensure that we get the value for velocityVect.
	void LateUpdate()
	{
		Debug.Log("Drawing Ballistic Arc");
		// we know this is highly inefficient but wanted to get it working asap
		Slingshot ss = gameObject.transform.parent.GetComponent<Slingshot>() as Slingshot;
		PullZone pz = ss.GetComponentInChildren<PullZone>();
		if((ss.slingshotHand == "left" && pz.loadedObject != null) || 
			(ss.slingshotHand == "right" && pz.loadedObject != null)) {
			
			Vector3 velocityVect = ss.CalculateV();
			lr.positionCount = (int)(timeMaximum / timeStep);

			Vector3 currPos = transform.position;

			int i = 0; // index


			for (float t = 0.0f; t < timeMaximum; t += timeStep)
			{
				lr.SetPosition(i, currPos);


				// project target impact point
				RaycastHit impact;

				if (Physics.Raycast(currPos, velocityVect, out impact, velocityVect.magnitude, layerMask))
				{

					lr.positionCount = i + 2;
					lr.SetPosition(i + 1, impact.point);

					if (impactCircle != null)
					{
						if (impactCircleInstance != null)
						{
							impactCircleInstance.transform.position = impact.point;
						}
						else
						{
							impactCircleInstance = Instantiate(impactCircle, impact.point, Quaternion.identity) as GameObject;
							impactCircleInstance.transform.parent = gameObject.transform;
						}
					}
					break;

				}
				// update position
				currPos = currPos + (velocityVect * timeStep);

				// account for gravity on arc
				velocityVect += (Physics.gravity * timeStep);
				i++;
			}
		}
	}
}