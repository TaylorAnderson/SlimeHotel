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
  public float speed = 0.05f;

  public int size;

  public static readonly int GREEN = 0;
  public static readonly int RED = 0;
  public static readonly int YELLOW = 0;

  public float postCombinePauseInSeconds = 1;
  public Color color = new Color(63, 171, 62);
  public GameObject outline;
  public GameObject pickupTrigger;
  [HideInInspector]
  public bool angry = false;
  private float timeToAnger = 10;
  private float timeToLeave = 5;
  private float angerTimer = 15;
  [HideInInspector]
  public bool leaving = false;
  [HideInInspector]
  public float valueMultiplier;
  protected bool toBeDeleted = false;

  public StateMachine<SlimeState> fsm = new StateMachine<SlimeState>(SlimeState.Knocking);
  private SpriteAnim animator;
  private int dir = 1;
  private float originalGravity;

  private float wobbleIntensity = 2f;
  private float wobbleSpeed = 20f;
  private float currentWobbleIntensity = 1.2f;
  private float currentWobbleSpeed = 1.2f;

  private float gyrateTimer = 0;

  private bool cashedOut = false;

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
    angry = false;
    animator.Resume();
  }
  void NormalUpdate() {
    Debug.Log(dir);
    this.velocity = (Vector3)Vector2.right * this.dir * speed;
    if (!platformSensor.colliding || wallSensor.colliding) {
      dir *= -1;
    }
    Debug.Log(velocity);
    this.transform.localScale = Vector3.right * dir + Vector3.up + Vector3.forward;
  }
  void NormalExit() {

  }
  void CombinedEnter() {
    SfxManager.instance.PlaySound(SoundType.SLIME_COMBINE);
    animator.Pause();
    this.currentWobbleIntensity = wobbleIntensity;
    this.currentWobbleSpeed = wobbleSpeed;
  }
  void CombinedUpdate() {

    gyrateTimer += Time.deltaTime;
    currentWobbleIntensity -= 0.03f;
    currentWobbleSpeed -= 0.03f;
    var xOffset = MathUtil.Map(Mathf.Sin(gyrateTimer * currentWobbleSpeed), -1, 1, 0.9f, 1.1f);
    var yOffset = MathUtil.Map(Mathf.Cos(gyrateTimer * currentWobbleSpeed), -1, 1, 0.9f, 1.1f);
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
    angerTimer = timeToAnger + timeToLeave;
  }
  void KnockingUpdate() {
    #region Grabbing collisions in front of us
    var results = new List<RaycastHit2D>();
    var filter = new ContactFilter2D();
    filter.useTriggers = true;
    int slimesHitting = 0;
    Door door = null;
    Physics2D.Linecast(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f + Vector3.right * dir * 1.5f, filter, results);

    #endregion

    #region Going through collisions to check for slimes, a door
    for (int i = 0; i < results.Count; i++) {
      if (results[i].collider.GetComponent<Slime>()) {
        slimesHitting++;
      }
      if (results[i].transform.GetComponent<Door>()) {
        door = results[i].transform.GetComponent<Door>();
      }
    }
    #endregion

    //we use 1 because we're likely colliding with ourselves here
    if (slimesHitting <= 1 || leaving) {
      this.velocity = Vector2.right * dir;
    }
    else {
      this.velocity = Vector2.zero;
    }
    if (door && door.closed) {
      door.slimeAtDoor = this;

      angerTimer -= Time.deltaTime;
      angry = angerTimer < timeToLeave;


      if (angerTimer < 0 && !leaving) {
        leaving = true;
        dir *= -1;
        door = null;
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
      }
    }

    if (platformSensor.colliding) {
      fsm.ChangeState(SlimeState.Normal);
    }
  }
  void KnockingExit() {
    angerTimer = timeToAnger;
    angry = false;
    leaving = false;
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

  public void Die() {
    GameManager.instance.LoseStar();
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
    if (col.GetComponent<Exit>()) {
      Debug.Log(angry);
      if (!angry && !cashedOut) {
        int scoreAdd = Mathf.RoundToInt(Mathf.Pow(10, size) * valueMultiplier);
        Debug.Log(scoreAdd);
        GameManager.instance.AddMoney(scoreAdd, col.GetComponent<Exit>());
        cashedOut = true;
        Debug.Log("get money");
      }
      Destroy(gameObject);
    }
  }
}
