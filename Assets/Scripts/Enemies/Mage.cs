using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;
public enum MageState {
  Rising,
  Floating,
  Firing,
  Overridden,
  Dead,
}
public class Mage : Enemy {
  public Collider2D floorDetector;
  public AnimationClip walkAnim;
  public AnimationClip riseAnim;
  public AnimationClip shootAnim;
  public SpriteAnim animator;
  public float speed = 100;

  public float shootInterval = 5;
  private float shootTimer = 0;

  private int floatDir;
  public StateMachine<MageState> fsm = new StateMachine<MageState>(MageState.Rising);

  public GameObject orb;

  private float timeUntilDestroy = 2;
  override public void Awake() {
    base.Awake();
    this.spriteRenderer = GetComponent<SpriteRenderer>();
    fsm.Bind(MageState.Rising, RisingStart, RisingUpdate, RisingEnd);
    fsm.Bind(MageState.Floating, FloatingStart, FloatingUpdate, FloatingEnd);
    fsm.Bind(MageState.Firing, FiringStart, FiringUpdate, FiringEnd);
    fsm.Bind(MageState.Dead, null, DeadUpdate, null);
    this.gravityModifier = 0;
    this.currentGravityModifier = 0;
    floatDir = Random.value > 0.5f ? -1 : 1;
  }
  override public void Update() {
    base.Update();
    fsm.Update();

  }
  override public void Throw(int dir) {
    base.Throw(dir);
    this.fsm.ChangeState(MageState.Overridden);
  }

  override public void PickUp() {
    base.PickUp();
    this.fsm.ChangeState(MageState.Overridden);
  }
  override public void HitWall() {
    Die(-throwDir);
  }
  override public void Drop() {
    Debug.Log("switching state");
    fsm.ChangeState(MageState.Floating);
  }
  public override void Die(int dir) {
    this.dead = true;
    if (fsm.currentState == MageState.Dead) return;
    SfxManager.instance.PlaySound(SoundType.HIT_WALL);
    pickupFSM.ChangeState(PickupState.None);
    this.velocity = Vector2.right * Mathf.Sign(dir) * 30 + Vector2.up * 60f;
    collider.enabled = false;
    this.transform.GetChild(2).gameObject.SetActive(false); // pickup trigger
    wallSensor.enabled = false;
    spriteRenderer.material.color = new Color(0.6f, 0.6f, 0.6f, 1f);
    transform.GetChild(1).gameObject.SetActive(false); //outline.  its hacky and im sorry
    fsm.ChangeState(MageState.Dead);
    this.gravityModifier = 50;
    this.currentGravityModifier = 50;
  }


  public void RisingStart() {
    animator.Play(riseAnim);
  }
  public void RisingUpdate() {
    velocity = Vector2.up * 3;
    var results = new List<Collider2D>();
    Physics2D.OverlapCollider(floorDetector, new ContactFilter2D(), results);
    for (int i = 0; i < results.Count; i++) {
      if (results[i].CompareTag("OneWay")) {
        fsm.ChangeState(MageState.Floating);
      }
    }
  }
  public void RisingEnd() {

  }
  public void FloatingStart() {
    animator.Play(walkAnim);

  }
  public void FloatingUpdate() {

    this.velocity = (Vector3)Vector2.right * this.floatDir * speed * Time.deltaTime;
    if (wallSensor.colliding) {
      floatDir *= -1;
    }
    this.transform.localScale = Vector3.right * floatDir + Vector3.up + Vector3.forward;
    shootTimer += Time.deltaTime;
    if (shootTimer > shootInterval) {
      shootTimer = 0;
      fsm.ChangeState(MageState.Firing);
    }
  }
  public void FloatingEnd() {

  }
  public void FiringStart() {
    velocity = Vector2.zero;
    animator.Play(shootAnim);
  }
  public void FiringUpdate() {
    if (animator.GetNormalisedTime() >= 1.0f) {
      fsm.ChangeState(MageState.Floating);
    }
  }
  public void FiringEnd() {

  }

  public void DeadUpdate() {
    spriteRenderer.color = new Color(50, 50, 50, 1);
    transform.localEulerAngles += Vector3.forward * 20f * Mathf.Sign(throwDir);
    timeUntilDestroy -= Time.deltaTime;
    if (timeUntilDestroy <= 0) {
      Destroy(gameObject);
    }
  }
  public void Shoot() {
    var orbCopy = Instantiate(orb);
    orbCopy.transform.position = transform.position;

  }
}
