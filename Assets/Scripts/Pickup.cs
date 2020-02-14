using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupState {
  None,
  Carried,
  Thrown
}

public class Pickup : PhysicsObject {

  protected bool hitWallAfterThrown = false;
  protected int throwDir = 0;
  public float throwSpeed = 100f;
  public StateMachine<PickupState> pickupFSM = new StateMachine<PickupState>(PickupState.None);
  public Sensor wallSensor;

  public bool dropped = false;

  private float throwWallBufferTimer = 0.05f;
  private float throwWallBufferTime = 0.05f;
  public virtual void Awake() {
    pickupFSM.Bind(PickupState.Carried, CarriedEnter, CarriedUpdate, CarriedExit);
    pickupFSM.Bind(PickupState.Thrown, ThrownEnter, ThrownUpdate, ThrownExit);
  }
  public override void Update() {
    pickupFSM.Update();
  }

  public virtual void PickUp() {
    pickupFSM.ChangeState(PickupState.Carried);
  }


  public void CarriedEnter() {

  }
  public void CarriedUpdate() {

  }
  public void CarriedExit() {

  }

  public virtual void Throw(int dir) {
    this.throwDir = dir;
    dropped = dir == 0;
    if (!dropped) {
      this.transform.localScale = Vector3.right * dir + Vector3.up + Vector3.forward;
      currentGravityModifier = 0;
    }
    hitWallAfterThrown = false;


    pickupFSM.ChangeState(PickupState.Thrown);

  }


  public void ThrownEnter() {
    throwWallBufferTimer = throwWallBufferTime;
  }
  public virtual void ThrownUpdate() {
    throwWallBufferTimer -= Time.deltaTime;
    velocity = Vector2.right * throwDir * throwSpeed;
    wallSensor.Recalculate();
    var results = new RaycastHit2D[20];
    ContactFilter2D filter = new ContactFilter2D();
    filter.useTriggers = false;
    filter.layerMask = wallSensor.layerMask;
    filter.useLayerMask = true;
    if (rb2d.Cast(Vector2.right * throwDir, filter, results, 1) > 1 && throwWallBufferTimer < 0) {
      velocity = Vector2.zero;
      currentGravityModifier = gravityModifier;
      hitWallAfterThrown = true;
      HitWall();
    }
    if (grounded && (hitWallAfterThrown || dropped)) {
      pickupFSM.ChangeState(PickupState.None);
      if (dropped) {
        Debug.Log("Calling drop");
        Drop();
      }
    }

  }
  public virtual void ThrownExit() {
  }
  public virtual void HitWall() {

  }
  public virtual void Drop() {

  }

}
