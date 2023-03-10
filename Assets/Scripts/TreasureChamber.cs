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
    public GameObject[] tresures = new GameObject[AmountOfTresures];
    GameManager gameManager;
    private EnvConfig envConfig;

    // Start is called before the first frame update
    void Start()
    {
        chamberColor = gameObject.tag;

        envConfig = gameObject.GetComponentInParent<EnvConfig>();
        gameManager = envConfig.gameManager;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {

        PlayerAgent player = other.GetComponent<PlayerAgent>();

        //To remove Error at the start.
        if (player == null) return;

        // Checks that the treasureChamber has the correct tag and that the player does not already carry one treasure

        if (gameObject.CompareTag(player.chamberColour + "C") && player.hasTreasure)
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
