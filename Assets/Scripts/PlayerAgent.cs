using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;
using Unity.MLAgents.Sensors;


public class PlayerRewards
{

    public float rewardInsideTreasureChamber;
    public float rewardtakingTreasureFromTreasureChamber;
    public float rewardtakingTreasureToOwnTreasureChamber;
    public float rewardRunningIntoBoundary;
    public float rewardGettingCaught;
    public float rewardWinningGame;


}



/// <summary>
/// A Player Machine Learning Agent
/// </summary>
public class PlayerAgent : Agent

{

    [Tooltip("Maxium velosity to the agent")]
    public int maxVelosity = 8;

    [Tooltip("Force to apply when moving")]
    public int moveForce = 4;

    [Tooltip("Speed to rotate")]
    public int rotspeed = 60;

    public float currentVelosity;

    [Tooltip("Display tresaure over the agents 'head'")]
    public GameObject treasureDisplay;

    [Tooltip("Whether the agent has Treasure or not")]
    public bool hasTreasure = false;

    [Tooltip("The treasure gameobject from the target")]
    public GameObject treasure;

    [Tooltip("Whether this is traning mode or gameplay mode")]
    public bool traningMode;

    [Tooltip("Whether the agent is in prision or not")]
    public bool inPrision = false;

    //TODO: Replace these ones with something better
    [Tooltip("Target Base")]
    public string targetColour;

    [Tooltip("Own Base")]
    public string chamberColour;

    //The rigidbody of the agent
    new private Rigidbody rigidbody;

    //Prison Reference
    private Prison prison;

    //Allows for smoother rotation changes
    private float smoothRotationChange = 0f;


    private GameManager gameManager;

    //Rewards

    [HideInInspector]
    public float rewardtakingTreasureFromTreasureChamber = 2f;
    [HideInInspector]
    public float rewardtakingTreasureToOwnTreasureChamber = 4f;
    [HideInInspector]
    public float penaltyRunningIntoBoundary = -0.2f;

    [HideInInspector]
    public float rewardWinningGame = 20f;

    [HideInInspector]
    public float rewardGettingCaught = -0.4f; //Not Used

    [HideInInspector]
    public float CurrentEpisodeSteps;

    private GameObject chamberT;
    private GameObject chamberD;

    //Whether the agent is frozen (intentionally not flying)
    private bool frozen = false;
    public float TargetBlue = 0.0f;
    public bool InEnemyAreaWithTreasure = false;


    private EnvConfig envConfig;

    /// <summary>
    /// Initalize the agent
    /// </summary>
    public override void Initialize()
    {
        envConfig = gameObject.GetComponentInParent<EnvConfig>();

        envConfig.setup();

        prison = envConfig.prison;
        gameManager = envConfig.gameManager;
        rigidbody = GetComponent<Rigidbody>();

        chamberD = envConfig.chamberD;
        chamberT = envConfig.chamberT;
    }

    /// <summary>
    /// Reset the agent when an episode begins
    /// </summary>
    public override void OnEpisodeBegin()
    {
        //To not give negative reward forever.
        InEnemyAreaWithTreasure = false;

        //Move to different Position depenting on team.
        if (gameObject.CompareTag("RedPlayer"))
        {
            transform.localPosition = new Vector3(-10, 1.5f, 0);
        }
        else
        {
            transform.localPosition = new Vector3(10, 1.5f, 0);
        }

        //Reset GameManager
        gameManager.ResetScore();

        //Remove possible treasures
        hasTreasure = false;
        treasure = null;
        treasureDisplay.SetActive(false);

        //New AI_272 to reset all factors. 
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        //Set all target Gameobject to true
        foreach (GameObject treasure in chamberT.GetComponent<TreasureChamber>().tresures)
        {
            treasure.SetActive(true);
        }

    }

    /// <summary>
    /// Called every step of the engine.
    /// 
    /// Called when and action is received from either the player or the netral network
    /// vectorAction[i] represents:
    ///Index 0: Move vector (+1=forwards, ((-1= backwards))) 
    ///Index 1: Rotation Y-axis (+1=clockwise , -1=anti-clockwise) 
    /// </summary>
    /// <param name="vectorAction">The actions to take</param>
    public override void OnActionReceived(float[] vectorAction)
    {


        if (frozen) return;

        // Gives the current speed of the player, used to create a terminal velocity for the player
        currentVelosity = rigidbody.velocity.magnitude;

        //Calculate local movement
        Vector3 move = transform.forward * vectorAction[0];

        if (vectorAction[0] > 0 && currentVelosity <= maxVelosity)
        {
            rigidbody.AddForce(move * moveForce);
        }


        // Get the current rotation
        float rotationY = transform.rotation.eulerAngles.y;

        //Calculate rotation of the Agent
        float rotationChange = vectorAction[1];

        // Calculate smooth rotation changes
        smoothRotationChange = Mathf.MoveTowards(smoothRotationChange, rotationChange, 10f * Time.fixedDeltaTime);

        // Calculate new rotation based on smoothed values

        float newRotation = rotationY + smoothRotationChange * Time.fixedDeltaTime * rotspeed;


        // Apply new rotation
        transform.rotation = Quaternion.Euler(0f, newRotation, 0f);


    }

