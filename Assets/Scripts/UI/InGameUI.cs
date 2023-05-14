using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InGameUI : UI
    {
        [SerializeField]
        private float topSpeed;  // as of now, has to be determined empirically 
        [SerializeField]
        private Image speedometer;
        [SerializeField]
        private CarController player;

        void Awake()
        {
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            speedometer.fillAmount = Mathf.Abs(Mathf.Round(player.currentSpeed) / topSpeed);
        }
    }
}
