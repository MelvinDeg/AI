using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A Single Treasure is stored in a <see cref="TreasureChamber"/>
/// </summary>
public class Treasure : MonoBehaviour
{
    private string chamberColor;
    private ArenaConfig arenaConfig;
    // Start is called before the first frame update
    void Start()
    {
        chamberColor = gameObject.GetComponentInParent<TreasureChamber>().chamberColor;
        arenaConfig = gameObject.GetComponentInParent<ArenaConfig>();

    }

    // Update is called once per frame
    void Update()
    {
     
       
    }

    /// <summary>
    /// Return Treasure to Chamber
    /// </summary>
    /// <param name="player">Player that should return Treasure</param>
    public static void ReturnTreasureToChamber(PlayerAgent player)
    {
         ArenaConfig arenaConfig = player.GetComponentInParent<ArenaConfig>();
        //Deactivate the visual cube that the player carries and shows the other one

        player.treasure.SetActive(true);
            player.hasTreasure = false;

        if (player.CompareTag("RedPlayer"))
        {
            arenaConfig.redTeamTreasure = false;
        } else
        {
            arenaConfig.blueTeamTreasure = false;
        }

        player.treasure = null;
        player.treasureDisplay.SetActive(false);
        
    }

}
