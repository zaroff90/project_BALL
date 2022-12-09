using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RS_CarsConfig : NonMonoSingleton<RS_CarsConfig> {

	private Dictionary<string, RS_CarTemplate> _Cars =  new Dictionary<string, RS_CarTemplate>();


	//--------------------------------------
	// Initialization
	//--------------------------------------

	public RS_CarsConfig () {
		LoadConfig();
	}


	//--------------------------------------
	// Public Methods
	//--------------------------------------

	public RS_CarTemplate GetCarTemplate(string CarId) {
		if(_Cars.ContainsKey(CarId)) {
			return _Cars[CarId];
		} else {
			return _Cars[RS_GameConfig.DEFAULT_CAR_ID];
		}
	}



	//--------------------------------------
	// Get / Set
	//--------------------------------------

	public Dictionary<string, RS_CarTemplate> Cars {
		get {
			return _Cars;
		}
	}

	public List<RS_CarTemplate> CarsList {
		get {
			List<RS_CarTemplate> _list =  new List<RS_CarTemplate>();

			foreach(KeyValuePair<string, RS_CarTemplate> pair in _Cars) {
				_list.Add(pair.Value);
			}

			return _list;
		}
	}



	//--------------------------------------
	// Private Methods
	//--------------------------------------

	private void LoadConfig() {
		RS_CarTemplate car;

		car =  new RS_CarTemplate("Black car");
		car.SetIndex(0);
		_Cars.Add(car.Id, car);

		car =  new RS_CarTemplate("Yellow muscle car");
		car.SetIndex(1);
		_Cars.Add(car.Id, car);

		car =  new RS_CarTemplate("Red car");
		car.SetIndex(2);
		_Cars.Add(car.Id, car);

		car =  new RS_CarTemplate("White car");
		car.SetIndex(3);
		_Cars.Add(car.Id, car);

		car =  new RS_CarTemplate("Yellow sport car");
		car.SetIndex(4);
		_Cars.Add(car.Id, car);
	}
		

}
