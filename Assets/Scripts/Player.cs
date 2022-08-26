using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    public NetworkVariable<Vector3> positionNetworkVariable = new();

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPointTransform;

    private int rotateSpeed = 200;

    private GameObject _projectileGameObject;
    private NetworkObject _projectileNetworkObject;


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }
    
    private void FixedUpdate()
    {
        transform.position = positionNetworkVariable.Value;

        RotatePlayer();
    }

    private void RotatePlayer()
    {
        if (IsClient && IsOwner)
        {
            Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal") * Time.fixedDeltaTime * rotateSpeed, 0);
            transform.Rotate(inputRotation, Space.World);
        }
    }

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            positionNetworkVariable.Value = GetRandomPositionOnPlane();
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }
    
    [ServerRpc]
    private void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        positionNetworkVariable.Value = GetRandomPositionOnPlane();
    }

    public void Fire()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnProjectile();
        }
        else
        {
            SubmitFireRequestServerRpc();
        }
    }
    
    [ServerRpc]
    private void SubmitFireRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        SpawnProjectile();
    }

    private void SpawnProjectile()
    {
        _projectileGameObject = Instantiate(_projectilePrefab, _spawnPointTransform.position, Quaternion.identity);

        _projectileNetworkObject = _projectileGameObject.GetComponent<NetworkObject>();
        _projectileNetworkObject.Spawn();
    }
    
    private static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }
}
