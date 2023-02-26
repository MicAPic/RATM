using UnityEngine;

namespace Utility
{
    public class VerticalScroll : MonoBehaviour
    {
        [SerializeField] 
        private float speed;
        private Vector3 _startPos;
        private float _repeatWidth;
        private RectTransform _rectTransform;
        
        // Start is called before the first frame update
        void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _startPos = _rectTransform.localPosition;
            _repeatWidth = _rectTransform.rect.height / 2;
        }

        // Update is called once per frame
        void Update()
        {
            var position = _rectTransform.position;
            position = Vector3.Lerp(position, 
                                    position + Vector3.down * speed, 
                                    Time.deltaTime);
            _rectTransform.position = position;

            if (_rectTransform.localPosition.y < _startPos.y - _repeatWidth)
            {
                _rectTransform.localPosition = _startPos;
            }
        }
    }
}
