using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerProjectile : NetworkBehaviour
{
    [SerializeField] private int speed = 15;
    private void Update()
    {
        transform.position += -transform.forward * speed * Time.deltaTime;
    }
}