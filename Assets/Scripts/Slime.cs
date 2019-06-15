using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

public enum SlimeState {
  Combined,
  Normal,
  Knocking,
  MovingIn,
  Overriden,
  Broken,
  Dead,
}
public class Slime : Pickup {
  public Sensor platformSensor;
  public float speed = 0.1f;

  public int size;

  public float postCombinePauseInSeconds = 1;
  public Color color = new Color(63, 171, 62);
  public GameObject outline;
  public GameObject pickupTrigger;

  protected bool toBeDeleted = false;

  public StateMachine<SlimeState> fsm = new StateMachine<SlimeState>(SlimeState.Knocking);
  private SpriteAnim animator;
  private int dir = 1;
  private float originalGravity;

  private float wobbleIntensity = 1.2f;
  private float wobbleSpeed = 20f;
  private float currentWobbleIntensity = 1.2f;
  private float currentWobbleSpeed = 1.5f;
  private bool hittingOtherSlime = false;

  public override void Awake() {
    base.Awake();

    spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

    animator = spriteRenderer.GetComponent<SpriteAnim>();
    ResetDir();
    fsm.Bind(SlimeState.Combined, CombinedEnter, CombinedUpdate, CombinedExit);
    fsm.Bind(SlimeState.Normal, NormalEnter, NormalUpdate, NormalExit);
    fsm.Bind(SlimeState.Knocking, KnockingEnter, KnockingUpdate, KnockingExit);
    fsm.Bind(SlimeState.Overriden, null, OverriddenUpdate, null);
    fsm.Bind(SlimeState.Broken, null, BrokenUpdate);
    fsm.Bind(SlimeState.Dead, null, DeadUpdate, null);
    SetColor(color);
  }

  // Update is called once per frame
  override public void Update() {
    base.Update();
    fsm.Update();
  }
  override public void PickUp() {
    base.PickUp();
    fsm.ChangeState(SlimeState.Overriden);
  }
  public override void Throw(int dir) {
    base.Throw(dir);
    fsm.ChangeState(SlimeState.Overriden);
  }

  public void SetColor(Color color) {
    this.color = color;
    this.spriteRenderer.material.SetVector("_NewColor", color);
  }
  public void ResetDir() {
    dir = Mathf.RoundToInt(transform.localScale.x);
  }

  void NormalEnter() {
    animator.Resume();
  }
  void NormalUpdate() {
    this.velocity = (Vector3)Vector2.right * this.dir * speed * Time.deltaTime;
    if (!platformSensor.colliding || wallSensor.colliding) {
      dir *= -1;
    }
    this.transform.localScale = Vector3.right * dir + Vector3.up + Vector3.forward;
  }
  void NormalExit() {

  }
  void CombinedEnter() {
    animator.Pause();
    this.currentWobbleIntensity = wobbleIntensity;
    this.currentWobbleSpeed = wobbleSpeed;
  }
  void CombinedUpdate() {
    currentWobbleIntensity -= 0.01f;
    currentWobbleSpeed -= 0.01f;
    var xOffset = MathUtil.Map(Mathf.Sin(Time.time * currentWobbleSpeed), -1, 1, 0.9f, 1.1f);
    var yOffset = MathUtil.Map(Mathf.Cos(Time.time * currentWobbleSpeed), -1, 1, 0.9f, 1.1f);
    this.spriteRenderer.transform.localScale = Vector3.right * xOffset + Vector3.up * yOffset;
    postCombinePauseInSeconds -= Time.deltaTime;
    if (postCombinePauseInSeconds <= 0) {
      fsm.ChangeState(SlimeState.Normal);
    }
  }
  void CombinedExit() {
    spriteRenderer.transform.localScale = Vector3.one;
  }

  void KnockingEnter() {


  }
  void KnockingUpdate() {
    if (!wallSensor.unmaskedInfo.collider || wallSensor.unmaskedInfo.collider == wallSensor.GetComponent<Collider2D>() || wallSensor.unmaskedInfo.collider == collider) {
      this.velocity = Vector2.right * dir;
    }
    else {
      this.velocity = Vector2.zero;
    }

  }
  void KnockingExit() {

  }

