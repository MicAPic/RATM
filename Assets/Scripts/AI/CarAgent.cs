using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;
using Path = System.IO.Path;
using Random = UnityEngine.Random;

namespace AI
{
    [RequireComponent(typeof(CarController))]
    public class CarAgent : Agent
    {
        public bool trainingMode;

        [FormerlySerializedAs("minimalZ")]
        [Header("Training Settings")]
        [Tooltip("If the agent is below this mark, he's considered to be out of bounds")]
        [SerializeField]
        private float minimalY;
        [Tooltip("If the agent is within this threshold, he is considered to have visited a bug location")]
        [SerializeField]
        private float threshold = 1.0f;
        [SerializeField]
        private float minimalFps = 15.0f;

        [Header("Inference Settings")]
        [SerializeField]
        private string bugsOutputLocation = @"Assets\ML-Agents\Output";

        private string _currentOutputLocation;

        [Serializable]
        private class Output
        {
            public List<Vector3> visitedOobLocations = new List<Vector3>();
            public List<Vector3> visitedFpsLocations = new List<Vector3>();
        }
        private readonly Output _output = new();
        
        [Header("Spawn Settings")]
        [SerializeField]
        private LayerMask roadMask;
        [SerializeField]
        private Collider stageAreaCollider;

        [Space]
        [SerializeField]
        private Transform sphereTransform;
        
        private bool _frozen = false;
        private CarController _carController;

        private void Awake()
        {
            _carController = GetComponent<CarController>();
        }
    
        // Start is called before the first frame update
        void Start()
        {
            if (!TryGetComponent<BehaviorParameters>(out var behaviorParameters)) return;
            
            if (!trainingMode)
            {
                if (behaviorParameters.Model == null)
                {
                    Debug.LogWarning("Training mode is off and no model has been assigned!");
                    return;
                }

                _currentOutputLocation = Path.Join(bugsOutputLocation, DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"));
                if (!Directory.Exists(_currentOutputLocation)) Directory.CreateDirectory(_currentOutputLocation);
            }
        }

        // Update is called once per frame
        void Update()
        {
            CheckForOutOfBounds();
            CheckFps();
        }

        private void OnApplicationQuit()
        {
            if (!trainingMode)
            {
                Debug.Log($"Writing visited bug locations to {_currentOutputLocation}...");
                
                var outputJson = JsonUtility.ToJson(_output, prettyPrint: true);
                File.WriteAllText(Path.Join(_currentOutputLocation, "output.json"), outputJson);
            }
        }

        /// <summary>
        /// Reset the agent when an episode begins
        /// </summary>
        public override void OnEpisodeBegin()
        {
            if (trainingMode)
            {
                _output.visitedOobLocations.Clear();
                _output.visitedFpsLocations.Clear();
            }
            MoveToSafeRandomPosition();
        }

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
            sensor.AddObservation(transform.localPosition);
        }

        private void CheckForOutOfBounds()
        {
            var position = transform.localPosition;
            if (position.y > minimalY) return;
            
            if (!HasVisitedBugLocation(position))
            {
                if (trainingMode)
                    AddReward(1.0f);
                AddBugLocation(position, "out of bounds");
                MoveToSafeRandomPosition();
            }
        }

        private void CheckFps()
        {
            var fps = 1 / Time.unscaledDeltaTime;
            if (fps > minimalFps) return;

            if (!HasVisitedBugLocation(transform.localPosition))
            {
                if (trainingMode)
                    AddReward(.1f);
                AddBugLocation(transform.localPosition, "fps");
            }
        }

        private void AddBugLocation(Vector3 currentPosition, string type)
        {
            Debug.LogWarning($"Found a(n) {type} bug at: {currentPosition}");
            Color color;
            switch (type)
            {
                case "fps":
                    _output.visitedFpsLocations.Add(new Vector3(currentPosition.x, 0.0f, currentPosition.z));
                    color = Color.red;
                    break;
                case "out of bounds":
                    _output.visitedOobLocations.Add(new Vector3(currentPosition.x, 0.0f, currentPosition.z));
                    color = Color.cyan;
                    break;
                default:
                    color = Color.white;
                    break;
            }

            var upward = transform.TransformDirection(Vector3.up) * 100;
            Debug.DrawRay(transform.position, upward, color, float.PositiveInfinity);
        }

        private bool HasVisitedBugLocation(Vector3 currentPosition)
        {
            currentPosition = new Vector3(currentPosition.x, 0.0f, currentPosition.z);
            return _output.visitedOobLocations.Any(location => Vector3.Distance(location, currentPosition) < threshold) ||
                   _output.visitedFpsLocations.Any(location => Vector3.Distance(location, currentPosition) < threshold);
        }

        private void MoveToSafeRandomPosition()
        {
            var safePositionFound = false;
            var attemptsRemaining = 100; // Prevent an infinite loop
            var potentialPosition = Vector3.zero;

            while (!safePositionFound && attemptsRemaining > 0)
            {
                attemptsRemaining--;
                
                var bounds = stageAreaCollider.bounds;
                var offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
                var offsetY = Random.Range(-bounds.extents.y, bounds.extents.y);
                var offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);
                
                potentialPosition = bounds.center + new Vector3(offsetX, offsetY, offsetZ);
                
                if (Physics.Raycast(potentialPosition, Vector3.down, 100.0f, roadMask))
                {
                    var colliders = Physics.OverlapSphere(potentialPosition, 5f);
                    safePositionFound = colliders.Length == 0;
                }
            }
            
            sphereTransform.position  = potentialPosition;
            sphereTransform.localRotation = Quaternion.identity;
        }
    }
}
