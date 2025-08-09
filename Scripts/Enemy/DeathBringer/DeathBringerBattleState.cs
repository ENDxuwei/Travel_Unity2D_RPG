using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡使者战斗状态
/// </summary>
public class DeathBringerBattleState : EnemyState
{
    private Enemy_DeathBringer enemy;
    private Transform player;
    private int moveDir;

    public DeathBringerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态，获取玩家位置
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        //if (player.GetComponent<PlayerStats>().isDead)
        //stateMachine.ChangeState(enemy.moveState);
    }

    /// <summary>
    /// 战斗状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            // 如果玩家进入攻击范围，攻击
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
                else
                    stateMachine.ChangeState(enemy.idleState);
            }
        }

        //转向
        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;

        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - .1f)
            return;

        // 控制速度
        if (Vector2.Distance(player.transform.position, enemy.transform.position) > 1)
            enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
        else
            enemy.SetZeroVelocity();

    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 判断能否攻击
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