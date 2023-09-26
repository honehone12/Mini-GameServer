using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Mini
{
    public abstract class SessionedBehaviour : AuthHttpClientBase
    {
        [System.Serializable]
        public class AuthorizedResponse
        {
            public string Uuid;
            public string OneTimeId;
        }

        protected virtual void Start()
        {
            _ = StartCoroutine(Authorize());
        }

        IEnumerator Authorize()
        {
            using var req = UnityWebRequest.Get(AuthRequestUrl("/authorize"));
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<AuthorizedResponse>(req.downloadHandler.text);
                OnSuccessfullyAuthorized(res.Uuid, res.OneTimeId);
            }
            else
            {
                Debug.LogError(req.error);
                yield return SceneManager.LoadSceneAsync("Login");
            }
        }

        protected abstract void OnSuccessfullyAuthorized(string uuid, string oneTimeId);
    }
}
