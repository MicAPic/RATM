using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(CarController))]
    public class CarAgent : Agent
    {
        public bool trainingMode;
        private bool _frozen = false;
        private CarController _carController;

        private void Awake()
        {
            _carController = GetComponent<CarController>();
        }
    
        // Start is called before the first frame update
        // void Start()
        // {
        //     
        // }

        // Update is called once per frame
        // void Update()
        // {
        //     
        // }

        /// <summary>
        /// Index 0: Braking & Acceleration values (from -1 to 1)<br></br>
        /// Index 1: Steering value (from -1 to 1)
        /// </summary>
        /// <param name="actions">Actions to take</param>
        public override void OnActionReceived(ActionBuffers actions)
        {
            if (_frozen) return;

            if (actions.ContinuousActions[0] < 0)
            {
                _carController.ProcessVerticalInput(actions.ContinuousActions[0], 0.0f);
            }
            else
            {
                _carController.ProcessVerticalInput(0.0f, actions.ContinuousActions[0]);
            }
            _carController.ProcessHorizontalInput(actions.ContinuousActions[1]);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.position);
        }
    }
}
