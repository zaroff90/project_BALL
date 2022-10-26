using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
public class Player : MonoBehaviour {


	// player name
	public string Name;
	public TypePlayer type = TypePlayer.CENTER;
	public float Speed = 1.0f;	
	public float Strong = 1.0f;
	public float Control = 1.0f;
			
	public enum TypePlayer {
			POINTGUARD,
			SHOOTINGGUARD,
			SMALLFORWARD,
			POWERFORWARD,
			CENTER
		};
		
	public Vector3 actualVelocityPlayer;
	public Sphere sphere;
	private GameObject[] players;
	private GameObject[] oponents;
	public Vector3 resetPosition;
	private Vector3 forwardReset;
	public Vector3 currentPosition;
	private float inputSteer;
	public Transform dunkTransformation;
	private Transform headTransform;	
	[HideInInspector]	
	public bool temporallyUnselectable = true;
	[HideInInspector]	
	public float timeToBeSelectable = 1.0f;	
	[HideInInspector]
	public bool inArea = false;
	public float maxDistanceFromPosition = 20.0f;
	public Transform nodeAnimationBall;
		
	public enum Player_State { 
		   PREPARE_TO_KICK_OFF,
		   RESTING,
		   GO_ORIGIN,
		   CONTROLLING,
		   PASSING,
		   SHOOTING,
		   MOVE_AUTOMATIC,
		   ONE_STEP_BACK,
		   STOLE_BALL,
		   OPONENT_ATTACK,
		   PICK_BALL,
		   THROW_IN,
		   THROW_INIT,
		   WAIT_FOR_PASS
		  };
	   
	public Player_State state;
	private float timeToRemove = 3.0f;	
	private float timeToPass = 1.0f;		
	public InGameState inGame;		
	private Quaternion initialRotation;			
	private float timeResting = 0.0f;
	private float periodTimeResting = 2.0f;
	private float timeToWaitPass = 0.0f;
	private float periodTimeToWaitPass = 2.0f;
	private float periodTimeToRepos = 1.0f;
	private float timeToRePos = 0.0f;
	private float timeToPause = 0.0f;
	private float periodTimeToPause = 1.0f;
	private float timeToResume = 0.0f;
	private float periodTimeToResume = 2.0f;

	private CapsuleCollider capsuleCollider;
	private float radiusCapsule;

	private Shoot_Script shootScript;

	void  Awake () {
			
		GetComponent<Animation>().Play ("idle");			
		GetComponent<Animation>()["walk"].speed = 3.0f;
		GetComponent<Animation>()["walk_control_ball"].speed = 3.0f;
		GetComponent<Animation>()["idle_ball"].speed = 3.0f;
		GetComponent<Animation>()["pass"].speed = 2.5f;
		GetComponent<Animation>()["shoot"].speed = 2.5f;
		GetComponent<Animation>()["jump_with_ball"].speed = 1.6f;

	}

	private Transform SearchHierarchyForBone(Transform current, string name)   
	{
		// check if the current bone is the bone we're looking for, if so return it
		if (current.name == name)
			return current;

		// search through child bones for the bone we're looking for
		for (int i = 0; i < current.childCount; ++i)
		{
			// the recursive step; repeat the search one step deeper in the hierarchy
			Transform found = SearchHierarchyForBone(current.GetChild(i), name);
			
			// a transform was returned by the search above that is not null,
			// it must be the bone we're looking for
			if (found != null)
				return found;
		}
		
		// bone with name was not found
		return null;
	}


