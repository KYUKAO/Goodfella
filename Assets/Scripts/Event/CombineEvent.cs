using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineEvent : MonoBehaviour
{
    public PlayerController issuer;
    public PlayerController receiver;
    public CombineEvent(PlayerController _issuer,PlayerController _receiver)
    {
        issuer = _issuer;
        receiver = _receiver;
    }    
}
