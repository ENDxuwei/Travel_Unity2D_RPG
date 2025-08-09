using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 史莱姆战斗状态
/// </summary>
public class SlimeBattleState : EnemyState
{
    private Enemy_Slime enemy;
    private Transform player;
    private int moveDir;


    public SlimeBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// 进入状态，如果玩家死亡，则转换为移动状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);

    }

    /// <summary>
    /// 战斗状态，追踪玩家，检测是否进入攻击范围
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            // 玩家进入攻击范围，转为攻击状态
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
                stateMachine.ChangeState(enemy.idleState);
        }

        // 根据玩家方向调整追踪方向
        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;

        // 设置速度，玩家进入攻击范围时速度归零
        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - .1f)
            return;

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);

    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 根据最后攻击的时间和攻击间隔，判断能否攻击
    /// </summary>
    /// <returns></returns>
    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            enemy.lastTimeAttacked = Time.time;
            return true;
        }
        else
            return false;
    }
}
