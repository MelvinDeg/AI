using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public int maxSpeed = 8;
    public int acceleration = 2;
    [Tooltip("Speed to rotate")]
    public int rotspeed = 30;
    new private Rigidbody rigidbody;
    public float vel;
    
    public GameObject treasureDisplay;
    public bool gotTreasure = false;

    //TODO: Save the treasure GameObject
    public GameObject treasure;
    /*public GameObject gameManager;
    GameManager gameMS; */
    // l, j and i for blue player
    private Prison prison;
    public string inputRight;
    public string inputLeft;
    public string inputForward;
    // what tag treasure has
    [Tooltip("What is")]
    public string targetColour;
    public string chamberColour;

    public bool inPrision = false;
    private EnvConfig envConfig;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        envConfig = gameObject.GetComponentInParent<EnvConfig>();
        prison = envConfig.prison;


    }


    void Update()
    {
        // Gives the current speed of the player, used to create a terminal velocity for the player
        vel = rigidbody.velocity.magnitude;
        
        // Rotates the player
        if (Input.GetKey(inputRight))
        {
            transform.Rotate(rotspeed * Time.deltaTime * Vector3.up);
        } else if (Input.GetKey(inputLeft)) {
            transform.Rotate(-rotspeed * Time.deltaTime * Vector3.up);
        }

        // Add force to the player as long as the player has not yer reached terminal velocity
        if (Input.GetKey(inputForward) && vel <= maxSpeed)
        {
            rigidbody.AddForce(transform.forward * acceleration);
        }

    
    }
    /*void OnTriggerEnter(Collider col)
    {

        // Checks that the treasure has the correct tag and that the player does not already carry one 
        if (col.CompareTag(targetColour) && gotTreasure == false)
        {
            // Activates the visual cube that the player carries and hides the other one
            col.gameObject.SetActive(false);
            gotTreasure = true;
            treasure.SetActive(true);
        }

        //FIX: Game Manager is asking for score

        if (col.CompareTag(chamberColour) && gotTreasure == true)
        {
            print("plus 1");
            if (gameObject.CompareTag("BluePlayer"))
            {

                gameMS.blueScore += 1;
            }
            else
            {
                gameMS.redScore += 1;
            }

            gotTreasure = false;
            treasure.SetActive(false);
        }
    } */

    void OnCollisionEnter(Collision col)
    {
        if (gameObject.CompareTag("RedPlayer") && transform.localPosition.x >= 5 && col.collider.CompareTag("BluePlayer"))
        {
            StartCoroutine(prison.PrisonTime(-18, 2, gameObject));

        } else if (gameObject.CompareTag("BluePlayer") && transform.localPosition.x <= -5 && col.collider.CompareTag("RedPlayer"))
        {
            StartCoroutine(prison.PrisonTime(18, -2, gameObject));
        }
    }

    /// <summary>
    /// Time in prision
    /// </summary>
    /// <param name="side">Were to spawn after beeing released from prision</param>
    /// <param name="cell">Were to spawn in prision after beeing caught</param>
    /// <returns></returns>
    /*IEnumerator PrisonTime(int side, int cell)
    {
        transform.position = new Vector3(cell, 4.5f, 19);
        rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        yield return new WaitForSeconds(gameMS.prisionTime); //Time in cell
        transform.position = new Vector3(side, 1.5f, 0);
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }*/
}
