using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform Player_1;
    public Transform Player_2;
    private Vector3 offset;
    private Camera camera;
    private void Start()
    {
        offset = transform.position - (Player_1.position + Player_2.position) / 2;
        camera = this.GetComponent<Camera>();
    }
    private void Update()
    {
        if (Player_1 == null || Player_2 == null)
            return;
        transform.position = (Player_1.position + Player_2.position) / 2 + offset;
        float distance = Vector3.Distance(Player_1.position, Player_2.position);
        float size = distance * 0.6f;
        if (distance <= 5)
            return;
        camera.orthographicSize = size;
    }
}
