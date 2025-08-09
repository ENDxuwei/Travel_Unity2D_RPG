using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡使者攻击状态
/// </summary>
public class DeathBringerAttackState : EnemyState
{
    private Enemy_DeathBringer enemy;
    public DeathBringerAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        //攻击增加传送概率
        enemy.chanceToteleport += 5;
    }

    /// <summary>
    /// 退出状态，计录最后攻击时间
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time;
    }

    /// <summary>
    /// 攻击状态，攻击触发后进行传送判断，不成功则进入战斗状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        if (triggerCalled)
        {
            if (enemy.CanTeleport())
                stateMachine.ChangeState(enemy.teleportState);
            else
                stateMachine.ChangeState(enemy.battleState);
        }
    }
}