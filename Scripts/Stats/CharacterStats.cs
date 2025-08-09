using System.Collections;
using UnityEngine;

/// <summary>
/// 属性分类
/// </summary>
public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    mana,
    fireDamage,
    iceDamage,
    lightingDamage
}

/// <summary>
/// 角色属性与战斗系统
/// </summary>
public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("主要属性")]
    public Stat strength; // 每一点力量提升1点攻击伤害和0.2%的暴击伤害
    public Stat agility;  // 每一点敏捷增加0.15点闪避和0.1%暴击率
    public Stat intelligence; // 每一点智力增加1点魔法伤害和5点法力值
    public Stat vitality; // 每一点活力增加5点生命值

    [Header("进攻属性")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;   // 默认值为150%

    [Header("防御属性")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("魔法属性")]
    public Stat maxMana;
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;


    public bool isIgnited;   // 燃烧造成持续伤害
    public bool isChilled;   // 冰冷降低20%护甲，附加减速
    public bool isShocked;   // 震惊降低20%命中率，附加感电


    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;


    private float igniteDamageCoodlown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;
    public int currentHealth;
    public int currentMana;

    public System.Action onHealthChanged;
    public System.Action onManaChanged;
    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }
    private bool isVulnerable;

    /// <summary>
    /// 数据和特效初始化
    /// </summary>
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        currentMana = GetMaxManaValue();

        fx = GetComponent<EntityFX>();
    }

    /// <summary>
    /// 处理状态效果的计时器
    /// </summary>
    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;


        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        //如果施加点燃，调用持续伤害函数
        if (isIgnited)
            ApplyIgniteDamage();
    }

    /// <summary>
    /// 调用协程，施加脆弱效果
    /// </summary>
    /// <param name="_duration"></param>
    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableCoroutine(_duration));

    /// <summary>
    /// 协程，持续一段时间的脆弱效果
    /// </summary>
    /// <param name="_duartion"></param>
    /// <returns></returns>
    private IEnumerator VulnerableCoroutine(float _duartion)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duartion);

        isVulnerable = false;
    }

    /// <summary>
    /// 调用协程，临时修改属性
    /// </summary>
    /// <param name="_modifier"></param>
    /// <param name="_duration"></param>
    /// <param name="_statToModify"></param>
    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        // 开始协程临时修改属性
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    /// <summary>
    /// 协程，属性的临时修改与恢复
    /// </summary>
    /// <param name="_modifier"></param>
    /// <param name="_duration"></param>
    /// <param name="_statToModify"></param>
    /// <returns></returns>
    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    /// <summary>
    /// 伤害指定目标，可以通过调整value值调整伤害倍率
    /// </summary>
    /// <param name="_targetStats"></param>
    public virtual void DoDamage(CharacterStats _targetStats,float value)
    {
        bool criticalStrike = false;

        if (_targetStats.isInvincible)
            return;

        //可以闪避，则跳过伤害
        if (TargetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        //可以暴击则将伤害*暴击伤害
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
        }

        fx.CreateHitFx(_targetStats.transform, criticalStrike);

        //计算护甲减伤
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(Mathf.RoundToInt(totalDamage * value));

        //普通攻击附带魔法伤害
        //DoMagicalDamage(_targetStats);
    }

    #region 魔法伤害和异常状态

    /// <summary>
    /// 造成混合魔法伤害，取决于三种法术伤害的优势属性
    /// </summary>
    /// <param name="_targetStats"></param>
    public virtual void DoMagicalDamage(CharacterStats _targetStats,float value)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightningDamage.GetValue();

        int totalMagicalDamage = Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(Mathf.RoundToInt(totalMagicalDamage * value));

        //防止三种都为0时出现异常判断死循环
        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;

        //施加优势属性的异常
        AttemptyToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage);
    }

    /// <summary>
    /// 造成仅雷系的魔法伤害
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <param name="value"></param>
    public virtual void DoLightningDamage(CharacterStats _targetStats, float value)
    {
        int _lightingDamage = lightningDamage.GetValue();
        int totalMagicalDamage = _lightingDamage + intelligence.GetValue();
        _targetStats.TakeDamage(Mathf.RoundToInt(totalMagicalDamage * value));
        AttemptyToApplyAilements(_targetStats, 0, 0, _lightingDamage + 1);
    }

    /// <summary>
    /// 造成仅火系的魔法伤害
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <param name="value"></param>
    public virtual void DoFireDamage(CharacterStats _targetStats, float value)
    {
        int _fireDamage = fireDamage.GetValue();
        int totalMagicalDamage = _fireDamage + intelligence.GetValue();
        _targetStats.TakeDamage(Mathf.RoundToInt(totalMagicalDamage * value));
        AttemptyToApplyAilements(_targetStats, _fireDamage + 1, 0, 0);
    }

    /// <summary>
    /// 造成仅冰系的伤害
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <param name="value"></param>
    public virtual void DoIceDamage(CharacterStats _targetStats, float value)
    {
        int _iceDamage = iceDamage.GetValue();
        int totalMagicalDamage = _iceDamage + intelligence.GetValue();
        _targetStats.TakeDamage(Mathf.RoundToInt(totalMagicalDamage * value));
        AttemptyToApplyAilements(_targetStats, 0, _iceDamage + 1, 0);
    }

    /// <summary>
    /// 通过对比三种元素伤害的大小，判断应该让敌人陷入哪一种异常状态
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <param name="_fireDamage"></param>
    /// <param name="_iceDamage"></param>
    /// <param name="_lightningDamage"></param>
    private void AttemptyToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        //当有元素伤害相同时，随机判断敌人应该陷入哪种异常
        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3f && _fireDamage > 0 && _fireDamage == Mathf.Max(_fireDamage,_iceDamage,_lightningDamage))
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);

                return;
            }

            if (Random.value < .5f && _iceDamage > 0 && _iceDamage == Mathf.Max(_fireDamage, _iceDamage, _lightningDamage))
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);

                return;
            }

            if (Random.value < .5f && _lightningDamage > 0 && _lightningDamage == Mathf.Max(_fireDamage, _iceDamage, _lightningDamage))
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;

            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .1f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightningDamage * .3f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    /// <summary>
    /// 施加异常状态
    /// </summary>
    /// <param name="_ignite"></param>
    /// <param name="_chill"></param>
    /// <param name="_shock"></param>
    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;


        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }

        if (_chill && canApplyChill)
        {
            chilledTimer = ailmentsDuration;
            isChilled = _chill;

            //减速时间
            float slowPercentage = .2f;

            //调用目标被减速函数
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }

        if (_shock && canApplyShock)
        {
            //如果目标不处于震惊状态，施加震惊（感电）
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            //如果目标已经被震惊，触发感电，雷击附近的敌人并传递状态
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                //感电
                HitNearestTargetWithShockStrike();
            }
        }

    }

    /// <summary>
    /// 施加震惊状态
    /// </summary>
    /// <param name="_shock"></param>
    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        shockedTimer = ailmentsDuration;
        isShocked = _shock;

        fx.ShockFxFor(ailmentsDuration);
    }

    /// <summary>
    /// 雷击附近最近的敌人并传递震惊状态
    /// </summary>
    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)            
                closestEnemy = transform;
        }


        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    /// <summary>
    /// 持续造成点燃伤害
    /// </summary>
    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead)
                Die();

            igniteDamageTimer = igniteDamageCoodlown;
        }
    }

    /// <summary>
    /// 设置点燃伤害
    /// </summary>
    /// <param name="_damage"></param>
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    /// <summary>
    /// 设置感电伤害
    /// </summary>
    /// <param name="_damage"></param>
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion

    /// <summary>
    /// 削减目标血量，调用伤害击退函数，调用特效协程
    /// </summary>
    /// <param name="_damage"></param>
    public virtual void TakeDamage(int _damage)
    {

        if (isInvincible)
            return;

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0 && !isDead)
            Die();

    }

    /// <summary>
    /// 血量恢复，提升血条，最多恢复到满血
    /// </summary>
    /// <param name="_amount"></param>
    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }

    /// <summary>
    /// 降低血量，降低血条，显示伤害
    /// </summary>
    /// <param name="_damage"></param>
    protected virtual void DecreaseHealthBy(int _damage)
    {
        //脆弱时额外造成20%伤害
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.2f);

        currentHealth -= _damage;

        if (_damage > 0)
            fx.CreatePopUpText(_damage.ToString(),Color.red);

        if (onHealthChanged != null)
            onHealthChanged();
    }

    /// <summary>
    /// 法力恢复，提升法力条，最多恢复到100%
    /// </summary>
    /// <param name="_amount"></param>
    public virtual void IncreaseManaBy(int _amount)
    {
        currentMana += _amount;

        if (currentMana > GetMaxManaValue())
            currentMana = GetMaxManaValue();

        if (onManaChanged != null)
            onManaChanged();
    }

    /// <summary>
    /// 降低法力，降低法力，显示伤害
    /// </summary>
    /// <param name="_damage"></param>
    protected virtual void DecreaseManaBy(int _amount)
    {
        currentMana -= _amount;

        if (_amount > 0)
            fx.CreatePopUpText(_amount.ToString(),Color.blue);

        if (onManaChanged != null)
            onManaChanged();
    }

    /// <summary>
    /// 死亡
    /// </summary>
    protected virtual void Die()
    {
        isDead = true;
    }

    /// <summary>
    /// 杀死实体
    /// </summary>
    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    /// <summary>
    /// 无敌
    /// </summary>
    /// <param name="_invincible"></param>
    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;


    #region 属性数据计算
    /// <summary>
    /// 计算公式，百分比=数值/（数值+因数）
    /// </summary>
    /// <param name="value"></param>
    /// <param name="coefficient"></param>
    /// <returns></returns>
    protected float Calculating(float value,float coefficient)
    {
        float valueL = value + coefficient;
        float newValue = value / valueL;
        return newValue;
    }

    /// <summary>
    /// 计算护甲减伤
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <param name="totalDamage"></param>
    /// <returns></returns>
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        //计算护甲，冰冷状态下减少20%
        //护甲减伤公式为护甲/（护甲+100）
        float armorDamageReduce;
        if (_targetStats.isChilled) 
            armorDamageReduce = Calculating(_targetStats.armor.GetValue() * .8f, 100);
        else
            armorDamageReduce = Calculating(_targetStats.armor.GetValue(), 100);

        totalDamage -= Mathf.RoundToInt(totalDamage * armorDamageReduce);

        return totalDamage;
    }

    /// <summary>
    /// 计算法术抗性减伤
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <param name="totalMagicalDamage"></param>
    /// <returns></returns>
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        //法术抗性计算为法抗/（法抗+100）
        float magicDamageReduce = Calculating(_targetStats.magicResistance.GetValue() ,100);
        totalMagicalDamage -= Mathf.RoundToInt(totalMagicalDamage * magicDamageReduce);
        return totalMagicalDamage;
    }

    /// <summary>
    /// 触发闪避
    /// </summary>
    public virtual void OnEvasion()
    {

    }

    /// <summary>
    /// 目标可以闪避伤害
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <returns></returns>
    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        //闪避值等于闪避+敏捷*0.15
        float totalEvasion = _targetStats.evasion.GetValue() + (_targetStats.agility.GetValue() * 0.15f);

        //闪避几率等于闪避值/（闪避值+100）
        float currentEvasion = Calculating(totalEvasion,100);

        //震惊状态时，给敌人附加额外的20%闪避概率，模拟命中下降
        if (isShocked)
            currentEvasion += 0.2f;

        if (Random.Range(0, 100) < currentEvasion * 100)
        {
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 暴击判定
    /// </summary>
    /// <returns></returns>
    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + Mathf.RoundToInt(agility.GetValue() * 0.1f);

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 触发暴击时调用，暴击伤害
    /// </summary>
    /// <param name="_damage"></param>
    /// <returns></returns>
    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + Mathf.RoundToInt(strength.GetValue() * 0.2f)) * .01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    /// <summary>
    /// 获取最大生命值
    /// </summary>
    /// <returns></returns>
    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    /// <summary>
    /// 获取最大法力值
    /// </summary>
    /// <returns></returns>
    public int GetMaxManaValue()
    {
        return maxMana.GetValue() + intelligence.GetValue() * 5;
    }


    #endregion

    /// <summary>
    /// 获取属性类型
    /// </summary>
    /// <param name="_statType"></param>
    /// <returns></returns>
    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelligence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicRes) return magicResistance;
        else if (_statType == StatType.mana) return maxMana;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightningDamage;

        return null;
    }
}
