using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public class Boundary
{
    public Vector2 min = Vector2.zero;
    public Vector2 max = Vector2.zero;
}


[RequireComponent(typeof(Image))]
public class Joystick_Script : MonoBehaviour
{

    /* Is this joystick a TouchPad? */
    public bool touchPad = false;
    /* In case the joystick is a touchPad, should its GUI be faded when inactive? */
    public bool fadeGUI = false;
    /* Control when position is output */
    public Vector2 deadZone = Vector2.zero;
    /* Normalize output after the dead-zone? */
    public bool normalize = false;
    /* Current tap count */
    public int tapCount = -1;

    /* The touchZone of the joystick */
    private RectTransform touchZone;

    /* Finger last used on this joystick */
    private int lastFingerId = -1;
    /* How much time there is left for a tap to occur */
    private float tapTimeWindow;
    private Vector2 fingerDownPos;
    /*
     * Currently unused.
    private float fingerDownTime;
    */
    private float firstDeltaTime;

    /* Joystick graphic */
    private Image gui;
    /* Default position / extents of the joystick graphic */
    private RectTransform defaultRect;
    /* Boundary for joystick graphic */
    private Boundary guiBoundary = new Boundary();
    /* Offset to apply to touch input */
    private Vector2 guiTouchOffset;
    /* Center of joystick */
    private Vector2 guiCenter;

    public bool isFingerDown
    {
        get
        {
            return (lastFingerId != -1);
        }
    }

    public int latchedFinger
    {
        set
        {
            /* If another joystick has latched this finger, then we must release it */
            if (lastFingerId == value)
            {
                Restart();
            }
        }
    }

    /* The position of the joystick on the screen ([-1, 1] in x,y) for clients to read. */
    public Vector2 position
    {
        get;
        private set;
    }


    private static string joysticksTag = "joystick";
    /* A static collection of all joysticks */
    private static List<Joystick_Script> joysticks;
    /* Has the joysticks collection been enumerated yet? */
    private static bool enumeratedJoysticks = false;
    /* Time allowed between taps */
    private static float tapTimeDelta = 0.3f;


    private void Reset()
    {
        try
        {
            gameObject.tag = joysticksTag;
        }
        catch (Exception)
        {
            Debug.LogError("The \""+joysticksTag+"\" tag has not yet been defined in the Tag Manager.");
            throw;
        }
    }


    private void Awake()
    {
        gui = GetComponent<Image>();
        if (gui.sprite == null)
        {
            Debug.LogError("Joystick object requires a valid texture!");
			gameObject.SetActive(false);
            return;
        }

        if (!enumeratedJoysticks)
        {
            try
            {
                /* Collect all joysticks in the game, so we can relay finger latching messages */
                GameObject[] objs = GameObject.FindGameObjectsWithTag(joysticksTag);
                joysticks = new List<Joystick_Script>(objs.Length);
                foreach (GameObject obj in objs)
                {
                    Joystick_Script newJoystick = obj.GetComponent<Joystick_Script>();
                    if (newJoystick == null)
                    {
                        throw new NullReferenceException("Joystick gameObject found without a suitable Joystick component.");
                    }
                    joysticks.Add(newJoystick);
                }
                enumeratedJoysticks = true;
            }
            catch (Exception exp)
            {
                Debug.LogError("Error collecting Joystick objects: "+exp.Message);
                throw;
            }
        }

        /* Store the default rect for the gui, so we can snap back to it */
        defaultRect = gui.gameObject.GetComponent<RectTransform>();

        defaultRect.position = new Vector3( defaultRect.position.x + transform.position.x * Screen.width, defaultRect.position.y + transform.position.y * Screen.height, defaultRect.position.z);// - Screen.height * 0.5f;

        transform.position = new Vector3(0, 0, transform.position.z);

        if (touchPad)
        {
            /* Use the rect from the gui as our touchZone */
            touchZone = defaultRect;
        }
        else
        {
            /* This is an offset for touch input to match with the top left corner of the GUI */
            guiTouchOffset.x = defaultRect.rect.width * 0.5f;
            guiTouchOffset.y = defaultRect.rect.height * 0.5f;

            /* Cache the center of the GUI, since it doesn't change */
            guiCenter.x = defaultRect.position.x + guiTouchOffset.x;
            guiCenter.y = defaultRect.position.y + guiTouchOffset.y;

            /* Let's build the GUI boundary, so we can clamp joystick movement */
            guiBoundary.min.x = defaultRect.position.x - guiTouchOffset.x;
            guiBoundary.max.x = defaultRect.position.x + guiTouchOffset.x;
            guiBoundary.min.y = defaultRect.position.y - guiTouchOffset.y;
            guiBoundary.max.y = defaultRect.position.y + guiTouchOffset.y;
        }

    }


