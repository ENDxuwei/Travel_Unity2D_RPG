using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 玩家属性状态
/// </summary>
public class PlayerStats : CharacterStats
{
    private Player player;

    [Header("属性加成")]
    private int _strength;
    private int _agility;
    private int _intelligence;
    private int _vitality;
    private int _damage;
    private int _health;
    private int _mana;
    private int _fireDamage;
    private int _iceDamage;
    private int _lightningDamage;

    private int _exp;

    [Header("等级信息")]
    public int level = 1;
    public Stat expToLevelUp;
    public int exp;

    [Header("基础属性")]
    public int baseAttribute;
    public int baseHealth;
    public int baseMana;
    public int baseDamage;
    public int baseMagic;

    public bool isLevelUp = false;

    public System.Action onExpChanged;


    /// <summary>
    /// 初始化，获取组件
    /// </summary>
    protected override void Start()
    {
        base.Start();

        player= GetComponent<Player>();
    }

    /// <summary>
    /// 处理状态效果，管理是否能够升级
    /// </summary>
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 承受伤害
    /// </summary>
    /// <param name="_damage"></param>
    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    /// <summary>
    /// 玩家死亡
    /// </summary>
    protected override void Die()
    {
        base.Die();
        player.Die();

        GameManager.instance.lostCurrencyAmount = Mathf.RoundToInt(0.3f * PlayerManager.instance.currency);
        PlayerManager.instance.currency = Mathf.RoundToInt(0.7f * PlayerManager.instance.currency);

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    /// <summary>
    /// 当玩家生命值减少时，提供额外反馈逻辑，高伤害时的击退、屏幕震动和音效；触发当前装备盔甲的特效
    /// </summary>
    /// <param name="_damage"></param>
    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (isDead)
            return;

        if (_damage > GetMaxHealthValue() * .3f )
        {
            player.SetupKnockbackPower(new Vector2(10,6));
            player.fx.ScreenShake(player.fx.shakeHighDamage);


            int randomSound = Random.Range(31, 35);
            AudioManager.instance.PlaySFX(randomSound, null);
            
        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
             currentArmor.Effect(player.transform);
    }

    /// <summary>
    /// 闪避时生成镜像
    /// </summary>
    public override void OnEvasion()
    {
        // player.skill.dodge.CreateMirageOnDodge();
    }

    /// <summary>
    /// 镜像分身伤害函数，镜像伤害为玩家本体的因数倍
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <param name="_multiplier"></param>
    public void CloneDoDamage(CharacterStats _targetStats,float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        //暴击判定
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
    }

    /// <summary>
    /// 修改经验条
    /// </summary>
    /// <param name="_amount"></param>
    public virtual void IncreaseExpBy(int _amount)
    {
        exp += _amount;
        CanLevelUp();

        if (onExpChanged != null)
            onExpChanged();
    }

    /// <summary>
    /// 判断是否能够升级
    /// </summary>
    public void CanLevelUp()
    {
        if (exp >= expToLevelUp.GetValue())
        {
            LevelUp();
        }
        else
            return;
    }

    /// <summary>
    /// 移除属性，等级提升，扣除经验，重新增加属性，更新UI
    /// </summary>
    [ContextMenu("LevelUp")]
    public void LevelUp()
    {   
        if(level > 1)
            RemoveLevelModifiers();
        Modify();
        exp -= expToLevelUp.GetValue();
        level ++;
        ApplyLevelModifiers();
        Inventory.instance.UpdateStatsUI();
    }

    #region 等级成长
    /// <summary>
    /// 提升当前等级增加的属性
    /// </summary>
    public void ApplyLevelModifiers()
    {
        strength.AddModifier(_strength);
        agility.AddModifier(_agility);
        intelligence.AddModifier(_intelligence);
        vitality.AddModifier(_vitality);

        damage.AddModifier(_damage);

        maxHealth.AddModifier(_health);

        maxMana.AddModifier(_mana);
        fireDamage.AddModifier(_fireDamage);
        iceDamage.AddModifier(_iceDamage);
        lightningDamage.AddModifier(_lightningDamage);

        expToLevelUp.AddModifier(_exp);
    }

    /// <summary>
    /// 移除当前等级增加的属性
    /// </summary>
    public void RemoveLevelModifiers()
    {
        strength.RemoveModifier(_strength);
        agility.RemoveModifier(_agility);
        intelligence.RemoveModifier(_intelligence);
        vitality.RemoveModifier(_vitality);

        damage.RemoveModifier(_damage);

        maxHealth.RemoveModifier(_health);

        maxMana.RemoveModifier(_mana);
        fireDamage.RemoveModifier(_fireDamage);
        iceDamage.RemoveModifier(_iceDamage);
        lightningDamage.RemoveModifier(_lightningDamage);

        expToLevelUp.RemoveModifier(_exp);
    }

    /// <summary>
    /// 计算升级需要增加的属性
    /// </summary>
    public void Modify()
    {
        _strength = Mathf.RoundToInt(baseAttribute * MathForAttribute(level));
        _agility = Mathf.RoundToInt(baseAttribute * MathForAttribute(level));
        _intelligence = Mathf.RoundToInt(baseAttribute * MathForAttribute(level));
        _vitality = Mathf.RoundToInt(baseAttribute * MathForAttribute(level));
        _damage = Mathf.RoundToInt(baseDamage * MathForLevel(level));
        _health = Mathf.RoundToInt(baseHealth * MathForLevel(level));
        _mana = Mathf.RoundToInt(baseMana * MathForLevel(level));
        _fireDamage = Mathf.RoundToInt(baseMagic * MathForLevel(level));
        _iceDamage = Mathf.RoundToInt(baseMagic * MathForLevel(level));
        _lightningDamage = Mathf.RoundToInt(baseMagic * MathForLevel(level));
        _exp = Mathf.RoundToInt(100 + MathForEXP(level));
    }

    /// <summary>
    /// 二次属性成长公式
    /// </summary>
    /// <param name="x"></param>
    private float MathForLevel(int x)
    {
        float y = 0.01f * Mathf.Pow(x, 2) + 0.01f * x + 0.98f;
        y--;
        return y;
    }

    /// <summary>
    /// 线性属性成长公式
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private float MathForAttribute(int x)
    {
        float y = 0.35f * (x-1);
        return y;
    }

    /// <summary>
    /// 经验二次成长公式
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private float MathForEXP(int x)
    {
        float y = 10f * Mathf.Pow(x, 2) + 100f * x - 100;
        return y;
    }

    #endregion


}
