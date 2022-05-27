using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    single,
    waitingForCombine,
    combine_under,
    combine_upper
};
public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidBody;

    public PlayerState state;
    public GameObject partner;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 processedInput;
    [Header("Input")]
    public string xInput;
    public string yInput;
    public KeyCode interact;

    [Header("Move")]
    public float speed;

    [Header("Combine")]
    public float combineDistance;//player can combine beneath the distance
    public float waitingTime;
    public BalanceSystem balanceSystem;

    private float partnerHeightWithRadius;
    private float partnerHeightOnly;



    // Start is called before the first frame update
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        state = PlayerState.single;
        partnerHeightOnly = partner.GetComponent<CapsuleCollider>().height;
        partnerHeightWithRadius = partnerHeightOnly+ partner.GetComponent<CapsuleCollider>().radius * 2;
    }

    // Update is called once per frame
    void Update()
    {
        //horizontal and vertical input
        horizontalInput = Input.GetAxis(xInput);
        verticalInput = Input.GetAxis(yInput);
        processedInput = transform.forward * verticalInput + transform.right * horizontalInput;
        if (horizontalInput == 0 && verticalInput == 0)
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);

        //Interact
        if (Input.GetKeyDown(interact))
        {
            var distance = Vector3.Distance(this.transform.position,partner.transform.position);
            if(distance > combineDistance)
            {
                return;
            }
            if(partner.GetComponent<PlayerController>().state==PlayerState.single)
            {
                StartCoroutine("WaitingForCombine");
            }
            else if( partner.GetComponent<PlayerController>().state == PlayerState.waitingForCombine)
            {
                Evently.Instance.Publish(new CombineEvent(partner.GetComponent<PlayerController>(),this));
            }
        }

        //check player state
        if(state==PlayerState.waitingForCombine)
        {
            var distance = Vector3.Distance(this.transform.position, partner.transform.position);
            if(distance>combineDistance)
            {
                state = PlayerState.single;
            }
        }
    }
    private void FixedUpdate()
    {
        if (state == PlayerState.combine_under)
            return;
        //move
        rigidBody.velocity = new Vector3(processedInput.x * speed * Time.fixedDeltaTime, rigidBody.velocity.y, processedInput.z * speed * Time.fixedDeltaTime);

        //rotate
        if (horizontalInput > 0 && verticalInput == 0)
            transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
        else if (horizontalInput > 0 && verticalInput > 0)
            transform.GetChild(0).localEulerAngles = new Vector3(0, 45, 0);
        else if (horizontalInput > 0 && verticalInput < 0)
            transform.GetChild(0).localEulerAngles = new Vector3(0, 135, 0);
        else if (horizontalInput == 0 && verticalInput > 0)
            transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
        else if (horizontalInput == 0 && verticalInput < 0)
            transform.GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
        else if (horizontalInput < 0 && verticalInput < 0)
            transform.GetChild(0).localEulerAngles = new Vector3(0, 225, 0);
        else if (horizontalInput < 0 && verticalInput == 0)
            transform.GetChild(0).localEulerAngles = new Vector3(0, 270, 0);
        else if (horizontalInput < 0 && verticalInput > 0)
            transform.GetChild(0).localEulerAngles = new Vector3(0, 315, 0);
    }
    //ienumrator for combining
    IEnumerator WaitingForCombine()
    {
        state = PlayerState.waitingForCombine;
        Debug.Log("wait for another player to confirm");
        yield return new WaitForSeconds(waitingTime);
        if(state== PlayerState.waitingForCombine)
        {
            state = PlayerState.single;
            Debug.Log("combine fail");
        }
    }
}
