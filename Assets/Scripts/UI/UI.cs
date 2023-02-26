using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UI : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
            #else
                Application.Quit();
            #endif
        }
    }
}
