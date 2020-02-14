using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;


public enum DoorState {
  Closed,
  Open,
  Knocking,
}
public class Door : MonoBehaviour {
  public AnimationClip openAnim;
  public AnimationClip closeAnim;
  public AnimationClip idleAnim;
  public AnimationClip knockAnim;
  public AnimationClip angryKnockAnim;

  public Collider2D passThruTrigger;
  public Collider2D activateTrigger;
  public LayerMask passThruMask;
  public Slime slimeAtDoor;
  private Collider2D barrier;
  [Autohook]
  public SpriteAnim animator;
  public GameObject knockSprite;
  public GameObject angryKnockSprite;

  private StateMachine<DoorState> fsm = new StateMachine<DoorState>(DoorState.Closed);
  private bool playerClose = false;
  [HideInInspector]
  public bool closed = true;
  public List<PhysicsObject> objectsPassingThru = new List<PhysicsObject>();
  // Start is called before the first frame update
  void Start() {
    barrier = GetComponent<BoxCollider2D>();

    fsm.Bind(DoorState.Closed, ClosedStart, ClosedUpdate, ClosedExit);
    fsm.Bind(DoorState.Open, OpenStart, OpenUpdate, OpenExit);
    fsm.Bind(DoorState.Knocking, KnockingStart, KnockingUpdate, KnockingExit);
  }

  // Update is called once per frame
  void Update() {
    fsm.Update();

    var filter = new ContactFilter2D();
    var results = new List<Collider2D>();
    Physics2D.OverlapCollider(activateTrigger, filter, results);
    playerClose = false;
    for (int i = 0; i < results.Count; i++) {
      if (results[i].GetComponent<Player>()) {
        playerClose = true;
      }
    }
  }
  public void ClosedStart() {
    if (!closed) {
      SfxManager.instance.PlaySound(SoundType.DOOR_CLOSE);
      animator.Play(closeAnim);
      closed = true;
    }
    barrier.enabled = true;
  }
  public void ClosedUpdate() {
    if (slimeAtDoor && !slimeAtDoor.leaving) {
      fsm.ChangeState(DoorState.Knocking);
    }
    if (playerClose) {
      fsm.ChangeState(DoorState.Open);
    }
  }
  public void ClosedExit() {

  }

  public void OpenStart() {
    slimeAtDoor = null;
    barrier.enabled = false;
    if (closed) {
      SfxManager.instance.PlaySound(SoundType.DOOR_OPEN);
      animator.Play(openAnim);
      closed = false;
    }
  }

  public void OpenUpdate() {
    ContactFilter2D contactFilter = new ContactFilter2D();
    contactFilter.useTriggers = false;
    contactFilter.SetLayerMask(passThruMask);
    contactFilter.useLayerMask = true;
    var results = new Collider2D[20];
    Physics2D.OverlapCollider(passThruTrigger, contactFilter, results);
    objectsPassingThru.Clear();
    for (int i = 0; i < results.Length; i++) {
      if (results[i] && results[i].GetComponent<PhysicsObject>()) {
        objectsPassingThru.Add(results[i].GetComponent<PhysicsObject>());
      }
    }
    if (objectsPassingThru.Count == 0 && !playerClose) {
      fsm.ChangeState(DoorState.Closed);
    }
  }
  public void OpenExit() {

  }

  public void KnockingStart() {
    animator.Play(knockAnim);
  }
  public void KnockingUpdate() {
    if (slimeAtDoor.angry && animator.GetCurrentAnimation() != angryKnockAnim) {
      animator.Play(angryKnockAnim);
    }

    if (slimeAtDoor.leaving) {
      animator.Play(idleAnim);
      HideKnock();
      fsm.ChangeState(DoorState.Closed);
    }

    if (playerClose) {
      fsm.ChangeState(DoorState.Open);
    }
  }
  public void KnockingExit() {
    HideKnock();
  }

  public void ShowKnock() {
    SfxManager.instance.PlaySound(SoundType.KNOCK, 0.8f);
    var sprite = slimeAtDoor.angry ? angryKnockSprite : knockSprite;
    sprite.SetActive(true);
  }
  public void HideKnock() {
    angryKnockSprite.SetActive(false);
    knockSprite.SetActive(false);
  }
}
