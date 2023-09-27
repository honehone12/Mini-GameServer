using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

namespace Mini
{
    public class NetworkJewel : NetworkBehaviour
    {
        [SerializeField]
        JewelSettingList settingList;
        [SerializeField]
        MeshRenderer meshRenderer;

        NetworkObject networkObjectComponent;
        ColorCode colorCodeForMaterial;
        int incrementOnTriggered;


        void Awake()
        {
            Assert.IsNotNull(settingList);
            Assert.IsNotNull(meshRenderer);
            Assert.IsTrue(TryGetComponent(out networkObjectComponent));
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                RequestJewelSettingServerRpc();
            }
        }

        public void ApplySetting(ColorCode colorCode, int increment)
        {
            colorCodeForMaterial = colorCode;
            incrementOnTriggered = increment;
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        public void RequestJewelSettingServerRpc(ServerRpcParams rpcParams = default)
        {
            ResponseJewelSettingClientRpc(
                colorCodeForMaterial,
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { rpcParams.Receive.SenderClientId }
                    }
                }
            );
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void ResponseJewelSettingClientRpc(ColorCode colorCode, ClientRpcParams rpcParams = default)
        {
            var setting = settingList.Find(colorCode);
            ApplySetting(setting.colorCode, setting.incrementOnCollect);
            meshRenderer.material = setting.material;
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsServer)
            {
                if (other.TryGetComponent<NetworkPlayer>(out var player) && 
                    other.TryGetComponent<NetworkJewelCollector>(out var collector))
                {
                    if (player.TryGetUserUuidServerCache(out var uuid))
                    {
                        collector.IncrementJewel(uuid, colorCodeForMaterial, incrementOnTriggered);
                        networkObjectComponent.Despawn();
                    }
                }
            }
        }
    }
}
