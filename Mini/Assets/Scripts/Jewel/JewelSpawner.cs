using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;
using Unity.Collections;

namespace Mini
{
    public class JewelSpawner : MonoBehaviour
    {
        [SerializeField]
        JewelSettingList settingList;
        [SerializeField]
        int numSpawn;
        [Space]
        [SerializeField]
        GameObject jewelPrefab;
        [SerializeField]
        BoxCollider areaCollider;

        void Awake()
        {
            Assert.IsNotNull(settingList);
            Assert.IsNotNull(jewelPrefab);
            Assert.IsNotNull(areaCollider);
            Assert.IsTrue(numSpawn > 0);
        }

        public void SpawnJewels()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            if (!jewelPrefab.TryGetComponent<SphereCollider>(out var sphere))
            {
                Assert.IsTrue(false);
            }
            var bounds = areaCollider.bounds;
            var min = bounds.min;
            var max = bounds.max;
            var prefabTransform = jewelPrefab.transform;
            var pos = prefabTransform.position;
            var rot = prefabTransform.rotation;
            var posList = new NativeList<Vector3>(Allocator.Temp);
            var dia = sphere.radius * 2;

            for (int i = 0; i < numSpawn; i++)
            {
                pos.x = Random.Range(min.x, max.x);
                pos.z = Random.Range(min.z, max.z);
                for (int j = 0, length = posList.Length; j < length; j++)
                {
                    while (Mathf.Abs((posList[j] - pos).magnitude) < dia)
                    {
                        pos.x = Random.Range(min.x, max.x);
                        pos.z = Random.Range(min.z, max.z);
                    }
                }
                posList.Add(pos);
                
                var go = Instantiate(jewelPrefab, pos, rot);
                var setting = settingList.Random();
                if (go.TryGetComponent<NetworkJewel>(out var nj))
                {
                    nj.ApplySetting(setting.colorCode, setting.incrementOnCollect);
                }

                if (go.TryGetComponent<NetworkObject>(out var no))
                {
                    no.Spawn(true);
                }
            }

            posList.Dispose();
        }
    }
}
