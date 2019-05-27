using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

public enum SlimeState {
  Combined,
  Normal,
  Carried,
  Thrown,
  Knocking,
  MovingIn
}
public class Slime : PhysicsObject {
  public Sensor wallSensor;
  public Sensor platformSensor;
  public float speed = 0.1f;
  public float throwSpeed = 100f;
  public int size;

  public float postCombinePauseInSeconds = 1;

  private int throwDir = 1;

  protected bool toBeDeleted = false;


  private StateMachine<SlimeState> fsm = new StateMachine<SlimeState>(SlimeState.Knocking);
  private SpriteAnim animator;
  private int dir = 1;
  private float originalGravity;

  private float wobbleIntensity = 1.2f;
  private float wobbleSpeed = 20f;
  private float currentWobbleIntensity = 1.2f;
  private float currentWobbleSpeed = 1.5f;

  private bool hitWallAfterThrown = false;

  void Awake() {
    spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    animator = spriteRenderer.GetComponent<SpriteAnim>();
    dir = Mathf.RoundToInt(transform.localScale.x);
    fsm.Bind(SlimeState.Combined, CombinedEnter, CombinedUpdate, CombinedExit);
    fsm.Bind(SlimeState.Normal, NormalEnter, NormalUpdate, NormalExit);
    fsm.Bind(SlimeState.Carried, CarriedEnter, CarriedUpdate, CarriedExit);
    fsm.Bind(SlimeState.Thrown, ThrownEnter, ThrownUpdate, ThrownExit);
    fsm.Bind(SlimeState.Knocking, KnockingEnter, KnockingUpdate, KnockingExit);
  }

  // Update is called once per frame
  void Update() {
    fsm.Update();
  }
  public void PickUp() {
    fsm.ChangeState(SlimeState.Carried);
  }
  public void Throw(int dir) {
    this.throwDir = dir;
    this.transform.localScale = Vector3.right * dir + Vector3.up + Vector3.forward;
    fsm.ChangeState(SlimeState.Thrown);
  }

  void NormalEnter() {
    animator.Resume();
  }
  void NormalUpdate() {
    this.velocity = (Vector3)Vector2.right * this.dir * speed * Time.deltaTime;
    if (wallSensor.colliding || !platformSensor.colliding) {
      dir *= -1;
    }
    this.transform.localScale = Vector3.right * dir + Vector3.up + Vector3.forward;
  }
  void NormalExit() {

  }

  void CarriedEnter() {
    animator.Pause();
  }
  void CarriedUpdate() {

  }
  void CarriedExit() {

  }

  void ThrownEnter() {
    hitWallAfterThrown = false;
    currentGravityModifier = 0;
    velocity = Vector2.right * throwDir * throwSpeed;
  }
  void ThrownUpdate() {
    Debug.Log(velocity.y);
    if (wallSensor.colliding && !wallSensor.info.collider.CompareTag("OneWay")) {
      velocity = Vector2.zero;
      currentGravityModifier = gravityModifier;
      hitWallAfterThrown = true;
    }
    if (grounded && hitWallAfterThrown) {
      fsm.ChangeState(SlimeState.Normal);
    }
  }
  void ThrownExit() {

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
      Debug.Log("reverting");
      fsm.ChangeState(SlimeState.Normal);
    }
  }
  void CombinedExit() {
    spriteRenderer.transform.localScale = Vector3.one;
  }
  
  void KnockingEnter () {
    

  }
  void KnockingUpdate() {
    if (!wallSensor.colliding) {
      this.velocity = Vector2.right * dir;
    }
    else {
      this.velocity = Vector2.zero;
    }
  }
  void KnockingExit() {

  }
  
  void OnTriggerEnter2D(Collider2D col) {
    if (!col.transform.parent || toBeDeleted) return;

    var otherSlime = col.transform.parent.GetComponent<Slime>();
    if (otherSlime && fsm.currentState == SlimeState.Thrown && !hitWallAfterThrown) {
      if (otherSlime.size == this.size && size < SlimeManager.instance.slimeProgression.Length - 1) {

        var nextSlime = SlimeManager.instance.slimeProgression[size + 1];
        var newSlimeObj = Instantiate(nextSlime);
        //align bottoms
        var midPos = (transform.position + otherSlime.transform.position) / 2;
        var newSlime = newSlimeObj.GetComponent<Slime>();
        newSlime.transform.position = Vector3.right * transform.position.x + Vector3.up * (GetBottom().y + newSlime.spriteRenderer.bounds.extents.y);
        this.toBeDeleted = otherSlime.toBeDeleted = true;
        newSlime.fsm.ChangeState(SlimeState.Combined);
        Destroy(gameObject);
        Destroy(otherSlime.gameObject);
      }

    }

    var door = col.transform.parent.GetComponent<Door>();
    if (door) {
      door.slimeAtDoor = this;
    }
  }
}
