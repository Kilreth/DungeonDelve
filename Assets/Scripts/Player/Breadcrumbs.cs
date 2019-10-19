﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadcrumbs : MonoBehaviour
{
    public GameObject Breadcrumb;           // prefab
    private GameObject breadcrumbsParent;   // empty object
    private GameObject player;
    private Collider playerCollider;
    [SerializeField]
    private float dropHeight = 0.7f;
    [SerializeField]
    private float throwStrength = 0.8f;
    [SerializeField]
    private float pickUpRadius = 0.5f;
    [SerializeField]
    private float pickUpDistance = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        breadcrumbsParent = new GameObject("Breadcrumbs");
        player = gameObject;
        playerCollider = player.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.Instance.GameState != GameState.Active)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            DropBreadcrumb();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpBreadcrumbs();
        }
    }

    private void PickUpBreadcrumbs()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 9;
        RaycastHit[] collisions = Physics.SphereCastAll(ray, pickUpRadius, pickUpDistance, layerMask);
        for (int i = 0; i < collisions.Length; ++i)
        {
            Destroy(collisions[i].transform.gameObject);
        }
    }

    private void DropBreadcrumb()
    {
        Vector3 position = new Vector3(player.transform.position.x,
                                       player.transform.position.y + dropHeight,
                                       player.transform.position.z);
        GameObject breadcrumb = Instantiate(Breadcrumb, position, Random.rotation, breadcrumbsParent.transform);
        breadcrumb.GetComponent<Rigidbody>().AddForce(
            player.transform.forward * GM.Instance.BlockScale * throwStrength, ForceMode.Impulse);
        Physics.IgnoreCollision(playerCollider, breadcrumb.GetComponent<Collider>());
    }
}
