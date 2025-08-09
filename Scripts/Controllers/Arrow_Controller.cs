using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弓箭管理器
/// </summary>
public class Arrow_Controller : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private string targetLayerName = "Player";

    [SerializeField] private float xVelocity;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private bool canMove;
    [SerializeField] private bool flipped;

    private CharacterStats myStats;

    /// <summary>
    /// 赋予速度
    /// </summary>
    private void Update()
    {
        if (canMove)
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    /// <summary>
    /// 箭矢属性设置
    /// </summary>
    /// <param name="_speed"></param>
    /// <param name="_myStats"></param>
    /// <param name="_damage"></param>
    public void SetUpArrow(float _speed, CharacterStats _myStats,int _damage)
    {
        xVelocity = _speed;

        myStats = _myStats;

        damage = _damage;
    }


    /// <summary>
    /// 碰撞检测，若发生碰撞，则插入并计算是否造成伤害
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            myStats.DoDamage(collision.GetComponent<CharacterStats>(),1);
            StuckInto(collision);

        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StuckInto(collision);
        }

    }

    /// <summary>
    /// 箭矢射中目标后插入，一段时间后销毁
    /// </summary>
    /// <param name="collision"></param>
    private void StuckInto(Collider2D collision)
    {
        //停止粒子系统
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponent<Collider2D>().enabled = false;
        canMove = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = collision.transform;

        Destroy(gameObject, Random.Range(5, 7));
    }

    /// <summary>
    /// 翻转箭矢
    /// </summary>
    public void FlipArrow()
    {
        if (flipped)
            return;

        xVelocity = xVelocity * -1;
        flipped = true;
        transform.Rotate(0, 180, 0);
        targetLayerName = "Enemy";

    }
}