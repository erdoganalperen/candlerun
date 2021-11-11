using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    void Update () {
       var appliedPos= player.transform.position + new Vector3(0, 4.5f, -3.4f);
       appliedPos.x = 0;
       transform.position = appliedPos;
    }
}
