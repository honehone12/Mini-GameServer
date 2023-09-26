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
            if (other.TryGetComponent<NetworkPlayer>(out _))
            {
                NetworkManager.Singleton.Shutdown();
                Destroy(NetworkManager.Singleton.gameObject);
                _ = StartCoroutine(LoaddScene());
            }
        }

        IEnumerator LoaddScene()
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}


