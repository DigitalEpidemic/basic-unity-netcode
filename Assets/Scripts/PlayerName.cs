using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<FixedString64Bytes> displayName;

    private Camera _mainCamera;
    private TMP_Text _displayNameText;
    private Vector3 _previousPlayerRotation;
    
    private void Awake()
    {
        _displayNameText = GetComponent<TMP_Text>();
        _mainCamera = Camera.main;
    }
    
    private void OnEnable()
    {
        displayName.OnValueChanged += HandleDisplayNameChanged;
    }

    private void OnDisable()
    {
        displayName.OnValueChanged -= HandleDisplayNameChanged;
    }


    public override void OnNetworkSpawn()
    {
        RotateToFaceCamera();
        _displayNameText.text = displayName.Value.ToString();
        
        if (!IsServer) return;
        if (GameManager.GetPlayerName(OwnerClientId) != null)
        {
            displayName.Value = GameManager.GetPlayerName(OwnerClientId);;
                
        }
    }

    private void LateUpdate()
    {
        if (!_previousPlayerRotation.y.Equals(transform.parent.rotation.eulerAngles.y))
        {
            RotateToFaceCamera();
        }
    }

    private void RotateToFaceCamera()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.transform.position);
        _previousPlayerRotation = transform.parent.rotation.eulerAngles;
    }
    
    private void HandleDisplayNameChanged(FixedString64Bytes oldDisplayName, FixedString64Bytes newDisplayName)
    {
        _displayNameText.text = newDisplayName.ToString();
    }
}
