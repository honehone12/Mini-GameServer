using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;
using Unity.Netcode.Components;
using Cinemachine;

namespace Mini
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkTransform))]
    public class NetworkCharacterController : NetworkBehaviour
    {
        [Header("Locomotion")]
        [SerializeField]
        float movementSpeed = 2.5f;
        [SerializeField]
        float rotationSpeed = 100.0f;
        [Header("Input")]
        [SerializeField]
        string movementHorizontal = "Horizontal";
        [SerializeField]
        string movementVertical = "Vertical";
        [SerializeField]
        string rotationHorizontal = "Mouse X";
        [SerializeField]
        string rotationVertical = "Mouse Y";
        [Header("Camera")]
        [SerializeField]
        CinemachineVirtualCamera virtualCamera;
        [SerializeField]
        Camera characterCamera;

        CharacterController controllerComponent;
        Transform transformComponent;
        // client use this buffer as last input, comparing and updating every frame.
        // server use this buffer as current input, updating with rpc and reading every frame; 
        Vector4 controlVectorBufer;

        void Awake()
        {
            Assert.IsNotNull(virtualCamera);
            Assert.IsNotNull(characterCamera);
            controllerComponent = GetComponent<CharacterController>();
            transformComponent = transform;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsClient && IsOwner)
            {
                Camera.SetupCurrent(characterCamera);
            }
            else
            {
                characterCamera.gameObject.SetActive(false);
                virtualCamera.gameObject.SetActive(false);
            }
        }

        void FixedUpdate()
        {
            if (IsClient && IsOwner)
            {
                // sending only newer input than last frame
                // reducing to 30fps
                if (Time.frameCount % 2 == 0)
                {
                    var controlVector = new Vector4(
                        Input.GetAxis(movementHorizontal),
                        Input.GetAxis(movementVertical),
                        Input.GetAxis(rotationHorizontal),
                        Input.GetAxis(rotationVertical)
                    );
                    if (controlVector != controlVectorBufer)
                    {
                        UpdateCharacterTransformServerRpc(controlVector);
                        controlVectorBufer = controlVector;
                    }
                }
            }

            if (IsServer)
            {
                var deltaTime = Time.fixedDeltaTime;
                var rotationVec = new Vector3(0f, controlVectorBufer.z, 0f);
                transformComponent.Rotate(rotationSpeed * deltaTime * rotationVec.normalized, Space.Self);
                var movementVec = new Vector3(controlVectorBufer.x, 0f, controlVectorBufer.y);
                movementVec = transformComponent.TransformDirection(movementVec.normalized);
                controllerComponent.Move(movementSpeed * deltaTime * movementVec);
            }
        }

        [ServerRpc(RequireOwnership = true, Delivery = RpcDelivery.Unreliable)]
        public void UpdateCharacterTransformServerRpc(Vector4 inputVector)
        {
            controlVectorBufer = inputVector;
        }
    }
}
