using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;

    public void FixedUpdate()
    {
        bulletMechanic();
    }

    public void bulletMechanic() {
        float speed = 50f;
        transform.position += direction * speed * Time.fixedDeltaTime;
    }


    public void setDir(Vector3 dir)
    {
        this.direction = dir.normalized;
        float deg = Mathf.Atan2(this.direction.y, this.direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,deg);
    }
}
