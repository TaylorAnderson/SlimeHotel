using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicParticle : MonoBehaviour {
  public Sprite spr1;
  public Sprite spr2;
  [Autohook]
  public SpriteRenderer sprRenderer;
  private float lifetime = 1;
  // Start is called before the first frame update
  void Start() {
    sprRenderer.sprite = Random.value > 0.5f ? spr1 : spr2;
  }

  // Update is called once per frame
  void Update() {
    transform.position += Vector3.down * 0.05f;
    lifetime -= Time.deltaTime;
    if (lifetime < 0) {
      Destroy(gameObject);
    }

  }
}
