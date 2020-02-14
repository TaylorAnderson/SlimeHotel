using System;
using System.Collections.Generic;
using System.Reflection;

public class StateMachine<T> where T : struct, IConvertible {

  public T State { get; private set; }


  private Dictionary<T, State> stateDict = new Dictionary<T, State>();

  public State currentStateObj;
  public T currentState;

  public Action<T> onChangeState;
  public StateMachine(T init) {
    if (!typeof(T).IsEnum) {
      throw new ArgumentException("T must be an enumeration");
    }

    // Cache state and transition functions
    foreach (T value in typeof(T).GetEnumValues()) {
      stateDict[value] = new State(value.ToString());
    }

    ChangeState(init);
  }

  public void ChangeState(T state) {

    if (this.currentStateObj != null && this.currentStateObj.onExit != null) {
      this.currentStateObj.onExit();
    }
    this.currentStateObj = this.stateDict[state];
    if (this.currentStateObj.onEnter != null) {
      this.currentStateObj.onEnter();
    }
    if (this.onChangeState != null) this.onChangeState.Invoke(state);
    this.currentState = state;
  }

  public void Update() {
    if (this.currentStateObj != null && this.currentStateObj.onUpdate != null) {
      this.currentStateObj.onUpdate();
    }
  }

  public void Bind(T state, Action onEnter = null, Action onUpdate = null, Action onExit = null) {
    var stateObj = stateDict[state];
    stateObj.onEnter = onEnter;
    stateObj.onUpdate = onUpdate;
    stateObj.onExit = onExit;
  }


}
public class State {
  public Action onEnter;
  public Action onUpdate;
  public Action onExit;
  public string name;

  public State(string name) {
    this.name = name;
  }
}


