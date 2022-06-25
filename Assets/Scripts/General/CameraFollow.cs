using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followTime = 10f;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        Vector3 newPosition = new Vector3(0f, offset.y, player.position.z + offset.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, followTime * Time.deltaTime);
    }
}
