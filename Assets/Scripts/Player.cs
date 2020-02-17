using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uween;
using UnityEngine.SceneManagement;
using PowerTools;
public enum PlayerState {
  Ground,
  Air,
  Throwing,
  Pickup
}
public enum PlayerAnim {
  Idle,
  Walk,
  Jump,
  Carry_Idle,
  Carry_Walk,
  Carry_Jump,
  Carry_Fall,
  Fall,
  Throw,
  Grab,
}

[System.Serializable]
public class AnimReference {
  public PlayerAnim ID;
  public AnimationClip animation;
}

public class Player : PhysicsObject {
  //PLEASE REMEMBER NOT TO TOUCH THESE IN HERE
  public float fallGravityModifier = 0f;
  public float speed = 5f;
  public float runSpeed = 7.5f;
  public float jumpSpeed = 5f;

  public float accel = 0.5f;

  public float diveSpeed = 10f;
  public float respawnTime = 1f;
  public bool visible = true;

  public float defaultFriction = 0.92f;
  public AnimReference[] animHookup;


  private SpriteAnim animator;

  //END FORBIDDEN ZONE

  private float originalJumpSpeed;
  private float friction = 0.9f;
  [HideInInspector]
  public StateMachine<PlayerState> fsm;

  private GameObject sprite;

  private Dictionary<PlayerAnim, AnimationClip> animations;

  private GameObject currentlyColliding;
  private float respawnTimer;
  private Vector2 respawnPos;

  private float hInput = 0f;
  private float prevHInput = 0f;


  private float currentSpeed;


  private bool isDead = false;

  private bool doubleJumped = false;

  private Pickup objectToGrab;
  private Pickup objectBeingCarried;

  public Transform carryPos;

  private float throwTimer = 0;
  private float pickupTimer = 0;
  private Vector2 prePickupVel;
  private Vector2 preThrowVel;




  public List<string> acceptedCollisionTags = new List<string> { "Enemy", "Crate", "Button" };

  void Awake() {


    sprite = transform.GetChild(0).gameObject;
    spriteRenderer = sprite.GetComponent<SpriteRenderer>();
    animator = sprite.GetComponent<SpriteAnim>();

    this.animations = new Dictionary<PlayerAnim, AnimationClip>();
    for (int i = 0; i < animHookup.Length; i++) {
      animations[animHookup[i].ID] = animHookup[i].animation;
    }

    this.originalJumpSpeed = this.jumpSpeed;

    respawnPos = this.transform.position;

    fsm = new StateMachine<PlayerState>(PlayerState.Air);
    fsm.Bind(PlayerState.Air, null, Air_Update, null);
    fsm.Bind(PlayerState.Ground, Ground_Enter, Ground_Update, Ground_Exit);
    fsm.Bind(PlayerState.Throwing, Throwing_Enter, Throwing_Update, Throwing_Exit);
    fsm.Bind(PlayerState.Pickup, Pickup_Enter, Pickup_Update, Pickup_Exit);
    fsm.onChangeState += (newState) => {
      if (fsm.currentState == PlayerState.Air && newState == PlayerState.Ground) {
        SfxManager.PlaySoundStatic(SoundType.LAND);
      }
    };
    currentSpeed = speed;
  }

  public void Ground_Enter() {
    Squash();
    doubleJumped = false;

  }

  public void Ground_Update() {

    PlayerAnim currentAnim = PlayerAnim.Idle;

    if (hInput != 0) {
      currentAnim = this.objectBeingCarried ? PlayerAnim.Carry_Walk : PlayerAnim.Walk;


    }
    else {
      currentAnim = this.objectBeingCarried ? PlayerAnim.Carry_Idle : PlayerAnim.Idle;
    }

    if (!animator.IsPlaying(animations[currentAnim])) {
      PlayAnim(currentAnim);
    }
    //END ANIMATION STUFF

    if (objectBeingCarried) {
      HandleObjectThrow();
    }
    else {
      CheckGrabs();
    }

    this.currentSpeed = this.speed;

    if (Sinput.GetButtonDown("Jump")) {
      SfxManager.instance.PlaySound(SoundType.JUMP);
      Squash(0.05f).Then(() => {
        Stretch();
        velocity.y = jumpSpeed;
      });
    }
    if (Sinput.GetButtonDown("Down")) {
      if (groundCollider.CompareTag("OneWay")) transform.position += Vector3.down * 0.5f;
    }

    this.currentGravityModifier = this.gravityModifier;

    //STATE CHANGES
    if (!grounded) {
      fsm.ChangeState(PlayerState.Air);
    }

    HandleLeftRightMovement();
  }

