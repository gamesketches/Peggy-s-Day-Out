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
	Text scoreText;
	Text flavorText;
	public Transform target;
	private Quaternion _lookRotation;
	private Vector3 _direction;

	// Use this for initialization
	void Start () {
		
	rb = GetComponent<Rigidbody>();
	collider = GetComponent<Collider>();
		_direction = transform.up;
		_lookRotation = transform.rotation;
	scoreText = uiShit.GetComponentInChildren<Text>();
	flavorText = uiShit.GetComponentsInChildren<Text>()[1];
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 jumpVector = new Vector3(0.0f, jumpPower, 0.0f);
		RaycastHit hit;
		Ray landingRay = new Ray(collider.bounds.center, Vector3.down);

		if(Physics.Raycast (landingRay, out hit, 3f)){
			if(flavorText.text != "You win!" && flavorText.text != "You Lose :("){
				flavorText.text = "";
			}
			Debug.Log(transform.up);
			//Debug.Break();
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
				Debug.Log("prelerp");
				Debug.Log(transform.up);
				Debug.Log (_direction);
				//Debug.Break();
				//transform.up = Vector3.Lerp(transform.up, _direction.normalized, Time.deltaTime);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, Time.deltaTime * 30);
				Debug.Log("postLerp");
				Debug.Log (transform.rotation);

		/*		Vector3 effectiveTarget = new Vector3(target.position.x, transform.position.y, transform.position.z);
				_direction = (effectiveTarget - transform.position).normalized;
				_lookRotation = Quaternion.LookRotation(_direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation,
				                                      Time.deltaTime * rotationSpeed);*/
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
			Debug.Log ("jump is happening for some reason");
			if((int)Mathf.Abs(spunDegrees) / 360 > 0){
				int trickVal = 360 * (int)(Mathf.Abs(spunDegrees) / 360);
				if(flavorText.text != trickVal.ToString() && 
				     flavorText.text != "You Win!" &&
				     flavorText.text != "You Lose :("){
					Debug.Log(flavorText.text);
					Debug.Log(trickVal.ToString());
					flavorText.text = trickVal.ToString();
					getPoints(trickVal);
				}

				//uiShit.GetComponentsInChildren<Text>()[1].text = trickVal.ToString();
				Debug.Log(360 * (int)(Mathf.Abs(spunDegrees) / 360));
			}
			GetComponent<TrailRenderer>().enabled = false;
		}
		if(rb.position.x < 100) {
			flavorText.text = "You Win!";
		}
		else if(rb.position.y < -10) {
			flavorText.text = "You Lose :(";
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
	public void getPoints(int pointsToAdd) {
		points += pointsToAdd;
		scoreText.text = points.ToString();
		//uiShit.GetComponentInChildren<Text>().text = points.ToString();
	}

	void OnTriggerEnter(Collider collision) {
		this.getPoints(100);
		Destroy(collision.gameObject);
	}


}
