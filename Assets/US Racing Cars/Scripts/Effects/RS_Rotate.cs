using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RS_Rotate : MonoBehaviour {
	public Vector3 v;
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(v * Time.deltaTime);
	}
}
