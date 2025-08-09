using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 混乱法球效果
/// </summary>
[CreateAssetMenu(fileName = "混乱法球", menuName = "Data/Item effect/混乱法球")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform _respawnPosition)
    {
        Player player = PlayerManager.instance.player;

        bool thirdAttack = player.primaryAttack.comboCounter == 2;

        if (thirdAttack)
        {
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _respawnPosition.position, player.transform.rotation);
            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0);

            Destroy(newIceAndFire, 1.5f);
        }
    }
}
