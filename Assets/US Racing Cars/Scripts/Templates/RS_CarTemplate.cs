using UnityEngine;
using System.Collections;

public class RS_CarTemplate  {
	
	private string _Id;
	private int _Index = 0;


	//--------------------------------------
	// Initialization
	//--------------------------------------

	public RS_CarTemplate(string carId) {
		_Id = carId;
	}

	//--------------------------------------
	// Public Methods
	//--------------------------------------


	public void SetIndex(int index) {
		_Index = index;
	}

	//--------------------------------------
	// Get / Set
	//--------------------------------------


	public string Id {
		get {
			return _Id;
		}
	}

	public int Index{
		get {
			return _Index;
		}
	}
}
