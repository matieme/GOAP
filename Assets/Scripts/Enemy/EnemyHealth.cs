using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : EntityLife
{
    public int startingHealth = 100;
    public int criticalLifeAmount;
    public float sinkSpeed = 0.5f;
    public int scoreValue = 10;
    public AudioClip deathClip;

    Animator anim;
    AudioSource enemyAudio;
    public ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;
    bool isSinking;

    public Action OnDeath;

    void Awake()
    {
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
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

    public bool IsInCriticalLife()
    {
        return criticalLifeAmount >= Life;
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
