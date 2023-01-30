using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvConfig : MonoBehaviour
{
    private string targetColour;
    private string chamberColour;


    [HideInInspector]
    public Prison prison;

    [HideInInspector]
    public GameManager gameManager;

    [HideInInspector]
    public GameObject chamberD;

    [HideInInspector]
    public GameObject chamberT;

    [HideInInspector]
    public PlayerAgent playerAgent;


    public void setup()
    {
         playerAgent= gameObject.transform.Find("Redplayer").GetComponent<PlayerAgent>();

        targetColour = playerAgent.targetColour;
        chamberColour = playerAgent.chamberColour;

        prison = gameObject.transform.Find("Prison").GetComponent<Prison>();
        gameManager = gameObject.transform.Find("GameManager").GetComponent<GameManager>();

        chamberD = gameObject.transform.Find(chamberColour + "TreasureChamber").gameObject;
        chamberT = gameObject.transform.Find(targetColour + "TreasureChamber").gameObject;

    }



}
