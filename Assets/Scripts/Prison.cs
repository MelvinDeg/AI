using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prison : MonoBehaviour
{
    private GameManager gameManager;
    private EnvConfig envConfig;
    //TODO: Run prision Stuff

    // Start is called before the first frame update
    void Start()
    {

        envConfig = gameObject.GetComponentInParent<EnvConfig>();
        gameManager = envConfig.gameManager;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// When Enter Prision
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        PlayerAgent playerAgent = other.GetComponent<PlayerAgent>();
        playerAgent.inPrision = true;
        print("IN PRISON");

    }
    /// <summary>
    /// When being released from Prision
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        PlayerAgent playerAgent = other.GetComponent<PlayerAgent>();
        playerAgent.inPrision = false;
        print("NOT IN PRISON");
    }
    /// <summary>
    /// Time in prision
    /// </summary>
    /// <param name="side">Were to spawn after beeing released from prision</param>
    /// <param name="cell">Were to spawn in prision after beeing caught</param>
    /// <returns></returns>
    public IEnumerator PrisonTime(int side, int cell, GameObject gameObject)
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        PlayerAgent playerAgent = gameObject.GetComponent<PlayerAgent>();

        if(playerAgent.treasure) Treasure.ReturnTreasureToChamber(playerAgent);
        
        //FIXED: Bug position
        gameObject.transform.localPosition = new Vector3(cell, 4.5f, 19);
        rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        yield return new WaitForSeconds(gameManager.prisionTime); //Time in cell
        gameObject.transform.localPosition = new Vector3(side, 1.5f, 0);
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }
}
