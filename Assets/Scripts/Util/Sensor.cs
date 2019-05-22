using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {
  private GameObject parent;
  [HideInInspector]
  public bool colliding = false;
  public RaycastHit2D info;
  public LayerMask layerMask;
  // Start is called before the first frame update
  void Start() {
    this.parent = transform.parent.gameObject;
  }

  // Update is called once per frame
  void Update() {
    this.info = Physics2D.Linecast(parent.transform.position, transform.position, layerMask);
    this.colliding = info.collider;
  }
}
