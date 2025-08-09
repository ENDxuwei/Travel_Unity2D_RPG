using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡使者传送状态
/// </summary>
public class DeathBringerTeleportState : EnemyState
{
    private Enemy_DeathBringer enemy;
    public DeathBringerTeleportState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态，赋予无敌
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        enemy.stats.MakeInvincible(true);
    }

    /// <summary>
    /// 传送状态，判断传送后状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            if (enemy.CanDoSpellCast())
                stateMachine.ChangeState(enemy.spellCastState);
            else
                stateMachine.ChangeState(enemy.battleState);
        }
    }

    /// <summary>
    /// 退出状态，结束无敌
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        enemy.stats.MakeInvincible(false);
    }
}