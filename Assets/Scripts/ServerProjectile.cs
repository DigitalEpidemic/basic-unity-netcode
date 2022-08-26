using System;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsServer)
            {
                Debug.Log("Hit Player");
                Destroy(transform.parent.gameObject);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Destroy(transform.parent.gameObject, 5f);
        }
    }
}
