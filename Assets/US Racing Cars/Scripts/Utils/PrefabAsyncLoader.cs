using UnityEngine;
using System.Collections;
using System;

public class PrefabAsyncLoader : MonoBehaviour {
	
	private string PrefabPath;

	public event Action<GameObject> ObjectLoadedAction = delegate {};
		 
	public static PrefabAsyncLoader Create(string name) {
	
		PrefabAsyncLoader loader =  new GameObject("PrefabAsyncLoader").AddComponent<PrefabAsyncLoader>();
		loader.LoadAsync(name);

		return loader;
	}


	public void LoadAsync(string name) {
		PrefabPath = name;
		StartCoroutine(Load());
	}



	public IEnumerator Load() {
		ResourceRequest request = Resources.LoadAsync(PrefabPath);

		yield return request;

		if(request.asset == null) {
			Debug.LogWarning("Prefab not found at path: "  + PrefabPath);
		}
		GameObject loadedObject =   UnityEngine.Object.Instantiate (request.asset) as GameObject;
		ObjectLoadedAction(loadedObject);
	}

}
