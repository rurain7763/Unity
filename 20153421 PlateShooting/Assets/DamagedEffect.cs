using UnityEngine;

public class DamagedEffect : MonoBehaviour
{
    ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!particle.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
