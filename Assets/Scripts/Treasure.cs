using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A Single Treasure  stored in a <see cref="TreasureChamber"/>
/// </summary>
public class Treasure : MonoBehaviour
{
    private string chamberColor;
    // Start is called before the first frame update
    void Start()
    {
        chamberColor = gameObject.GetComponentInParent<TreasureChamber>().chamberColor;
    }

    // Update is called once per frame
    void Update()
    {
     
       
    }

    /*void OnTriggerEnter(Collider other)
    {
        GameObject agent = other.gameObject;
        //PlayerAgent agentScript = agent.GetComponent<PlayerAgent>();
        PlayerAgent player = other.GetComponent<PlayerAgent>();
        //Save Reference to Treasure
        player.treasure = gameObject;

        // Checks that the treasure's chamber has the correct tag and that the player does not already carry one

        if (gameObject.CompareTag(player.targetColour) && player.hasTreasure == false)
        {
            player.AddReward(player.rewardtakingTreasureFromTreasureChamber);
            // Activates the visual cube that the player carries and hides the other one
            gameObject.SetActive(false);
            player.hasTreasure = true;
            player.treasureDisplay.SetActive(true);
        }

    }*/
    /// <summary>
    /// Return Treasure to Chamber
    /// </summary>
    /// <param name="player">Player that should return Treasure</param>
    public static void ReturnTreasureToChamber(PlayerAgent player)
    {
        //Deactivate the visual cube that the player carries and shows the other one

            player.treasure.SetActive(true);
            player.hasTreasure = false;
            player.treasure = null;
            player.treasureDisplay.SetActive(false);
        
    }

}