    /// <summary>
    /// Collect vector observations from the environment
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {

        // Observe the agent's local rotation (1 observation)
        float rotation = transform.rotation.y;
        sensor.AddObservation(rotation);

        //Observe the agent's localPosition (3 observations)
        Vector3 localPosition = transform.localPosition;
        sensor.AddObservation(localPosition);

        float velosityRb = rigidbody.velocity.magnitude;

        //Observe the agent's speed (1 observation)
        //sensor.AddObservation(velosityRb);

        Vector3 directionSpeed = rigidbody.velocity.normalized;

        //Observe the direction of speed of the  agent's  (3 observation)
        //sensor.AddObservation(directionSpeed);

        //Direction Chamber

        float chamberDir = Vector3.Dot(transform.forward, (chamberD.transform.localPosition - transform.localPosition).normalized); //(transform.position - chamberD.transformD.position).normalize

        //(1 observation)
        sensor.AddObservation(chamberDir);

        //Distance Chamber Distance

        float chamberDis = (transform.localPosition - chamberD.transform.localPosition).magnitude; //transform.localposition.magnitude - chamberD.localpositon.magnitude.

        //(1 observation)
        sensor.AddObservation(chamberDis);

        //Target chamber direction
        float targetchamberDir = Vector3.Dot(transform.forward, (chamberT.transform.localPosition - transform.localPosition).normalized);

        //(1 observation)
        sensor.AddObservation(targetchamberDir);

        //Target Chamber Distance

        float targetchamberDis = (transform.localPosition - chamberT.transform.localPosition).magnitude;

        //(1 observation)
        sensor.AddObservation(targetchamberDis);

        //Observe whether the agent is in prison or not (1 observation)
        sensor.AddObservation(inPrision);

        //Observe whether the agent has a treasure (1 observation)
        sensor.AddObservation(hasTreasure);


        //10 total observations
    }

    /// <summary>
    /// When Behavior Type is set to "Heuristic Only" on the agent's Behavior Parameter
    /// <see cref="OnActionReceived(float[])"/> instead of using the neural network
    /// </summary>
    /// <param name="actionsOut">An output action array</param>
    public override void Heuristic(float[] actionsOut)
    {

        float forward = 0f;
        float rotation = 0f;

        //Forward

        if (Input.GetKey(KeyCode.UpArrow)) forward = 1f;


        if (Input.GetKey(KeyCode.RightArrow)) rotation = 1f;
        else if (Input.GetKey(KeyCode.LeftArrow)) rotation = -1f;


        actionsOut[0] = forward;
        actionsOut[1] = rotation;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //TODO: Draw a line to HomeChamber and Target Chamber
        Debug.DrawLine(transform.position, chamberT.transform.position, Color.green);
        Debug.DrawLine(transform.position, chamberD.transform.position, Color.red);
        CurrentEpisodeSteps = Academy.Instance.StepCount;


        float targetchamberDir = Vector3.Dot(transform.forward, (chamberT.transform.localPosition - transform.localPosition).normalized);

        TargetBlue = targetchamberDir;

    }



    void OnCollisionEnter(Collision other)
    {
        print("Yeee");

        if (gameObject.CompareTag("RedPlayer") && transform.localPosition.x >= 5 && other.collider.CompareTag("BluePlayer"))
        {
            print("NO!");
            AddReward(rewardGettingCaught);
            StartCoroutine(prison.PrisonTime(-18, 2, gameObject));

        }
        else if (gameObject.CompareTag("BluePlayer") && transform.localPosition.x <= -5 && other.collider.CompareTag("RedPlayer"))
        {
            print("NO!");
            AddReward(rewardGettingCaught);
            StartCoroutine(prison.PrisonTime(18, -2, gameObject));
        }
        else
        {
            print("Penalty Border");
            AddReward(penaltyRunningIntoBoundary);
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "bluefloor" && hasTreasure)
        {
            InEnemyAreaWithTreasure = false;
        }
        else if (
          other.gameObject.name == "bluefloor")
        {
            // print("ExitBlueArea");
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "bluefloor" && hasTreasure)
        {
            InEnemyAreaWithTreasure = true;
            //print("EnterBlueArea Treasure");
        }
        else if (
           other.gameObject.name == "bluefloor")
        {
            //print("EnterBlueArea");
        }
    }




    //Red player compadedel.
    /*private void OnTriggerEnter(Collider other)
    {
         if (other.gameObject.CompareTag("CloseToBlueC") && !hasTreasure)
        {
            AddReward(closeToTreasureChamber);
            print("Near blue Chamber");
        } else if(other.gameObject.CompareTag("CloseToRedC") && hasTreasure){
            AddReward(closeToTreasureChamber);
            print("Near Red Chamber");
        } else if (other.gameObject.CompareTag("RedBase") && hasTreasure)
        {
            AddReward(insideZone);
            print("Red Zone");
        }
        else if (other.gameObject.CompareTag("BlueBase") && !hasTreasure)
        {
            AddReward(insideZone);
            print("Blue Zone");
        } */


    /*private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("CloseToBlueC") && !hasTreasure)
        {
            AddReward(-closeToTreasureChamber);
            print("Exit Near blue Chamber");
        }
        else if (other.gameObject.CompareTag("CloseToRedC") && hasTreasure)
        {
            AddReward(-closeToTreasureChamber);
            print("Exit Near Red Chamber");
        }
    } */
}