	void  Start (){

		shootScript = GameObject.FindObjectOfType( typeof( Shoot_Script )) as Shoot_Script;

		if ( GetComponentInChildren<MeshRenderer>() )
			GetComponentInChildren<MeshRenderer>().enabled = false;

		// get players and oponents and save it in both arrays
		players = GameObject.FindGameObjectsWithTag("PlayerTeam");
		oponents = GameObject.FindGameObjectsWithTag("OponentTeam");
		resetPosition = transform.position;
		forwardReset = transform.forward;
		headTransform =  SearchHierarchyForBone( transform, "Bip001 Head");

		initialRotation = transform.rotation * headTransform.rotation;

		capsuleCollider = GetComponent<CapsuleCollider>();
		radiusCapsule = capsuleCollider.radius;

		nodeAnimationBall.position = transform.position + new Vector3(0,1.0f,0) + transform.forward*0.5f;

	}

	// ask if someone is in front of me
	bool NoOneInFront( GameObject[] team_players ) {
		
		
		foreach( GameObject go in team_players ) {
			
			Vector3 relativePos = transform.InverseTransformPoint( go.transform.position ); 
			
			if ( relativePos.z > 0.0f )
				return true;		
		}
		
		return false;
		
	}
		
	
	// control of actual player	
	void Case_Controlling() {

		// if i have the ball
		if ( sphere.inputPlayer == gameObject ) {
					
			// if movement in input
			if ( sphere.fVertical != 0.0f || sphere.fHorizontal != 0.0f ) {
						
				Vector3 right = inGame.transform.right;
				Vector3 forward = inGame.transform.forward;
					
				right *= sphere.fHorizontal;
				forward *= sphere.fVertical;
					
				Vector3 target = transform.position + right + forward;
				target.y = transform.position.y;
							
				float speedForAnimation = 5.0f;
				
				// if this player is owner of Ball....
				if ( sphere.owner == gameObject ) {
				
					GetComponent<Animation>().Play ("walk_control_ball");

				}
				else {

					GetComponent<Animation>().Play("walk");
						
				}
					
				transform.LookAt( target );
				actualVelocityPlayer = transform.forward*speedForAnimation*Time.deltaTime*Speed;
				transform.position += actualVelocityPlayer;
									
					
			} else {
		
				// stop and bouncing ball
				if ( sphere.owner == this.gameObject ) {
					GetComponent<Animation>().Play("idle_ball");

				} else {
					GetComponent<Animation>().Play("idle");
				}
			}
				
				
			// pass
			if ( sphere.bPassButton && sphere.owner == gameObject ) {
				state = Player_State.PASSING;
				GetComponent<Animation>().Play("pass");
				timeToBeSelectable = 1.0f;
				temporallyUnselectable = true;
				sphere.bPassButton = false;


//				Time.timeScale = 0.1f;
			}
					



			if ( Vector3.Angle( transform.forward, new Vector3 (dunkTransformation.position.x, transform.position.y, dunkTransformation.position.z ) - transform.position  ) < 45.0f ) {
				shootScript.GetComponent<Image>().enabled = true;
			} else {
				shootScript.GetComponent<Image>().enabled = false;
			}



			// shoot
			if ( sphere.bShootButton && sphere.owner == gameObject) {
			
				if ( Vector3.Angle( transform.forward, new Vector3 (dunkTransformation.position.x, transform.position.y, dunkTransformation.position.z ) - transform.position  ) < 45.0f ) {

					if ( this.inArea )
						GetComponent<Animation>().Play("jump_with_ball");
					else
						GetComponent<Animation>().Play("shoot");

					timeToBeSelectable = 2.0f;
					temporallyUnselectable = true;
					state = Player_State.SHOOTING;
				}
				sphere.bShootButton = false;
			}
					
		} else {
		
			state = Player_State.MOVE_AUTOMATIC;
				
		}
			
	}



