using UnityEngine;

public class Pausable : MonoBehaviour {
  [HideInInspector]
  public Rigidbody2D rb2d;
  public virtual void Start() {
    rb2d = GetComponent<Rigidbody2D>();
  }
  public virtual void Update() {
    if (!GameManager.instance.paused && !GameManager.instance.gameEnded) {
      PausableUpdate();
      if (rb2d) {
        rb2d.constraints = RigidbodyConstraints2D.None;
      }
    }
    else {
      if (rb2d) {
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
      }
    }
  }
  public virtual void FixedUpdate() {
    if (!GameManager.instance.paused && !GameManager.instance.gameEnded) {
      PausableFixedUpdate();
    }
  }

  public virtual void PausableUpdate() {
    //override this
  }
  public virtual void PausableFixedUpdate() {

  }

  void OnPauseGame() {

  }
}
