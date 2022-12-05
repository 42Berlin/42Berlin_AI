using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerGame : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float jumpForce;
    [SerializeField] private Animator anim;
    

    [Header("Settings")]
    [SerializeField] private float groundDistance = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDistance);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            anim.SetBool("jump", true);
            rb.velocity = Vector3.up * jumpForce;
        }
    }
}
