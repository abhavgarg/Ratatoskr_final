﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

    public float maxSpeed = 10f;
    bool facingRight = true;
    Animator anim;
    bool movementAllowed = true;

    bool grounded = false;
    public GameObject groundCheck;
    float groundRadius = 0.75f;
    public LayerMask whatIsGround;
    public float jumpForce = 700f;
    Camera cam;
    bool day = true;
 

    public GameObject[] dayassets;
    public GameObject[] nightassets;
    public GameObject[] dayback;
    public GameObject[] nightback;
    RaycastHit hit;
    private UnityEngine.AudioSource[] source;
    public GameObject aud;


    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        dayassets = GameObject.FindGameObjectsWithTag("day_obj");
        nightassets = GameObject.FindGameObjectsWithTag("night_obj");
        dayback = GameObject.FindGameObjectsWithTag("day_bgd");
        nightback = GameObject.FindGameObjectsWithTag("night_bgd");
        Toggle();
        aud = GameObject.FindGameObjectWithTag("Music");
        source = GetComponents<UnityEngine.AudioSource>();
        source[3].volume = 1f;
        source[3].Play();
    }
	
	// Update is called once per frame - Good for input/game mechanic updating
	void Update ()
    {
        
        if(grounded && Input.GetKeyDown(KeyCode.Space) && movementAllowed)
        {
            anim.SetBool("ground", false);
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
            source[0].volume = 0.1f;
            source[0].Play();
        }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {

            source[1].volume = 0.5f;
            source[1].Play();

            if (!day)
            {
                Toggle();
                day = true;
            }

            else if (day)
            {
                ToggleBack();
                day = false;
            }
        }
        //aud.GetComponent<Audio>().StopMusic();
    }

    // Called at set interval each time. Good for physics
    void FixedUpdate()
    {
        Rigidbody2D rigidBodyComp = GetComponent<Rigidbody2D>();

        grounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundRadius, whatIsGround);
        anim.SetBool("ground", grounded);

        anim.SetFloat("vSpeed", rigidBodyComp.velocity.y);

        float move = Input.GetAxis("Horizontal");

        if (movementAllowed)
        {
            rigidBodyComp.velocity = new Vector2(move * maxSpeed, rigidBodyComp.velocity.y);
            anim.SetFloat("speed", Mathf.Abs(move));

            if (move > 0 && !facingRight)
            {
                Flip();
            }
            else if (move < 0 && facingRight)
            {
                Flip();
            }
        }
        else
        {
            if(rigidBodyComp.velocity.x > 0)
            {
                float newXVelocity = rigidBodyComp.velocity.x - maxSpeed * Time.deltaTime;
                rigidBodyComp.velocity = new Vector2(Mathf.Max(0f, newXVelocity), rigidBodyComp.velocity.y);
            }
            else if(rigidBodyComp.velocity.y < 0)
            {
                float newXVelocity = rigidBodyComp.velocity.x + maxSpeed * Time.deltaTime;
                rigidBodyComp.velocity = new Vector2(Mathf.Min(0f, newXVelocity), rigidBodyComp.velocity.y);
            }

            anim.SetFloat("speed", Mathf.Abs(rigidBodyComp.velocity.x / maxSpeed));
        }

        if (Physics.SphereCast(rigidBodyComp.transform.position, 0.5f, -(transform.up), out hit, 0.2f, whatIsGround))
        {
            rigidBodyComp.transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal));
        }

        if (rigidBodyComp.position.y <= -5.5f)
        {
            PlayerEndGame();
        }
        //aud.GetComponent<UnityEngine.AudioSource>().Stop();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
		if((other.gameObject.tag == "PickupObject") && movementAllowed)
        {
            other.gameObject.GetComponent<PickupControllerScript>().onPickupTriggered();
        }
		else if((other.gameObject.tag == "ExitGate") && movementAllowed)
        {
            PlayerEndGame();
        }
    }

	void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.tag == "Enemy" && movementAllowed)
		{
			this.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
			anim.SetBool("dead", true);
			PlayerEndGame();
		}
	}

    private void Toggle()
    {
        foreach (GameObject aa in dayassets)
        {
            // aa.SetActive(true);

            EdgeCollider2D[] myColliders = aa.GetComponents<EdgeCollider2D>();

            foreach (EdgeCollider2D c in myColliders)
            {
                c.enabled = true;
            }
            /*if (aa.GetComponent<EdgeCollider2D>())
            {
                aa.GetComponent<EdgeCollider2D>().enabled = true;
            }*/
            if (aa.GetComponent<SpriteRenderer>())
            {
                aa.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1.0f);
            }

            if (aa.GetComponent<PlatformEffector2D>())
            {
                aa.GetComponent<PlatformEffector2D>().enabled = true;
            }

            if (aa.GetComponent<PolygonCollider2D>())
            {
                aa.GetComponent<PolygonCollider2D>().enabled = true;
            }

        }


        foreach (GameObject bb in nightassets)
        {
            //bb.SetActive(false);

            EdgeCollider2D[] myColliders = bb.GetComponents<EdgeCollider2D>();

            foreach (EdgeCollider2D c in myColliders)
            {
                c.enabled = false;
            }
            /*if (bb.GetComponent<EdgeCollider2D>())
            {
                bb.GetComponent<EdgeCollider2D>().enabled = false;
                
            }*/
            if (bb.GetComponent<SpriteRenderer>())
            {
                bb.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
            }

            if (bb.GetComponent<PlatformEffector2D>())
            {
                bb.GetComponent<PlatformEffector2D>().enabled = false;
            }

            if (bb.GetComponent<PolygonCollider2D>())
            {
                bb.GetComponent<PolygonCollider2D>().enabled = false;
            }
        }

        foreach (GameObject aa in dayback)
        {
             aa.SetActive(true);
        }


        foreach (GameObject bb in nightback)
        {
            bb.SetActive(false);
        }

    }
    private void ToggleBack()
    {
        foreach (GameObject aa in dayassets)
        {
            //aa.SetActive(false);

            EdgeCollider2D[] myColliders = aa.GetComponents<EdgeCollider2D>();

            foreach (EdgeCollider2D c in myColliders)
            {
                c.enabled = false;
            }

            /*if (aa.GetComponent<EdgeCollider2D>())
            {
                aa.GetComponent<EdgeCollider2D>().enabled = false;
            }*/
            if (aa.GetComponent<SpriteRenderer>())
            {
                aa.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
            }

            if (aa.GetComponent<PlatformEffector2D>())
            {
                aa.GetComponent<PlatformEffector2D>().enabled = false;
            }

            if (aa.GetComponent<PolygonCollider2D>())
            {
                aa.GetComponent<PolygonCollider2D>().enabled = false;
            }
        }

        foreach (GameObject bb in nightassets)
        {
            //bb.SetActive(true);

            EdgeCollider2D[] myColliders = bb.GetComponents<EdgeCollider2D>();

            foreach (EdgeCollider2D c in myColliders)
            {
                c.enabled = true;
            }
            /*if (bb.GetComponent<EdgeCollider2D>())
            {
                bb.GetComponent<EdgeCollider2D>().enabled = true;
            }*/
            if (bb.GetComponent<SpriteRenderer>())
            {
                bb.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1.0f);
            }
            if (bb.GetComponent<PlatformEffector2D>())
            {
                bb.GetComponent<PlatformEffector2D>().enabled = true;
            }

            if (bb.GetComponent<PolygonCollider2D>())
            {
                bb.GetComponent<PolygonCollider2D>().enabled = true;
            }
        }
        foreach (GameObject aa in dayback)
        {
            aa.SetActive(false);
        }

        foreach (GameObject bb in nightback)
        {
            bb.SetActive(true);
        }
    }

    private void PlayerEndGame()
    {
        movementAllowed = false;
        source[2].volume = 0.6f;
        source[2].Play();
        source[3].Stop();
        GameObject gameOverView = GameObject.Find("GameOverView");
        gameOverView.GetComponent<GameOverControllerScript>().BeginEndGame();
    }
}
