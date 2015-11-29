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
	private Quaternion _lookRotation;
	AudioSource snowSound;
	private ParticleSystem particles;
	public ParticleSystem coinSplash;

	// Use this for initialization
	void Start () {
		
	rb = GetComponent<Rigidbody>();
	collider = GetComponent<Collider>();
	_lookRotation = transform.rotation;
	scoreText = uiShit.GetComponentInChildren<Text>();
	flavorText = uiShit.GetComponentsInChildren<Text>()[1];
	particles = GetComponentInChildren<ParticleSystem>();
	snowSound = GetComponent<AudioSource>();
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
			float horizontal = Input.GetAxis ("Horizontal");
			float vertical = Input.GetAxis ("Vertical");
			if(horizontal != 0){
				if(!dragging) {
					rb.velocity += new Vector3(3f, 0.0f, 0.0f);
					dragging = true;
					Debug.Log("we draggin");
				}
				float rotation = horizontal * rotationSpeed * -1;
				float drag = vertical;
				rb.drag = baseDrag + Mathf.Abs(rotation) + drag;
				transform.Rotate(0, rotation, 0);
				rb.velocity = Quaternion.AngleAxis(rotation, Vector3.up) * rb.velocity;
				if(Input.GetKeyUp(KeyCode.DownArrow)){
					rb.AddRelativeForce(new Vector3(0.0f, rb.velocity.x, rb.velocity.z));
					rb.drag = baseDrag;
					dragging = false;
					Input.ResetInputAxes();
				    }
			     }
			else {
			   transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, Time.deltaTime * 30);
				}
			rb.drag = vertical;
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
			snowSound.Stop();
			float rotation = Input.GetAxis ("Horizontal") * jumpRotationSpeed;
			transform.Rotate (0, 0, rotation * -1);
			rb.drag = baseDrag;
			spunDegrees += rotation;
			if((int)Mathf.Abs(spunDegrees) / 360 > 0){
				int trickVal = 360 * (int)(Mathf.Abs(spunDegrees) / 360);
				if(flavorText.text != trickVal.ToString() && 
				     flavorText.text != "You Win!" &&
				     flavorText.text != "You Lose :("){
					flavorText.text = trickVal.ToString();
					getPoints(trickVal);
				}

			}
			particles.Stop();
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
			particles.Play();
			snowSound.Play();
		}
		else if(collision.gameObject.tag == "Finish"){
			maxMagnitude = 0;
		}
		else if(collision.gameObject.tag == "Coin"){
			Debug.Log("lol");
		}
	}
	public void getPoints(int pointsToAdd) {
		points += pointsToAdd;
		scoreText.text = points.ToString();
	}

	void OnTriggerEnter(Collider collision) {
		this.getPoints(100);
		Instantiate(coinSplash, transform.position, new Quaternion(90, 0, 0, 0));
		Destroy(collision.gameObject);
	}


}
