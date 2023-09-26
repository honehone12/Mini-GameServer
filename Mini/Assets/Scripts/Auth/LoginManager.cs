using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Mini
{
    public class LoginManager : AuthHttpClientBehaviour
    {
        public void OnSignUpButtonClicked()
        {
            _ = StartCoroutine(LoadRegisterScene());
        }

        IEnumerator LoadRegisterScene()
        {
            yield return SceneManager.LoadSceneAsync("Register");
        }

        public void OnLoginButtonClicked()
        {
            _ = StartCoroutine(SendLoginRequest());
        }

        IEnumerator SendLoginRequest()
        {
            var form = new WWWForm();
            form.AddField("email", emailTextBuffer);
            form.AddField("password", passwordTextBuffer);
            using var req = UnityWebRequest.Post(AuthRequestUrl("/login"), form);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                requestOKPanel.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                yield return SceneManager.LoadSceneAsync("Home");
            }
            else
            {
                Debug.LogError(req.error);
            }
        }
    }
}
