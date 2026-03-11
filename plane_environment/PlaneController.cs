using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEditor;

public class PlaneController : Agent
{
    [SerializeField] Transform shooter;
    Vector3 positionHolder;

    public void Awake()
    {
        positionHolder = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = positionHolder;
        shooter.GetComponent<ShootingScript>().bulletCount = 0;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        transform.GetComponent<Rigidbody2D>().velocity = 
            new Vector3(actions.ContinuousActions[1], actions.ContinuousActions[0]) * 100f;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Vertical");
        actions[1] = Input.GetAxis("Horizontal");
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Bullet"  || collision.tag == "Wall")
        {
            shooter.GetComponent<ShootingScript>().deleteAllPrefab();
            AddReward(-1f);
            EndEpisode();
        }
    }

    public void GainReward()
    {
        float distance = (positionHolder - transform.position).magnitude < 1 ? 1: (positionHolder - transform.position).magnitude;
        AddReward(1f/distance);
    }
}
