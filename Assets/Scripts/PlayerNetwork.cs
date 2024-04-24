using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public float movementSpeed = 3;

    [SerializeField] Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;

    Rigidbody rb;

    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<FixedString64Bytes> message = new NetworkVariable<FixedString64Bytes>("Testing!");

    // Use override OnNetworkSpawn for anything networking instead of Start or Awake.
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + "; randomNumber: " + randomNumber.Value + "; Message: "+message.Value);
        };
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
 
    void Update()
    {
        // Code only executes if they are the owner
        if(!IsOwner) return;

        // TESTING NETWORK VARIABLE
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Destroy(spawnedObjectTransform.gameObject);
        }

        // Movement controls
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Keep player looking in the direction of travel
        Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput);

        if(horizontalInput!=0 || verticalInput!=0)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }

        // Update player position based on controls
        rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);

    }

    // Another way to send data is through RPCs (Remote Procedure Call). Function name must contain "ServerRpc".
    // This code runs only on the server, not the client!
    [ServerRpc]
    private void TestServerRpc()
    {
        Debug.Log("TestServerRpc " + OwnerClientId);
        spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log("TestClientRpc");
    }
}
