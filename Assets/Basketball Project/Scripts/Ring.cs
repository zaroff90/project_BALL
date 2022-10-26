using UnityEngine;
using System.Collections;

// this class is attached to the rings in field, used fo determine is Ball enter on basket or not
public class Ring : MonoBehaviour {

	private MeshCollider meshCollider;
	public Sphere sphere;
	public InGameState inGame;


	// Use this for initialization
	void Start () {

		meshCollider = GetComponent<MeshCollider>();
	}


	void OnCollisionEnter( Collision other ) {

		int result = Random.Range( 0, (int)(sphere.distThrowed/2.0f) );
		Vector3 posRelative = transform.InverseTransformPoint( sphere.transform.position );
		if ( result == 0 && posRelative.y > 0.0f ) {
			meshCollider.enabled = false;
			other.gameObject.GetComponent<Rigidbody>().velocity = -Vector3.up;
			StartCoroutine( SetRingColliderAgain() );
		}

	}



	IEnumerator SetRingColliderAgain() {

		if ( inGame.HasOponents() )
		inGame.state = InGameState.GameState.SCORED;
		sphere.transform.position = meshCollider.transform.position;
		yield return new WaitForSeconds( 1.0f );
		meshCollider.enabled = true;


		if ( this.gameObject.tag == "PlayerRing" ) {
			inGame.scoredbyPlayer = false;
			inGame.scoredbyOponent = true;

			if ( sphere.distThrowed > 7.0f )
				inGame.score_visiting += 3;
			else
				inGame.score_visiting += 2;

		} else if ( this.gameObject.tag == "OponentRing" ) {
			inGame.scoredbyPlayer = true;
			inGame.scoredbyOponent = false;

			if ( sphere.distThrowed > 7.0f )
				inGame.score_local += 3;
			else
				inGame.score_local += 2;

		}


	}
}
