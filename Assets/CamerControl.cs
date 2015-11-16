﻿using UnityEngine;
using System.Collections;

public class CamerControl : MonoBehaviour {

	public GameObject player;
	
	private Vector3 offset;
	private Quaternion lastRotation; 
	private Collider collider;

	// Use this for initialization
	void Start () {
		offset = transform.position - player.transform.position;

		collider = player.GetComponent<Collider>();
		lastRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = player.transform.position + offset;
		RaycastHit hit;
		Ray landingRay = new Ray(collider.bounds.center, Vector3.down);
		// If grounded, follow player
		if(Physics.Raycast(landingRay, out hit, 3f)) {
			lastRotation = transform.rotation;
		}
		// Else lock rotation
		else {
			transform.rotation = lastRotation;
		}
	}
}
