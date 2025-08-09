using System.Collections;
using UnityEngine;

/// <summary>
/// 殉道者战斗状态
/// </summary>
public class ShadyBattleState : EnemyState
{
    private Transform player;
    private Enemy_Shady enemy;
    private int moveDir;

    public float defaultSpeed;

    public ShadyBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    /// <summary>
    /// 进入状态，赋予更快速度，获取玩家位置，判断玩家是否死亡
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        defaultSpeed = enemy.moveSpeed;

        enemy.moveSpeed = enemy.battleStateMoveSpeed;


        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);

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

            // 当玩家进入攻击范围，殉道者死亡
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
                enemy.stats.KillEntity();
        }
        else
        {
            //当玩家距离过远或战斗时间超时，转换为静止状态
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
                stateMachine.ChangeState(enemy.idleState);
        }

        //转向控制
        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;

        //速度控制
        if (Vector2.Distance(player.transform.position, enemy.transform.position) > 1)
            enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
        else
            enemy.SetZeroVelocity();

    }

    /// <summary>
    /// 退出状态，速度恢复默认速度
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        enemy.moveSpeed = defaultSpeed;
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