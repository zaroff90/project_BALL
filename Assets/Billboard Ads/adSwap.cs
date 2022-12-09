using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adSwap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int randomNumber = Random.Range(0, transform.childCount);
        transform.GetChild(randomNumber).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
