using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Mini
{
    [System.Serializable]
    public class AuthorizedResponse
    {
        public string Uuid;
    }

    public abstract class SessionedBehaviour : HttpClientBehaviour
    {
        protected virtual void Start()
        {
            _ = StartCoroutine(Authorize());
        }

        IEnumerator Authorize()
        {
            using var req = UnityWebRequest.Get(RequestUrl("/authorize"));
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<AuthorizedResponse>(req.downloadHandler.text);
                OnSuccessfullyAuthorized(res.Uuid);
            }
            else
            {
                Debug.LogError(req.error);
                yield return SceneManager.LoadSceneAsync("Login");
            }
        }

        protected abstract void OnSuccessfullyAuthorized(string uuid);
    }
}
