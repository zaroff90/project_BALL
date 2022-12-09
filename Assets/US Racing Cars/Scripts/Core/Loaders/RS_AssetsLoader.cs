using UnityEngine;
using System.Collections;

public class RS_AssetsLoader {




	public static GameObject LoadGameObject(string assetName) {
		return Object.Instantiate(Resources.Load(assetName)) as GameObject; 
	}

}
