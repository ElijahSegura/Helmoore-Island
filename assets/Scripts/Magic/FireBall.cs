using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

    private int level = 0;
    private float maxDistance = 2f;
    private float damage = 20f;
    public GameObject explosion;

	void OnCollisionEnter(Collision col)
    {
        if(!col.collider.tag.Equals("Enemy"))
        {
            Explosion(col);
            GetComponent<MeshRenderer>().enabled = false;
            GetComponentInChildren<ParticleSystem>().Stop();
            Destroy(gameObject, 1f);
        }
        else
        {
            col.collider.GetComponent<Mob>().damage((float)(damage + (damage * (0.2 * level))));
            GetComponent<MeshRenderer>().enabled = false;
            GetComponentInChildren<ParticleSystem>().Stop();
            Destroy(gameObject, 1f);
        }
        GameObject.Instantiate(explosion).transform.position = transform.position;
    }

    private void Explosion(Collision col)
    {
        Collider[] hits = Physics.OverlapSphere(col.contacts[0].point, maxDistance);
        foreach (Collider ray in hits)
        {
            if(!ray.tag.Equals("Enemy"))
            {
                Rigidbody rb = ray.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(15, transform.position, maxDistance, 1f, ForceMode.Impulse);
                }
            }
            else
            {
                float distance = Vector3.Distance(ray.transform.position, transform.position);
                ray.GetComponent<Mob>().damage(((float)(damage + (damage * (0.2 * level)))) * ((maxDistance - distance) / maxDistance));
            }
        }
    }
}
