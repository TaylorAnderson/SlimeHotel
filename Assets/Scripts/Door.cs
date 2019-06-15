using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

public class Door : MonoBehaviour {
  public AnimationClip openAnim;
  public AnimationClip closeAnim;
  public AnimationClip idleAnim;
  public AnimationClip knockAnim;

  public Collider2D passThruTrigger;
  public LayerMask passThruMask;
  public Slime slimeAtDoor;
  private Collider2D barrier;
  [Autohook]
  public SpriteAnim animator;
  public bool open = false;

  private bool toBeClosed = false;
  public List<PhysicsObject> objectsPassingThru = new List<PhysicsObject>();
  // Start is called before the first frame update
  void Start() {
    barrier = GetComponent<BoxCollider2D>();
  }

  // Update is called once per frame
  void Update() {
    if ((animator.GetCurrentAnimation() == closeAnim || animator.GetCurrentAnimation() == idleAnim) && slimeAtDoor) {
      animator.Play(knockAnim);
    }


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
    if (objectsPassingThru.Count == 0 && toBeClosed) {
      toBeClosed = false;
      Close();
    }
  }
  public void Open() {
    if (animator.GetCurrentAnimation() == openAnim) { return; }
    slimeAtDoor = null;
    animator.Play(openAnim);
    open = true;
    barrier.enabled = false;
  }
  public void Close() {
    if (animator.GetCurrentAnimation() == closeAnim) { return; }
    if (objectsPassingThru.Count > 0) {
      toBeClosed = true;
      return;
    }

    animator.Play(closeAnim);
    open = false;
    barrier.enabled = true;
  }
}
