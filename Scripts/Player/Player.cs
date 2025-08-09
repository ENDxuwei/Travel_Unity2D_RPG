using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 玩家类
/// </summary>
public class Player : Entity
{
    [Header("攻击设置")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    public bool isBusy { get; private set; }

    [Header("玩家移动设置")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("冲刺设置")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDir { get; private set; }


    public SkillManager skill { get; private set; }
    public GameObject sword { get; private set; }
    public PlayerFX fx { get; private set; }


    #region 状态
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }    
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerDashState dashState { get; private set; }

    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }

    public PlayerAimSwordState aimSowrd { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackholeState blackHole { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    /// <summary>
    /// 初始化状态机
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSowrd = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackholeState(this, stateMachine, "Jump");

        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    /// <summary>
    /// 初始化组件和属性
    /// </summary>
    protected override void Start()
    {
        base.Start();

        fx = GetComponent<PlayerFX>();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    /// <summary>
    /// 处理状态更新和输入检测
    /// </summary>
    protected override void Update()
    {
        //暂停时跳过
        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();

        CheckForDashInput();


        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
            skill.crystal.CanUseSkill();

        if (Input.GetKeyDown(KeyCode.Alpha1))
             Inventory.instance.UseFlask();
    }

    /// <summary>
    /// 玩家被减速
    /// </summary>
    /// <param name="_slowPercentage"></param>
    /// <param name="_slowDuration"></param>
    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);

    }

    /// <summary>
    /// 玩家恢复正常速度
    /// </summary>
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    /// <summary>
    /// 分配一把新飞剑
    /// </summary>
    /// <param name="_newSword"></param>
    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    /// <summary>
    /// 回收飞剑,进入接住飞剑状态
    /// </summary>
    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    /// <summary>
    /// 协程,使玩家在动作进行时得到短暂僵直,防止玩家在连招间隙输入操作导致玩家移位
    /// </summary>
    /// <param name="_seconds"></param>
    /// <returns></returns>
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    /// <summary>
    /// 动画触发器
    /// </summary>
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();


    /// <summary>
    /// 玩家冲刺
    /// </summary>
    private void CheckForDashInput()
    {   
        //防止贴墙滑墙状态仍然可以冲刺导致抽搐
        if (IsWallDetected())
            return;

        if (skill.dash.dashUnlocked == false)
            return;

        //技能解锁后按Shift冲刺
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {

            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }

    /// <summary>
    /// 玩家死亡
    /// </summary>
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    /// <summary>
    /// 设置玩家受到的冲击力为0
    /// </summary>
    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }
}
