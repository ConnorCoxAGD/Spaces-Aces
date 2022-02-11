﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetableObject),typeof(Rigidbody))]
public class AIBehaviour : MonoBehaviour
{
    public GameObject explosionObject;  
    public bool isAwaitingRespawn;
    public float speed = 100;
    private Quaternion randRot = new Quaternion(0,0,0,0);
    private Rigidbody _rb;

    public enum DecisionStates{
        RandomFlying,
    }

    private void Awake() {
        StartCoroutine(DecisionTime());
        _rb = GetComponent<Rigidbody>();
    }

    private IEnumerator DecisionTime(){
        float randTime = Random.Range(2,15);
        yield return new WaitForSeconds(randTime);
        randRot = Random.rotation;
        StartCoroutine(DecisionTime());
        
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Crash Hazard") || collision.gameObject.layer == LayerMask.NameToLayer("Player")){

            Eliminate();
            return;
        }
        if(collision.gameObject.layer == LayerMask.NameToLayer("GunAmmo") || collision.gameObject.layer == LayerMask.NameToLayer("Missile")){
            Eliminate();
            return;
          
        }
    }

    public void Eliminate(){
        var deathLocation = transform.position;
        Object.Instantiate(explosionObject, deathLocation, transform.rotation);

        isAwaitingRespawn = true;

        gameObject.SetActive(false);
    }

    private void FixedUpdate() {
        Debug.DrawRay(transform.position, Vector3.forward, Color.yellow);
        if(Physics.Raycast(transform.position, Vector3.forward, 1000)){
            randRot = Random.rotation;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, randRot, 100 * Time.deltaTime);
        _rb.AddRelativeForce(0,0,speed, ForceMode.Acceleration);
        
    }
}
