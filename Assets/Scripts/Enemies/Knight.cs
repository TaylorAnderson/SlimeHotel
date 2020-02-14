using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;
public enum KnightState {
  Normal,
  Readying,
  Stabbing,
  Dead,
  Overridden
}
public class Knight : Enemy {
  public float speed = 150;
  public StateMachine<KnightState> fsm = new StateMachine<KnightState>(KnightState.Normal);
  public float timeToReady = 0.6f;

  private SpriteAnim animator;
  public AnimationClip walkingAnim;
  public AnimationClip readyAnim;
  public AnimationClip stabAnim;
  private float readyTimer = 0;
  private float stabTimer = 0;
  private float timeToStab = 0.2f;
  private float stabCooldown = 2f;
  private float stabCooldownTimer = 0f;

  private float timeUntilDestroy = 2;
  // Update is called once per frame
  override public void Awake() {
    base.Awake();
    dir = Random.value > 0.5f ? -1 : 1;
    this.spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<SpriteAnim>();

    fsm.Bind(KnightState.Normal, NormalEnter, NormalUpdate);
    fsm.Bind(KnightState.Dead, null, DeadUpdate);
    fsm.Bind(KnightState.Readying, ReadyingStart, ReadyingUpdate);
    fsm.Bind(KnightState.Stabbing, StabbingStart, StabbingUpdate);
  }
  override public void Update() {
    base.Update();
    fsm.Update();
    stabCooldownTimer -= Time.deltaTime;
  }

  override public void Throw(int dir) {
    base.Throw(dir);
    this.fsm.ChangeState(KnightState.Overridden);
  }

  override public void PickUp() {
    base.PickUp();
    this.fsm.ChangeState(KnightState.Overridden);
  }

  override public void HitWall() {
    Die(-throwDir);
  }

  override public void Drop() {
    fsm.ChangeState(KnightState.Normal);
  }

  public void NormalEnter() {
    animator.Play(walkingAnim);
  }
  public void NormalUpdate() {
    this.velocity = (Vector3)Vector2.right * this.dir * speed * Time.deltaTime;
    if (wallSensor.colliding) {
      dir *= -1;
    }
    this.transform.localScale = Vector3.right * dir + Vector3.up + Vector3.forward;

    var results = new List<RaycastHit2D>();

    rb2d.Cast(Vector3.right * dir * 10, new ContactFilter2D(), results, 1);

    for (int i = 0; i < results.Count; i++) {
      if (results[i].collider.GetComponent<Slime>() && stabCooldownTimer < 0) {
        fsm.ChangeState(KnightState.Readying);
        break;
      }
    }
  }

  public void ReadyingStart() {
    readyTimer = 0;
    velocity = Vector2.zero;
    animator.Play(readyAnim);

  }
  public void ReadyingUpdate() {
    readyTimer += Time.deltaTime;
    if (readyTimer > timeToReady) {
      fsm.ChangeState(KnightState.Stabbing);
    }
  }

  public void StabbingStart() {
    stabTimer = 0;
    velocity = Vector2.right * dir * 30;
    animator.Play(stabAnim);
  }
  public void StabbingUpdate() {
    stabTimer += Time.deltaTime;
    if (stabTimer > timeToStab) {
      fsm.ChangeState(KnightState.Normal);
    }
  }

  public void DeadUpdate() {
    spriteRenderer.color = new Color(50, 50, 50, 1);
    transform.localEulerAngles += Vector3.forward * 20f * Mathf.Sign(throwDir);

    timeUntilDestroy -= Time.deltaTime;
    if (timeUntilDestroy <= 0) {
      Destroy(gameObject);
    }
  }



  public override void Die(int dir) {
    this.dead = true;
    if (fsm.currentState == KnightState.Dead) return;
    SfxManager.instance.PlaySound(SoundType.HIT_WALL);
    pickupFSM.ChangeState(PickupState.None);
    this.velocity = Vector2.right * Mathf.Sign(dir) * 30 + Vector2.up * 60f;
    collider.enabled = false;
    this.transform.GetChild(2).gameObject.SetActive(false); // pickup trigger
    wallSensor.enabled = false;
    spriteRenderer.material.color = new Color(0.6f, 0.6f, 0.6f, 1f);
    transform.GetChild(1).gameObject.SetActive(false); //outline.  its hacky and im sorry
    fsm.ChangeState(KnightState.Dead);
  }

  private void OnTriggerEnter2D(Collider2D other) {
    var parent = other.transform.parent;
    if (parent) {
      var door = parent.GetComponent<Door>();
      if (door) {
      }
    }
    if (parent && parent.GetComponent<Slime>() && fsm.currentState == KnightState.Stabbing && stabCooldownTimer < 0) {
      stabCooldownTimer = stabCooldown;
      StartCoroutine(GameManager.instance.PauseForSeconds(0.05f, () => {
        parent.GetComponent<Slime>().Die();
        Camera.main.Shake(0.5f);
      }));
    }
  }

  private void OnTriggerExit2D(Collider2D other) {
    if (other.transform.parent) {
      var door = other.transform.parent.GetComponent<Door>();
      if (door) {
      }
    }
  }
}
