using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家状态机
/// </summary>
public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    /// <summary>
    /// 状态机初始化
    /// </summary>
    /// <param name="_startState"></param>
    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    /// <summary>
    /// 状态切换函数
    /// </summary>
    /// <param name="_newState"></param>
    public void ChangeState(PlayerState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