  public void CheckGrabs() {
    if (Sinput.GetButtonDown("Grab")) {
      var results = new Collider2D[10];
      var filter = new ContactFilter2D();
      filter.useTriggers = true;

      Physics2D.OverlapCollider(collider, filter, results);
      for (int i = 0; i < results.Length; i++) {
        if (results[i] && (results[i].GetComponent<Pickup>() || (results[i].transform.parent && results[i].transform.parent.GetComponent<Pickup>()))) {
          objectToGrab = results[i].transform.parent ? results[i].transform.parent.GetComponent<Pickup>() : results[i].GetComponent<Pickup>();
          fsm.ChangeState(PlayerState.Pickup);
          break;
        }
      }
    }
  }

  public void Ground_Exit() {
  }

  public void Air_Update() {
    if (this.velocity.y > 0) PlayAnim(this.objectBeingCarried ? PlayerAnim.Carry_Jump : PlayerAnim.Jump);
    else PlayAnim(this.objectBeingCarried ? PlayerAnim.Carry_Fall : PlayerAnim.Fall);

    if (Sinput.GetButtonUp("Jump")) {
      velocity.y *= 0.5f;
    }

    if (velocity.y > 0) this.currentGravityModifier = this.gravityModifier;
    else this.currentGravityModifier = this.fallGravityModifier;

    if (grounded) fsm.ChangeState(PlayerState.Ground);
    if (Sinput.GetButtonDown("Jump") && !doubleJumped) {
      doubleJumped = true;
      SfxManager.instance.PlaySound(SoundType.JUMP);
      Stretch();
      velocity.y = jumpSpeed;
    }

    if (Sinput.GetButtonDown("Down") && objectBeingCarried && objectBeingCarried.GetComponent<Slime>()) {
      objectBeingCarried.transform.parent = null;
      objectBeingCarried.transform.position += Vector3.down * (spriteRenderer.bounds.size.y);

      objectBeingCarried.Throw(0);
      objectBeingCarried = null;
      objectToGrab = null;
    }


    HandleLeftRightMovement();
    HandleObjectThrow();
    if (!objectBeingCarried) {
      CheckGrabs();
    }
  }

  public void Throwing_Enter() {
    SfxManager.instance.PlaySound(SoundType.THROW);
    PlayAnim(PlayerAnim.Throw);
    throwTimer = 0;
    preThrowVel = velocity;


  }
  public void Throwing_Update() {
    throwTimer += Time.deltaTime;
    if (throwTimer >= animations[PlayerAnim.Throw].length) {
      fsm.ChangeState(PlayerState.Air);
    }
    if (throwTimer >= animations[PlayerAnim.Throw].length / 2 && objectBeingCarried) {
      if (objectBeingCarried.spriteRenderer.bounds.size.x > spriteRenderer.bounds.size.x) {
        var center = spriteRenderer.bounds.center;
        var lineLeft = Physics2D.Linecast(center, center + Vector3.left * 1000, layerMask);
        var lineRight = Physics2D.Linecast(center, center + Vector3.right * 1000, layerMask);
        var distLeft = Mathf.Abs(lineLeft.point.x - center.x);
        var distRight = Mathf.Abs(lineRight.point.x - center.x);
        var leftGood = (distLeft > objectBeingCarried.spriteRenderer.bounds.size.x);
        var rightGood = (distRight > objectBeingCarried.spriteRenderer.bounds.size.x);
        if (leftGood && !rightGood) {
          objectBeingCarried.transform.position += Vector3.left * (objectBeingCarried.spriteRenderer.bounds.extents.x - spriteRenderer.bounds.extents.x + 0.1f);
        }
        if (rightGood && !leftGood) {
          objectBeingCarried.transform.position += Vector3.right * (objectBeingCarried.spriteRenderer.bounds.extents.x - spriteRenderer.bounds.extents.x + 0.05f);
        }
      }
      objectBeingCarried.transform.parent = null;
      objectBeingCarried.transform.position += Vector3.down * (spriteRenderer.bounds.size.y - 0.01f);

      objectBeingCarried.Throw(spriteRenderer.flipX ? -1 : 1);
      objectBeingCarried = null;
      objectToGrab = null;
    }
    velocity = Vector2.zero;
  }
  public void Throwing_Exit() {
    velocity = preThrowVel;

  }

