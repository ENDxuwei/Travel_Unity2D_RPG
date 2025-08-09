using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 黑洞技能系统
/// </summary>
public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackholeTimer;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private bool playerCanDisapear = true;

    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();

    public bool playerCanExitState { get; private set; }

    /// <summary>
    /// 黑洞技能构造函数
    /// </summary>
    /// <param name="_maxSize"></param>
    /// <param name="_growSpeed"></param>
    /// <param name="_shrinkSpeed"></param>
    /// <param name="_amountOfAttacks"></param>
    /// <param name="_cloneAttackCooldown"></param>
    /// <param name="_blackholeDuration"></param>
    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown, float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;

        blackholeTimer = _blackholeDuration;

        //如果镜像被水晶替代，那么玩家不需要隐身
        if (SkillManager.instance.clone.crystalInseadOfClone)
            playerCanDisapear = false;
    }

    /// <summary>
    /// 黑洞生命周期管理
    /// </summary>
    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if (blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;

            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackHoleAbility();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        //模拟黑洞缓慢增大
        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        //模拟黑洞收缩，最终消除
        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    #region 召唤镜像攻击

    /// <summary>
    /// 释放镜像攻击
    /// </summary>
    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;

        DestroyHotKeys();
        cloneAttackReleased = true;
        //防止攻击进行后仍有敌人进入导致出现多余的热键窗口
        canCreateHotKeys = false;

        if (playerCanDisapear)
        {
            playerCanDisapear = false;

            //确定目标使玩家透明，模拟闪烁效果
            PlayerManager.instance.player.fx.MakeTransprent(true);
        }
    }

    /// <summary>
    /// 镜像攻击逻辑
    /// </summary>
    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;

            //随机在敌人左右召唤镜像
            if (Random.Range(0, 100) > 50)
                xOffset = 0.8f;
            else
                xOffset = -0.8f;

            //根据玩家携带的技能选择使用水晶还是镜像进行攻击
            if (SkillManager.instance.clone.crystalInseadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }

            amountOfAttacks--;

            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility", 1f);
            }
        }
    }

    #endregion

    /// <summary>
    /// 黑洞技能结束，不再可以进行攻击判定，销毁热键
    /// </summary>
    private void FinishBlackHoleAbility()
    {
        DestroyHotKeys();
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;
        PlayerManager.instance.player.fx.MakeTransprent(false);
    }


    /// <summary>
    /// 敌人进入黑洞范围时，冻结敌人，生成热键窗口
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);
        }
    }

    /// <summary>
    /// 黑洞缩小时，取消对敌人的影响
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
            collision.GetComponent<Enemy>().FreezeTime(false);
    }

    #region 热键

    /// <summary>
    /// 创造热键显示窗口并分配给范围内的敌人
    /// </summary>
    /// <param name="collision"></param>
    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("没有足够的热键!");
            return;
        }

        if (!canCreateHotKeys)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    /// <summary>
    /// 将热键确认的敌人加入列表
    /// </summary>
    /// <param name="_enemyTransform"></param>
    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);

    /// <summary>
    /// 使用后销毁热键
    /// </summary>
    private void DestroyHotKeys()
    {
        if (createdHotKey.Count <= 0)
            return;

        for (int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }
    
    #endregion
}
