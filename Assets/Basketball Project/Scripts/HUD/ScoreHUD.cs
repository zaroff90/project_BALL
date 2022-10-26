using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreHUD : MonoBehaviour {

	private InGameState inGame;
	
	// Use this for initialization
	void Start () {
		
		inGame = GameObject.FindObjectOfType( typeof( InGameState ) ) as InGameState;		
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		GetComponentInChildren<Text> ().text = inGame.score_local + " - " + inGame.score_visiting;
	}
}
