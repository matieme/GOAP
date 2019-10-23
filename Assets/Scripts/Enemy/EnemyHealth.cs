using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : EntityLife
{
    public int startingHealth = 100;
    public float sinkSpeed = 0.5f;
    public int scoreValue = 10;
    public AudioClip deathClip;

    Animator anim;
    AudioSource enemyAudio;
    ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;
    bool isSinking;

    public Action OnDeath;

    void Awake()
    {
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();
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
        if (_isDeath)
            return;

        enemyAudio.Play();
        Damaged(amount);

        hitParticles.transform.position = hitPoint;

        hitParticles.Play();

        if (Life <= 0)
        {
            Death();
        }
    }


    void Death()
    {
        Destroyed();
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