	// Oponent control
	void Case_Oponent_Attack() {

		if ( sphere.owner != this.gameObject )
			state = Player_State.MOVE_AUTOMATIC;


		timeToPause += Time.deltaTime;
		if ( timeToPause > periodTimeToPause ) {


			timeToResume += Time.deltaTime;
			if ( timeToResume > periodTimeToResume )
			{
				timeToPause = 0.0f;
				periodTimeToPause = UnityEngine.Random.Range ( 0.5f, 1.0f );
			}

		
			GetComponent<Animation>().Play("idle_ball");
			sphere.GetComponent<Rigidbody>().isKinematic = true;


		} else {

			actualVelocityPlayer = transform.forward*5.0f*Time.deltaTime;
			TurnControlPlayer( dunkTransformation.position );
			actualVelocityPlayer = transform.forward*Time.deltaTime*5.5f*Speed;
			transform.position += actualVelocityPlayer;


			timeToPass -= Time.deltaTime;
				
			if ( timeToPass < 0.0f && NoOneInFront( oponents ) ) {	
				sphere.GetComponent<Rigidbody>().isKinematic = false;
				timeToPass = UnityEngine.Random.Range( 1.0f, 5.0f);	
				state = Player_State.PASSING;
				GetComponent<Animation>().Play("pass");
				timeToBeSelectable = 2.0f;
				temporallyUnselectable = true;
			} else {

				GetComponent<Animation>().Play("walk_control_ball");
				sphere.GetComponent<Rigidbody>().isKinematic = true;
			
			}
				
			float distance = (dunkTransformation.position - transform.position).magnitude;
			Vector3 relative = transform.InverseTransformPoint(dunkTransformation.position);
				
			if ( distance < 8.0f && relative.z > 0 ) {


				if ( Vector3.Angle( transform.forward, new Vector3 (dunkTransformation.position.x, transform.position.y, dunkTransformation.position.z ) - transform.position  ) < 45.0f ) {

					sphere.GetComponent<Rigidbody>().isKinematic = false;

					state = Player_State.SHOOTING;

					if ( this.inArea )
						GetComponent<Animation>().Play("jump_with_ball");
					else
						GetComponent<Animation>().Play("shoot");

					timeToBeSelectable = 2.0f;
					temporallyUnselectable = true;
				}
					
			}
		}
			
	}



	void LateUpdate() {



		if ( inGame.state == InGameState.GameState.PLAYING && !temporallyUnselectable) {
			// if someone from my team has ball then...
			if ( (sphere.owner && sphere.owner.tag == this.gameObject.tag) || (inGame.lastTouched && inGame.lastTouched.tag == this.gameObject.tag) ) {

				timeToRePos += Time.deltaTime;

				if ( timeToRePos > periodTimeToRepos ) {
					periodTimeToRepos = UnityEngine.Random.Range( 1.0f, 2.0f);
					timeToRePos = 0.0f;

					float zdirection = inGame.lastTouched.transform.forward.z;

					if ( forwardReset.z < 0.0f ) {
						zdirection = Mathf.Clamp ( zdirection, -1.0f, -0.5f );
					} else {
						zdirection = Mathf.Clamp ( zdirection, 0.5f, 1.0f );
					}

					currentPosition.z = inGame.lastTouched.transform.position.z + ( zdirection * UnityEngine.Random.Range (1.0f, 5.0f) )  ;
				}

			}  else {
				
				currentPosition = resetPosition;
			}

		}
		// turn head if necesary
		Vector3 relativePos = transform.InverseTransformPoint( sphere.gameObject.transform.position );
		
		if ( relativePos.z > 0.0f ) {
	
			Quaternion lookRotation = Quaternion.LookRotation (sphere.transform.position + new Vector3(0, 1.0f,0) - headTransform.position);
			headTransform.rotation = lookRotation * initialRotation ;			
			headTransform.eulerAngles = new Vector3( headTransform.eulerAngles.x, headTransform.eulerAngles.y, -90.0f);
			
		}




				
	}
	
