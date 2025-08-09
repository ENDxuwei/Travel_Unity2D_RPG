using System.Collections;
using UnityEngine;

/// <summary>
/// 弓箭手被眩晕状态
/// </summary>
public class ArcherStunnedState : EnemyState
{
    private Enemy_Archer enemy;

    public ArcherStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// 进入状态，开启红色闪烁特效，开始计时
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, .1f); 

        stateTimer = enemy.stunDuration;

        rb.velocity = new Vector2(-enemy.facingDir * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    /// <summary>
    /// 退出状态，关闭特效
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        enemy.fx.Invoke("CancelColorChange", 0);
    }

    /// <summary>
    /// 晕眩状态，计时
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);

    }
}