using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

    public GameObject bulletPrefab;

    public Transform BulletSpawn;

    public float bulletSpeed = 30;

    public float lifeTime = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    private void Fire()
    {
        GameObject Bullet = Instantiate(bulletPrefab);

        Physics.IgnoreCollision(Bullet.GetComponent<Collider>(), BulletSpawn.parent.GetComponent<Collider>());
    
        Bullet.transform.position = BulletSpawn.position;

        Vector3 rotation = Bullet.transform.rotation.eulerAngles;

        Bullet.transform.rotation = Quaternion.Euler(rotation.x, transform.eulerAngles.y, rotation.z);

        Bullet.GetComponent<Rigidbody>().AddForce(BulletSpawn.forward * bulletSpeed, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(Bullet, lifeTime));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject Bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(Bullet);
    }
}
