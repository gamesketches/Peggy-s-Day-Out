using UnityEngine;
using System.Collections;

public class playerControlScript : MonoBehaviour {

	public float gravityZ;
	public float gravityY;
	Rigidbody rb;
	Vector3 velocity;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		rb.AddForce(new Vector3(rb.velocity.x + (gravityZ * Time.deltaTime), 
		                      (rb.velocity.y + (gravityY * Time.deltaTime)) * -1,
		                        0f));
	}
}
