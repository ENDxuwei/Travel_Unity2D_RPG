using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡使者动画触发器
/// </summary>
public class Enemy_DeathBringerTrigger : EnemyAnimationTriggers
{
    private Enemy_DeathBringer enemydeathBringer => GetComponentInParent<Enemy_DeathBringer>();

    private void Relocate() => enemydeathBringer.FindPosition();

    private void MakeInvisivle() => enemydeathBringer.fx.MakeTransprent(true);
    private void MakeVisivle() => enemydeathBringer.fx.MakeTransprent(false);


}