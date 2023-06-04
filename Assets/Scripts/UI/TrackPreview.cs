using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class TrackPreview : MonoBehaviour
    {
        [SerializeField] 
        private CanvasGroup preview;
        [SerializeField] 
        private float fadeSpeed;
        [SerializeField] 
        private float markerAnimationSpeed;
        [SerializeField] 
        private RectTransform marker;
        [SerializeField] 
        private Vector2[] loopVertexPositions;

        private Sequence _sequence;
    
        // Start is called before the first frame update
        void Start()
        {
            _sequence = DOTween.Sequence();

            for (var i = 1; i < loopVertexPositions.Length; i++)
            {
                var vertex = loopVertexPositions[i];
                var distance = vertex - loopVertexPositions[i - 1];
                var sectionAnimationDuration = distance.sqrMagnitude / markerAnimationSpeed;
                Debug.Log(sectionAnimationDuration);
                
                _sequence.Append(marker.DOAnchorPos(vertex, sectionAnimationDuration).SetEase(Ease.Linear));
            }

            _sequence.SetLoops(-1, LoopType.Restart);
        }

        public void FadeGroup(float toAlpha)
        {
            preview.DOFade(toAlpha, fadeSpeed);
            if (toAlpha == 1.0)
            {
                _sequence.Restart();
            }
        }
    }
}
