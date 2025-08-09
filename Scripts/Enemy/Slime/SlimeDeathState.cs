using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 史莱姆死亡状态
/// </summary>
public class SlimeDeathState : EnemyState
{

    private Enemy_Slime enemy;
    public SlimeDeathState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// 进入状态，速度归零，播放最后状态的动画，调整计时器，关闭碰撞器
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetBool(enemy.lastAnimBoolName, true);
        enemy.anim.speed = 0;
        enemy.cd.enabled = false;

        stateTimer = .18f;
    }

    /// <summary>
    /// 计时结束后，赋予一个向上的力，模拟敌人死亡时先被打飞然后坠落消失
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 10);
    }
}