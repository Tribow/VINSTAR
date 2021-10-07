using UnityEngine;

public class particle_die_script : MonoBehaviour
{
    ParticleSystem particle_death;

    // Start is called before the first frame update
    void Start()
    {
        particle_death = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, particle_death.main.duration - 1);
    }
}
