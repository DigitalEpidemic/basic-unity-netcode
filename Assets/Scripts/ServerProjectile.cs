using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerProjectile : NetworkBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int speed = 15;

    private GameObject _explosionGameObject;
    private NetworkObject _explosionNetworkObject;
    
    private void Update()
    {
        transform.position += -transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            Debug.Log("Hit Player");
            if (IsServer)
            {
                SpawnExplosion();
                Destroy(transform.parent.gameObject);
            }
        }
    }

    private void SpawnExplosion()
    {
        _explosionGameObject = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        _explosionNetworkObject = _explosionGameObject.GetComponent<NetworkObject>();
        _explosionNetworkObject.Spawn();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Destroy(transform.parent.gameObject, 5f);
        }
    }
}
