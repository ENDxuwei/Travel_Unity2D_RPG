using UnityEngine;

/// <summary>
/// 玩家使用黑洞技能时的状态
/// </summary>
public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;


    private float defaultGravity;
    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 动画触发器
    /// </summary>
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    /// <summary>
    /// 进入状态，玩家上升，暂时取消重力
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;

        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
    }

    /// <summary>
    /// 退出状态，恢复默认重力
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
        // player.fx.MakeTransprent(false);
    }

    /// <summary>
    /// 黑洞状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 15);

        if (stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -.1f);

            if (!skillUsed)
            {
                if (player.skill.blackhole.CanUseSkill())
                    skillUsed = true;
            }
        }

        //技能结束后直接转换为空中状态
        if (player.skill.blackhole.SkillCompleted())
            stateMachine.ChangeState(player.airState);
    }
}
