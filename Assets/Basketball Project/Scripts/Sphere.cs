using UnityEngine;
using System.Collections;

// this class handles the input in-game ( including Dpad ), also have 2 tool functions to know players near ball.
public class Sphere : MonoBehaviour {


	public float distThrowed;	// distance from throw to basket		
	public GameObject owner;	// the player it owns the ball
	public GameObject inputPlayer;	// player selected
	public GameObject lastInputPlayer;	// last player selected
	private GameObject[] players;
	private GameObject[] oponents;
	public Transform blobPlayerSelected;
	public float timeToSelectAgain = 0.0f;
	public GameObject lastCandidatePlayer;
	
	[HideInInspector]	
	public float fHorizontal;
	[HideInInspector]	
	public float fVertical;
	[HideInInspector]	
	public bool bPassButton;
	[HideInInspector]	
	public bool bShootButton;
	[HideInInspector]	
	public bool passButtonEnded = false;
	[HideInInspector]	
	public bool shootButtonEnded = false;
	
	public Joystick_Script joystick;	
	public InGameState inGame;
	public float timeShootButtonPressed = 0.0f;


	// Use this for initialization
	void Start () {
		// get players, joystick, InGame and Blob
		players = GameObject.FindGameObjectsWithTag("PlayerTeam");		
		oponents = GameObject.FindGameObjectsWithTag("OponentTeam");
		if ( GameObject.FindGameObjectWithTag("joystick") )
			joystick = GameObject.FindGameObjectWithTag("joystick").GetComponent<Joystick_Script>();
		inGame = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InGameState>();
		blobPlayerSelected = GameObject.FindGameObjectWithTag("PlayerSelected").transform;	
	}

	
	// Update is called once per frame
	void Update () {

		
		// get input
		fVertical = Input.GetAxis("Vertical");
		fHorizontal = Input.GetAxis("Horizontal");

#if UNITY_IOS || UNITY_ANDROID

		fVertical += joystick.position.y;
		fHorizontal += joystick.position.x;
#endif

		bPassButton = false;
		bShootButton = false;
		bPassButton = Input.GetKey(KeyCode.Z) || passButtonEnded;
		bShootButton = Input.GetKey(KeyCode.X) || shootButtonEnded;

		// if the ball has owner....
		if ( owner ) {
			if ( fVertical == 0.0f && fHorizontal == 0.0f && owner.tag == "PlayerTeam" || owner.tag == "OponentTeam" ) {
				if ( gameObject.GetComponent<Rigidbody>().isKinematic == false )
					gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
				
			}
		}		
		
		
		
		if ( inGame.state == InGameState.GameState.PLAYING ) {
			ActivateNearestPlayer();
			if ( !owner || owner.tag == "PlayerTeam" )
				ActivateNearestOponent();
		}
	}

	// activate nearest oponent to ball;
	void ActivateNearestOponent() {
	
		float distance = 100000.0f;
		GameObject candidatePlayer = null;
		foreach ( GameObject oponent in oponents ) {			
			
			if ( !oponent.GetComponent<Player>().temporallyUnselectable ) {
				
				oponent.GetComponent<Player>().state = Player.Player_State.MOVE_AUTOMATIC;
				
				Vector3 relativePos = transform.InverseTransformPoint( oponent.transform.position );
				
				float newdistance = relativePos.magnitude;
				
				if ( newdistance < distance ) {
				
					distance = newdistance;
					candidatePlayer = oponent;
					
				}
			}
			
		}
		
		// set in STOLE_BALL if player found
		if ( candidatePlayer )
			candidatePlayer.GetComponent<Player>().state = Player.Player_State.STOLE_BALL;
		else if ( oponents.Length > 0 ) {
			oponents[0].GetComponent<Player>().state = Player.Player_State.STOLE_BALL;
		}
		
		
	}
	
	// activate nearest player to ball
	void ActivateNearestPlayer() {
		
		lastInputPlayer = inputPlayer;
		
		float distance = 1000000.0f;
		GameObject candidatePlayer = null;
		foreach ( GameObject player in players ) {	

			if ( !player.GetComponent<Player>().temporallyUnselectable ) {
				
				Vector3 relativePos = transform.InverseTransformPoint( player.transform.position );
				
				float newdistance = relativePos.magnitude;
				
				if ( newdistance < distance ) {
				
					distance = newdistance;
					candidatePlayer = player;
					
				}
			}
			
		}
		
		timeToSelectAgain += Time.deltaTime;
		if ( timeToSelectAgain > 0.5f ) {
			inputPlayer = candidatePlayer;
			timeToSelectAgain = 0.0f;
		} else {
			candidatePlayer = lastCandidatePlayer;
		}
		
		lastCandidatePlayer = candidatePlayer;
		
		
		if ( inputPlayer && candidatePlayer ) {
			blobPlayerSelected.transform.position = new Vector3( candidatePlayer.transform.position.x, candidatePlayer.transform.position.y+0.1f, candidatePlayer.transform.position.z);
			blobPlayerSelected.transform.LookAt( new Vector3( blobPlayerSelected.position.x + fHorizontal, blobPlayerSelected.position.y, blobPlayerSelected.position.z + fVertical  ) );
	
		
			// if player is in any of this states then just CONTROLLING
			if ( inputPlayer.GetComponent<Player>().state == Player.Player_State.MOVE_AUTOMATIC ||
			     inputPlayer.GetComponent<Player>().state == Player.Player_State.RESTING ||
			     inputPlayer.GetComponent<Player>().state == Player.Player_State.CONTROLLING 
			     
			    )
			{
				inputPlayer.GetComponent<Player>().state = Player.Player_State.CONTROLLING;
			}
		} 
	}
	
		
	void FixedUpdate() {

		if ( this.transform.position.y < 0.0f )
			this.transform.position = new Vector3( transform.position.x, 0.0f, transform.position.z );


	}
	
}
