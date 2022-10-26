using UnityEngine;
using System.Collections;


// just a simple camera to handle smotth camera movements and rotations
public class Camera_Game : MonoBehaviour {
	
	private Vector3 target;	
	public Vector3 targetOffsetPos;
	private Vector3 oldPos;	
	private Vector3 oldTarget;
	public Sphere sphere;
	
	// Use this for initialization
	void Start () {

		target = sphere.transform.position;
	}
	
	// Behaviour of camera to follow the ball or player depending
	void Update () {
	
		oldPos = transform.position;

		Vector3 newPos = Vector3.zero;
		if ( !sphere.owner  ) {
			newPos = new Vector3( target.x+targetOffsetPos.x, target.y+targetOffsetPos.y, target.z+targetOffsetPos.z );
			target = sphere.transform.position;
		} else {
			newPos = new Vector3( sphere.owner.transform.position.x+targetOffsetPos.x, sphere.owner.transform.position.y+targetOffsetPos.y, sphere.owner.transform.position.z+targetOffsetPos.z );
			target = sphere.owner.transform.position;
		}

		Vector3 tempTarget = Vector3.Lerp ( oldTarget, target, 0.04f );
		transform.position = Vector3.Lerp ( oldPos, newPos, 0.5f );
		transform.LookAt(  tempTarget );
		oldTarget = tempTarget;

	}
}
