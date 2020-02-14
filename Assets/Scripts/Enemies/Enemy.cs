using UnityEngine;
using System.Collections.Generic;

public class Enemy : Pickup {
  public float dir;
  public bool dead;

  override public void ThrownUpdate() {
    base.ThrownUpdate();
    var filter = new ContactFilter2D();
    var results = new List<Collider2D>();
    Physics2D.OverlapCollider(collider, filter, results);

    for (int i = 0; i < results.Count; i++) {
      var col = results[i];
      var otherEnemy = col?.transform?.parent?.GetComponent<Enemy>();
      if (!otherEnemy) otherEnemy = col.gameObject.GetComponent<Enemy>();
      if (otherEnemy && otherEnemy != this && !hitWallAfterThrown) {
        this.currentGravityModifier = 15;
        otherEnemy.currentGravityModifier = 15;
        otherEnemy.Die(throwDir);
        this.Die(-throwDir);
      }
    }

  }
  public virtual void Die(int dir) {
    this.pickupFSM.ChangeState(PickupState.None);
  }

}
