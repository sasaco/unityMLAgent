using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class RollerAgent : Agent
{
    public Transform target; // TargetのTransform
    Rigidbody rBody;         // RollerAgentのRigitBody

    /// <summary>
    /// ゲームオブジェクト生成時に呼ばれる
    /// </summary>
    public override void Initialize()
    {
        // RollerAgentのRigitBodyの参照取得
        this.rBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// エピソード開始時に呼ばれる
    /// </summary>
    public override void OnEpisodeBegin()
    {
        // RollerEgentが床から落下しているとき
        if(this.transform.localPosition.y < 0)
        {
            // RollerAgentの位置と速度をリセット
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }

        // Targetの位置リセット
        this.target.localPosition = new Vector3(
            Random.value*8-4, 0.5f, Random.value*8-4);
    }

    /// <summary>
    /// 観察取得時に呼ばれる
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.target.localPosition.x); //TargetのX座標
        sensor.AddObservation(this.target.localPosition.z); //TargetのZ座標
        sensor.AddObservation(this.transform.localPosition.x); // RollerAgentのX座標
        sensor.AddObservation(this.transform.localPosition.z); // RollerAgentのZ座標
        sensor.AddObservation(this.rBody.velocity.x);   // RollerAgentのX速度
        sensor.AddObservation(this.rBody.velocity.z);   // RollerAgentのZ速度
    }

    /// <summary>
    /// 行動決定時に呼ばれる
    /// </summary>
    public override void OnActionReceived(ActionBuffers actions)
    {
        // RollerAgentに力を加える
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        this.rBody.AddForce(controlSignal * 10);

        // RollerAgentがTargetの位置にたどり着いた時
        float distanceToTarget = Vector3.Distance(
            this.transform.localPosition, this.target.localPosition);
        if(distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // RollerAgentが床から落下した時
        if(this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }

    }

    /// <summary>
    /// Heuristicモードの行動決定時に呼ばれる
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
