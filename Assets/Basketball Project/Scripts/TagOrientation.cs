using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TagOrientation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		transform.LookAt( Camera.main.transform.position );
		transform.Rotate( new Vector3( 0, 180f,0) );

//		GetComponent<Text>().text = transform.parent.parent.GetComponent<Player>().state.ToString();


	}
}
