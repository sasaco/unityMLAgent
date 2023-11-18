using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class RollerAgent : Agent
{
    public Transform target; // Target��Transform
    Rigidbody rBody;         // RollerAgent��RigitBody

    /// <summary>
    /// �Q�[���I�u�W�F�N�g�������ɌĂ΂��
    /// </summary>
    public override void Initialize()
    {
        // RollerAgent��RigitBody�̎Q�Ǝ擾
        this.rBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// �G�s�\�[�h�J�n���ɌĂ΂��
    /// </summary>
    public override void OnEpisodeBegin()
    {
        // RollerEgent�������痎�����Ă���Ƃ�
        if(this.transform.localPosition.y < 0)
        {
            // RollerAgent�̈ʒu�Ƒ��x�����Z�b�g
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }

        // Target�̈ʒu���Z�b�g
        this.target.localPosition = new Vector3(
            Random.value*8-4, 0.5f, Random.value*8-4);
    }

    /// <summary>
    /// �ώ@�擾���ɌĂ΂��
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.target.localPosition.x); //Target��X���W
        sensor.AddObservation(this.target.localPosition.z); //Target��Z���W
        sensor.AddObservation(this.transform.localPosition.x); // RollerAgent��X���W
        sensor.AddObservation(this.transform.localPosition.z); // RollerAgent��Z���W
        sensor.AddObservation(this.rBody.velocity.x);   // RollerAgent��X���x
        sensor.AddObservation(this.rBody.velocity.z);   // RollerAgent��Z���x
    }

    /// <summary>
    /// �s�����莞�ɌĂ΂��
    /// </summary>
    public override void OnActionReceived(ActionBuffers actions)
    {
        // RollerAgent�ɗ͂�������
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        this.rBody.AddForce(controlSignal * 10);

        // RollerAgent��Target�̈ʒu�ɂ��ǂ蒅������
        float distanceToTarget = Vector3.Distance(
            this.transform.localPosition, this.target.localPosition);
        if(distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // RollerAgent�������痎��������
        if(this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }

    }

    /// <summary>
    /// Heuristic���[�h�̍s�����莞�ɌĂ΂��
    /// </summary>
    public override void Heuristic(in ActionBuffers actions)
    {
        var actionsOut = actions.ContinuousActions;
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");

    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
