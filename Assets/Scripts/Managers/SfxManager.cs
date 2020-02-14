using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using deVoid.Utils;
using UnityEngine.SceneManagement;
public enum SoundType {
  NONE,
  JUMP,
  GRAB,
  THROW,
  HIT_WALL,
  SLIME_COMBINE,
  DOOR_OPEN,
  DOOR_CLOSE,
  STEP,
  KNOCK,
  LAND
}

[System.Serializable]
public class Sound {
  public AudioClip sound;
  public SoundType type;
}
public class SoundPlayer {
  public AudioSource player;
  public bool inUse = false;
  public SoundPlayer(AudioSource player) {
    this.player = player;
  }
}
public class SfxManagerInitSignal : ASignal { };
public class SfxManager : MonoBehaviour {
  public static SfxManager instance;
  public Sound[] sounds;
  public float volumeMultiplier;
  public float currentVolumeMultiplier;

  private List<SoundPlayer> players = new List<SoundPlayer>();

  private List<int> currentlyLoopingSounds = new List<int>();
  private List<SoundType> currentlyPlayingSounds = new List<SoundType>();
  private bool muted = false;
  private Dictionary<SoundType, AudioClip> soundDictionary = new Dictionary<SoundType, AudioClip>();


  // Start is called before the first frame update
  void Start() {

    //Check if instance already exists
    if (instance == null)
      instance = this;

    else if (instance != this)
      Destroy(gameObject);

    //DontDestroyOnLoad(gameObject);

    CreateAudioSources(100);


    for (int i = 0; i < sounds.Length; i++) {
      this.soundDictionary[sounds[i].type] = sounds[i].sound;
    }

    Signals.Get<SfxManagerInitSignal>().Dispatch();

    this.currentVolumeMultiplier = volumeMultiplier;

    SceneManager.sceneLoaded += OnSceneLoad;
  }

  private void OnSceneLoad(Scene next, LoadSceneMode mode) {
    StopAllSounds();
  }

  public void Update() {
    // input manager used here
    /*if (InputManager.input.Mute.WasPressed) {
      this.muted = !muted;
    }*/
    if (GameManager.instance) {
      AudioListener.volume = muted ? 0 : GameManager.instance.pausedForPauseScreen ? 0.5f : 1;
    }
  }

  public static int PlaySoundStatic(SoundType soundType, float volume = 1, bool looping = false) {
    return SfxManager.instance.PlaySound(soundType, volume, looping);
  }

  public int PlaySound(SoundType soundType, float volume = 1, bool looping = false) {

    if (soundType == SoundType.NONE) return -1;
    if (!looping) {
      var sameSoundsPlaying = 0;
      for (int i = 0; i < this.currentlyPlayingSounds.Count; i++) {
        if (this.currentlyPlayingSounds[i] == soundType) sameSoundsPlaying++;
      }
      if (sameSoundsPlaying > 3) return -1;
    }
    var playerIndex = GetAvailablePlayer();
    var player = this.players[playerIndex];
    if (player == null) return -1;
    if (looping) {
      player.player.clip = this.soundDictionary[soundType];
      player.player.loop = true;
      player.player.volume = this.currentVolumeMultiplier * volume;
      player.player.Play();
      currentlyLoopingSounds.Add(playerIndex);
    }
    else {

      this.currentlyPlayingSounds.Add(soundType);

      player.player.volume = 1;
      player.player.PlayOneShot(this.soundDictionary[soundType], volume * this.currentVolumeMultiplier);
      StartCoroutine(FreeUpSourceAfterSoundEnds(player, this.soundDictionary[soundType], soundType));
    }

    return playerIndex;

  }

  /**
  If playerToken provided is -1, it will return the first player in the pool playing that sound.
   */
  public void StopLoopingSound(SoundType soundType, int playerToken) {
    if (playerToken > this.players.Count - 1) {
      return;
    }
    if (playerToken == -1) {
      for (int i = 0; i < players.Count; i++) {
        if (players[i].player.clip == soundDictionary[soundType]) {
          playerToken = i;
        }
      }
      if (playerToken == -1) return;
    }
    if (this.players[playerToken] != null && currentlyLoopingSounds.IndexOf(playerToken) >= 0) {
      this.currentlyLoopingSounds.Remove(playerToken);
      Debug.Log("stopping sound");
      this.players[playerToken].player.Stop();
      this.players[playerToken].inUse = false;
    }
    else {
      //Debug.LogWarning("StopLoopingSound failed; player not found for that playerIndex");
    }
  }

  public void StopAllSounds() {
    for (int i = 0; i < this.players.Count; i++) {
      if (players[i].player) {
        players[i].player.Stop();
      }

    }
  }

  private int GetAvailablePlayer() {
    for (int i = 0; i < this.players.Count; i++) {
      if (!this.players[i].inUse) {
        this.players[i].inUse = true;
        return i;
      }
    }
    Debug.Log("ran out of audio sources, creating more");
    CreateAudioSources(20);
    return GetAvailablePlayer();
  }

  private void CreateAudioSources(int sources) {
    for (int i = 0; i < sources; i++) {
      var player = new GameObject();
      var soundPlayer = new SoundPlayer(player.AddComponent<AudioSource>());
      this.players.Add(soundPlayer);
      soundPlayer.player.playOnAwake = false;
      player.transform.parent = transform;
    }
  }

  private IEnumerator FreeUpSourceAfterSoundEnds(SoundPlayer player, AudioClip sound, SoundType soundType) {
    yield return new WaitForSeconds(sound.length);
    this.currentlyPlayingSounds.Remove(soundType);

    player.inUse = false;
  }
}
