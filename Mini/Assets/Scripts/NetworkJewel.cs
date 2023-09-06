using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

namespace Mini
{
    public class NetworkJewel : NetworkBehaviour
    {
        [SerializeField]
        JewelMaterialList materialList;
        [SerializeField]
        MeshRenderer meshRenderer;

        JewelType jewelType = JewelType.NotSelected;
        NetworkObject networkObjectComponent;

        void Awake()
        {
            Assert.IsNotNull(materialList);
            Assert.IsNotNull(meshRenderer);
            Assert.IsTrue(TryGetComponent<NetworkObject>(out networkObjectComponent));
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            var jewelMat = materialList.Random();
            jewelType = jewelMat.type;
            meshRenderer.material = jewelMat.material;
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsServer)
            {
                if (other.TryGetComponent<NetworkCharacterController>(out var player))
                {
                    networkObjectComponent.Despawn();
                }
            }
        }
    }
}
