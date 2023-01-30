using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int durationOfGame = 10;
    public int secLeft;
    public float timer = 0.0f;
    private float period = 1.0f;
    public int redScore = 0;
    public int blueScore = 0;
    private bool gameOver = false;

    public GameObject Player;

    public bool activeTimer = true;

    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI textDisplayBlue;
    public TextMeshProUGUI textDisplayRed;
    public TextMeshProUGUI totalSteps;
    public TextMeshProUGUI episodeSteps;

    private EnvConfig envConfig;

    public int prisionTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        envConfig = gameObject.GetComponentInParent<EnvConfig>();

        if (activeTimer)
        {
            StartCoroutine("DoCheck");
            secLeft = durationOfGame;
            timerDisplay.text = "Time: " + secLeft;
        }
        }



    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timerDisplay) { 
        //Update Score
        textDisplayBlue.text = "Blue Score: " + blueScore;
        textDisplayRed.text = "Red Score: " + redScore;
            totalSteps.text = " ";
            episodeSteps.text = "EpisodeSteps: " + envConfig.playerAgent.StepCount;
        }
        //If no treasures left, End Episode
        if (redScore >= 3)
        {
            PlayerAgent playerAgent = Player.GetComponent<PlayerAgent>();
            playerAgent.AddReward(playerAgent.rewardWinningGame);
            playerAgent.EndEpisode();
            print("AGENT HAS BEATED THE GAME");
        }


        if (secLeft <= 0 && !gameOver)
        {
            gameOver = true;
            StopCoroutine("DoCheck");
            if (redScore > blueScore)
            {
                print("Red won");
            } else if (blueScore > redScore)
            {
                print("Blue won");
            }
            else
            {
                print("Tie");
            }
            ResetScore();
        }
        
    }

    IEnumerator DoCheck()
    {
        print("ttt");
        for (; ; )
        {
            secLeft -= 1;
            yield return new WaitForSeconds(period);
            //Print out the new time
            timerDisplay.text = "Time: " + secLeft;
        }
    }
    
    public void ResetScore()
    {
        StopAllCoroutines(); //One Coroutine already running
        redScore = 0;
        blueScore = 0;
        secLeft = durationOfGame;
        timer = 0;
        gameOver = false;
        if(activeTimer) StartCoroutine("DoCheck");
    } 

}
