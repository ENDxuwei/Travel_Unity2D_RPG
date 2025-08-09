using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人属性与战斗系统
/// </summary>
public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stat soulsDropAmount;
    public Stat expDropAmount;

    [Header("等级信息")]
    public int level = 1;

    /// <summary>
    /// 初始化组件
    /// </summary>
    protected override void Start()
    {
        soulsDropAmount.SetDefaultValue(100);
        ApplyLevelModifiers();

        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    public int GetLevel()
    {
        return level;
    }

    #region 等级成长
    /// <summary>
    /// 等级属性增加值
    /// </summary>
    private void ApplyLevelModifiers()
    {
        ModifyForAttribute(strength);
        ModifyForAttribute(agility);
        ModifyForAttribute(intelligence);
        ModifyForAttribute(vitality);

        Modify(damage);

        Modify(maxHealth);
        ModifyForAromr(armor);
        ModifyForAromr(magicResistance);

        Modify(maxMana);
        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightningDamage);

        ModifyForDrop(soulsDropAmount);
        ModifyForDrop(expDropAmount);
    }

    /// <summary>
    /// 血量，攻击力和魔法伤害等级属性修正，二次幂增加
    /// </summary>
    /// <param name="_stat"></param>
    private void Modify(Stat _stat)
    {
        float baseValue = _stat.GetValue();
        float multiplier = baseValue * (MathForLevel(level)-1);
        _stat.AddModifier(Mathf.RoundToInt(multiplier));
    }

    /// <summary>
    /// 属性成长公式
    /// </summary>
    /// <param name="x"></param>
    private float MathForLevel(int x)
    {
        float y = 0.026f * Mathf.Pow(x,2) + 0.093f * x + 0.881f;
        return y;
    }

    /// <summary>
    /// 四维成长，分段线性增加
    /// </summary>
    /// <param name="_stat"></param>
    private void ModifyForAttribute(Stat _stat)
    {
        float baseValue = _stat.GetValue();
        int modifier = 0;

        // 1-10级增长率10%，11-30级5%，21+级3%
        if (level <= 10)
            modifier = Mathf.RoundToInt(baseValue * 0.1f * (level - 1));
        else if (level <= 30)
            modifier = Mathf.RoundToInt(baseValue * 0.1f * 9) +
                       Mathf.RoundToInt(baseValue * 0.05f * (level - 10));
        else
            modifier = Mathf.RoundToInt(baseValue * 0.1f * 9) +
                       Mathf.RoundToInt(baseValue * 0.05f * 20) +
                       Mathf.RoundToInt(baseValue * 0.03f * (level - 30));

        _stat.AddModifier(modifier);
    }

    /// <summary>
    /// 护甲抗性成长公式，线性增加
    /// </summary>
    /// <param name="_stat"></param>
    private void ModifyForAromr(Stat _stat)
    {
        float baseValue = _stat.GetValue();
        int modifier = Mathf.RoundToInt(baseValue * 0.005f * level);
        _stat.AddModifier(modifier);
    }

    /// <summary>
    /// 掉落物成长公式，线性增加
    /// </summary>
    /// <param name="_stat"></param>
    private void ModifyForDrop(Stat _stat)
    {
        float baseValue = _stat.GetValue();
        int modifier = Mathf.RoundToInt(baseValue * 0.1f * (level-1));
        _stat.AddModifier(modifier);
    }

    #endregion

    /// <summary>
    /// 伤害
    /// </summary>
    /// <param name="_damage"></param>
    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    /// <summary>
    /// 敌人死亡，玩家获得经验值
    /// </summary>
    protected override void Die()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        base.Die();
        enemy.Die();

        //玩家获得经验，生成掉落物
        PlayerManager.instance.currency += soulsDropAmount.GetValue();
        playerStats.IncreaseExpBy(expDropAmount.GetValue());
        myDropSystem.GenerateDrop();

        Destroy(gameObject, 5f);
    }
}
