using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {
  private GameObject parent;
  [HideInInspector]
  public bool colliding = false;
  public RaycastHit2D info;
  public RaycastHit2D unmaskedInfo; //for when u want all info on whats in front of you
  public LayerMask layerMask;

  private float minDist = 0;
  // Start is called before the first frame update
  void Start() {
    this.parent = transform.parent.gameObject;
    this.minDist = parent.GetComponent<PhysicsObject>().spriteRenderer.bounds.extents.x;
  }

  // Update is called once per frame
  void Update() {
    Recalculate();
  }

  public void Recalculate() {
    var posToMoveFrom = parent.transform.position;
    this.info = Physics2D.Linecast(posToMoveFrom, transform.position, layerMask);
    this.unmaskedInfo = Physics2D.Linecast(posToMoveFrom, transform.position, ~(this.gameObject.layer));
    this.colliding = (info.collider && info.collider != this.GetComponent<Collider2D>());
  }
}
