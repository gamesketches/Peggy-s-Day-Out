using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterControl : MonoBehaviour {
	public Canvas uiShit;
	public float rotationSpeed;
	public float strafeSpeed;
	public float jumpRotationSpeed;
	public float jumpPower;
	public float baseDrag;
	public float maxMagnitude;
	public float turnBoost;
	bool grounded = false;
	bool dragging = false;
	Rigidbody rb;
	Collider collider;
	float spunDegrees = 0;
	int points = 0;

	// Use this for initialization
	void Start () {
		
	rb = GetComponent<Rigidbody>();
	collider = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 jumpVector = new Vector3(0.0f, jumpPower, 0.0f);
		RaycastHit hit;
		Ray landingRay = new Ray(collider.bounds.center, Vector3.down);

		if(Physics.Raycast (landingRay, out hit, 3f)){
			float horizontal = Input.GetAxis ("Horizontal");
			float vertical = Input.GetAxis ("Vertical");
			if(horizontal != 0){
				if(!dragging) {
					rb.velocity += new Vector3(0.0f, 3f, 0.0f);
					dragging = true;
				}
				float rotation = horizontal * rotationSpeed;
				float drag = vertical;
				rb.drag = baseDrag + Mathf.Abs(rotation) + drag;
				transform.Rotate(rotation, 0, 0);
				rb.velocity = Quaternion.AngleAxis(rotation, Vector3.up) * rb.velocity;
				if(Input.GetKeyUp(KeyCode.DownArrow)){
					rb.AddRelativeForce(new Vector3(0.0f, rb.velocity.x, rb.velocity.z));
					rb.drag = baseDrag;
					dragging = false;
					Input.ResetInputAxes();
				}
			}
			else {
				if(horizontal != 0) {
					rb.AddForce(new Vector3(0f, 0f, horizontal * strafeSpeed));
		     	}
			}
			if(vertical != 0) {
				rb.drag = vertical;
			}
			if(Input.GetButton("Jump")) {
				rb.velocity += jumpVector;
				grounded = false;
			}
			if(rb.velocity.magnitude > maxMagnitude) {
				rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxMagnitude);
			}
		}
		// jumping code
		else {
			float rotation = Input.GetAxis ("Horizontal") * jumpRotationSpeed;
			transform.Rotate (rotation, 0, 0);
			rb.drag = baseDrag;
			spunDegrees += rotation;
			if((int)Mathf.Abs(spunDegrees) / 360 > 0){
				Debug.Log(360 * (int)(Mathf.Abs(spunDegrees) / 360));
			}
			GetComponent<TrailRenderer>().enabled = false;
		}
		if(rb.position.x < 100) {
			uiShit.GetComponentsInChildren<Text>()[1].text = "You win!";
		}
		else if(rb.position.y < -10) {
			uiShit.GetComponentsInChildren<Text>()[1].text = "You Lose :(";
		}
	}

	void OnCollisionEnter (Collision collision)
	{
		if(collision.gameObject.tag == "Ground"){
			grounded = true;
			spunDegrees = 0;
			GetComponent<TrailRenderer>().enabled = true;
		}
		else if(collision.gameObject.tag == "Finish"){
			Debug.Log("You lose");
		}
		else if(collision.gameObject.tag == "Coin"){
			Debug.Log("lol");
		}
	}
	public void getCoins() {
		points += 100;
		uiShit.GetComponentInChildren<Text>().text = points.ToString();
	}

	void OnTriggerEnter(Collider collision) {
		this.getCoins();
		Destroy(collision.gameObject);
	}


}
