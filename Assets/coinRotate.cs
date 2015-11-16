using UnityEngine;
using System.Collections;

public class coinRotate : MonoBehaviour {

	public float rotationSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0, rotationSpeed, 0);
	}

	void OnCollisionEnter (Collision collision)
	{
		if(collision.gameObject.tag == "Player"){
			this.gameObject.SetActive(false);
		}
	}
}
