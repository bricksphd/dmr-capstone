﻿using UnityEngine;

namespace FiscalShock.Procedural {
    public class KillZone : MonoBehaviour {
        private GameObject loadingScreen;
        private LoadingScreen loadScript;

        private void Start() {
            loadingScreen = GameObject.Find("LoadingScreen");
            loadScript = (LoadingScreen)loadingScreen.GetComponent<LoadingScreen>();
        }

        private void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.tag == "Player") {
                loadScript.startLoadingScreen("LoseGame");
                GameObject musicPlayer = GameObject.Find("DungeonMusic");
                Destroy(musicPlayer);
                PlayerFinance.startNewDay();
            }
        }
    }
}