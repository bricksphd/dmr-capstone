﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI pocketChange;
    public TextMeshProUGUI debtTracker;
    public GameObject compassImage;
    public RectTransform escapeCompass;
    public TextMeshProUGUI shotLoss;
    public TextMeshProUGUI earn;

    /* Variables set at runtime */
    public Transform playerTransform { get; set; }
    public Transform escapeHatch { get; set; }

    void Start() {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        shotLoss.text = "";
        earn.text = "";
    }

    /// <summary>
    /// Currently updates every frame, could also instead just be asked to
    /// update by functions that change the HUD if that is a performance
    /// concern.
    /// </summary>
    void Update() {
        pocketChange.text = "" + PlayerFinance.cashOnHand.ToString("F2");
        debtTracker.text = "DEBT: -" + StateManager.totalDebt.ToString("F2");

        if (escapeHatch != null && playerTransform != null) {
            compassImage.SetActive(true);
            Vector3 dir = playerTransform.position - escapeHatch.position;
            float playerHeading = playerTransform.rotation.eulerAngles.y;
            float angleToEscape = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float compassAngle = playerHeading - angleToEscape + 185;  // orig image does not point at 0 deg at z-rot 0, correction factor is 185
            escapeCompass.rotation = Quaternion.Slerp(escapeCompass.rotation, Quaternion.Euler(new Vector3(0, 0, compassAngle)), Time.deltaTime*10);
        } else {
            compassImage.SetActive(false);
        }
    }
}