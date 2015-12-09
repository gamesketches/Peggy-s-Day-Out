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
	bool dragging = false;
	Rigidbody rb;
	Collider collider;
	float spunDegrees = 0;
	int points = 0;
	Text scoreText;
	Text flavorText;
	private bool gameStarted;
	private Vector3 startingPosition;
	private int idleFrames;
	AudioSource snowSound;
	AudioSource chirpSound;
	AudioSource landSound;
	private ParticleSystem particles;
	public ParticleSystem coinSplash;

	// Use this for initialization
	void Start () {
		
	rb = GetComponent<Rigidbody>();
	rb.isKinematic = true;
	collider = GetComponent<Collider>();
	scoreText = uiShit.GetComponentInChildren<Text>();
	flavorText = uiShit.GetComponentsInChildren<Text>()[1];
	particles = GetComponentInChildren<ParticleSystem>();
	particles.Stop();
	snowSound = GetComponents<AudioSource>()[1];
	chirpSound = GetComponents<AudioSource>()[0];
	landSound = GetComponents<AudioSource>()[2];
	gameStarted = false;
	idleFrames = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(gameStarted){
			particles.emissionRate = 20 + rb.velocity.magnitude;
			particles.gravityModifier = (rb.velocity.magnitude / 60.0f) * -1;
			Vector3 jumpVector = new Vector3(0.0f, jumpPower, 0.0f);
			RaycastHit hit, frontHit, backHit;
			Vector3 pos = transform.position;
			float offset = 0.5f;
			Ray landingRay = new Ray(collider.bounds.center, Vector3.down);
			Debug.DrawRay(collider.bounds.center, Vector3.down);

			if(Physics.Raycast (landingRay, out hit, 3f)){
				if(flavorText.text != "You Win!" && flavorText.text != "You Lose :("){
					flavorText.text = "";
				}
				Physics.Raycast(pos - offset * transform.forward, -Vector3.up, out backHit);
				Physics.Raycast(pos + offset * transform.forward, -Vector3.up, out frontHit);
				transform.forward = frontHit.point - backHit.point;
				float horizontal = Input.GetAxis ("Horizontal");
				float vertical = Input.GetAxis ("Vertical");
				if(horizontal != 0){
					idleFrames = 0;
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
					idleFrames++;

					if(idleFrames > 10){
						Vector3 bra = Vector3.Cross(backHit.normal, Vector3.down);
						bra = Vector3.Cross(backHit.normal, bra);

						if(bra != Vector3.zero){
							transform.forward = Vector3.RotateTowards(transform.forward, bra * -1,
							                                          Time.deltaTime * rotationSpeed * -1, 1);
								}
						}
					}
				rb.drag = vertical;
				if(Input.GetButton("Jump")) {
					rb.velocity += jumpVector;
				}
				if(rb.velocity.magnitude > maxMagnitude) {
					rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxMagnitude);
				}
			}
			// jumping code
			else {
				//Debug.Log("in air");
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
						chirpSound.Play();
					}

				}
				particles.Stop();
			}
			if(rb.position.y < -10) {
				flavorText.text = "You Lose :(";
			}
			if(flavorText.text == "You Lose :(" || flavorText.text == "You Win!") {
				if(Input.anyKey){
					Application.LoadLevel(Application.loadedLevel);
				}
			}
		}
		else {
			CamerControl camera = GameObject.Find("Main Camera").GetComponent<CamerControl>();
			if(camera.haveWeStartedYet()) {
				rb.isKinematic = false;
				gameStarted = true;
				Text[] mates = uiShit.GetComponentsInChildren<Text>();
				mates[2].text = "";
				mates[3].text = "";
				scoreText.text = "0";
			}
		}
	}

	void OnCollisionEnter (Collision collision)
	{
		if(collision.gameObject.tag == "Ground"){
			spunDegrees = 0;
			particles.Play();
			if(!snowSound.isPlaying){
				snowSound.Play();
				landSound.Play();
			}
		}
		else if(collision.gameObject.tag == "Finish"){
			maxMagnitude = 0;
			flavorText.text = "You Win!";

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
