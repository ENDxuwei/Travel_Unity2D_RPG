using UnityEngine;

/// <summary>
/// 玩家反击状态
/// </summary>
public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfulCounterAttack", false);
    }

    /// <summary>
    /// 退出专案柜台
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 反击状态，速度设置为0
    /// </summary>
    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            // 反击箭矢
            if (hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().FlipArrow();
                SuccesfulCounterAttack();
            }

            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    SuccesfulCounterAttack();

                    player.skill.parry.UseSkill(); 

                    if (canCreateClone)
                    {
                        canCreateClone = false;
                        player.skill.parry.MakeMirageOnParry(hit.transform);
                    }
                }
            }
        }

        if (stateTimer < 0 || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    private void SuccesfulCounterAttack()
    {
        //必须大于1，防止与动画失调
        stateTimer = 10;
        player.anim.SetBool("SuccessfulCounterAttack", true);
    }
}
