using System.Collections;
using UnityEngine;

/// <summary>
/// 殉道者地面状态
/// </summary>
public class ShadyGroundedState : EnemyState
{

    protected Transform player;
    protected Enemy_Shady enemy;

    public ShadyGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    /// <summary>
    /// 进入状态，获取玩家信息
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
    /// 地面状态，如果玩家进入寻敌范围，切换为战斗状态
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