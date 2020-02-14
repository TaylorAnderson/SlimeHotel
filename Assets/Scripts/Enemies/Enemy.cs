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
      var otherSlime = col?.transform?.parent?.GetComponent<Slime>();
      if (otherSlime && otherSlime.color == this.color && otherSlime != this && !hitWallAfterThrown) {
        if (otherSlime.size == this.size && size < SlimeManager.instance.slimeProgression.Length - 1) {
          var nextSlime = SlimeManager.instance.slimeProgression[size + 1];
          var newSlimeObj = Instantiate(nextSlime);
          //align bottoms
          var midPos = (transform.position + otherSlime.transform.position) / 2;
          var newSlime = newSlimeObj.GetComponent<Slime>();
          newSlime.SetColor(this.color);
          newSlime.transform.position = Vector3.right * transform.position.x + Vector3.up * (GetBottom().y + newSlime.spriteRenderer.bounds.extents.y + 0.01f);
          this.toBeDeleted = otherSlime.toBeDeleted = true;
          newSlime.fsm.ChangeState(SlimeState.Combined);
          Destroy(gameObject);
          Destroy(otherSlime.gameObject);
        }
      }
    }

  }
  public virtual void Die(int dir) {
    this.pickupFSM.ChangeState(PickupState.None);
  }

}
