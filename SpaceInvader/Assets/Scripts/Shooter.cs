using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifeTime = 5f;
    [SerializeField] private float baseFiringRate = 0.2f;
    [SerializeField] private float firingRateVariance = 0;
    [SerializeField] private float minimumFiringRate = 0.1f;
    [SerializeField] private bool useAI;
    [SerializeField] private List<Transform> firePoints;

    [HideInInspector]
    public bool isFiring;

    private List<Coroutine> firingCors;
    //private Coroutine firingCor;
    private Vector2 moveDirection;
    private void Start()
    {
        firingCors = new List<Coroutine>();
        if (useAI)
        {
            isFiring = true;
            moveDirection = transform.up * -1;
        }
        else
        {
            moveDirection = transform.up;
        }
    }

    private void Update()
    {
        Fire();
    }

    void Fire()
    {
        //if (isFiring && firingCor == null)
        //{
        //    firingCor = StartCoroutine(FireContinuously());
        //}
        //else if(!isFiring && firingCor != null)
        //{
        //    StopCoroutine(firingCor);
        //    firingCor = null;
        //}
        if (isFiring && firingCors.Count == 0)
        {
            for (int i = 0; i < firePoints.Count; i++)
            {
                firingCors.Add(StartCoroutine(FireContinuously(firePoints[i])));
            }
        }
        else if (!isFiring && firingCors.Count > 0)
        {
            for (int i = 0; i < firingCors.Count; i++)
            {
                StopCoroutine(firingCors[i]);
            }
            firingCors.Clear();
        }
    }

    IEnumerator FireContinuously(Transform firePoint)
    {
        while (true)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = moveDirection * projectileSpeed;
            }

            Destroy(projectile, projectileLifeTime);

            float timeToNextProjectile =
                Random.Range(baseFiringRate - firingRateVariance, baseFiringRate + firingRateVariance);

            timeToNextProjectile = Mathf.Clamp(timeToNextProjectile, minimumFiringRate, float.MaxValue);

            yield return new WaitForSeconds(timeToNextProjectile);
        }
    }
}
