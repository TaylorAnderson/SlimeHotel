using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;
public enum Upgrade {
  Shoes,
  Ads,
  Sword,
  Assistant,
  Boards,
  Silence
}
[System.Serializable]
public struct UpgradeData {
  public string name;
  [TextArea(1, 30)]
  public string description;
  public int price;
}
[System.Serializable]
public struct UpgradeDataTemp {
  public Upgrade upgrade;
  public UpgradeData data;
}

public class ShopWindowClosedSignal : ASignal { };

public class ShopWindow : MonoBehaviour {

  public UpgradeOption[] options;
  private ItemWindow currentItemWindow;
  public ItemWindow itemWindowPrefab;
  public UIButton closeButton;
  public Dictionary<Upgrade, UpgradeData> upgradeData = new Dictionary<Upgrade, UpgradeData>();
  public UpgradeDataTemp[] upgradeDataArray;
  public ShopWindowClosedSignal onCloseSignal = new ShopWindowClosedSignal();


  // Start is called before the first frame update
  void Start() {
    for (int j = 0; j < upgradeDataArray.Length; j++) {
      upgradeData[upgradeDataArray[j].upgrade] = upgradeDataArray[j].data;
    }
    for (int i = 0; i < options.Length; i++) {
      options[i].shopWindow = this;
      options[i].Init();
      if (GameManager.instance.upgrades.IndexOf(options[i].upgrade) == -1) {
        options[i].buttonSignal.AddListener(OnButtonPressed);
      }
    }

    closeButton.buttonSignal.AddListener(OnClose);

  }

  public void OnClose(UIButton button) {
    this.onCloseSignal.Dispatch();
    Destroy(this.gameObject);

  }

  public void CheckForPurchased() {
    for (int i = 0; i < options.Length; i++) {
      if (GameManager.instance.upgrades.IndexOf(options[i].upgrade) != -1) {
        options[i].Deactivate();
      }
    }
  }

  // Update is called once per frame
  void Update() {

  }

  void OnButtonPressed(UIButton button) {
    Debug.Log("Hello?");
    if (button is UpgradeOption) {
      this.currentItemWindow = Instantiate(itemWindowPrefab, transform).GetComponent<ItemWindow>();
      this.currentItemWindow.shopWindow = this;
      this.currentItemWindow.upgrade = (button as UpgradeOption).upgrade;
      this.currentItemWindow.Init();
      this.currentItemWindow.itemWindowClosedSignal.AddListener(OnItemWindowClosed);
    }
  }
  void OnItemWindowClosed(bool itemBought) {
    if (itemBought) {
      CheckForPurchased();
    }
  }
}
