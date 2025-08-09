using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡使者静止状态
/// </summary>
public class DeathBringerIdleState : EnemyState
{
    private Enemy_DeathBringer enemy;

    private Transform player;
    public DeathBringerIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态，获取玩家位置，计时
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
        player = PlayerManager.instance.player.transform;

    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.PlaySFX(24, enemy.transform);
    }

    /// <summary>
    /// 静止状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        // 玩家进入范围，开始boss战
        if (Vector2.Distance(player.transform.position, enemy.transform.position) < 7)
            enemy.bossFightBegun = true;


        if (Input.GetKeyDown(KeyCode.V))

            stateMachine.ChangeState(enemy.teleportState);

        // boss战开始时切换为战斗状态
        if (stateTimer < 0 && enemy.bossFightBegun)
            stateMachine.ChangeState(enemy.battleState);


    }

}