﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// A script the was made to manage creating the clone objects for feedback.
/// </summary>
public class FeedbackController : MonoBehaviour
{
    private Queue<TextMeshProUGUI> shotLosses { get; } = new Queue<TextMeshProUGUI>();
    private int numLossesToDisplay = 60;
    private Queue<TextMeshProUGUI> earns { get; } = new Queue<TextMeshProUGUI>();
    private int numEarnsToDisplay = 16;
    public TextMeshProUGUI shotLoss;
    public TextMeshProUGUI earn;
    public Image hitVignette;

    public void Start() {
        for (int i = 0; i < numLossesToDisplay; ++i) {
            TextMeshProUGUI sh = Instantiate(shotLoss);
            sh.transform.SetParent(transform);
            sh.enabled = false;
            shotLosses.Enqueue(sh);
        }
        for (int i = 0; i < numEarnsToDisplay; ++i) {
            TextMeshProUGUI ea = Instantiate(earn);
            ea.transform.SetParent(transform);
            ea.enabled = false;
            earns.Enqueue(ea);
        }
    }

    /// <summary>
    /// Creates the feedback on the screen for money lost when shooting.
    /// </summary>
    /// <param name="cost"></param>
    public void shoot(float cost) {
        TextMeshProUGUI clone = shotLosses.Dequeue();
        clone.text = "-" + (cost.ToString("F0"));
        clone.color = new Color(clone.color.r, clone.color.g, clone.color.b, 1f);
        clone.transform.localPosition = new Vector3(Screen.width*-0.15f, 0, 0);
        clone.transform.Translate(Random.Range(-50f, 0), Random.Range(-50f, 50f), Random.Range(-50f, 50f), Space.Self);
        clone.enabled = true;
        shotLosses.Enqueue(clone);

        StartCoroutine(timeout(clone, 2f));
    }

    /// <summary>
    /// Creates the feedback on the screen for money gained from killing enemies.
    /// </summary>
    /// <param name="amount"></param>
    public void profit(float amount) {
        TextMeshProUGUI clone = earns.Dequeue();
        clone.text = "+" + (amount.ToString("F0"));
        clone.color = new Color(clone.color.r, clone.color.g, clone.color.b, 1f);
        clone.transform.localPosition = new Vector3(Screen.width*0.4f, 0, 0);
        clone.transform.Translate(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f), Space.Self);
        clone.enabled = true;
        earns.Enqueue(clone);

        StartCoroutine(timeout(clone, 2f));
    }

    /// <summary>
    /// Coroutine to time out the created clones.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator timeout(TextMeshProUGUI text, float duration) {
        for (float i = duration; i >= 0; i -= Time.deltaTime) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, i/duration);
            yield return null;
        }
        text.enabled = false;
        yield return null;
    }
}
