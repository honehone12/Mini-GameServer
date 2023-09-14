using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

namespace Mini
{
    public class NetworkJewel : NetworkBehaviour
    {
        [SerializeField]
        int incrementOnCollect = 1;
        [Space]
        [SerializeField]
        JewelMaterialList materialList;
        [SerializeField]
        MeshRenderer meshRenderer;

        ColorCode colorCode = ColorCode.NotSelected;
        NetworkObject networkObjectComponent;

        void Awake()
        {
            Assert.IsNotNull(materialList);
            Assert.IsNotNull(meshRenderer);
            Assert.IsTrue(TryGetComponent(out networkObjectComponent));
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            var jewelMat = materialList.Random();
            colorCode = jewelMat.colorCode;
            meshRenderer.material = jewelMat.material;
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
                        collector.IncrementJewel(uuid, colorCode, incrementOnCollect);
                        networkObjectComponent.Despawn();
                    }
                }
            }
        }
    }
}
