﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//This script controls the health of enemy bots
public class EnemyHealth : MonoBehaviour {
    public float startingHealth = 30;
    public GameObject explosion;
    public GameObject bigExplosion;
    public AudioClip hitSoundClip;
    public AudioSource hitSound;
    public float pointValue = 20;
    private float totalHealth;
    private GameObject lastBulletCollision;
    public AnimationManager animationManager;
    private bool dead;
    private Queue<GameObject> explosions = new Queue<GameObject>();
    private readonly int smallExplosionLimit = 12;
    private Queue<GameObject> bigExplosions = new Queue<GameObject>();
    private readonly int bigExplosionLimit = 6;
    public GameObject stunEffect;
    private FeedbackController feed;
    private Rigidbody ragdoll;

    void Start() {
        feed = GameObject.FindGameObjectWithTag("HUD").GetComponent<FeedbackController>();
        totalHealth = startingHealth;
        ragdoll = gameObject.GetComponent<Rigidbody>();

        for (int i = 0; i < smallExplosionLimit; ++i) {
            GameObject splode = Instantiate(explosion, gameObject.transform.position + transform.up, gameObject.transform.rotation);
            splode.transform.parent = transform;
            splode.SetActive(false);
            explosions.Enqueue(splode);
        }

        for (int i = 0; i < bigExplosionLimit; ++i) {
            GameObject splode = Instantiate(bigExplosion, gameObject.transform.position + transform.up, gameObject.transform.rotation);
            splode.transform.parent = transform;
            splode.SetActive(false);
            bigExplosions.Enqueue(splode);
        }
    }

    public void stun(float duration) {
        if (!dead) {
            StartCoroutine(stunRoutine(duration));
        }
    }

    private IEnumerator stunRoutine(float duration) {
        stunEffect.SetActive(true);
        EnemyShoot es = gameObject.GetComponentInChildren<EnemyShoot>();
        EnemyMovement em = gameObject.GetComponentInChildren<EnemyMovement>();
        es.enabled = false;
        em.stunned = true;
        yield return new WaitForSeconds(duration);

        em.enabled = true;
        em.stunned = false;
        stunEffect.SetActive(false);
        ragdoll.isKinematic = true;

        yield return null;
    }

    public void takeDamage(float damage, int paybackMultiplier=0) {
        float prevHealth = totalHealth;
        totalHealth -= damage;

        if (totalHealth <= 0 && !dead) {
            // Get up to half the original health as payback, adjusted due to fish cannon scoring too much cash because it OHKOs right now
            float profit = pointValue + (Mathf.Clamp(prevHealth, 1, startingHealth/2) * paybackMultiplier);
            StateManager.cashOnHand += profit;
            float deathDuration = animationManager.playDeathAnimation();
            GetComponent<EnemyMovement>().enabled = false;
            GetComponent<EnemyShoot>().enabled = false;
            animationManager.animator.PlayQueued("shrink");
            Destroy(gameObject, deathDuration + 0.5f);
            dead = true;
            feed.profit(profit);
        }
    }

    public void showDamageExplosion(Queue<GameObject> queue, float volumeMultiplier = 0.65f) {
        // Play sound effect and explosion particle system
        if (queue == null) {
            queue = bigExplosions;
        }
        GameObject explode = queue.Dequeue();
        explode.SetActive(true);
        hitSound.PlayOneShot(hitSoundClip, volumeMultiplier * Settings.volume);
        explosions.Enqueue(explode);
        explode.transform.position = transform.position + transform.up;
        explode.transform.rotation = transform.rotation;
        explode.transform.parent = gameObject.transform;
        StartCoroutine(explode.GetComponent<Explosion>().timeout());
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Bullet" || col.gameObject.tag == "Missile") {
            if (col.gameObject == lastBulletCollision) {
                return;
            }
            lastBulletCollision = col.gameObject;

            // Reduce health
            BulletBehavior bullet = col.gameObject.GetComponent<BulletBehavior>();
            if (bullet == null) {  // try checking parents
                bullet = col.gameObject.GetComponentInParent<BulletBehavior>();
            }
            if (bullet.grounded) {  // only airborne projectiles should hit for now
                return;
            }
            takeDamage(bullet.damage, 1);
            if (col.gameObject.tag == "Bullet") {
                showDamageExplosion(explosions, 0.4f);
            } else if (col.gameObject.tag == "Missile") {
                showDamageExplosion(bigExplosions, 0.65f);
            }

            // Doesn't work for new bots
            // If bot goes under 50% health, make it look damaged
            /*
            if (totalHealth <= startingHealth / 2 && (totalHealth + bulletDamage) > startingHealth / 2) {
                if (gameObject.tag == "Blaster") {
                    for (int i = 0; i < 2; i++) {
                        Vector3 randomDirection = new Vector3(Random.value, Random.value, Random.value).normalized;
                        gameObject.transform.GetChild(0).gameObject.
                        transform.GetChild(0).gameObject.
                        transform.GetChild(2).gameObject.transform.GetChild(i)
                        .gameObject.transform.GetChild(0).gameObject.transform.rotation = Quaternion.LookRotation(randomDirection);
                    }
                }
                if (gameObject.tag == "Lobber") {
                    for (int i = 0; i < 2; i++) {
                        gameObject.transform.GetChild(0).gameObject.
                        transform.GetChild(0).gameObject.
                        transform.GetChild(4).gameObject.transform.GetChild(2 * i)
                        .gameObject.transform.position += new Vector3(0, 0.1f, 0);
                    }
                }
            }
            */
        }
    }
}
