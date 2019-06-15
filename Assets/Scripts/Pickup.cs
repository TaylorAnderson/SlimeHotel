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
  public virtual void Awake() {
    pickupFSM.Bind(PickupState.Carried, CarriedEnter, CarriedUpdate, CarriedExit);
    pickupFSM.Bind(PickupState.Thrown, ThrownEnter, ThrownUpdate, ThrownExit);
  }
  public override void Update() {
    pickupFSM.Update();

    if (name == "Knight(Clone)") {
      Debug.Log(pickupFSM.currentState);

    }
  }

  public virtual void PickUp() {
    pickupFSM.ChangeState(PickupState.Carried);
  }
  public virtual void Throw(int dir) {
    this.throwDir = dir;
    this.transform.localScale = Vector3.right * dir + Vector3.up + Vector3.forward;

    hitWallAfterThrown = false;
    currentGravityModifier = 0;

    pickupFSM.ChangeState(PickupState.Thrown);
    wallSensor.Recalculate();

  }

  public void CarriedEnter() {

  }
  public void CarriedUpdate() {

  }
  public void CarriedExit() {

  }


  public void ThrownEnter() {
  }
  public void ThrownUpdate() {
    velocity = Vector2.right * throwDir * throwSpeed;
    wallSensor.Recalculate();
    if (wallSensor.colliding) {
      velocity = Vector2.zero;
      currentGravityModifier = gravityModifier;
      hitWallAfterThrown = true;
      HitWall();
    }
    if (grounded && hitWallAfterThrown) {
      pickupFSM.ChangeState(PickupState.None);
    }

  }
  public virtual void ThrownExit() {
  }
  public virtual void HitWall() {

  }


}
