using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A Single Treasure is stored in a <see cref="TreasureChamber"/>
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
