using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddyGuy : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 2;
    [SerializeField] private AudioClip deathSound;

    private Transform[] _waypointsList;
    private Transform _currentWaypoint;
    private int _waypointIndex = 1; //starts at 1 intentionally. waypointsList was adding the transform of the parent object and I was being lazy here :D
    private Animator _anim;
    private Rigidbody[] _rigidBodies;

    private bool _waiting = false;

    private void Awake()
    {
        _rigidBodies = GetComponentsInChildren<Rigidbody>();
    }

    private void Start()
    {
        _waypointsList = GameObject.FindGameObjectWithTag("Waypoints").GetComponentsInChildren<Transform>();
        _currentWaypoint = _waypointsList[1];
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        if (_waiting)
            return;

        _anim.SetBool("isMoving", true);
        //rotation gubbins from https://docs.unity3d.com/ScriptReference/Vector3.RotateTowards.html
        Vector3 dir = _currentWaypoint.position - transform.position;
        Vector3 lookDir = Vector3.RotateTowards(transform.forward, dir, _rotationSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(lookDir);

        //check if we are at the waypoint
        if (Vector3.Distance(transform.position, _currentWaypoint.position) <= .5f)
        {
            _waiting = true;
            _anim.SetBool("isMoving", false);
            StartCoroutine(GetNextWaypoint());
            return;
        }
    }

    private IEnumerator KillBuddyGuy()
    {
        Debug.Log("Shindeiru");
        yield return new WaitForSeconds(2);
        Debug.Log(gameObject.name + "Nani!?");
        yield return new WaitForSeconds(1);
        _anim.enabled = false; //this will let the ragdoll take over.
        foreach (Rigidbody rb in _rigidBodies)
        {
            rb.isKinematic = false;
        }
        AudioManager.instance.PlaySFX(deathSound);
        Destroy(gameObject, 3f);
    }

    private IEnumerator GetNextWaypoint()
    {
        //increase the index
        _waypointIndex++;

        //check if we are at the last waypoint
        if (_waypointIndex >= _waypointsList.Length)
        {
            Debug.Log("Omae wa mo...");
            StartCoroutine(KillBuddyGuy());
            yield break;
        }

        //Idle at the waypoint for a bit
        yield return new WaitForSeconds(3);

        //set new waypoint
        _currentWaypoint = _waypointsList[_waypointIndex];
        _waiting = false;
    }
}
