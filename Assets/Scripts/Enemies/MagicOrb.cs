using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MagicOrb : PhysicsObject {
  public GameObject particleObject;
  public Transform[] positions;
  public GameObject outline;

  private float particleInterval = 0.1f;
  private float particleTimer = 0;
  public int currentPos = 0;
  public float lifetime = 2;

  public float flickerCounter = 0;


  override public void Start() {
    base.Start();
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  override public void Update() {
    base.Update();
    this.velocity = Vector2.up * 4;
    gravityModifier = 0;
    currentGravityModifier = 0;

    particleTimer += Time.deltaTime;
    if (particleTimer > particleInterval) {
      particleTimer = 0;
      //var particleX = Random.Range(transform.position.x - spriteRenderer.bounds.extents.x, transform.position.x + spriteRenderer.bounds.extents.x);
      //var particleY = Random.Range(transform.position.y - spriteRenderer.bounds.extents.y, transform.position.y + spriteRenderer.bounds.extents.y);
      var particleX = positions[currentPos].position.x;
      var particleY = positions[currentPos].position.y;
      currentPos++;
      currentPos %= positions.Length;
      var newParticle = Instantiate(particleObject);
      newParticle.transform.position = new Vector3(particleX, particleY, 0);
    }
    lifetime -= Time.deltaTime;
    if (lifetime < 1) {
      flickerCounter += Time.deltaTime;
      if (flickerCounter > 0.05) {
        spriteRenderer.enabled = false;
        outline.SetActive(false);
        if (flickerCounter > 0.1) {
          spriteRenderer.enabled = true;
          outline.SetActive(true);
          flickerCounter = 0;
        }
      }
    }
    if (lifetime < 0) {
      Destroy(gameObject);
    }


  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.transform.parent?.GetComponent<Slime>()) {
      other.transform.parent.GetComponent<Slime>().Die();
      Destroy(gameObject);
    }
  }

  private float Snap(float number, float multiple) {
    return Mathf.Round(number / multiple) * multiple;
  }

}
