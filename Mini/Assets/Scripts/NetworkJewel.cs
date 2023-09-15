using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

namespace Mini
{
    public class NetworkJewel : NetworkBehaviour
    {
        [SerializeField]
        MeshRenderer meshRenderer;

        ColorCode colorCode = ColorCode.NotSelected;
        int increment = 0;
        NetworkObject networkObjectComponent;


        void Awake()
        {
            Assert.IsNotNull(meshRenderer);
            Assert.IsTrue(TryGetComponent(out networkObjectComponent));
        }

        public void ApplySetting(JewelSetting setting)
        {
            colorCode = setting.colorCode;
            increment = setting.incrementOnCollect;
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
                        collector.IncrementJewel(uuid, colorCode, increment);
                        networkObjectComponent.Despawn();
                    }
                }
            }
        }
    }
}
