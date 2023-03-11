using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drum : MonoBehaviour, IDamagable
{
    [SerializeField] float Health = 300F;
    [SerializeField] GameObject explosionParticle;
    [SerializeField] Rigidbody rBody;
    [SerializeField] float destructionDamage = 100, destructionRange = 10, destructionForce = 100;

    float waitTime;

    private void Awake()
    {
        waitTime = Random.value * 3F;
    }

    public void AddDamage(float damage, Vector3 force)
    {
        DecreaseHealth(damage);
        if(rBody) rBody.velocity = force;
    }

    public void AddDamage(float damage)
    {
        DecreaseHealth(damage);
    }

    public void Destroy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, destructionRange);
        IDamagable damagable;
        float distance, damageMultiplier;
        Vector3 direction;

        foreach(Collider collider in colliders)
        {
            damagable = collider.gameObject.GetComponent<IDamagable>();
            if (damagable == null || damagable == (IDamagable) this) continue;

            direction = collider.transform.position - transform.position;
            distance = direction.magnitude;
            direction.Normalize();

            damageMultiplier = (destructionRange - distance) / destructionRange;

            damagable.AddDamage(destructionDamage * damageMultiplier, direction * destructionForce * damageMultiplier);
        }

        GameObject damageObject = Instantiate(explosionParticle);
        damageObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
    public void DecreaseHealth(float damage)
    {
        if (Health <= 0) return;
        Health -= damage;
        if (Health <= 0) Invoke("Destroy", waitTime);
    }
}
