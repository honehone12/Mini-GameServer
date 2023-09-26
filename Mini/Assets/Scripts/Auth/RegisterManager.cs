using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Mini
{
    public class RegisterManager : AuthHttpClientBehaviour
    {
        public void OnLoginButtonClicked()
        {
            _ = StartCoroutine(LoadLoginScene());
        }

        IEnumerator LoadLoginScene()
        {
            yield return SceneManager.LoadSceneAsync("Login");
        }

        public void OnRegisterButtonClicked()
        {
            _ = StartCoroutine(SendRegisterRequest());
        }

        IEnumerator SendRegisterRequest()
        {
            var form = new WWWForm();
            form.AddField("email", emailTextBuffer);
            form.AddField("password", passwordTextBuffer);
            using var req = UnityWebRequest.Post(AuthRequestUrl("/register"), form);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                requestOKPanel.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                yield return SceneManager.LoadSceneAsync("Login");
            }
            else
            {
                Debug.LogError(req.error);
            }
        }
    }
}
