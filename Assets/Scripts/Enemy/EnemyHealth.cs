using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public float sinkSpeed = 0.5f;
    public int scoreValue = 10;
    public AudioClip deathClip;

    Animator anim;
    AudioSource enemyAudio;
    ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;
    bool isDead;
    bool isSinking;

    public Action OnDeath;


    void Awake()
    {
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        currentHealth = startingHealth;
    }

    void Update()
    {
        if (isSinking)
        {
            transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
        }
    }


    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (isDead)
            return;

        enemyAudio.Play();

        currentHealth -= amount;

        hitParticles.transform.position = hitPoint;

        hitParticles.Play();

        if (currentHealth <= 0)
        {
            Death();
        }
    }


    void Death()
    {
        isDead = true;
        capsuleCollider.isTrigger = true;
        anim.SetTrigger(StringTagManager.animDead);

        enemyAudio.clip = deathClip;
        enemyAudio.Play();

        if (OnDeath != null)
        {
            OnDeath();
        }
    }

    public void StartSinking()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        isSinking = true;
        Destroy(gameObject, 2f);
    }
}
