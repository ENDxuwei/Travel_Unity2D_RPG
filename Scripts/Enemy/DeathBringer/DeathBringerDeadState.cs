using UnityEngine;

/// <summary>
/// 死亡使者死亡状态
/// </summary>
public class DeathBringerDeadState : EnemyState
{
    private Enemy_DeathBringer enemy;
    public DeathBringerDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态
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
    /// 死亡状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 10);
    }
}