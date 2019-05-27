using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

public class Door : MonoBehaviour
{
    public AnimationClip openAnim;
    public AnimationClip closeAnim;
    public AnimationClip idleAnim;
    public AnimationClip knockAnim;
    public Slime slimeAtDoor;
    [Autohook]
    public SpriteAnim animator;
    public bool open = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((animator.GetCurrentAnimation() == closeAnim || animator.GetCurrentAnimation() == idleAnim) && slimeAtDoor) {
          animator.Play(knockAnim);
        }
    }
    public void Open() {
      animator.Play(openAnim);
      open = false;
    }
    public void Close() {
      animator.Play(closeAnim);
      open = true;
    }
}
