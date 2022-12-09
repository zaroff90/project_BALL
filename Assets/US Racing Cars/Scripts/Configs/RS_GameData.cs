using UnityEngine;
using System.Collections;

public class RS_GameData  {

	private const string GARAGE_SELECTED_CAR = "GARAGE_SELECTED_CAR";
	public static string GarageSelectedCar {
		get {
			if(PlayerPrefs.HasKey(GARAGE_SELECTED_CAR)) {
				return PlayerPrefs.GetString(GARAGE_SELECTED_CAR);
			} else {
				return RS_GameConfig.DEFAULT_CAR_ID;
			}
		}

		set {
			PlayerPrefs.SetString(GARAGE_SELECTED_CAR, value);
		}
	}

}
