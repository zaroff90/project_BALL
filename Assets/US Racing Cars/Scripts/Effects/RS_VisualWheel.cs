using UnityEngine;
using System.Collections;

public class RS_VisualWheel : MonoBehaviour {

	public WheelCollider wheel;
	public GameObject BreakBlock = null;

	private Vector3 _BreakBlockPosDif;

	void Start() {
		if(BreakBlock != null) {
			BreakBlock.transform.parent = transform.parent;
			_BreakBlockPosDif =  transform.position - BreakBlock.transform.position;
		}
	}

	void FixedUpdate () {


		Vector3 position;
		Quaternion rotation;
		wheel.GetWorldPose(out position, out rotation);
		
		transform.position = position;
		transform.rotation = rotation;

		if(BreakBlock != null) {
		
			BreakBlock.transform.position = transform.position - _BreakBlockPosDif;
		}


	}
}
