using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineSystem : MonoBehaviour
{
    public GameObject balanceSystem;
    private float partnerHeightWithRadius;
    private float partnerHeightOnly;
    private PlayerController issuer;
    private PlayerController receiver;
    private void OnEnable()
    {
        Evently.Instance.Subscribe<CombineEvent>(OnCombine);
        Evently.Instance.Subscribe<SeperateEvent>(OnSeperate);
    }
    private void OnDisable()
    {
        Evently.Instance.Unsubscribe<CombineEvent>(OnCombine);
        Evently.Instance.Unsubscribe<SeperateEvent>(OnSeperate);
    }
    private void OnCombine(CombineEvent evt)
    {
        partnerHeightOnly = evt.issuer.GetComponent<CapsuleCollider>().height;
        partnerHeightWithRadius = partnerHeightOnly + evt.issuer.GetComponent<CapsuleCollider>().radius * 2;
        issuer = evt.issuer;
        receiver = evt.receiver;
        Debug.Log("combine success");
        //set state
        issuer.state = PlayerState.combine_upper;
        receiver.state = PlayerState.combine_under;

        //disable under's move and collider
        receiver.GetComponent<Rigidbody>().isKinematic = true;
        receiver.GetComponent<Collider>().enabled = false;

        //expand upper's collider
        var collider = issuer.GetComponent<CapsuleCollider>();
        collider.height *= 2f;
        collider.center = new Vector3(0, -partnerHeightOnly / 2.0f, 0);

        //set parent to sync the move
        var receiverTrans = receiver.transform;
        issuer.transform.position = new Vector3(receiverTrans.position.x, receiverTrans.position.y + partnerHeightWithRadius / 2.0f, receiverTrans.position.z);
        receiver.transform.parent = issuer.transform;
        //awake balance system
        balanceSystem.SetActive(true);
        balanceSystem.GetComponent<BalanceSystem>().balanceKey = receiver.interact;
       // balanceSystem.gameObject.SetActive(true);
    }
    private void OnSeperate(SeperateEvent  evt)
    {
        //set state
        issuer.state = PlayerState.single;
        receiver.state = PlayerState.single;
        //disable under's move and collider
        receiver.GetComponent<Rigidbody>().isKinematic = false;
        receiver. GetComponent<Collider>().enabled = true;
        //resize the collider
        var collider = issuer.GetComponent<CapsuleCollider>();
        collider.height /= 2f;
        collider.center = new Vector3(0, 0, 0);

        receiver.transform.parent = null;
        //disable balance system
        balanceSystem.SetActive(false);
        // balanceSystem.gameObject.SetActive(false);
        issuer = null;
        receiver = null;
    }
}
