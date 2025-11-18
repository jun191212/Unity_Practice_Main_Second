using UnityEngine;

public class ParticleEffectController : MonoBehaviour
{
    [Header("Particle Settings")]
    public ParticleSystem particleSystem;
    public float lifetime = 2f;
    public bool playOnAwake = true;

    void Start()
    {
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        if (particleSystem != null && playOnAwake)
        {
            particleSystem.Play();
        }

        Destroy(gameObject, lifetime);
    }

    public void PlayEffect()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    public void StopEffect()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
    }
}