  void OverriddenUpdate() {
    if (pickupFSM.currentState == PickupState.None) {
      fsm.ChangeState(SlimeState.Normal);
    }
  }

  void BrokenUpdate() {
    if (grounded) {
      fsm.ChangeState(SlimeState.Normal);
    }
  }

  void DeadUpdate() {
    transform.localEulerAngles += Vector3.forward * 20f * Mathf.Sign(throwDir);
  }

  public void Die(int dir) {
    if (size > 0) {
      var smallerSlime = SlimeManager.instance.slimeProgression[size - 1];
      var leftSlime = Instantiate(smallerSlime).GetComponent<Slime>();
      var rightSlime = Instantiate(smallerSlime).GetComponent<Slime>();
      leftSlime.transform.position = transform.position + Vector3.up;
      rightSlime.transform.position = transform.position + Vector3.up;
      rightSlime.fsm.ChangeState(SlimeState.Broken);
      leftSlime.fsm.ChangeState(SlimeState.Broken);
      leftSlime.velocity = new Vector2(-15, 50);
      rightSlime.velocity = new Vector2(15, 50);

      leftSlime.SetColor(color);
      rightSlime.SetColor(color);
      Destroy(this.gameObject);
    }
    else {
      DieForReal(dir);
    }
  }
  public void DieForReal(int dir) {
    if (fsm.currentState == SlimeState.Dead) return;
    pickupFSM.ChangeState(PickupState.None);
    this.velocity = Vector2.right * Mathf.Sign(dir) * 30 + Vector2.up * 60f;
    collider.enabled = false;
    this.pickupTrigger.SetActive(false);
    wallSensor.enabled = false;
    spriteRenderer.material.color = new Color(0.6f, 0.6f, 0.6f, 1f);
    outline.SetActive(false);
    fsm.ChangeState(SlimeState.Dead);
  }

  void OnTriggerEnter2D(Collider2D col) {
    if (toBeDeleted) return;

    if (col.transform.parent) {
      var otherSlime = col.transform.parent.GetComponent<Slime>();
      if (otherSlime && fsm.currentState == SlimeState.Knocking) {
        this.hittingOtherSlime = true;
      }

      if (otherSlime && otherSlime.color == this.color && otherSlime != this && pickupFSM.currentState == PickupState.Thrown && !hitWallAfterThrown) {
        if (otherSlime.size == this.size && size < SlimeManager.instance.slimeProgression.Length - 1) {

          var nextSlime = SlimeManager.instance.slimeProgression[size + 1];
          var newSlimeObj = Instantiate(nextSlime);
          //align bottoms
          var midPos = (transform.position + otherSlime.transform.position) / 2;
          var newSlime = newSlimeObj.GetComponent<Slime>();
          newSlime.SetColor(this.color);
          newSlime.transform.position = Vector3.right * transform.position.x + Vector3.up * (GetBottom().y + newSlime.spriteRenderer.bounds.extents.y);
          this.toBeDeleted = otherSlime.toBeDeleted = true;
          newSlime.fsm.ChangeState(SlimeState.Combined);
          Destroy(gameObject);
          Destroy(otherSlime.gameObject);
        }

      }

      var door = col.transform.parent.GetComponent<Door>();
      if (door) {
        if (fsm.currentState == SlimeState.Knocking) {
          if (!door.open && col.CompareTag("SlimeAtDoorTrigger")) {
            door.slimeAtDoor = this;
          }
        }
      }
    }

    if (col.GetComponent<Exit>()) {
      Debug.Log("hi?!");
      GameManager.instance.AddScore(Mathf.RoundToInt(Mathf.Pow(10, size)), col.GetComponent<Exit>());
      Destroy(gameObject);
    }
  }
  private void OnTriggerExit2D(Collider2D col) {
    if (col.transform.parent) {
      var otherSlime = col.transform.parent.GetComponent<Slime>();
      if (otherSlime && fsm.currentState == SlimeState.Knocking) {
        this.hittingOtherSlime = false;
      }
      var door = col.transform.parent.GetComponent<Door>();
      if (door) {
        if (col.CompareTag("Passthru")) {
          if (fsm.currentState == SlimeState.Knocking) {
            fsm.ChangeState(SlimeState.Normal);
          }
        }
      }
    }
  }
}
