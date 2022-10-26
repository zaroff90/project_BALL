
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// throw handling in D-Pad
public class Shoot_Script : MonoBehaviour {

	public GameObject sphere;

	void Update () {
		// control touch input for mobile device
    	for (int touchIndex = 0; touchIndex<Input.touchCount; touchIndex++){
      		Touch currentTouch = Input.touches[touchIndex];
      		if (currentTouch.phase == TouchPhase.Ended && Contains(GetComponent<Image>().rectTransform,currentTouch.position)) {
				sphere.GetComponent<Sphere>().shootButtonEnded = true;
			}
    	}
	}
    public bool Contains(RectTransform contain, Vector2 pos)
    {
        bool isTrue = false;
        if (pos.x >= contain.rect.min.x && pos.x <= contain.rect.max.x)
        {
            if (pos.y >= contain.rect.min.y && pos.y <= contain.rect.max.y)
            {
                isTrue = true;
            }
        }
        return isTrue;
    }
}
