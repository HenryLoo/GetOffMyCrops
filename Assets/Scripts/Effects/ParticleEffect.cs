using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    public int NumParticles;
    private ParticleSystem _particleSystem;
    private float _currentTicks;


    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    // Use this for initialization
    void Start()
    {
        _particleSystem.Emit( NumParticles );
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the particle effect if duration is over
        if( _currentTicks >= _particleSystem.main.duration  )
        {
            _particleSystem.Stop();
            Destroy( this.gameObject );
        }

        _currentTicks += Time.deltaTime;
    }
}
