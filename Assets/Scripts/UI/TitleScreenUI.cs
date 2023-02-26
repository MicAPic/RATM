using System.Collections;
using UnityEngine;

namespace UI
{
    public class TitleScreenUI : UI
    {
        // TODO: move the material reference to a GameManager
        [SerializeField] 
        private Material _transitionMaterial;
        
        // Update is called once per frame
        void Update()
        {
            if (Input.anyKey)
            {
                StartCoroutine(PrepareTransition());
            }
        }

        private IEnumerator PrepareTransition()
        {
            yield return new WaitForEndOfFrame();
            
            var screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            var screenRegion = new Rect(0, 0, Screen.width, Screen.height);
            screenTexture.ReadPixels(screenRegion, 0, 0, false);
            screenTexture.Apply(); // render the texture on GPU
            _transitionMaterial.SetTexture("_FadeTex", screenTexture);
                
            LoadScene("MainMenu");
        }
    }
}
