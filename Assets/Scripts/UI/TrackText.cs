using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TrackText : MonoBehaviour
    {
        private TMP_Text _text;
        
        void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        void Start()
        {
            if (GameManager.Instance.mainTrack != null)
            {
                _text.text = "Next up: " + GameManager.Instance.mainTrack.name;
            }

            var textAnimation = DOTween.Sequence();
            textAnimation.PrependInterval(6.0f);
            textAnimation.Append(_text.DOText("Activating noise cancelling...", 1.3f));
        }
    }
}
