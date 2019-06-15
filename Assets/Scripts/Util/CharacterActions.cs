using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class CharacterActions : PlayerActionSet {

  public PlayerAction Left;
  public PlayerAction Right;
  public PlayerAction Jump;
  public PlayerOneAxisAction Move;
  public PlayerAction Run;
  public PlayerAction Grab;
  public PlayerAction Pause;
  public PlayerAction Restart;
  public PlayerAction Mute;

  public CharacterActions() {
    Left = CreatePlayerAction("Move Left");
    Right = CreatePlayerAction("Move Right");
    Jump = CreatePlayerAction("Jump");
    Move = CreateOneAxisPlayerAction(Left, Right);
    Grab = CreatePlayerAction("Grab");
    Pause = CreatePlayerAction("Pause");
    Restart = CreatePlayerAction("Restart");
    Mute = CreatePlayerAction("Mute");
  }
}
