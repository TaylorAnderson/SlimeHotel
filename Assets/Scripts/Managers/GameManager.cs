using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using deVoid.Utils;
using UnityEngine.SceneManagement;



public class Data {
  public int income;
  public Data(int income) {
    this.income = income;
  }
}

public enum GameState {
  Title,
  Day,
  Shop,
  Closed,

}
public class GameManager : MonoBehaviour {
  public static GameManager instance;

  public bool paused = false;
  public bool pausedForPauseScreen = false;
  public bool gameEnded = false;
  public int combo = 1;
  public int income = 1000000;
  public int bestIncome = 0;
  public int stars = 1;
  public StateMachine<GameState> fsm = new StateMachine<GameState>(GameState.Title);

  public List<Upgrade> upgrades = new List<Upgrade>();



  private float comboKillTime = 10;
  private float comboKillTimer = 10;
  [SerializeField] protected GameObject shop;
  [SerializeField] protected GameObject gameOver;
  [SerializeField] protected GameObject title;
  [SerializeField] protected GameObject titleExitAnim;
  [SerializeField] protected SuperTextMesh gameOverIncome;
  [SerializeField] protected SuperTextMesh gameOverBestIncome;


  public EndPauseSignal endPauseSignal;

  private float secondsInDay = 120;
  private float currentSecondsInDay = 120;





  void Awake() {
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = 60;
    Screen.fullScreen = false;
    //Screen.SetResolution(1280, 720, false);
    //Check if instance already exists
    if (instance == null)
      instance = this;

    else if (instance != this)
      Destroy(gameObject);



    fsm.Bind(GameState.Title, TitleEnter, null, null);
    fsm.Bind(GameState.Day, DayEnter, DayUpdate, null);
    fsm.Bind(GameState.Shop, ShopEnter, null, ShopExit);
    fsm.Bind(GameState.Closed, ClosedEnter, ClosedUpdate, ClosedExit);
    fsm.ChangeState(GameState.Title);
  }



  // Update is called once per frame
  void Update() {
    fsm.Update();

    if (this.combo > 1) {
      comboKillTimer -= Time.deltaTime;
      //this.comboMeter.fillAmount = MathUtil.Map(this.comboKillTimer, this.comboKillTime, 0, 0, 1);
      if (comboKillTimer <= 0) {
        //this.DecrementCombo();
        comboKillTimer = comboKillTime;
      }
    }
    if (GameManager.instance.paused) {
      Time.timeScale = 0;
    }
    else {
      Time.timeScale = 1;
    }
    if (Sinput.GetButtonDown("Pause")) {
      GameManager.instance.pausedForPauseScreen = !GameManager.instance.pausedForPauseScreen;
      GameManager.instance.paused = !GameManager.instance.paused;
    }
    if (Sinput.GetButtonDown("Restart")) {
      SceneManager.LoadSceneAsync(0);
    }
  }

  public void TitleEnter() {
    paused = true;
    title.GetComponentInChildren<UIButton>().buttonSignal.AddListener(StartPressed);
  }
  public void DayEnter() {
    this.paused = false;
    shop.SetActive(false);
    currentSecondsInDay = secondsInDay;
  }
  public void DayUpdate() {
    currentSecondsInDay -= Time.deltaTime;
    if (currentSecondsInDay < 0) {
      //fsm.ChangeState(GameState.Shop);
    }
  }

  public void ShopEnter() {
    Signals.Get<ShopWindowClosedSignal>().AddListener(ExitShopState);

    shop.SetActive(true);
    this.paused = true;
  }
  public void ShopUpdate() {

  }
  public void ShopExit() {
    shop.SetActive(false);
  }

  public void ClosedEnter() {
    SaveAndRetrieveBestIncome();
    paused = true;
    gameOver.SetActive(true);
    gameOver.GetComponentInChildren<UIButton>().buttonSignal.AddListener(RestartPressed);
    gameOverIncome.text = "Income: $" + DisplayUtil.InsertCommas(this.income);
    gameOverBestIncome.text = "Best Income: $" + DisplayUtil.InsertCommas(this.bestIncome);
  }
  public void ClosedUpdate() {

  }
  public void ClosedExit() {

  }

  public void StartPressed(UIButton button) {
    fsm.ChangeState(GameState.Day);
    titleExitAnim.SetActive(true);
  }
  public void RestartPressed(UIButton button) {
    this.fsm.ChangeState(GameState.Day);
    SceneManager.LoadSceneAsync(0);
  }

  public void ExitShopState() {
    fsm.ChangeState(GameState.Day);
  }

  public IEnumerator PauseForSeconds(float seconds, Action endPauseAction = null) {
    this.paused = true;

    if (endPauseAction != null) Signals.Get<EndPauseSignal>().AddListener(endPauseAction);
    yield return new WaitForSecondsRealtime(seconds);
    Signals.Get<EndPauseSignal>().Dispatch();
    Signals.Get<EndPauseSignal>().RemoveListener(endPauseAction);
    this.paused = false;
    //just doing a test
  }

  public void AddMoney(int score, Exit exit) {
    this.income += score;
  }

  public void IncrementCombo() {
    comboKillTimer = comboKillTime;
    combo++;

  }

  public void DecrementCombo() {
    if (combo == 1) return;
    this.combo = (int)(combo / 2);
  }

  public void ClearCombo() {
    combo = 1;
  }

  public void LoseStar() {
    stars--;
    if (stars == 0) {
      fsm.ChangeState(GameState.Closed);
    }
  }
  public void Save(int score) {
    string path = Application.persistentDataPath + "/";
    File.WriteAllText(path + "times.json", JsonUtility.ToJson(new Data(score), true));
  }

  public Data Load() {
    string path = Application.persistentDataPath + "/" + "times.json";
    try {
      if (File.Exists(path)) {
        return JsonUtility.FromJson<Data>(File.ReadAllText(path));
      }
      return null;

    } catch (System.Exception ex) {
      Debug.Log(ex.Message);
      return null;
    }
  }

  public static void ResetInstance() {
    //GameManager.instance = Instantiate(GameManager.instance.gameObject).GetComponent<GameManager>();
  }

  public void SaveAndRetrieveBestIncome() {

    //SfxManager.instance.StopLoopingSound(SoundType.GAME_MUSIC, -1);

    var data = Load();

    if (data == null || this.income > data.income) {
      bestIncome = income;
      Save(income);
    }
    else {
      bestIncome = data.income;
    }
  }

  public IEnumerator StartGameOverAfterDelay(float delay) {
    yield return new WaitForSecondsRealtime(delay);
    SfxManager.instance.StopAllSounds();

  }
}
