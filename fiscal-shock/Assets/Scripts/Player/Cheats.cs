﻿using UnityEngine;
using FiscalShock.Demo;
using FiscalShock.GUI;

public class Cheats : MonoBehaviour {
    public string teleportToEscapeKey = "f2";
    public string teleportToDelveKey = "f3";
    public string robinHood = "f1";
    public string toggleGraphMesh = "f4";
    public string enableWallDestruction = "f8";
    public GameObject player;
    public PlayerMovement playerMovement;

    public bool destroyWalls;

    void Update() {
        if (Input.GetKeyDown(teleportToEscapeKey)) {
            GameObject escape = GameObject.Find("Escape Point");
            Vector3 warpPoint = escape.transform.position;
            playerMovement.teleport(new Vector3(warpPoint.x - Random.Range(-2, 2), warpPoint.y + 4, warpPoint.z + Random.Range(-2, 2)));
        }
        if (Input.GetKeyDown(teleportToDelveKey)) {
            GameObject delve = GameObject.Find("Delve Point");
            Vector3 warpPoint = delve.transform.position;
            playerMovement.teleport(new Vector3(warpPoint.x - Random.Range(-2, 2), warpPoint.y + 4, warpPoint.z + Random.Range(-2, 2)));
        }
        if (Input.GetKeyDown(robinHood)) {
            StateManager.cashOnHand += 500;
            Debug.Log("Added 500 monies");
        }
        if (Input.GetKeyDown(toggleGraphMesh)) {
            ProceduralMeshRenderer pmr = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ProceduralMeshRenderer>();
            pmr.enabled = !pmr.enabled;
            if (!pmr.enabled) {
                GameObject go = GameObject.Find("Vertices Display");
                Destroy(go);
                pmr.alreadyDrew = false;
            }
            Debug.Log($"Toggled mesh view to {pmr.enabled}");
        }
        if (Input.GetKeyDown(enableWallDestruction)) {
            destroyWalls = !destroyWalls;
            Debug.Log($"Toggled wall destruction: {destroyWalls}");
        }
    }
}
