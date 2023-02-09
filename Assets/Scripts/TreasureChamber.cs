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

        //To remove Error in the start.
        if (player == null) return;

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

    
        else if (gameObject.CompareTag(player.targetColour + "C") && !player.hasTreasure)
        {

            foreach (GameObject treasure in tresures)
            {
                if (treasure.activeSelf)
                {
                    player.treasure = treasure;

                    // Checks that the treasure's chamber has the correct tag and that the player does not already carry one


                        player.AddReward(player.rewardtakingTreasureFromTreasureChamber);
                        // Activates the visual cube that the player carries and hides the other one
                        treasure.SetActive(false);
                        player.hasTreasure = true;
                        player.treasureDisplay.SetActive(true);
                        break;
                                       
                }

                }

                print("Blue Base");
        }

    }

    
}
