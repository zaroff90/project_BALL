using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public static int coins = 0;
    public GameObject screenBuy;
    public GameObject screenNo;
    public SpriteRenderer playerFaceSp;
    public Transform playerFaceSelectedObj;
    private int playerFaceIndex;
    public int[] headCost;
    public string[] headName;
    public int PlayerFaceIndex
    {
        set
        {
            playerFaceSelectedObj.GetChild(1).gameObject.SetActive(!(value == 0));
            playerFaceSelectedObj.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.shopHead0.Length - 1));

            playerFaceSp.sprite = AssetManager.Use.shopHead0[value];

            playerFaceIndex = value;
        }
        get
        {
            return playerFaceIndex;
        }
    }

    public SpriteRenderer playerBodySp;
    public Transform playerBodySelectedObj;
    private int playerBodyIndex;
    public int[] jerseyCost;
    public string[] jerseyName;
    public int PlayerBodyIndex
    {
        set
        {
            playerBodySelectedObj.GetChild(1).gameObject.SetActive(!(value == 0));
            playerBodySelectedObj.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.shopJerseys.Length - 1));

            playerBodySp.sprite = AssetManager.Use.shopJerseys[value];

            playerBodyIndex = value;
        }
        get
        {
            return playerBodyIndex;
        }
    }

    public SpriteRenderer playerShoesSp1, playerShoesSp2;
    public Transform playerShoesSelectedObj;
    private int playerShoesIndex;
    public int[] shoesCost;
    public string[] shoesName;
    public int PlayerShoesIndex
    {
        set
        {
            playerShoesSelectedObj.GetChild(1).gameObject.SetActive(!(value == 0));
            playerShoesSelectedObj.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.shopShoes.Length - 1));

            playerShoesSp1.sprite = playerShoesSp2.sprite = AssetManager.Use.shopShoes[value];

            playerShoesIndex = value;
        }
        get
        {
            return playerShoesIndex;
        }
    }

    public SpriteRenderer ballSp;
    private int ballIndex;
    public int[] ballCost;
    public string[] ballName;
    public int BallIndex
    {
        set
        {
            ballSp.transform.GetChild(1).gameObject.SetActive(!(value == 0));
            ballSp.transform.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.shopBalls.Length - 1));

            ballSp.sprite = AssetManager.Use.shopBalls[value];

            ballIndex = value;
        }
        get
        {
            return ballIndex;
        }
    }

    public SpriteRenderer groundSp;
    private int groundIndex;
    public int[] courtCost;
    public string[] courtName;
    public int GroundIndex
    {
        set
        {
            groundSp.transform.GetChild(1).gameObject.SetActive(!(value == 0));

            groundSp.transform.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.shopCourt.Length - 1));
            groundSp.sprite = AssetManager.Use.shopIconCourt[value];

            groundIndex = value;
        }
        get
        {
            return groundIndex;
        }
    }

    private Transform tran;

    void Start ()
	{
        Init();
        coins = PlayerPrefs.GetInt("coins", 0);
	}

    void Init()
    {
        /*
        int playerNum = PlayerPrefs.GetInt(VariablesName.PlayerNumber, 0);
        int playerBodyNum = PlayerPrefs.GetInt(VariablesName.PlayerBodyNumber, 0);
        int shoesNum = PlayerPrefs.GetInt(VariablesName.ShoesNumber, 0);

        int ballNum = PlayerPrefs.GetInt(VariablesName.BallNumber, 0);
        int groundNum = PlayerPrefs.GetInt(VariablesName.GroundNum, 0);

        int dayAndNightNum = PlayerPrefs.GetInt(VariablesName.DayAndNight, 0);
        int weatherNum = PlayerPrefs.GetInt(VariablesName.Weather, 0);

        int timeNum = PlayerPrefs.GetInt(VariablesName.Time, 0);
        int aiLevel = PlayerPrefs.GetInt(VariablesName.AILevel, 0);

        PlayerFaceIndex = playerNum;
        PlayerBodyIndex = playerBodyNum;
        PlayerShoesIndex = shoesNum;
        BallIndex = ballNum;
        GroundIndex = groundNum;
        */
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name.Contains("PlayerFaceArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerFaceIndex++;
                    GameObject.Find("Label Head").transform.GetChild(0).GetComponent<TextMesh>().text = headName[PlayerFaceIndex];
                    GameObject.Find("Label Head").transform.GetChild(1).GetComponent<TextMesh>().text = headCost[PlayerFaceIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("PlayerFaceArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerFaceIndex--;
                    GameObject.Find("Label Head").transform.GetChild(0).GetComponent<TextMesh>().text = headName[PlayerFaceIndex];
                    GameObject.Find("Label Head").transform.GetChild(1).GetComponent<TextMesh>().text = headCost[PlayerFaceIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("PlayerBodyArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerBodyIndex++;
                    GameObject.Find("Label Jersey").transform.GetChild(0).GetComponent<TextMesh>().text = jerseyName[PlayerBodyIndex];
                    GameObject.Find("Label Jersey").transform.GetChild(1).GetComponent<TextMesh>().text = jerseyCost[PlayerBodyIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("PlayerBodyArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerBodyIndex--;
                    GameObject.Find("Label Jersey").transform.GetChild(0).GetComponent<TextMesh>().text = jerseyName[PlayerBodyIndex];
                    GameObject.Find("Label Jersey").transform.GetChild(1).GetComponent<TextMesh>().text = jerseyCost[PlayerBodyIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("ShoesArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerShoesIndex++;
                    GameObject.Find("Label Shoes").transform.GetChild(0).GetComponent<TextMesh>().text = shoesName[PlayerShoesIndex];
                    GameObject.Find("Label Shoes").transform.GetChild(1).GetComponent<TextMesh>().text = shoesCost[PlayerShoesIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("ShoesArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerShoesIndex--;
                    GameObject.Find("Label Shoes").transform.GetChild(0).GetComponent<TextMesh>().text = shoesName[PlayerShoesIndex];
                    GameObject.Find("Label Shoes").transform.GetChild(1).GetComponent<TextMesh>().text = shoesCost[PlayerShoesIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("BallsArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    BallIndex++;
                    GameObject.Find("Label Ball").transform.GetChild(0).GetComponent<TextMesh>().text = ballName[BallIndex];
                    GameObject.Find("Label Ball").transform.GetChild(1).GetComponent<TextMesh>().text = ballCost[BallIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("BallsArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    BallIndex--;
                    GameObject.Find("Label Ball").transform.GetChild(0).GetComponent<TextMesh>().text = ballName[BallIndex];
                    GameObject.Find("Label Ball").transform.GetChild(1).GetComponent<TextMesh>().text = ballCost[BallIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("GroundArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    GroundIndex++;
                    GameObject.Find("Label Court").transform.GetChild(0).GetComponent<TextMesh>().text = courtName[GroundIndex];
                    GameObject.Find("Label Court").transform.GetChild(1).GetComponent<TextMesh>().text = courtCost[GroundIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("GroundArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    GroundIndex--;
                    GameObject.Find("Label Court").transform.GetChild(0).GetComponent<TextMesh>().text = courtName[GroundIndex];
                    GameObject.Find("Label Court").transform.GetChild(1).GetComponent<TextMesh>().text = courtCost[GroundIndex].ToString() + " coins";
                }
                else if (hit.collider.gameObject.name.Contains("buyHead"))
                {
                    tran = hit.transform;
                    AnimDown();
                    BuyHead();
                }
                else if (hit.collider.gameObject.name.Contains("buyJersey"))
                {
                    tran = hit.transform;
                    AnimDown();
                    BuyJersey();
                }
                else if (hit.collider.gameObject.name.Contains("buyShoes"))
                {
                    tran = hit.transform;
                    AnimDown();
                    BuyShoes();
                }
                else if (hit.collider.gameObject.name.Contains("buyBall"))
                {
                    tran = hit.transform;
                    AnimDown();
                    BuyBall();
                }
                else if (hit.collider.gameObject.name.Contains("buyCourt"))
                {
                    tran = hit.transform;
                    AnimDown();
                    BuyCourt();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            AnimUp();
        }
    }

    void AnimDown()
    {
        tran.localScale = new Vector3(0.9f, 0.9f, 1);
        AssetManager.Use.PlaySound(7);
    }

    void AnimUp()
    {
        if (tran != null)
        {
            tran.localScale = new Vector3(1, 1, 1);
        }
    }

    public bool CheckCoins(int price)
    {
        if (coins >= price)
        {
            return true;
        }
        else
        {
            screenNo.SetActive(true);
            return false;
        }
    }

    public void BuyHead()
    {
        switch (PlayerFaceIndex)
        {
            case 0:
                if (CheckCoins(headCost[PlayerFaceIndex]))
                {
                    if (global.dlcHead1 == 0)
                    {
                        global.dlcHead1 = 1;
                        PlayerPrefs.SetInt("dlcHead1", 1);
                        coins -= headCost[PlayerFaceIndex];
                        PlayerPrefs.SetInt("coins", coins);
                    }
                }
                break;
            default:
                Debug.Log("code");
                break;
        }
    }
    public void BuyJersey()
    {
        switch (PlayerBodyIndex)
        {
            case 0:
                if (CheckCoins(jerseyCost[PlayerBodyIndex]))
                {
                    if (global.dlcJersey1 == 0)
                    {
                        global.dlcJersey1 = 1;
                        PlayerPrefs.SetInt("dlcJersey1", 1);
                        coins -= jerseyCost[PlayerBodyIndex];
                        PlayerPrefs.SetInt("coins", coins);
                    }
                }
                break;
            default:
                Debug.Log("code");
                break;
        }
    }
    public void BuyShoes()
    {
        switch (PlayerShoesIndex)
        {
            case 0:
                if (CheckCoins(shoesCost[PlayerShoesIndex]))
                {
                    if (global.dlcShoes1 == 0)
                    {
                        global.dlcShoes1 = 1;
                        PlayerPrefs.SetInt("dlcShoes1", 1);
                        coins -= shoesCost[PlayerShoesIndex];
                        PlayerPrefs.SetInt("coins", coins);
                    }
                }
                break;
            default:
                Debug.Log("code");
                break;
        }
    }
    public void BuyBall()
    {
        switch (BallIndex)
        {
            case 0:
                if (CheckCoins(ballCost[BallIndex]))
                {
                    if (global.dlcBalls1 == 0)
                    {
                        global.dlcBalls1 = 1;
                        PlayerPrefs.SetInt("dlcBalls1", 1);
                        coins -= ballCost[BallIndex];
                        PlayerPrefs.SetInt("coins", coins);
                    }
                }
                break;
            default:
                Debug.Log("code");
                break;
        }
    }
    public void BuyCourt()
    {
        switch (GroundIndex)
        {
            case 0:
                if (CheckCoins(courtCost[GroundIndex]))
                {
                    if (global.dlcCourt1 == 0)
                    {
                        global.dlcCourt1 = 1;
                        PlayerPrefs.SetInt("dlcCourt1", 1);
                        coins -= courtCost[GroundIndex];
                        PlayerPrefs.SetInt("coins", coins);
                    }
                }
                break;
            default:
                Debug.Log("code");
                break;
        }
    }
}
