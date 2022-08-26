# basic-unity-netcode
- Creates UI buttons to join as 1 of 3 modes:
    - Host (Client & Server)
    - Client
    - Server

- After selecting a mode, it creates a UI buttons for your player
    - Moving:
        - On `Host`, it will say `Move` (updates the position NetworkVariable directly)
        - On `Client`, it will say `Request Position Change` (updates position NetworkVariable inside ServerRpc call)
    - Firing:
        - On `Host`, it will say `Fire`
        - On `Client`, it will say `Request Fire`
