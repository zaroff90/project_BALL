using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Xml;
using System.IO;


// main class in game handling all states
public class InGameState : MonoBehaviour {
	
	public enum GameState {
		PLAYING,
		PREPARE_TO_KICK_OFF,
		SCORED,
		THROW_IN,
		THROW_IN_CHASING,
		THROW_IN_DOING,
		THROW_IN_DONE,
	};
	
	
	public GameState state;
	public bool scoredbyPlayer = false;
	public bool scoredbyOponent = true;
	private GameObject[] players;
	private GameObject[] oponents;
	private GameObject keeper;
	private GameObject keeper_oponent;
	public GameObject lastTouched;
	public float timeToChangeState = 0.0f;
	public Vector3 positionSide;
	public Sphere sphere;
	public Vector3 target_throw_in;
	private GameObject whoLastTouched;
	public GameObject candidateToThrowIn;
	private float timeToSaqueOponent = 1.0f;
	[HideInInspector]
	public bool baseLine = false;	
	private float timeToKickOff = 1.0f;
	public GameObject lastCandidate = null;
	public int score_local = 0;
	public int score_visiting = 0;
	public Transform target_oponent_goal;
	public ScorerTimeHUD scorerTime;
	public Material localMaterial;
	public Material visitMaterial;
	public Transform throwPlayer;
	public Transform throwOponent;
	public Transform center;
	private GameObject candidateToGetPass;


	// Use this for initialization
	void Start () {

		// search Players and Oponents
		players = GameObject.FindGameObjectsWithTag("PlayerTeam");
		oponents = GameObject.FindGameObjectsWithTag("OponentTeam");
	}

	public bool HasOponents() {

		return oponents.Length > 0?true:false;
	}
	

	float timeToSideLinePass = 0.0f;
	// Update is called once per frame
	void Update () {
	
	
		// little time between states
		timeToChangeState -= Time.deltaTime;
		
		if ( timeToChangeState < 0.0f ) {
		

			// Handle all states related to match
			switch (state) {
				
			case GameState.PLAYING:

				if (scorerTime && scorerTime.minutes > 39.0f ) {
					SceneManager.LoadScene( "Basket_Match",LoadSceneMode.Single);
				}
				

				break;
	
				case GameState.THROW_IN:
				
					whoLastTouched = lastTouched;

					foreach ( GameObject go in players ) {
						go.GetComponent<Player>().state = Player.Player_State.RESTING;
					}
					foreach ( GameObject go in oponents ) {
						go.GetComponent<Player>().state = Player.Player_State.RESTING;
					}

					sphere.owner = null;
		
					if ( whoLastTouched.tag == "PlayerTeam" )
						candidateToThrowIn = SearchPlayerNearBall( oponents );
					else	
						candidateToThrowIn = SearchPlayerNearBall( players );
						
					candidateToThrowIn.transform.position = new Vector3( positionSide.x, candidateToThrowIn.transform.position.y, positionSide.z);
				
					if ( whoLastTouched.tag == "PlayerTeam" ) {
						candidateToThrowIn.GetComponent<Player>().temporallyUnselectable = true;
						candidateToThrowIn.GetComponent<Player>().timeToBeSelectable = 20.0f;
						candidateToThrowIn.transform.LookAt( SearchPlayerNearBall( oponents ).transform.position);
					}
					else {
						candidateToThrowIn.GetComponent<Player>().temporallyUnselectable = true;
						candidateToThrowIn.GetComponent<Player>().timeToBeSelectable = 20.0f;
						candidateToThrowIn.transform.LookAt( center ); 
					}
				
					candidateToThrowIn.transform.Rotate(0, sphere.fHorizontal*10.0f, 0);
					candidateToThrowIn.GetComponent<Player>().state = Player.Player_State.THROW_IN;
					sphere.GetComponent<Rigidbody>().isKinematic = true;
					sphere.gameObject.transform.position = candidateToThrowIn.GetComponent<Player>().nodeAnimationBall.position;
					target_throw_in = candidateToThrowIn.transform.position + candidateToThrowIn.transform.forward;
					candidateToThrowIn.GetComponent<Animation>()["pass_sideline"].wrapMode = WrapMode.Loop;
					candidateToThrowIn.GetComponent<Animation>()["pass_sideline"].speed = 0.0f;
					candidateToThrowIn.GetComponent<Animation>().Play("pass_sideline");
					state = GameState.THROW_IN_CHASING;
				
				break;

				case GameState.THROW_IN_CHASING:

					candidateToThrowIn.transform.position = new Vector3( positionSide.x, candidateToThrowIn.transform.position.y, positionSide.z);
					candidateToThrowIn.transform.LookAt( target_throw_in );
					candidateToThrowIn.GetComponent<Player>().state = Player.Player_State.THROW_IN;
				
					sphere.GetComponent<Rigidbody>().isKinematic = true;
					sphere.gameObject.transform.position = candidateToThrowIn.GetComponent<Player>().nodeAnimationBall.position;

					sphere.owner = candidateToThrowIn;

					if ( whoLastTouched.tag != "PlayerTeam" ) {
				
						if ( !baseLine ) 
							target_throw_in += new Vector3( 0,0,-sphere.fHorizontal/10.0f);
						else
							target_throw_in += new Vector3( sphere.fVertical/10.0f,0,0);

						candidateToGetPass = SelectPassPlayer( candidateToThrowIn.transform, this.players );


						if (sphere.bPassButton) {
							sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
							candidateToThrowIn.GetComponent<Animation>()["pass_sideline"].speed = 2.5f;
							candidateToThrowIn.GetComponent<Animation>()["pass_sideline"].wrapMode = WrapMode.Once;
							candidateToThrowIn.GetComponent<Animation>().Play("pass_sideline");
							StartCoroutine( DisableColliderDuringTime( candidateToThrowIn, 2.0f ) );
							state = GameState.THROW_IN_DOING;
		
						}
						
					} else {
					
						timeToSaqueOponent -= Time.deltaTime;

						candidateToGetPass = SelectPassPlayer( candidateToThrowIn.transform, this.oponents );

						if ( timeToSaqueOponent < 0.0f ) {					
							timeToSaqueOponent = 1.0f;
							sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
							candidateToThrowIn.GetComponent<Animation>()["pass_sideline"].speed = 2.5f;
							candidateToThrowIn.GetComponent<Animation>()["pass_sideline"].wrapMode = WrapMode.Once;
							candidateToThrowIn.GetComponent<Animation>().Play("pass_sideline");
							StartCoroutine( DisableColliderDuringTime( candidateToThrowIn, 2.0f ) );
							state = GameState.THROW_IN_DOING;
						}
					
					}
				
				break;	
				
				case GameState.THROW_IN_DOING:
				
					timeToSideLinePass += Time.deltaTime;
			
					if ( timeToSideLinePass > 0.1f ) {

						timeToSideLinePass = 0.0f;
						Vector3 directionBall = (candidateToGetPass.transform.position - candidateToThrowIn.transform.position).normalized;
						float distanceBall = (candidateToGetPass.transform.position - candidateToThrowIn.transform.position).magnitude*1.4f;
						distanceBall = Mathf.Clamp( distanceBall, 5.0f, 40.0f );
						sphere.GetComponent<Rigidbody>().isKinematic = false;
						sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(directionBall.x*distanceBall, distanceBall/4.0f, directionBall.z*distanceBall );

						state = GameState.THROW_IN_DONE;
					}
				
				
				break;

				case GameState.THROW_IN_DONE:
					sphere.owner = null;
					whoLastTouched = candidateToThrowIn;
					candidateToThrowIn.GetComponent<Player>().timeToBeSelectable= 2.0f;
					candidateToThrowIn.GetComponent<Player>().temporallyUnselectable = false;
					candidateToThrowIn.GetComponent<Player>().state = Player.Player_State.MOVE_AUTOMATIC;
					state = GameState.PLAYING;
				
				break;

			case GameState.SCORED:

				foreach ( GameObject go in players ) {
					if ( go.GetComponent<Player>().state != Player.Player_State.RESTING )
						go.GetComponent<Player>().state = Player.Player_State.GO_ORIGIN;
				}
				foreach ( GameObject go in oponents ) {
					if ( go.GetComponent<Player>().state != Player.Player_State.RESTING )
						go.GetComponent<Player>().state = Player.Player_State.GO_ORIGIN;
				}

				timeToKickOff -= Time.deltaTime;
				
				if ( timeToKickOff < 0.0f ) {
					timeToKickOff = 3.0f;
					state = InGameState.GameState.PREPARE_TO_KICK_OFF;
				}
				
				
			break;



			case GameState.PREPARE_TO_KICK_OFF:
	
				if ( scoredbyOponent ) {
					lastTouched = oponents[0];
					Camera.main.GetComponent<InGameState>().positionSide = throwPlayer.position;
					Camera.main.GetComponent<InGameState>().baseLine = true;
					this.state = GameState.THROW_IN;

				} else if ( scoredbyPlayer ) {
					lastTouched = players[0];
					Camera.main.GetComponent<InGameState>().positionSide = throwOponent.position;
					Camera.main.GetComponent<InGameState>().baseLine = true;
					this.state = GameState.THROW_IN;
				}

				break;

			}
		
		}
		
	}

