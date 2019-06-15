using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class InputManager : MonoBehaviour {
  public static CharacterActions input;
  public bool locked = false;
  // Start is called before the first frame update
  void Start() {
    if (InputManager.input == null) {
      InputManager.input = new CharacterActions();
    }
    DontDestroyOnLoad(gameObject);
    input.Left.AddDefaultBinding(Key.A);
    input.Left.AddDefaultBinding(Key.LeftArrow);
    input.Left.AddDefaultBinding(InputControlType.LeftStickLeft);

    input.Right.AddDefaultBinding(Key.D);
    input.Right.AddDefaultBinding(Key.RightArrow);
    input.Right.AddDefaultBinding(InputControlType.LeftStickRight);
    input.Pause.AddDefaultBinding(Key.Escape);
    input.Pause.AddDefaultBinding(InputControlType.Start);

    input.Restart.AddDefaultBinding(Key.R);

    input.Mute.AddDefaultBinding(Key.M);



  }

  // Update is called once per frame
  void Update() {
    input.Enabled = locked;

  }
}
