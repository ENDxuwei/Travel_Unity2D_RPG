using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 自爆管理器
/// </summary>
public class Explosive_Controller : MonoBehaviour
{
    private Animator anim;
    private CharacterStats myStats;
    private float growSpeed = 15;
    private float maxSize = 6;
    private float explosionRadius;

    private bool canGrow = true;

    /// <summary>
    /// 增加大小，在达到临界时爆炸
    /// </summary>
    private void Update()
    {
        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
            // 动态调整爆炸半径
            explosionRadius = transform.localScale.x / 2; 
        }

        if (maxSize - transform.localScale.x < .01f)
        {
            canGrow = false;
            anim.SetTrigger("Explode");
        }
    }


    /// <summary>
    /// 设置爆炸
    /// </summary>
    /// <param name="_myStats"></param>
    /// <param name="_groSpeed"></param>
    /// <param name="_maxSize"></param>
    /// <param name="_radius"></param>
    public void SetupExplosive(CharacterStats _myStats, float _groSpeed, float _maxSize, float _radius)
    {
        anim = GetComponent<Animator>();

        myStats = _myStats;
        growSpeed = _groSpeed;
        maxSize = _maxSize;
        explosionRadius = _radius;
    }


    /// <summary>
    /// 动画触发，造成伤害
    /// </summary>
    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<CharacterStats>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                myStats.DoFireDamage(hit.GetComponent<CharacterStats>(),1);
            }
        }
    }

    /// <summary>
    /// 自我销毁
    /// </summary>
    private void SelfDestroy() => Destroy(gameObject);

}