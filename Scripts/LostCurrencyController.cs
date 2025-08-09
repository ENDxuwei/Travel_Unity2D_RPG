using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家货币损失，当玩家接触到货币物体时，会自动拾取，随后销毁货币物体
/// </summary>
public class LostCurrencyController : MonoBehaviour
{
    public int currency;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Debug.Log("找回了遗失的魂");
            PlayerManager.instance.currency += currency;
            Destroy(this.gameObject);
        }
    }
}
