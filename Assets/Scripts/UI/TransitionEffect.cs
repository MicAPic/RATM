using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TransitionEffect : MonoBehaviour
    {
        [SerializeField] 
        private float transitionDuration = 1.0f;
        private Material _material;
        private static readonly int Progress = Shader.PropertyToID("_Progress");

        void Awake()
        {
            // TODO: move the material reference to a GameManager
            _material = GetComponent<RawImage>().material;
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            for (var t = 0.0f; t <= 1.0f; t += Time.deltaTime / transitionDuration)
            {
                var progress = Mathf.Lerp(1.0f, 0.0f, t);
                _material.SetFloat(Progress, progress);
                yield return null;
            }
            _material.SetFloat(Progress, 0.0f);
            
            GetComponent<RawImage>().raycastTarget = false;
        }

    }
}