	void  Update() {
					
		if ( sphere.owner == this.gameObject ) {
			
			if ( GetComponent<Animation>().IsPlaying("idle_ball") || GetComponent<Animation>().IsPlaying("walk_control_ball") || GetComponent<Animation>().IsPlaying("pass") || GetComponent<Animation>().IsPlaying("pass_sideline") || GetComponent<Animation>().IsPlaying("shoot") || GetComponent<Animation>().IsPlaying("jump_with_ball") ) {
				
				sphere.transform.position = nodeAnimationBall.position;
				sphere.GetComponent<Rigidbody>().isKinematic = true;
				
			} else {
				nodeAnimationBall.position = transform.position + new Vector3(0,1.0f,0) + transform.forward*0.5f;
				sphere.GetComponent<Rigidbody>().isKinematic = false;

			}
		}


		switch ( state ) {

			case Player_State.THROW_IN:
				
			break;

			case Player_State.THROW_INIT:
				
			break;
				
	 		case Player_State.CONTROLLING:
				if ( gameObject.tag == "PlayerTeam" )
					Case_Controlling();
			break;

			case Player_State.OPONENT_ATTACK:
				if ( gameObject.tag == "OponentTeam" ) 
					Case_Oponent_Attack();			
			break;
				
				
			case Player_State.PICK_BALL:

				transform.position += transform.forward * Time.deltaTime * 5.0f;
				if (GetComponent<Animation>().IsPlaying("fight") == false) {
					
					if ( gameObject.tag == "OponentTeam" )
						state = Player_State.OPONENT_ATTACK;
					else if ( gameObject.tag == "PlayerTeam" )
						state = Player_State.MOVE_AUTOMATIC;
						
				}

			break;
				

			case Player_State.SHOOTING:
				
				if (GetComponent<Animation>().IsPlaying("shoot") == false && GetComponent<Animation>().IsPlaying("jump_with_ball") == false ) {
					sphere.GetComponent<Sphere>().shootButtonEnded = false;
					state = Player_State.MOVE_AUTOMATIC;
				}

				if ( sphere.owner ) {
					sphere.GetComponent<Rigidbody>().isKinematic = true;
					sphere.transform.position = nodeAnimationBall.position;
				}

			
			break;



		case Player_State.WAIT_FOR_PASS:

			temporallyUnselectable = false;

			timeToWaitPass += Time.deltaTime;

			GetComponent<Animation>().Play ("idle");

			if ( timeToWaitPass > periodTimeToWaitPass ) {
				timeToWaitPass = 0.0f;
				state = Player_State.RESTING;
			}

			transform.LookAt( new Vector3( sphere.transform.position.x, transform.position.y, sphere.transform.position.z)  );

			break;
			
		case Player_State.PASSING:
								
				break;
	 	
		case Player_State.GO_ORIGIN:
				
				GetComponent<Animation>().Play("walk");
				// now we just find the relative position of the waypoint from the car transform,
				// that way we can determine how far to the left and right the waypoint is.
				Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3( 
															resetPosition.x, 
															resetPosition.y, 
															resetPosition.z ) );
		
				TurnControlPlayer( resetPosition );

				transform.position += transform.forward*5.0f*Time.deltaTime*Speed;		//	transform.position += transform.forward*3.0f*Time.deltaTime;

				if ( RelativeWaypointPosition.magnitude < 0.5f ) {
					state = Player_State.RESTING;
				}
					
	 							
			break;

			case Player_State.MOVE_AUTOMATIC:
				
				timeToRemove += Time.deltaTime;				
				float distance = (transform.position - currentPosition).magnitude;
								
				// if we get out of bounds of our player we come back to initial position
				if ( distance > maxDistanceFromPosition ) {

					TurnControlPlayer( currentPosition );			
					GetComponent<Animation>().Play("walk");
					transform.position += transform.forward*4.5f*Time.deltaTime*Speed;
					
				} // if not we go to Ball...
				else {
			
					transform.LookAt( new Vector3( sphere.transform.position.x, transform.position.y, sphere.transform.position.z) );
					GetComponent<Animation>().Play ("idle");
			
				}
				
			break;

				
	 
	 		case Player_State.RESTING:

				transform.LookAt( new Vector3( sphere.GetComponent<Transform>().position.x, transform.position.y ,sphere.GetComponent<Transform>().position.z)  );
				GetComponent<Animation>().Play("idle");

				timeResting += Time.deltaTime;
				if ( timeResting > periodTimeResting ) {
					timeResting = 0.0f;
					periodTimeResting = UnityEngine.Random.Range ( 1.0f, 4.0f );
				}
		
	 		
	 		break;
				
				
			case Player_State.ONE_STEP_BACK:
			
				if (GetComponent<Animation>().IsPlaying("jump_backwards_bucle") == false)
					state = Player_State.MOVE_AUTOMATIC;

				transform.position -= transform.forward*Time.deltaTime*4.0f;	
				
			break;
				
				
			case Player_State.STOLE_BALL:
				
				GetComponent<Animation>().Play("walk");
				TurnControlPlayer( sphere.transform.position);
				transform.position += transform.forward*4.5f*Time.deltaTime*Speed;
				
				
			break;

						
		};

			
		// after pass or shoot player get in a Unselectable state some little time
		timeToBeSelectable -= Time.deltaTime;
				
