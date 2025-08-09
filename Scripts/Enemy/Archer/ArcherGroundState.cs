using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弓箭手敌人地面状态
/// </summary>
public class ArcherGroundState : EnemyState
{

    protected Transform player;
    protected Enemy_Archer enemy;

    public ArcherGroundState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    /// <summary>
    /// 进入状态，获取玩家位置
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 地面状态，如果玩家进入寻敌范围，切换至战斗状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.agroDistance)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}