	IEnumerator DisableColliderDuringTime( GameObject _player, float _time ) {
		_player.GetComponent<CapsuleCollider>().enabled = false;
		yield return new WaitForSeconds( _time );
		_player.GetComponent<CapsuleCollider>().enabled = true;
	}

	// Search player more close to the ball
	GameObject SearchPlayerNearBall( GameObject[] arrayPlayers) {
		
	    GameObject candidatePlayer = null;
		float distance = 1000.0f;
		foreach ( GameObject player in arrayPlayers ) {			
			
			if ( !player.GetComponent<Player>().temporallyUnselectable ) {
				
				Vector3 relativePos = sphere.transform.InverseTransformPoint( player.transform.position );		
				float newdistance = relativePos.magnitude;
				
				if ( newdistance < distance ) {
				
					distance = newdistance;					
					candidatePlayer = player;					

				}
			}
			
		}
						
		return candidatePlayer;	
	}

	GameObject SelectPassPlayer( Transform trans, GameObject[] _players ) {
		
		sphere.GetComponent<Rigidbody>().isKinematic = false;
		sphere.owner = null;
		
		GameObject bestCandidatePlayer = null;
		float bestCandidateCoord = 1000.0f;
		

		foreach ( GameObject go in _players ) {
			
			if ( go != trans.gameObject ) {

				Vector3 relativePos = candidateToThrowIn.transform.InverseTransformPoint( new Vector3( go.transform.position.x, go.transform.position.y, go.transform.position.z  ) );
				
				float magnitude = relativePos.magnitude;
				float direction = Mathf.Abs(relativePos.x);
				
				if ( relativePos.z > 0.0f && direction < 5.0f && magnitude < 20.0f && (direction < bestCandidateCoord) ) {
					bestCandidateCoord = direction;
					bestCandidatePlayer = go;
					
				}
			}
			
		}


		if ( bestCandidateCoord == 1000.0f ) {
			if ( bestCandidatePlayer != _players[0] )
				bestCandidatePlayer = _players[0];
			else
				bestCandidatePlayer = _players[1];

		}
			
		return bestCandidatePlayer;

	}



}