  public void Pickup_Enter() {
    PlayAnim(PlayerAnim.Grab);
    SfxManager.instance.PlaySound(SoundType.GRAB);
    pickupTimer = 0;
    objectBeingCarried = objectToGrab;
    objectBeingCarried.velocity = Vector2.zero;
    prePickupVel = this.velocity;
  }
  public void Pickup_Update() {
    velocity = Vector2.zero;
    pickupTimer += Time.deltaTime;
    if (pickupTimer > animations[PlayerAnim.Grab].length) {
      fsm.ChangeState(PlayerState.Ground);
    }
  }
  public void Pickup_Exit() {
    objectBeingCarried.PickUp();
    objectBeingCarried.transform.parent = transform;
    objectBeingCarried.transform.position = (Vector3)this.carryPos.position;
    velocity = prePickupVel;
  }

  protected override void ComputeVelocity() {

    this.friction = defaultFriction;

    //WEARING
    jumpSpeed = originalJumpSpeed;

  }

  public override void PausableUpdate() {
    fsm.Update();
  }

  public TweenSY Squash(float time = 0.1f, float returnDelay = 0f) {
    TweenSX.Add(sprite, time, 1.4f).EaseOutBack().Then(() => { TweenSX.Add(sprite, time, 1).EaseOutBack().Delay(returnDelay); });
    return TweenSY.Add(sprite, time, 0.5f).EaseOutBack().Then(() => { TweenSY.Add(sprite, time, 1).EaseOutBack().Delay(returnDelay); });
  }

  public void Stretch() {
    TweenSX.Add(sprite, 0.1f, 0.7f).EaseOutBack().Then(() => { TweenSX.Add(sprite, 0.1f, 1).EaseOutBack(); });
    TweenSY.Add(sprite, 0.1f, 1.2f).EaseOutBack().Then(() => { TweenSY.Add(sprite, 0.1f, 1).EaseOutBack(); });
  }

  public void HandleLeftRightMovement() {
    hInput = Sinput.GetAxisRaw("Horizontal");

    spriteRenderer.flipX = hInput != 0 ? hInput < 0 : spriteRenderer.flipX;
    //this sorta weird setup  means that the player can still MOVE really fast (if propelled by external forces)
    //its just that he cant go super fast just by player input alone

    //force should be clamped so it doesnt let velocity extend past currentSpeed
    var force = accel * hInput * Time.deltaTime * 100;
    force = Mathf.Clamp(force + velocity.x, -currentSpeed, currentSpeed) - velocity.x;

    //basically making sure the adjusted force of the input doesn't act against the input itself
    if (Mathf.Sign(force) == Mathf.Sign(hInput)) velocity.x += force;


    if (Mathf.Abs(hInput) < 0.01f) {
      velocity.x *= 0.8f;
    }
  }
  public void HandleObjectThrow() {
    if (!objectBeingCarried) return;
    if (Sinput.GetButtonDown("Grab")) {
      fsm.ChangeState(PlayerState.Throwing);
    }
  }
  public void PlayAnim(PlayerAnim anim) {
    if (!animator.IsPlaying(animations[anim])) {
      animator.Play(animations[anim]);

    }
  }
  public void Step() {
    //SfxManager.instance.PlaySound(SoundType.STEP, 0.1f);
  }
  public void OnTriggerEnter2D(Collider2D other) {
    Looper looper = other.GetComponent<Looper>();
    if (looper) {
      this.transform.position = looper.other.transform.position;
    }
  }

}

