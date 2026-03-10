using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] private Transform pfBullet;
    [SerializeField] private Transform target;
    [SerializeField] private Transform prefabHolder;

    float timer;
    int shooterPos;
    float environmentTime;
    public int bulletCount = 0;

    private void Start()
    {
        timer = 1f;
        shooterPos = 0;
        environmentTime = 0f;
    }

    private void FixedUpdate()
    {
        environmentTime += Time.fixedDeltaTime;
        if (environmentTime > timer)
        {
            Shoot();
            environmentTime = 0f;
        }
    }
    private void Shoot()
    {
        bulletCount++;
        target.GetComponent<PlaneController>().AddReward(0.05f);
        shooterPos = Random.Range(0, transform.childCount);
        Transform chosen = transform.GetChild(shooterPos);
        Transform usedBullet = Instantiate(pfBullet, chosen.position, Quaternion.identity, prefabHolder);
        usedBullet.GetComponent<Bullet>().setDir(target.position - chosen.position);
        Destroy(usedBullet.gameObject, 3f);
        if (bulletCount > prefabHolder.childCount) {
            target.GetComponent<PlaneController>().GainReward();
            bulletCount = prefabHolder.childCount;
        }
    }

    public void deleteAllPrefab()
    {
        foreach(Transform prefab in prefabHolder)
        {
            Destroy(prefab.gameObject);
        }
    }
}
