using System.Collections;
using UnityEngine;

/// <summary>
/// 弓箭手战斗状态
/// </summary>
public class ArcherBattleState : EnemyState
{
    private Transform player;
    private Enemy_Archer enemy;
    private int moveDir;


    public ArcherBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态，获取玩家位置，判断玩家是否死亡
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);

    }

    /// <summary>
    /// 战斗状态，判断是否需要挑走，是否可以攻击
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            //小于安全距离，后跳
            if (enemy.IsPlayerDetected().distance < enemy.safeDistance)
            {
                if (CanJump())
                    stateMachine.ChangeState(enemy.jumpState);
            }

            //进入攻击距离，切换为攻击状态
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            // 玩家距离过远或战斗时间结束，切换为静止状态
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
                stateMachine.ChangeState(enemy.idleState);
        }

        BattleStateFlipController();

    }

    /// <summary>
    /// 控制转向
    /// </summary>
    private void BattleStateFlipController()
    {
        if (player.position.x > enemy.transform.position.x && enemy.facingDir == -1)
            enemy.Flip();
        else if (player.position.x < enemy.transform.position.x && enemy.facingDir == 1)
            enemy.Flip();
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 判断是否可以攻击
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

    /// <summary>
    /// 判断能否向后跳
    /// </summary>
    /// <returns></returns>
    private bool CanJump()
    {
        if (enemy.GroundBenhundCheck() == false || enemy.WallBehind() == true)
            return false;

        if (Time.time >= enemy.lastTimeJumped + enemy.jumpCooldown)
        {
            enemy.lastTimeJumped = Time.time;
            return true;
        }

        return false;
    }
}
