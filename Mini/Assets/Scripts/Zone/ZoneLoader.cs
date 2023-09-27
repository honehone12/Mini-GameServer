using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace Mini
{
    [RequireComponent(typeof(BoxCollider))]
    public class ZoneLoader : MonoBehaviour
    {
        [SerializeField]
        string sceneName;
        [SerializeField]
        BoxCollider trigger;

        void Awake()
        {
            Assert.IsNotNull(trigger);
            Assert.IsFalse(string.IsNullOrEmpty(sceneName));
        }

        void OnDrawGizmos()
        {
            if (trigger != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(trigger.transform.position, trigger.size);
            }
            else
            {
                trigger = GetComponent<BoxCollider>();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (NetworkManager.Singleton.IsClient )
            {
                if (other.TryGetComponent<NetworkPlayer>(out var player))
                {
                    if (player.IsOwner)
                    {
                        _ = StartCoroutine(ShutdownThenLoaddNewScene());
                    }
                }
            }
        }

        IEnumerator ShutdownThenLoaddNewScene()
        {
            var nm = NetworkManager.Singleton;
            nm.Shutdown();
            yield return new WaitWhile(() => nm.ShutdownInProgress);

            Destroy(nm.gameObject);

            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}


