using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡区域，使落入区域的实体死亡并被销毁，防止无限坠落
/// </summary>
public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterStats>() != null)
            collision.GetComponent<CharacterStats>().KillEntity();
        else
            Destroy(collision.gameObject);
    }
}
