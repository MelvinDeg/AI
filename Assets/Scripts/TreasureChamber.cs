using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Treasure Chamber contains <see cref="Treasure"/>
/// </summary>
public class TreasureChamber : MonoBehaviour
{

    private const int AmountOfTresures = 3;
    [HideInInspector]
    public string chamberColor;
    public  GameObject[] tresures = new GameObject[AmountOfTresures];
    GameManager gameManager;
    private EnvConfig envConfig;

    // Start is called before the first frame update
    void Start()
    {
        /*for (int i = 1; i <= AmountOfTresures; i++)
        {
            tresures[i-1] = gameObject.transform.Find("Treasure ("+i.ToString()+")").gameObject;
        }
       
            
        Debug.Log(tresures.Length); */
        chamberColor = gameObject.tag;

        envConfig = gameObject.GetComponentInParent<EnvConfig>();
        gameManager = envConfig.gameManager;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// When
    /// </summary>
    /// <param name="other">The Agent</param>
    /// 


    /*private void OnTriggerExit(Collider other)
    {
        PlayerAgent player = other.GetComponent<PlayerAgent>();

        if (gameObject.CompareTag(player.chamberColour + "C") && !player.hasTreasure)
        {
            player.AddReward(-player.rewardtakingTreasureToOwnTreasureChamber);
        }
    } */

    void OnTriggerEnter(Collider other)
    {
        //GameObject agent = other.gameObject;
        //PlayerAgent agentScript = agent.GetComponent<PlayerAgent>();
        PlayerAgent player = other.GetComponent<PlayerAgent>();
        // Checks that the treasureChamber has the correct tag and that the player does not already carry one treasure
        // 


        //TODO: Check if Player has returned to HomeChamber

        if (gameObject.CompareTag(player.chamberColour +"C") && player.hasTreasure)
        {
            print("plus 1");
            

            player.AddReward(player.rewardtakingTreasureToOwnTreasureChamber);
            if (other.CompareTag("BluePlayer"))
            {

                gameManager.blueScore += 1;
                
            }
            else
            {
                gameManager.redScore += 1;

            }
            //Remove Reference to Treasure
            player.treasure = null;
            player.hasTreasure = false;
            player.treasureDisplay.SetActive(false);
            //if(gameObject.CompareTag(player.targetColour + "c"))
        }

        //Maybe include that the Agent can not stack rewards.
        else if (gameObject.CompareTag(player.targetColour + "C") && !player.hasTreasure)
        {
            player.AddReward(player.rewardInsideTreasureChamber);
            print("Blue Base");
        }

    }

    
}
