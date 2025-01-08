using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    [Header("Inventory")]
    public Inventory inventory;

    //[Header("Movement")]
   // public float moveSpeed;
   // Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
       // rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       // float x = Input.GetAxis("Horizontal");
       // float z = Input.GetAxis("Vertical");

       // Vector3 direction = new Vector3(x,0,z);

       // Vector3 velocity = direction * moveSpeed;
       // rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }
}
