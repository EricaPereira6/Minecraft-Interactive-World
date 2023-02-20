using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveToTarget : Agent
{

    public Transform targetTransform;
    public Material winMaterial, loseMaterial, whiteMaterial;
    public MeshRenderer redLightRender, greenLightRender;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3.5f, 3.5f), transform.localPosition.y, Random.Range(-3.5f, -0.5f));
        targetTransform.localPosition = new Vector3(Random.Range(-3.5f, 3.5f), targetTransform.localPosition.y, Random.Range(0.5f, 3.5f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveSpeed = 5f;
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("target"))
        {
            SetReward(+1f);
            greenLightRender.material = winMaterial;
            redLightRender.material = whiteMaterial;
        }
        if (other.gameObject.CompareTag("wall"))
        {
            SetReward(-1f);
            redLightRender.material = loseMaterial;
            greenLightRender.material = whiteMaterial;
        }

        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxis("Horizontal");
        ca[1] = Input.GetAxis("Vertical");
    }
}
