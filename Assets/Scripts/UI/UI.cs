using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField] 
        private Material _transitionMaterial;
        
        public void LoadScene(string sceneName)
        {
            StartCoroutine(PrepareTransition(sceneName));
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
            #else
                Application.Quit();
            #endif
        }
        
        private IEnumerator PrepareTransition(string sceneToLoadAfter)
        {
            yield return new WaitForEndOfFrame();
            
            var screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            var screenRegion = new Rect(0, 0, Screen.width, Screen.height);
            screenTexture.ReadPixels(screenRegion, 0, 0, false);
            screenTexture.Apply(); // render the texture on GPU
            _transitionMaterial.SetTexture("_FadeTex", screenTexture);
                
            SceneManager.LoadScene(sceneToLoadAfter);
        }
    }
}
