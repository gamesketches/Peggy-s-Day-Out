using UnityEngine;
using System.Collections;

public class CamerControl : MonoBehaviour {

	public GameObject player;
	public Vector3 targetOffset;
	
	private Vector3 offset;
	private Vector3 tempPosition;
	private Quaternion lastRotation; 
	private Collider collider;
	private int intro;
	private bool jump;
	private float fraction;

	// Use this for initialization
	void Start () {
		offset = targetOffset;
		jump = true;
		intro = 0;
		collider = player.GetComponent<Collider>();
		lastRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(intro > 1){
			if(player.transform.position.y > -10){
			//tempPosition = player.transform.position + offset;
			RaycastHit hit;
			Ray landingRay = new Ray(collider.bounds.center, Vector3.down);
			
			// If grounded, follow player
			if(Physics.Raycast(landingRay, out hit, 3f)) {
				lastRotation = transform.rotation;
				tempPosition = player.transform.position + offset;
				if(jump){
						jump = false;
						tempPosition = transform.position;
						Vector3 blarg = new Vector3(Mathf.PerlinNoise(transform.position.x * 1f, transform.position.x * -0.5f),//transform.position + new Vector3(0, 10, 0);
						                                              Mathf.PerlinNoise(transform.position.y * -0.5f, transform.position.y * 0.5f) * 5,
						                            0.0f);//Mathf.PerlinNoise(transform.position.y * -0.1f, transform.position.y * 0.1f) * 5);
						//transform.position.x = Mathf.PerlinNoise(transform.position.x * -1.0f, transform.position.x * 1.0f);//transform.position + new Vector3(0, 10, 0);
						//transform.position.y = Mathf.PerlinNoise(transform.position.y * -1.0f, transform.position.y * 1.0f);
						transform.position += blarg;
						fraction = 0f;
						Debug.Log (blarg);
					}
					else if(transform.position.y != tempPosition.y){
						fraction += Time.deltaTime * .5f;
						transform.position = Vector3.Lerp(transform.position, tempPosition, fraction);
					}
				else{
						transform.position = tempPosition;
					}
			   }
			// Else lock rotation
			   else {
				jump = true;
				transform.rotation = lastRotation;
				transform.position = player.transform.position + offset;
			   }
			
			//tempPosition = transform.position;
		    }
			else {
				transform.position = tempPosition;
				transform.LookAt(player.transform);
			}
	}
	else if(intro > 0) {
			transform.LookAt(player.transform);
			transform.Translate(Vector3.right * Time.deltaTime);
			Vector3 targetPosition = player.transform.position + targetOffset;
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
			Vector3 newOffset = transform.position - player.transform.position;
			if( newOffset.y >= targetOffset.y){
				intro++;
				transform.LookAt(player.transform.position);
				offset = newOffset;
			}
		}
	else {
			if(Input.anyKeyDown) {
			intro += 1;
			GetComponent<AudioSource>().Play();
			}
		}
	}

	public bool haveWeStartedYet() {
		return intro > 0;
	}
}
