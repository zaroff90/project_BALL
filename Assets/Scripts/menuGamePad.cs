using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuGamePad : MonoBehaviour
{
    public GameObject current;
    public Button cancel;
    public bool canMove = true;
    public float delay = 0.5f;

    private void OnEnable()
    {
        canMove = false;
        this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, current.GetComponent<RectTransform>().rect.width);
        this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, current.GetComponent<RectTransform>().rect.height);
        this.GetComponent<RectTransform>().position = current.GetComponent<RectTransform>().position;
        StartCoroutine(InputDelay());
    }
    void Update()
    {
        if (Input.GetAxis("Horizontal") > .25)
        {
            if (current.GetComponent<menuOption>().optionRight)
            CursorMove(current.GetComponent<menuOption>().optionRight);
        }
        if (Input.GetAxis("Horizontal") < -.25)
        {
            if (current.GetComponent<menuOption>().optionLeft)
                CursorMove(current.GetComponent<menuOption>().optionLeft);
        }
        if (Input.GetAxis("Vertical") > .25)
        {
            if (current.GetComponent<menuOption>().optionUp)
                CursorMove(current.GetComponent<menuOption>().optionUp);
        }
        if (Input.GetAxis("Vertical") < -.25)
        {
            if (current.GetComponent<menuOption>().optionDown)
                CursorMove(current.GetComponent<menuOption>().optionDown);
        }
        if (Input.GetAxis("LeftAnalogHorizontal") > .25)
        {
            if (current.GetComponent<menuOption>().optionRight)
                CursorMove(current.GetComponent<menuOption>().optionRight);
        }
        if (Input.GetAxis("LeftAnalogHorizontal") < -.25)
        {
            if (current.GetComponent<menuOption>().optionLeft)
                CursorMove(current.GetComponent<menuOption>().optionLeft);
        }
        if (Input.GetAxis("LeftAnalogVertical") < -.25)
        {
            if (current.GetComponent<menuOption>().optionUp)
                CursorMove(current.GetComponent<menuOption>().optionUp);
        }
        if (Input.GetAxis("LeftAnalogVertical") > .25)
        {
            if (current.GetComponent<menuOption>().optionDown)
                CursorMove(current.GetComponent<menuOption>().optionDown);
        }
        if (Input.GetAxis("A")>.25)
        {
            current.GetComponent<Button>().onClick.Invoke();
        }
        if (Input.GetAxis("B") > .25)
        {
            cancel.onClick.Invoke();
        }
    }

    public void CursorMove(GameObject option)
    {
        if (canMove)
        {
            
            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, option.GetComponent<RectTransform>().rect.width);
            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, option.GetComponent<RectTransform>().rect.height);
            this.GetComponent<RectTransform>().position = option.GetComponent<RectTransform>().position;
            current = option;
            canMove = false;
            StartCoroutine(InputDelay());
        }
    }

    private IEnumerator InputDelay()
    {
        yield return new WaitForSeconds(delay);
        canMove = true;
    }
}