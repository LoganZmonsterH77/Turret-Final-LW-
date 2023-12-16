using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingTest : MonoBehaviour
{
    /*
        1. shoot bullets on mouse click
        2. create bullets if we don't have them in the queue.
        3. bullets that are "destroyed" are added to the queue
        4. bullets are "destroyed" after 1 second or when hitting anything.
    */

    public Queue<Rigidbody> bulletPool = new Queue<Rigidbody>();

    [SerializeField] Rigidbody bulletPrefab;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse0)) { ShootBullet(); }
    }

    void ShootBullet() {
        Rigidbody bullet;                                                       // create a reference to a rigidbody.
        if(bulletPool.Count > 0) {
            bullet = bulletPool.Dequeue();                                      // take a bullet from the pool 
            bullet.gameObject.SetActive(true);                                  // turn the bullet back on.
            bullet.velocity = Vector3.zero;                                     // set bullet velocity to 0;
            StartCoroutine(bullet.GetComponent<Bullet>().Wait());               // tell the bullet to turn itself back off soon.
            
        } else {
            bullet = Instantiate(bulletPrefab);                                 // create a bullet 
            bullet.gameObject.GetComponent<Bullet>().poolScript = this;         // when we create the bullet, assign ourselves
        }

        bullet.transform.position = this.transform.position + Vector3.forward;  // set bullet location and rotation
        bullet.transform.rotation = Quaternion.identity;
        
        bullet.AddForce(Vector3.forward * 50, ForceMode.Impulse);               // shoot the bullet.
    }
}
