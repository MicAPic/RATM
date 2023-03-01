using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : UI
    {
        [SerializeField] 
        private RectTransform disc;
        private static readonly int Active = Animator.StringToHash("Active");

        private void Awake()
        {
            // allow raycasts through transparent parts of the disc
            disc.GetComponent<Image>().alphaHitTestMinimumThreshold = 1.0f;
        }
        
        // Update is called once per frame
        void Update()
        {
            var mouseVerticalPos = Input.GetAxis("Mouse Y");
            
            disc.Rotate(0, 0, -mouseVerticalPos);
        }

        public void ToggleRadialGroup(Animator controller)
        {
            controller.SetBool(Active, !controller.GetBool(Active));
        }
    }
}
