using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class PlayerRewards
{
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

    [HideInInspector]
    public string TeamColor;
    

    //The rigidbody of the agent
    new private Rigidbody rigidbody;

    //Prison Reference
    private Prison prison;

    //Allows for smoother rotation changes
    private float smoothRotationChange = 0f;

    //TODO: Add observation If other Team has Tresaure.

    //Rewards

    [HideInInspector]
    public float rewardtakingTreasureFromTreasureChamber = 2f / 10;
    [HideInInspector]
    public float rewardtakingTreasureToOwnTreasureChamber = 4f / 10;
    [HideInInspector]
    public float penaltyRunningIntoBoundary = -0.2f/10;

    [HideInInspector]

    public float PenaltyGettingCaught = -0.4f/10;
    [HideInInspector]
    public float rewardWinningGame = 20f/10;

    
    public int CurrentEpisodeSteps;

    //Whether the agent is frozen (intentionally not moving)
    private bool frozen = false;
    public float TargetBlue = 0.0f;
    private ArenaConfig arenaConfig;

    public BehaviorParameters BehaviorParameters;


    private void Start()
    {
        CurrentEpisodeSteps = Academy.Instance.StepCount;
        arenaConfig = gameObject.GetComponentInParent<ArenaConfig>();

        arenaConfig.Initialize();

        prison = arenaConfig.prison;
        rigidbody = GetComponent<Rigidbody>();
    }


    /// <summary>
    /// Initalize the agent
    /// </summary>
    public override void Initialize()
    {
        BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        if (BehaviorParameters.TeamId == 0)
        {
            TeamColor = "Red";
            print("RED");
        }
        else
        {
            TeamColor = "Blue";
            print("BLUE");
        }


    }

    /// <summary>
    /// Reset the agent when an episode begins
    /// </summary>
    public override void OnEpisodeBegin()
    {


        //Move to different Position depenting on team.
        if (gameObject.CompareTag("RedPlayer"))// Red Team
        {
            transform.localPosition = new Vector3(-10, 1.5f, 0);
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            transform.localPosition = new Vector3(10, 1.5f, 0);
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }


        arenaConfig.Setup();

        //Remove possible treasures
        hasTreasure = false;
        treasure = null;
        treasureDisplay.SetActive(false);

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

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

        //Direction Blue Chamber

        float blueChamberDir = Vector3.Dot(transform.forward, (arenaConfig.BlueChamber.transform.localPosition - transform.localPosition).normalized); //(transform.position - chamberD.transformD.position).normalize

        //(1 observation)
        sensor.AddObservation(blueChamberDir);

        //Distance Blue Chamber 

        float blueChamberDis = (transform.localPosition - arenaConfig.BlueChamber.transform.localPosition).magnitude; //transform.localposition.magnitude - chamberD.localpositon.magnitude.

        //(1 observation)
        sensor.AddObservation(blueChamberDis);

        //Direction Red Chamber 
        float redChamberDir = Vector3.Dot(transform.forward, (arenaConfig.RedChamber.transform.localPosition - transform.localPosition).normalized);

        //(1 observation)
        sensor.AddObservation(redChamberDir);

        //Red Chamber Distance 

        float redChamberDis = (transform.localPosition - arenaConfig.RedChamber.transform.localPosition).magnitude;

        //(1 observation)
        sensor.AddObservation(redChamberDis);

        //Observe whether the ownagent is in prison or not (1 observation)
        sensor.AddObservation(inPrision);

        //Observe whether the Opponent agent is in prison or not (1 observation)
        sensor.AddObservation(arenaConfig.BlueTeam[0].inPrision);

        //Observe whether blue team agent has a treasure (1 observation)
        sensor.AddObservation(arenaConfig.blueTeamTreasure);

        //Observe whether red team agent has a treasure (1 observation)
        sensor.AddObservation(arenaConfig.redTeamTreasure);

        sensor.AddObservation(arenaConfig.redScore);

        sensor.AddObservation(arenaConfig.blueScore);

        //14 total observations
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

        if (gameObject.CompareTag("RedPlayer"))
        {
            if (Input.GetKey(KeyCode.UpArrow)) forward = 1f;

            if (Input.GetKey(KeyCode.RightArrow)) rotation = 1f;
            else if (Input.GetKey(KeyCode.LeftArrow)) rotation = -1f;
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) forward = 1f;


            if (Input.GetKey(KeyCode.D)) rotation = 1f;
            else if (Input.GetKey(KeyCode.A)) rotation = -1f;
        }



        actionsOut[0] = forward;
        actionsOut[1] = rotation;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //TODO: Remove This.
        //TODO: Draw a line to HomeChamber and Target Chamber

        if (gameObject.CompareTag("RedPlayer"))
        {
            Debug.DrawLine(transform.position, arenaConfig.BlueChamber.transform.position, Color.green);
            Debug.DrawLine(transform.position, arenaConfig.RedChamber.transform.position, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, arenaConfig.RedChamber.transform.position, Color.green);
            Debug.DrawLine(transform.position, arenaConfig.BlueChamber.transform.position, Color.red);
        }


        CurrentEpisodeSteps = Academy.Instance.StepCount;


        //float targetchamberDir = Vector3.Dot(transform.forward, (chamberT.transform.localPosition - transform.localPosition).normalized);

        //TargetBlue = targetchamberDir;

    }

    void OnCollisionEnter(Collision other)
    {

        if (gameObject.CompareTag("RedPlayer") && transform.localPosition.x >= 5 && other.collider.CompareTag("BluePlayer"))
        {
            AddReward(PenaltyGettingCaught);
            StartCoroutine(prison.PrisonTime(-18, 2, gameObject));

            //Adding Positive Reward for the player that Caught the other
            other.gameObject.GetComponent<PlayerAgent>().AddReward(-PenaltyGettingCaught);


        }
        else if (gameObject.CompareTag("BluePlayer") && transform.localPosition.x <= -5 && other.collider.CompareTag("RedPlayer"))
        {

            AddReward(PenaltyGettingCaught);
            StartCoroutine(prison.PrisonTime(18, -2, gameObject));

            //Adding Positive Reward for the player that Caught the other
            other.gameObject.GetComponent<PlayerAgent>().AddReward(-PenaltyGettingCaught);
        }
        else if (other.collider.CompareTag("Boundary"))
        {
            print("Penalty Border");
            AddReward(penaltyRunningIntoBoundary);
        }

    }

    /*
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
    } */

}



