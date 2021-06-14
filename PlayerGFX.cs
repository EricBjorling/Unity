using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    public Rigidbody2D rb;
    public bool facingRight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.x >= 0.01f)
        {
            facingRight = true;
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else if (rb.velocity.x <= -0.01f)
        {
            facingRight = false;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
