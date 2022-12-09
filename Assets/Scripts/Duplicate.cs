using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject clone = GameObject.Find(this.gameObject.name);
        if (clone != null)
        {
            if (clone != this.gameObject)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
