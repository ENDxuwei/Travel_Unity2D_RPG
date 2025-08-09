using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 骷髅敌人处于地面时的状态,通过此状态进行非战斗和战斗状态的转换
/// </summary>
public class SkeletonGroundedState : EnemyState
{
    protected Enemy_Skeleton enemy;
    protected Transform player;

    public SkeletonGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态
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
    /// 地面状态，检测寻敌范围内是否有玩家
    /// </summary>
    public override void Update()
    {
        base.Update();

        //玩家进入寻敌范围（视野）或玩家与敌人距离过近
        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.agroDistance )
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
