using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class MovingAgent : Agent
{
    public int checkpointIndex;
    public Rigidbody rigidBody;
    public Transform currentCheckpoint;
    public List<Wheel> wheels;
    public int maxPoints;
    public float speedCount;

    private void Awake()
    {
        CheckPointChecker.InitializeCheckpoints();
        int axelhelper = 0;
        Transform wheelList = transform.Find("Wheels").Find("Colliders");
        foreach (Transform wheel in wheelList)
        {
            Axel acceleration;
            if (axelhelper < 2) acceleration = Axel.Front;
            else acceleration = Axel.Rear;
            wheels.Add(new Wheel(wheel.gameObject, wheel.GetComponent<WheelCollider>(), acceleration));
            axelhelper++;
        }
        maxPoints = CheckPointChecker.getMaxCheckPoints();
    }

    public override void OnEpisodeBegin()
    {
        checkpointIndex = 0;
        currentCheckpoint = CheckPointChecker.getCheckPointFromIndex(checkpointIndex);
        rigidBody = transform.GetComponent<Rigidbody>();
        transform.localPosition = new Vector3(1.76f, 0.13f, 8.93f);
        transform.localRotation = Quaternion.Euler(0, 180, 0);
        foreach (Wheel wheel in wheels)
        {
            wheel.wheelCollider.rotationSpeed = 0;
        }
        rigidBody.velocity = Vector3.zero;
        rigidBody.Sleep();
        speedCount = rigidBody.velocity.magnitude;

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var steerAction = actions.ContinuousActions[0];
        var moveAction = actions.ContinuousActions[1];
        Move(moveAction);
        Steer(steerAction);
        AddReward(-0.001f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 obs = transform.GetComponent<Rigidbody>().velocity;
        sensor.AddObservation(currentCheckpoint.localPosition);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(obs);
        sensor.AddObservation(currentCheckpoint.position - transform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        /*ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = 1;
        actions[1] = 1;
        if (Input.GetKey(KeyCode.W)) actions[1] = 2;
        if (Input.GetKey(KeyCode.S)) actions[1] = 0;
        if (Input.GetKey(KeyCode.A)) actions[0] = 0;
        if (Input.GetKey(KeyCode.D)) actions[0] = 2;*/
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == currentCheckpoint)
        {
            AddReward(1f);
            if (checkpointIndex == CheckPointChecker.getMaxCheckPoints())
            {
                AddReward(1f);
                EndEpisode();
            }
            checkpointIndex++;
            currentCheckpoint = CheckPointChecker.getCheckPointFromIndex(checkpointIndex - 1);
        }
        else if (other.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    //Controller for driving
    public enum Axel
    {
        Front,
        Rear
    }
    
    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;

        public Wheel(GameObject go, WheelCollider wc, Axel axel)
        {
            this.wheelModel = go;
            this.wheelCollider = wc;
            this.axel = axel;
        }
    }

    [SerializeField]public float maxAcceleration = 100.0f;
    [SerializeField]public float brakeAcceleration = 1000.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;



    void Move(float moveInput)
    {
        float speedVector = Mathf.Cos(Vector3.Angle(transform.forward, rigidBody.velocity));
        Vector3 speedIndicator = transform.forward * speedVector; 
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = 400 * moveInput * maxAcceleration * Time.fixedDeltaTime;
            if(speedVector > 0 && moveInput < 0)
            {
                if (wheel.axel == Axel.Rear) wheel.wheelCollider.brakeTorque = 1000 * brakeAcceleration * Time.fixedDeltaTime;
                else wheel.wheelCollider.brakeTorque = 0;
            }
            else
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
        
    }
    void Steer(float steerInput)
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = _steerAngle;
            }
        }
    }
}
