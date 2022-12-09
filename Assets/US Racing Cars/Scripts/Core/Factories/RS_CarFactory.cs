using UnityEngine;
using System;
using System.Collections;

public class RS_CarFactory : MonoBehaviour {
	
	private static RS_CarCreationMode _Mode = RS_CarCreationMode.Garage;

	public static event Action<GameObject> CarLoadedAction =  delegate {};


	//--------------------------------------
	// Public Methods
	//--------------------------------------


	public static void CreateCar(string id, RS_CarCreationMode creationMode) {
		_Mode = creationMode;


		string  car_resourse_id = RS_GameConfig.RESOURCES_CARS_PATH + id;

		PrefabAsyncLoader loader = PrefabAsyncLoader.Create(car_resourse_id);
		loader.ObjectLoadedAction += Loader_ObjectLoadedAction;
	}



	//--------------------------------------
	// Private Methods
	//--------------------------------------

	static void Loader_ObjectLoadedAction (GameObject car) {


		switch(_Mode) {
		case RS_CarCreationMode.Garage:
			CarLoadedAction(car);
			break;
		}

	}
}
