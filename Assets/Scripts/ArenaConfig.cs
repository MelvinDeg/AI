using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;

public class ArenaConfig : MonoBehaviour
{

    [HideInInspector]
    public Prison prison;




    //TOOD: Chamber blue/red.

    //TODO: Find all players

    //TODO: Check witch team a player belongs to.

    /*[HideInInspector]
    public GameObject chamberD;

    [HideInInspector]
    public GameObject chamberT; */


    public PlayerAgent[] redTeam;
    public PlayerAgent[] BlueTeam;

    public int redScore = 0;
    public int blueScore = 0;

    public bool redTeamTreasure = false;
    public bool blueTeamTreasure = false;


    public GameObject RedChamber;
    public GameObject BlueChamber;

    private EnvironmentParameters m_EnvParameters;
    private StatsRecorder m_StatsRecorder;

    public int StepCount;
    [HideInInspector]
    public float prisionTime = 10;

    //Display
    [Header("Canvas")]
    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI textDisplayBlue;
    public TextMeshProUGUI textDisplayRed;

    /// <summary>
    /// Runs one Time to Setup Relation in one Enviroment
    /// </summary>
    public void Initialize()
    {
        prison = gameObject.transform.Find("Prison").GetComponent<Prison>();

        m_StatsRecorder = Academy.Instance.StatsRecorder; //Add custom Things to Tensorflow.
        m_EnvParameters = Academy.Instance.EnvironmentParameters;

    }

    private void Update()
    {
        timerDisplay.text = "EpisodeTime: " + redTeam[0].StepCount / 5;
        textDisplayRed.text = "Red Score: " + redScore;
        textDisplayBlue.text = "Blue Score: " + blueScore;


        if (redScore >= 3) { 
        
            
            redTeam[0].AddReward(redTeam[0].rewardWinningGame / ((float)redTeam[0].StepCount / 2500));
            BlueTeam[0].AddReward(-BlueTeam[0].rewardWinningGame / ((float)BlueTeam[0].StepCount / 2500));
            redTeam[0].EndEpisode();
            BlueTeam[0].EndEpisode();
            print("RED AGENT HAS BEATED THE GAME");
        }
        else if (blueScore >= 3)
        {
            BlueTeam[0].AddReward(BlueTeam[0].rewardWinningGame / ((float)BlueTeam[0].StepCount / 2500));
            redTeam[0].AddReward(-redTeam[0].rewardWinningGame / ((float)redTeam[0].StepCount / 2500));
            BlueTeam[0].EndEpisode();
            redTeam[0].EndEpisode();
            print("BLUE AGENT HAS BEATED THE GAME");
        }
    }

    /// <summary>
    /// Reset the enviroment
    /// </summary>
    public void Setup()
    {
        redScore = 0;
        blueScore = 0;


        //Set all RedChamber Treasures to true
        foreach (GameObject treasure in RedChamber.GetComponent<TreasureChamber>().tresures)
        {
            treasure.SetActive(true);
        }

        //Set all BlueChamber Treasures to true
        foreach (GameObject treasure in BlueChamber.GetComponent<TreasureChamber>().tresures)
        {
            treasure.SetActive(true);
        }
    }
}



