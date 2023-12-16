using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ObjectPoolingTest poolScript;

    void Start() {
        StartCoroutine(Wait());
    }

    public IEnumerator Wait() {
        yield return new WaitForSeconds(1);
        DisableBullet();
    }

    // void OnCollisionEnter(Collision other) {
    //     DisableBullet();
    // }

    void DisableBullet() {
        // have I already been disabled? if so, do nothing.
        if(this.gameObject.activeSelf == false) return;

        poolScript.bulletPool.Enqueue(this.GetComponent<Rigidbody>());      // add the bullet to the object pool
        this.gameObject.SetActive(false);                                   // turn the bullet off
    }
}
