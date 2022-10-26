using UnityEngine;
using System.Collections;

// this script is asciated to a trigger to know if a player is inside or outside of this area
public class Area : MonoBehaviour {

	public Sphere sphere;

	void OnTriggerEnter( Collider other ) {

		if ( other.gameObject.GetComponent<Player>() ) {

			other.gameObject.GetComponent<Player>().inArea = true;
		}
	}
	
	void OnTriggerExit( Collider other ) {

		if ( other.gameObject.GetComponent<Player>() ) {
			other.gameObject.GetComponent<Player>().inArea = false;
		}
	}

}
