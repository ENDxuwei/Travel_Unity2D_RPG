using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 装备类型
/// </summary>
public enum EquipmentType
{
    Weapon,
    Staff,
    Armor,
    Fibre,
    Amulet,
    Pendant,
    Flask
}

public enum EquipmentRarity
{
    N,
    R,
    SR,
    UR
}

/// <summary>
/// 装备数据类SO文件，管理游戏中可装备物品的属性和效果
/// </summary>
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    public EquipmentRarity equipmentRarity;

    [Header("独特效果")]
    public float itemCooldown;
    public ItemEffect[] itemEffects;


    [Header("主要属性")]
    public int strength;
    public int agility;
    public int intelligence;
    public int vitality;

    [Header("进攻属性")]
    public int damage;
    public int critChance;
    public int critPower;

    [Header("防御属性")]
    public int health;
    public int armor;
    public int evasion;
    public int magicResistance;

    [Header("魔法属性")]
    public int mana;
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;


    [Header("制作工艺")]
    
    public List<InventoryItem> craftingMaterials;

    private int descriptionLength;

    /// <summary>
    /// 在敌人位置触发效果
    /// </summary>
    /// <param name="_enemyPosition"></param>
    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }

    /// <summary>
    /// 装备时为玩家添加属性加成
    /// </summary>
    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.maxMana.AddModifier(mana);
        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);
    }

    /// <summary>
    /// 卸下装备时移除属性加成
    /// </summary>
    public void RemoveModifiers() 
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);


        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);
        

        playerStats.maxHealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.maxMana.RemoveModifier(mana);
        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);
    }

    /// <summary>
    /// 生成装备的属性描述文本，包括基础属性和特效说明
    /// </summary>
    /// <returns></returns>
    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "力量");
        AddItemDescription(agility, "敏捷");
        AddItemDescription(intelligence, "智力");
        AddItemDescription(vitality, "体力");

        AddItemDescription(damage, "攻击力");
        AddItemDescription(critChance, "暴击几率");
        AddItemDescription(critPower, "暴击伤害");

        AddItemDescription(health, "生命值");
        AddItemDescription(evasion, "闪避");
        AddItemDescription(armor, "护甲");
        AddItemDescription(magicResistance, "魔法抗性");

        AddItemDescription(mana, "法力");
        AddItemDescription(fireDamage, "火焰伤害");
        AddItemDescription(iceDamage, "寒冰伤害");
        AddItemDescription(lightningDamage, "闪电伤害 ");

        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i].effectDescription.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("技能: " + itemEffects[i].effectDescription);
                descriptionLength++;
            }
        }

        if (descriptionLength < 5)
        {
            for (int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }
        
        return sb.ToString();
    }


    /// <summary>
    /// 添加物品描述
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_name"></param>
    private void AddItemDescription(int _value, string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();

            if (_value > 0)
                sb.Append("+ " + _value + " " + _name);

            descriptionLength++;
        }       
    }

    /// <summary>
    /// 显示装备名称时将类型转换为中文
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public string TypeToString(EquipmentType _type)
    {
        if (_type == EquipmentType.Weapon) return ("武器");
        else if (_type == EquipmentType.Staff) return ("法器");
        else if (_type == EquipmentType.Armor) return ("护甲");
        else if (_type == EquipmentType.Fibre) return ("插板");
        else if (_type == EquipmentType.Amulet) return ("护身符");
        else if (_type == EquipmentType.Pendant) return ("饰品");
        else if (_type == EquipmentType.Flask) return ("药水");

        return null;
    }

    /// <summary>
    /// 根据稀有度显示颜色
    /// </summary>
    /// <param name="_rarity"></param>
    /// <returns></returns>
    public Color ColorToRarity(EquipmentRarity _rarity)
    {
        if(_rarity == EquipmentRarity.N) return Color.gray;
        else if (_rarity == EquipmentRarity.R) return Color.green;
        else if (_rarity == EquipmentRarity.SR) return Color.blue;
        else if (_rarity == EquipmentRarity.UR) return Color.red;

        return Color.white;
    }
}
