using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu, gameOverMenu, inGameButton;
    public Camera cameraButton;

    public GameObject ball, ballShadow, hitEffect, effectsParent;
    public SpriteRenderer stands, ground;
    public PlayerScripts player1, player2;
    public TextMesh p1Name, p2Name;
    public TextMesh p1ShotCountLabel, p2ShotCountLabel;

    public TextMesh timeLabel;
    public TextMesh p1ScoreLabel, p2ScoreLabel;

    private int p1Score, p2Score;
    private Vector3 player1BallStartPos, player2BallStartPos;
    private int time;

    private int p1ShotCount, p2ShotCount;
    public int Player1ShotCount
    {
        set
        {
            p1ShotCount = value;

            if (value > 0)
            {
                p1ShotCountLabel.text = (7 - value).ToString();
            }
            else
            {
                p1ShotCountLabel.text = "0";
            }

            if (value > 7)
            {
                ball.GetComponent<Rigidbody2D>().isKinematic = true;
                ball.GetComponent<Rigidbody2D>().simulated = false;
                gameMode = GameMode.PAUSE;
                CreateBall(2);
                AssetManager.Use.PlaySound(6);
            }
        }
        get
        {
            return p1ShotCount;
        }
    }

    public int Player2ShotCount
    {
        set
        {
            p2ShotCount = value;

            if (value > 0)
            {
                p2ShotCountLabel.text = (7 - value).ToString();
            }
            else
            {
                p2ShotCountLabel.text = "0";
            }

            if (value > 7)
            {
                ball.GetComponent<Rigidbody2D>().isKinematic = true;
                ball.GetComponent<Rigidbody2D>().simulated = false;
                gameMode = GameMode.PAUSE;
                CreateBall(1);
                AssetManager.Use.PlaySound(6);
            }
        }
        get
        {
            return p2ShotCount;
        }
    }

    [HideInInspector]
    public GameMode gameMode;

    void Awake()
    {
        gameMode = GameMode.AWAKE;
    }

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            player1.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(1));
            player2.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(2));
        }
        Physics2D.gravity = new Vector2(0, -20f);

        lastFixedDeltaTime = Time.fixedDeltaTime;

        ballShadow.GetComponent<Renderer>().sortingOrder = -50;

        Init();

        time = AssetManager.Use.timesArray[PlayerPrefs.GetInt(VariablesName.Time , 0)];
        timeLabel.text = time.ToString("00");
        InvokeRepeating("SetTime", 1, 1);

        p1Score = p2Score = 0;
        p1ScoreLabel.text = p2ScoreLabel.text = "0";

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            Hashtable hashP1Score = new Hashtable();
            hashP1Score.Add("P1Score", p1Score);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashP1Score);

            Hashtable hashP2Score = new Hashtable();
            hashP2Score.Add("P2Score", p2Score);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashP2Score);
        }

        player1BallStartPos = new Vector3(-2.5f, 1.7f, 0);
        player2BallStartPos = new Vector3(2.5f, 1.7f, 0);

        CreateBall();
    }

    void Init()
    {
        stands.sprite = AssetManager.Use.standsSprites[PlayerPrefs.GetInt(VariablesName.DayAndNight, 0)];
        ball.GetComponent<SpriteRenderer>().sprite = AssetManager.Use.ballSprites[PlayerPrefs.GetInt(VariablesName.BallNumber, 0)];

        ground.sprite = AssetManager.Use.groundSprites[PlayerPrefs.GetInt(VariablesName.GroundNum, 0)];

        if (!PhotonNetwork.IsConnected)
        {
            //////////////////////////

            int player1Number = PlayerPrefs.GetInt(VariablesName.PlayerNumber, 0);

            Sprite[] myFaceTemp = new Sprite[2];
            myFaceTemp[0] = AssetManager.Use.playerSprite0[player1Number];
            myFaceTemp[1] = AssetManager.Use.playerSprite1[player1Number];
            player1.myFace = myFaceTemp;

            player1.myBody = AssetManager.Use.playerBodySprite[PlayerPrefs.GetInt(VariablesName.PlayerBodyNumber, 0)];

            player1.myShoes = AssetManager.Use.shoesSprites[PlayerPrefs.GetInt(VariablesName.ShoesNumber, 0)];
            p1Name.text = AssetManager.Use.playersName[player1Number];
            player1.Init();

            /////////////////////////

            List<int> tmpList = new List<int>();
            for (int i = 0; i < AssetManager.Use.playerSprite0.Length; i++)
            {
                if (i != player1Number)
                {
                    tmpList.Add(i);
                }
            }

            int player2Number = tmpList[Random.Range(0, tmpList.Count)];

            myFaceTemp = new Sprite[2];
            myFaceTemp[0] = AssetManager.Use.playerSprite0[player2Number];
            myFaceTemp[1] = AssetManager.Use.playerSprite1[player2Number];
            player2.myFace = myFaceTemp;

            player2.myBody = AssetManager.Use.playerBodySprite[Random.Range(0, AssetManager.Use.playerBodySprite.Length)];

            player2.myShoes = AssetManager.Use.shoesSprites[Random.Range(0, AssetManager.Use.shoesSprites.Length)];
            p2Name.text = AssetManager.Use.playersName[player2Number];
            player2.Init();

            ////////////////////
        }
        if (PhotonNetwork.IsConnected)
        {
            //////////////////////////

            int player1Number = (int)PhotonNetwork.CurrentRoom.GetPlayer(1).CustomProperties["Player"];

            Sprite[] myFaceTemp = new Sprite[2];
            myFaceTemp[0] = AssetManager.Use.playerSprite0[player1Number];
            myFaceTemp[1] = AssetManager.Use.playerSprite1[player1Number];
            player1.myFace = myFaceTemp;

            player1.myBody = AssetManager.Use.playerBodySprite[(int)PhotonNetwork.CurrentRoom.GetPlayer(1).CustomProperties["Body"]];

            player1.myShoes = AssetManager.Use.shoesSprites[(int)PhotonNetwork.CurrentRoom.GetPlayer(1).CustomProperties["Shoes"]];
            p1Name.text = AssetManager.Use.playersName[player1Number];
            player1.Init();

            /////////////////////////

            int player2Number = (int)PhotonNetwork.CurrentRoom.GetPlayer(2).CustomProperties["Player"];

            myFaceTemp = new Sprite[2];
            myFaceTemp[0] = AssetManager.Use.playerSprite0[player2Number];
            myFaceTemp[1] = AssetManager.Use.playerSprite1[player2Number];
            player2.myFace = myFaceTemp;

            player2.myBody = AssetManager.Use.playerBodySprite[(int)PhotonNetwork.CurrentRoom.GetPlayer(2).CustomProperties["Body"]];

            player2.myShoes = AssetManager.Use.shoesSprites[(int)PhotonNetwork.CurrentRoom.GetPlayer(2).CustomProperties["Shoes"]];
            p2Name.text = AssetManager.Use.playersName[player2Number];
            player2.Init();

            ////////////////////
        }


        effectsParent.transform.GetChild(PlayerPrefs.GetInt(VariablesName.Weather, 0)).gameObject.SetActive(true);
        AssetManager.Use.BackgroundEffectSound();
    }

    void LateUpdate()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            p1Score = (int)PhotonNetwork.CurrentRoom.CustomProperties["P1Score"];
            p2Score = (int)PhotonNetwork.CurrentRoom.CustomProperties["P2Score"];
            p1ScoreLabel.text = p1Score.ToString();
            p2ScoreLabel.text = p2Score.ToString();
        }
        Vector3 tmp = ballShadow.transform.position;
        tmp.x = ball.transform.position.x;
        ballShadow.transform.position = tmp;

        if (gameMode == GameMode.PLAY)
        {
            if (tmp.x >= -0.3f && tmp.x <= 0.3f)
            {
                Player1ShotCount = 0;
                Player2ShotCount = 0;
            }
        }
        else if (gameMode == GameMode.SLOWMOTION)
        {
            //DoSlowmotion();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && gameMode != GameMode.END)
        {
            AssetManager.Use.PlaySound(7);

            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);

            if (pauseMenu.activeInHierarchy)
            {
                lastTimeScale = Time.timeScale;
                lastGameMode = gameMode;

                Time.timeScale = 0;
                gameMode = GameMode.PAUSE;

                inGameButton.SetActive(false);
            }
            else
            {
                Time.timeScale = lastTimeScale;
                gameMode = lastGameMode;

                inGameButton.SetActive(true);
            }
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            int touchCount = Input.touchCount;

            if (touchCount > 0)
            {
                for (int i = 0; i < touchCount; i++)
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(cameraButton.ScreenToWorldPoint(Input.GetTouch(i).position), Vector2.zero);

                        if (hit.collider != null)
                        {
                            if (hit.collider.gameObject.name.Contains("HomeBtn"))
                            {
                                AssetManager.Use.PlaySound(7);
                                Time.timeScale = 1;
                                Time.fixedDeltaTime = lastFixedDeltaTime;

                                SceneManager.LoadScene("Main");
                            }
                            else if (hit.collider.gameObject.name.Contains("RestartBtn"))
                            {
                                AssetManager.Use.PlaySound(7);
                                Time.timeScale = 1;
                                Time.fixedDeltaTime = lastFixedDeltaTime;

                                SceneManager.LoadScene("Game");
                            }
                            else if (hit.collider.gameObject.name.Contains("PauseBtn") && gameMode != GameMode.END)
                            {
                                AssetManager.Use.PlaySound(7);

                                pauseMenu.SetActive(!pauseMenu.activeInHierarchy);

                                if (pauseMenu.activeInHierarchy)
                                {
                                    lastTimeScale = Time.timeScale;
                                    lastGameMode = gameMode;

                                    Time.timeScale = 0;
                                    gameMode = GameMode.PAUSE;

                                    inGameButton.SetActive(false);
                                }
                                else
                                {
                                    Time.timeScale = lastTimeScale;
                                    gameMode = lastGameMode;

                                    inGameButton.SetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(cameraButton.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.name.Contains("HomeBtn"))
                    {
                        AssetManager.Use.PlaySound(7);
                        Time.timeScale = 1;
                        Time.fixedDeltaTime = lastFixedDeltaTime;

                        SceneManager.LoadScene("Main");
                    }
                    else if (hit.collider.gameObject.name.Contains("RestartBtn"))
                    {
                        AssetManager.Use.PlaySound(7);
                        Time.timeScale = 1;
                        Time.fixedDeltaTime = lastFixedDeltaTime;

                        SceneManager.LoadScene("Game");
                    }
                    else if (hit.collider.gameObject.name.Contains("PauseBtn") && gameMode != GameMode.END)
                    {
                        AssetManager.Use.PlaySound(7);

                        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);

                        if (pauseMenu.activeInHierarchy)
                        {
                            lastTimeScale = Time.timeScale;
                            lastGameMode = gameMode;

                            Time.timeScale = 0;
                            gameMode = GameMode.PAUSE;

                            inGameButton.SetActive(false);
                        }
                        else
                        {
                            Time.timeScale = lastTimeScale;
                            gameMode = lastGameMode;

                            inGameButton.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    ///////////////////////////////////////////////////////

    void SetTime()
    {
        if (gameMode == GameMode.PLAY)
        {
            time--;
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                Hashtable hashTime = new Hashtable();
                hashTime.Add("Time", time);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashTime);
            }
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                time = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
            }

            if (time >= 0)
            {
                timeLabel.text = time.ToString("00");

                float x = ball.transform.position.x;

                if (x < -0.3f)
                {
                    Player1ShotCount++;
                    Player2ShotCount = 0;
                }
                else if (x > 0.3f)
                {
                    Player1ShotCount = 0;
                    Player2ShotCount++;
                }
            }
            else
            {
                gameMode = GameMode.END;
                ball.GetComponent<Rigidbody2D>().isKinematic = true;
                ball.GetComponent<Rigidbody2D>().simulated = false;

                /////////// gameOver 

                AssetManager.Use.PlaySound(9);

                inGameButton.SetActive(false);

                gameOverMenu.transform.GetChild(2).GetComponent<Renderer>().sortingOrder = 15;

                gameOverMenu.transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = player1.myBody;
                gameOverMenu.transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = player1.myShoes;
                gameOverMenu.transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = player1.myShoes;
                gameOverMenu.transform.GetChild(0).GetChild(5).GetComponent<Renderer>().sortingOrder = 15;
                gameOverMenu.transform.GetChild(0).GetChild(5).GetComponent<TextMesh>().text = p1Name.text;
                gameOverMenu.transform.GetChild(0).GetChild(6).GetComponent<Renderer>().sortingOrder = 15;
                gameOverMenu.transform.GetChild(0).GetChild(6).GetComponent<TextMesh>().text = p1Score.ToString();


                gameOverMenu.transform.GetChild(1).GetChild(2).GetComponent<SpriteRenderer>().sprite = player2.myBody;
                gameOverMenu.transform.GetChild(1).GetChild(3).GetComponent<SpriteRenderer>().sprite = player2.myShoes;
                gameOverMenu.transform.GetChild(1).GetChild(4).GetComponent<SpriteRenderer>().sprite = player2.myShoes;
                gameOverMenu.transform.GetChild(1).GetChild(5).GetComponent<Renderer>().sortingOrder = 15;
                gameOverMenu.transform.GetChild(1).GetChild(5).GetComponent<TextMesh>().text = p2Name.text;
                gameOverMenu.transform.GetChild(1).GetChild(6).GetComponent<Renderer>().sortingOrder = 15;
                gameOverMenu.transform.GetChild(1).GetChild(6).GetComponent<TextMesh>().text = p2Score.ToString();


                if (p1Score > p2Score)
                {
                    print("Player1 Win");

                    gameOverMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = player1.myFace[0];
                    gameOverMenu.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = player2.myFace[1];
                    gameOverMenu.transform.GetChild(2).GetComponent<TextMesh>().text = p1Name.text + " Win";
                }
                else if (p1Score < p2Score)
                {
                    print("Player2 Win");
                    gameOverMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = player1.myFace[1];
                    gameOverMenu.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = player2.myFace[0];
                    gameOverMenu.transform.GetChild(2).GetComponent<TextMesh>().text = p2Name.text + " Win";
                }
                else
                {
                    print("Draw Game");
                    gameOverMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = player1.myFace[1];
                    gameOverMenu.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = player2.myFace[1];
                    gameOverMenu.transform.GetChild(2).GetComponent<TextMesh>().text = "Game Draw";
                }

                Invoke("SetGAmeOverMenuTrue", 1);
            }
        }
    }

    void SetGAmeOverMenuTrue()
    {
        gameOverMenu.SetActive(true);
    }

    private float slowdownFactor = 0.01f;
    private float slowdownLength = 0.01f;

    private float lastTimeScale;
    private float lastFixedDeltaTime;
    private GameMode lastGameMode;

    void DoSlowmotion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        Time.timeScale += (0.2f / slowdownLength) * Time.unscaledDeltaTime;
    }

    public IEnumerator CreateBallWait(int num)
    {
        yield return new WaitForSeconds(1);
        CreateBall(num);
    }

    public void CreateBall(int playerNum = 0)
    {
        gameMode = GameMode.PAUSE;
        Time.timeScale = 1;
        Time.fixedDeltaTime = lastFixedDeltaTime;

        ball.GetComponent<Rigidbody2D>().isKinematic = true;
        ball.GetComponent<Rigidbody2D>().simulated = false;
        ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        Player1ShotCount = Player2ShotCount = 0;

        if (playerNum == 0)
        {
            AssetManager.Use.PlaySound(8);

            playerNum = Random.Range(1, 3);

            if (playerNum == 1)
            {
                ball.transform.position = player2BallStartPos;
            }
            else
            {
                ball.transform.position = player1BallStartPos;
            }

            StartCoroutine(StartGame());
        }
        else if (playerNum == 1)
        {
            p1Score++;
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                Hashtable hashP1Score = new Hashtable();
                hashP1Score.Add("P1Score", p1Score);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashP1Score);

                Hashtable hashP2Score = new Hashtable();
                hashP2Score.Add("P2Score", p2Score);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashP2Score);
            }
            p1ScoreLabel.text = p1Score.ToString();
            StartCoroutine(SetBallPos(player2BallStartPos));
            StartCoroutine(player2.ChangeFace());
        }
        else
        {
            p2Score++;
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                Hashtable hashP1Score = new Hashtable();
                hashP1Score.Add("P1Score", p1Score);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashP1Score);

                Hashtable hashP2Score = new Hashtable();
                hashP2Score.Add("P2Score", p2Score);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashP2Score);
            }
            p2ScoreLabel.text = p2Score.ToString();
            StartCoroutine(SetBallPos(player1BallStartPos));
            StartCoroutine(player1.ChangeFace());
        }
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
        ball.GetComponent<Rigidbody2D>().isKinematic = false;
        ball.GetComponent<Rigidbody2D>().simulated = true;
        gameMode = GameMode.PLAY;
    }

    private IEnumerator SetBallPos(Vector3 pos)
    {
        yield return new WaitForSeconds(1);
        ball.transform.position = pos;
        StartCoroutine(StartGame());
    }

    public void CreateHitEffect(Vector3 pos)
    {
        Instantiate(hitEffect, pos, Quaternion.identity);
    }
}