    public void Enable()
    {
        enabled = true;
    }


    public void Disable()
    {
        enabled = false;
    }


    public void Restart()
    {
        /* Release the finger control and set the joystick back to the default position */
        //gui.pixelInset = defaultRect;
        lastFingerId = -1;
        position = Vector2.zero;
        fingerDownPos = Vector2.zero;

        if (touchPad && fadeGUI)
        {
            gui.color = new Color(gui.color.r, gui.color.g, gui.color.b, 0.025f);
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

    private void Update()
    {

        int count = Input.touchCount;

        /* Adjust the tap time window while it still available */
        if (tapTimeWindow > 0)
        {
            tapTimeWindow -= Time.deltaTime;
        }
        else
        {
            tapCount = 0;
        }

        if (count == 0)
        {
            Restart();
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 guiTouchPos = touch.position - guiTouchOffset;

                bool shouldLatchFinger = false;
                if (touchPad && Contains(touchZone,touch.position))
                {
                    shouldLatchFinger = true;
                }
                else if (Contains(gui.rectTransform,guiTouchPos))
                {
                    shouldLatchFinger = true;
                }
                /* Latch the finger if this is a new touch */
                if (shouldLatchFinger && (lastFingerId == -1 || lastFingerId != touch.fingerId))
                {

                    if (touchPad)
                    {
                        if (fadeGUI)
                        {
                            gui.color = new Color(gui.color.r, gui.color.g, gui.color.b, 0.15f);
                        }
                        lastFingerId = touch.fingerId;
                        fingerDownPos = touch.position;
                        /*
                         * Currently unused.
                        fingerDownTime = Time.time;
                        */
                    }

                    lastFingerId = touch.fingerId;

                    /* Accumulate taps if it is within the time window */
                    if (tapTimeWindow > 0)
                    {
                        tapCount++;
                    }
                    else
                    {
                        tapCount = 1;
                        tapTimeWindow = tapTimeDelta;
                    }

                    /* Tell other joysticks we've latched this finger */
                    foreach (Joystick_Script j in joysticks)
                    {
                        if (j == this)
                        {
                            continue;
                        }
                        j.latchedFinger = touch.fingerId;
                    }
                }

                if (lastFingerId == touch.fingerId)
                {
                    /*
                        Override the tap count with what the iOS SDK reports if it is greater.
                        This is a workaround, since the iOS SDK does not currently track taps
                        for multiple touches.
                    */
                    if (touch.tapCount > tapCount)
                    {
                        tapCount = touch.tapCount;
                    }

                    if (touchPad)
                    {
                        /* For a touchpad, let's just set the position directly based on distance from initial touchdown */
                        position = new Vector2
                            (
                              Mathf.Clamp((touch.position.x - fingerDownPos.x) / (touchZone.rect.width / 2), -1, 1),
                              Mathf.Clamp((touch.position.y - fingerDownPos.y) / (touchZone.rect.height / 2), -1, 1)
                            );
                    }
                    else
                    {
                        /* Change the location of the joystick graphic to match where the touch is */
                        /*
                        gui.rectTransform.position = new recc
                            (
                              Mathf.Clamp(guiTouchPos.x, guiBoundary.min.x, guiBoundary.max.x),
                              Mathf.Clamp(guiTouchPos.y, guiBoundary.min.y, guiBoundary.max.y),
                              gui.pixelInset.width,
                              gui.pixelInset.height
                            );
                        */
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        Restart();
                    }
                }
            }

        }

        if (!touchPad)
        {
            /* Get a value between -1 and 1 based on the joystick graphic location */
            position = new Vector2
                (
                  (gui.rectTransform.position.x + guiTouchOffset.x - guiCenter.x) / guiTouchOffset.x,
                  (gui.rectTransform.position.y + guiTouchOffset.y - guiCenter.y) / guiTouchOffset.y
                );
        }

        /* Adjust for dead zone */
        float absoluteX = Mathf.Abs(position.x);
        float absoluteY = Mathf.Abs(position.y);

        if (absoluteX < deadZone.x)
        {
            /* Report the joystick as being at the center if it is within the dead zone */
            position = new Vector2(0, position.y);
        }
        else if (normalize)
        {
            /* Rescale the output after taking the dead zone into account */
            position = new Vector2(Mathf.Sign(position.x) * (absoluteX - deadZone.x) / (1 - deadZone.x), position.y);
        }

        if (absoluteY < deadZone.y)
        {
            /* Report the joystick as being at the center if it is within the dead zone */
            position = new Vector2(position.x, 0);
        }
        else if (normalize)
        {
            /* Rescale the output after taking the dead zone into account */
            position = new Vector2(position.x, Mathf.Sign(position.y) * (absoluteY - deadZone.y) / (1 - deadZone.y));
        }
    }


}