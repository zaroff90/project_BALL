using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScorerTimeHUD : MonoBehaviour {
	
	public float timeMatch = 0.0f;
	public int minutes = 0;
	public int seconds = 0;
	public float TRANSFORM_TIME = 1.0f;
	private InGameState inGame;
	
	// Use this for initialization
	void Start () {
	
		inGame = GameObject.FindObjectOfType( typeof( InGameState ) ) as InGameState;		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (inGame.state == InGameState.GameState.PLAYING) {	
			timeMatch += Time.deltaTime * TRANSFORM_TIME;
		}		

		int d = (int)(timeMatch * 100.0f);
		minutes = d / (60 * 100);
		seconds = (d % (60 * 100)) / 100;
				
		string time = string.Format ("{0:00}:{1:00}", minutes, seconds);
		GetComponent<Text> ().text = time;
	
	}
}