		if ( timeToBeSelectable < 0.0f )
			temporallyUnselectable = false;
		else
			temporallyUnselectable = true;

	}
		
	
	void OnCollisionEnter( Collision coll ) {
	
		if ( coll.collider.transform.gameObject.tag == this.gameObject.tag && sphere.owner == this.gameObject) {
//			this.state = Player_State.CONTROLLING;
		}


		if ( coll.collider.transform.gameObject.tag == "Ball"  && !gameObject.GetComponent<Player>().temporallyUnselectable ) {


			if ( inGame.state == InGameState.GameState.PLAYING )
				inGame.lastTouched = gameObject;

			Vector3 relativePos = transform.InverseTransformPoint( sphere.gameObject.transform.position );
		
			// detects if player is getting ball
			if ( relativePos.y < 2.20f && relativePos.y > -2.0f && relativePos.z > -1.0f) { 
			

				GameObject ball = coll.collider.transform.gameObject;
				ball.GetComponent<Sphere>().owner = gameObject;
				coll.rigidbody.rotation = Quaternion.identity;

				if ( gameObject.tag == "OponentTeam" ) {
					state = Player.Player_State.OPONENT_ATTACK;
				} else if ( gameObject.tag == "PlayerTeam" ) {
					state = Player.Player_State.CONTROLLING;
				} 

			}

		}



		if (coll.collider.transform.gameObject.tag == "PlayerTeam" || coll.collider.transform.gameObject.tag == "OponentTeam"  ) {

			ContactPoint cp = coll.contacts[0];
			Vector3 normalTipped = new Vector3( cp.normal.x, 0.0f, cp.normal.z );
			Vector3 temp = new Vector3 (transform.position.x + capsuleCollider.center.x, cp.point.y ,transform.position.z + capsuleCollider.center.z);
			float distFromCenterToPointContact = (cp.point - temp).magnitude;	
			transform.position += normalTipped * ( radiusCapsule - distFromCenterToPointContact + 0.05f );
		
		}

		
	}

	
	void OnDrawGizmos() {
		
		
		Gizmos.color = new Color( 1.0f, 1.0f, 1.0f, 0.3f );
		Gizmos.DrawSphere( currentPosition, maxDistanceFromPosition  );
		
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine( transform.position + Vector3.one, currentPosition );
		
	}


	void _EventEndPass() {


//		sphere.passButtonEnded = false;
		state = Player_State.RESTING;

	}

	void _EventThrow() {

		sphere.GetComponent<Rigidbody>().isKinematic = false;

		if ( sphere.owner == this.gameObject && inGame.state == InGameState.GameState.PLAYING) {
			sphere.owner = null;

			if ( gameObject.tag == "PlayerTeam" || gameObject.tag == "OponentTeam" ) {

				Vector3 dir = ( dunkTransformation.position - sphere.transform.position).normalized;
				float dist = ( dunkTransformation.position - sphere.transform.position).magnitude;

				sphere.distThrowed = dist;


				if ( dist < 3.0f )
					sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(dir.x*dist*1.8f, inArea?3.5f:5.5f, dir.z*dist*1.8f );
				else if ( dist > 3.0f && dist < 6.0f )
					sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(dir.x*dist*1.4f, inArea?4.0f:6.0f, dir.z*dist*1.4f );
				else if ( dist > 6.0f )
					sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(dir.x*dist*1.2f, inArea?5.0f:7.0f, dir.z*dist*1.2f );
			
			}

			
		}

	
	}




	void _EventPass() {


		sphere.GetComponent<Rigidbody>().isKinematic = false;

		if ( sphere.owner == this.gameObject) {

			sphere.owner = null;


			GameObject bestCandidatePlayer = null;
			float bestCandidateCoord = 1000.0f;
			
			if ( gameObject.tag == "PlayerTeam" ) {
				
				foreach ( GameObject go in players ) {
					
					if ( go != gameObject ) {
						Vector3 relativePos = transform.InverseTransformPoint( new Vector3( go.transform.position.x, go.transform.position.y, go.transform.position.z  ) );
						
						float magnitude = relativePos.magnitude;
						float direction = Mathf.Abs(relativePos.x);
						
						if ( relativePos.z > 0.0f && direction < 5.0f && magnitude < 15.0f && (direction < bestCandidateCoord) ) {
							bestCandidateCoord = direction;
							bestCandidatePlayer = go;
							
						}
					}
					
				}
				
			} else if ( gameObject.tag == "OponentTeam" ) {
				
				foreach ( GameObject go in oponents ) {
					
					if ( go != gameObject ) {
						Vector3 relativePos = transform.InverseTransformPoint( new Vector3( go.transform.position.x, go.transform.position.y, go.transform.position.z  ) );
						
						float magnitude = relativePos.magnitude;
						float direction = Mathf.Abs(relativePos.x);
						
						if ( relativePos.z > 0.0f && direction < 5.0f && magnitude < 15.0f && (direction < bestCandidateCoord) ) {
							bestCandidateCoord = direction;
							bestCandidatePlayer = go;		
						}
						
					}
					
				}
				
			}

			if ( state == Player_State.THROW_IN ) {
				bestCandidateCoord = 1000.0f;
			}


			if ( bestCandidateCoord != 1000.0f ) {

				bestCandidatePlayer.GetComponent<Player>().state = Player_State.WAIT_FOR_PASS;

				if ( gameObject.tag == "PlayerTeam")
					sphere.inputPlayer = bestCandidatePlayer;
			
				Vector3 directionBall = (bestCandidatePlayer.transform.position - sphere.transform.position).normalized;
				float distanceBall = (bestCandidatePlayer.transform.position - sphere.transform.position).magnitude*1.8f;
				distanceBall = Mathf.Clamp( distanceBall, 10.0f, 40.0f );
				sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(directionBall.x*distanceBall, distanceBall/5.0f, directionBall.z*distanceBall );
				
			} else {
				// if not found a candidate just throw the ball forward....
				sphere.gameObject.GetComponent<Rigidbody>().velocity = transform.forward*10.0f;
				
			}
			
		}
		
	}

	void TurnControlPlayer (Vector3 target)
	{
		float angle = Vector3.Angle (target - transform.position, transform.forward) / 8.0f;
		Vector3 relativePosDir = transform.InverseTransformPoint (target);
		
		if (relativePosDir.x > 0.01f) {
			transform.Rotate (0.0f, angle, 0.0f);
		} else if ( relativePosDir.x < 0.01f ){
			transform.Rotate (0.0f, -angle, 0.0f);
		}
	}
	

	}
	
