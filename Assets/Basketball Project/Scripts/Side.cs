using UnityEngine;
using System.Collections;


// class attached to triggers outside of field to determine whether players or ball is outside of limits
public class Side : MonoBehaviour {
	
	public Sphere sphere;
	public bool baseLine = false;
	public Transform position;
	// Use this for initialization
	void Start () {
		
		sphere = (Sphere)GameObject.FindObjectOfType( typeof(Sphere) );		
	}
	

	void OnTriggerEnter( Collider other) {


		// Detect if Players are outside of field
		if ( (other.gameObject.tag == "PlayerTeam" || other.gameObject.tag == "OponentTeam") && Camera.main.GetComponent<InGameState>().state == InGameState.GameState.PLAYING ) {
		
			if ( other.gameObject != sphere.owner ) {
				other.gameObject.GetComponent<Player>().temporallyUnselectable = true;
				other.gameObject.GetComponent<Player>().timeToBeSelectable = 0.5f;
				other.gameObject.GetComponent<Player>().state = Player.Player_State.GO_ORIGIN;
			}
			
		}

		// Detect if Ball is outside
		if ( other.gameObject.tag == "Ball" && Camera.main.GetComponent<InGameState>().state == InGameState.GameState.PLAYING ) {
			
			sphere.owner = null;
			Camera.main.GetComponent<InGameState>().state = InGameState.GameState.THROW_IN;
			Camera.main.GetComponent<InGameState>().positionSide = sphere.gameObject.transform.position;
			Camera.main.GetComponent<InGameState>().baseLine = baseLine;

		}
		
		
		
	}
	
	
}
