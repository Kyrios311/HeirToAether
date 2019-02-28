using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AetherPlayerController : MonoBehaviour {

    //Anim variables
    public Animator animator;
    //Respawn variables
    public List<Transform> respawnTransforms;


    //player stats
    bool dead = false;
    private float hp = 100f;
    private float hitTimer = 2f;
    //Movement variables
    Rigidbody rb;
    float distToGround;
    [Header("Movement Settings")]
    public int playerJump;
    public float speed = 20;

    //input
    float horizontalInput;
    float verticalInput;
    Vector3 movementVector = new Vector3(0, 0, 0);
    Camera cam;

    [Header("Tether Settings")]
    //tether variables
    public LineRenderer line;
    public float maxTetherDistance = 50f;
    private float fireTether1Timer = 1; //Tether timers
    public Transform tetherUIPoint1; //Tether UI points to shoot from
    public Image tetherUIImage;
    public Image tetherBackground;
    public Transform playerTetherObject;
    GameObject latchPoint;
    //Vector3 latchPoint;
    bool tether1Active = false;

    [Header("AMS Particle Systems")]
    //boost variables
    float boostTime = 100;
    private float regenRate = 7.5f;
    int boostMultiplier = 5;
    public ParticleSystem AMS1; //AetherManueverSystem
    public ParticleSystem AMS2;
    public ParticleSystem AMSVU;
    public ParticleSystem AMSVD;

    //attack variables
    float attackTimer = 1; //Timer to string combos
    int sequentialAttack = 1; //Variable to keep track of where in combo player is

    [Header("Tether UI Refs")]
    //UI Variables
    public Slider amsSlider;
    public Slider healthSlider;
    private Color redTrans = new Color(1,0,0,.35f);
    private Color whiteTrans = new Color(1, 1, 1, .35f);

    [Header("Audio")]
    //Sound variables
    public List<AudioClip> amsNoises;
    public AudioSource playerAudio;
    // Use this for initialization
    void Start () {
        //Turn off Particle emission on start
        AMS1.enableEmission = false;
        AMS2.enableEmission = false;
        AMSVU.enableEmission = false;
        AMSVD.enableEmission = false;

        //Instantiate the latchpoint gameobject to use for tethering
        latchPoint = Instantiate(new GameObject("latchPoint"));

        distToGround = GetComponent<Collider>().bounds.extents.y;
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if(hitTimer < 2f)
        {
            hitTimer += Time.deltaTime;
        }
        healthSlider.value = hp;
        animator.SetFloat("AttackTimer", attackTimer);
        animator.SetFloat("Speed", rb.velocity.magnitude);
        if (tether1Active)
        {
            line.SetPosition(1, playerTetherObject.position);
        }
        else
        {
            line.positionCount = 1;
        }

        if(attackTimer < 1)
        {
            attackTimer += Time.deltaTime;
        }
        else
        {
            sequentialAttack = 1;
        }

        if(fireTether1Timer < 1)
        {
            fireTether1Timer += Time.deltaTime;
            tetherBackground.color= redTrans;
        }
        else
        {
            tetherBackground.color = whiteTrans;
        }

        amsSlider.value = boostTime;
        PlayerInput();

	}

    void PlayerInput()
    {
        if(!dead)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            CheckTetherDistance(tetherUIPoint1.transform.position);
            if (Input.GetButtonDown("Fire2"))
            {
                if (ShootTether(tetherUIPoint1.transform.position) && fireTether1Timer >= 1)
                {
                    playerAudio.clip = amsNoises[Random.Range(0, amsNoises.Count - 1)];
                    playerAudio.Play();
                    tether1Active = true;
                    fireTether1Timer = 0;
                    animator.SetBool("Tethered", true);
                }
                else if (tether1Active)
                {
                    tether1Active = false;
                }
            }
            if (Input.GetButton("Fire2") && tether1Active)
            {
                line.SetPosition(0, latchPoint.transform.position);
            }
            if (Input.GetButtonUp("Fire2"))
            {
                tether1Active = false;
                animator.SetBool("Tethered", false);
            }
            if (tether1Active)
            {
                TetherInput();
                if (IsGrounded() == false)
                {
                    animator.SetBool("IsInAir", true);
                    AirInput();
                }
                else
                {
                    GroundInput();
                }

            }
            else if (IsGrounded() == true)
            {
                animator.SetBool("IsInAir", false);
                AMS1.enableEmission = false;
                AMS2.enableEmission = false;
                GroundInput();
            }
            else
            {
                animator.SetBool("IsInAir", true);
                AirInput();
            }
        }
        else
        {
            tether1Active = false;
            animator.SetBool("Tethered", false);
        }
    }

    //Player movement type methods
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround+ 0.4f);
    }
    void GroundInput()
    {
        rb.drag = 1;
        //Else recharge AMS
        if(boostTime < 100)
        {
            boostTime += Time.deltaTime * regenRate;
        }
        else
        {
            boostTime = 100;
        }
        

        movementVector = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) * new Vector3(horizontalInput, 0, verticalInput).normalized;
        if(horizontalInput != 0 || verticalInput != 0)
        {
            if (horizontalInput == 0 && verticalInput > 0) //Up input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
            }
            else if (horizontalInput > 0 && verticalInput > 0) //Up Right Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y + 45, 0);
            }
            else if (horizontalInput > 0 && verticalInput == 0) // Right Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y + 90, 0);
            }
            else if (horizontalInput > 0 && verticalInput < 0) // Down Right Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y + 135, 0);
            }
            else if (horizontalInput == 0 && verticalInput < 0)  // Down Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y - 180, 0);
            }
            else if (horizontalInput < 0 && verticalInput < 0) // Down Left Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y - 135, 0);
            }
            else if (horizontalInput < 0 && verticalInput == 0) // Left Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y - 90, 0);
            }
            else if (horizontalInput < 0 && verticalInput > 0) // Up Left Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y - 45, 0);
            }
            GetComponent<Rigidbody>().AddForce(movementVector * speed, ForceMode.Force);
        }
        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 7000 * playerJump, 0));
        }

        if (Input.GetButtonDown("Fire1"))
        {
            transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
            Swing();
        }
    }
    void AirInput()
    {
        rb.drag = .35f;
        if (boostTime < 100)
        {
            boostTime += Time.deltaTime * regenRate;
        }
        else
        {
            boostTime = 100;
        }
        //Set the players movement relative to camera
        movementVector = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) * new Vector3(horizontalInput, 0, verticalInput).normalized;

        //If the player isn't inputting, don't rotate the player
        if (horizontalInput != 0 || verticalInput != 0 && boostTime > 5) //Check to make sure AMS is charged, and if so, move player and drain AMS
        {
            if (horizontalInput == 0 && verticalInput > 0) //Up input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
            }
            else if(horizontalInput > 0 && verticalInput > 0) //Up Right Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y + 45, 0);
            }
            else if(horizontalInput > 0 && verticalInput == 0) // Right Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y + 90, 0);
            }
            else if (horizontalInput > 0 && verticalInput < 0) // Down Right Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y + 135, 0);
            }
            else if (horizontalInput == 0 && verticalInput < 0)  // Down Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y - 180, 0);
            }
            else if (horizontalInput < 0 && verticalInput < 0) // Down Left Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y - 135, 0);
            }
            else if (horizontalInput < 0 && verticalInput == 0) // Left Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y - 90, 0);
            }
            else if (horizontalInput < 0 && verticalInput > 0) // Up Left Input
            {
                transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y - 45, 0);
            }
            GetComponent<Rigidbody>().AddForce(movementVector * speed, ForceMode.Force);
            boostTime -= Time.deltaTime * 15;
            AMS1.enableEmission = true;
            AMS2.enableEmission = true;
        }
        else
        {
            AMS1.enableEmission = false;
            AMS2.enableEmission = false;
        }

        if (Input.GetButtonDown("AMS_Down"))
        {
            if (boostTime > 10)  //Check to make sure AMS is charged, and if so, move player and drain AMS
            {
                
                //AMSV1.Play();
                //AMSV2.Play();
                GetComponent<Rigidbody>().AddForce(new Vector3(0, -200, 0), ForceMode.Impulse);
                boostTime -= 15;
                AMSVD.enableEmission = true;
            }
        }
        else if (Input.GetButtonDown("Jump"))
        {
            if (boostTime > 15)  //Check to make sure AMS is charged, and if so, move player and drain AMS
            {
                //AMSV1.Play();
                //AMSV2.Play();
                GetComponent<Rigidbody>().AddForce(new Vector3(0, 165, 0), ForceMode.Impulse);
                boostTime -= 30;
                AMSVU.enableEmission = false;
            }
        }
        else
        {
            AMSVU.enableEmission = false;
            AMSVD.enableEmission = false;
        }
        
        if(Input.GetButtonDown("Fire1"))
        {
            transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
            Swing();
        }
    }
    void TetherInput()
    {
        TetherPull();
    }
    //Tether Methods

    /// <summary>
    /// Attempt to shoot a tether 
    /// </summary>
    /// <param name="tetherSide"> Pass Vec3 for starting position of tether </param>
    /// <returns></returns>
    bool ShootTether(Vector3 UIPoint) 
    {
        RaycastHit hit;
        Physics.Raycast(cam.ScreenPointToRay(UIPoint), out hit, maxTetherDistance);
        if (hit.collider != null && hit.collider.CompareTag("tetherAble"))
        {
            latchPoint.transform.position = hit.point;
            latchPoint.transform.SetParent(hit.collider.gameObject.transform);
            
            line.positionCount = 2;
            line.SetPosition(0, latchPoint.transform.position);
            line.SetPosition(1, transform.position);
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckTetherDistance(Vector3 UIPoint)
    {
        RaycastHit hit;
        Physics.Raycast(cam.ScreenPointToRay(UIPoint), out hit, maxTetherDistance);
        if (hit.collider != null && hit.collider.CompareTag("tetherAble"))
        {
            tetherUIImage.color = Color.green;
        }
        else
        {
            tetherUIImage.color = Color.red;
        }
    }

    private void TetherPull()
    {
        float pointDistance = Vector3.Distance(transform.position, latchPoint.transform.position);
        if(pointDistance >= 30f)
        {
            GetComponent<Rigidbody>().AddForce((latchPoint.transform.position - transform.position) * (boostMultiplier * 3.5f));
        }
        else if (pointDistance >= 20f)
        {
            GetComponent<Rigidbody>().AddForce((latchPoint.transform.position - transform.position) * (boostMultiplier*2));
        }
        else if (pointDistance >= 10f)
        {
            GetComponent<Rigidbody>().AddForce((latchPoint.transform.position - transform.position) * (boostMultiplier *1.5f));
        }
    }

    //Collisions
    private void OnTriggerEnter(Collider other)
    {
        if (hitTimer >= 2)
        {
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("hit");
                hp -= 5;
                if (hp <= 0)
                {
                    dead = true;
                    PlayerDeath("fall");
                }
            }
            else if (other.CompareTag("enemyShot"))
            {
                Debug.Log("hit");
                hp -= 20;
                if (hp <= 0)
                {
                    dead = true;
                    PlayerDeath("fall");
                }
            }
        }
        else
        {
            if (other.CompareTag("enemyShot"))
            {
                Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("lowerBound"))
        {
            PlayerDeath("fall");
        }
        else if (other.CompareTag("powerup"))
        {
            regenRate = 16;
        }
        else if (other.CompareTag("NextLevel"))
        {
            dead = true;
            cam.SendMessage("NextLevel");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (hitTimer >= 2)
        {
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("hit");
                hp -= 5;
                if (hp <= 0)
                {
                    dead = true;
                    PlayerDeath("fall");
                }
                hitTimer = 0;
            }
            else if (other.CompareTag("enemyShot"))
            {
                Debug.Log("hit");
                hp -= 5;
                if (hp <= 0)
                {
                    dead = true;
                    PlayerDeath("fall");
                }
                hitTimer = 0;
            }
        }
        else
        {
            if (other.CompareTag("enemyShot"))
            {
                Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("lowerBound"))
        {
            PlayerDeath("fall");
        }
        else if (other.CompareTag("powerup"))
        {
            regenRate = 16;
        }
        else if (other.CompareTag("NextLevel"))
        {
            dead = true;
            cam.SendMessage("NextLevel");
        }
    }

    //Combat methods
    private void Swing()
    {
        Debug.Log(attackTimer);
        if (sequentialAttack == 1 && attackTimer > 0)
        {
            animator.SetInteger("Attack", 1);
            animator.SetTrigger("Attack1");
            sequentialAttack = 2;
            attackTimer = 0;
        }
        else if(sequentialAttack == 2 && attackTimer < 1 && attackTimer > 0)
        {
            animator.SetInteger("Attack", 2);
            animator.SetTrigger("Attack2");
            sequentialAttack = 3;
        }
        else if (sequentialAttack == 3 && attackTimer < 1 && attackTimer > 0)
        {
            animator.SetInteger("Attack", 3);
            animator.SetTrigger("Attack3");
            sequentialAttack = 1;
            attackTimer = -1.5f;
        }
    }

    private void PlayerDeath(string deathType)
    {
        dead = true;
        if(deathType == "fall")
        {
            cam.SendMessage("PlayerDeath");
        }
    }

    private void FallRespawn()
    {
        hp = 100;
        dead = false;
        Transform nearest;
        float distance;
        float testDistance;
        nearest = respawnTransforms[0];
        distance = Vector3.Distance(nearest.position, transform.position);
        testDistance = distance;
        foreach (Transform g in respawnTransforms)
        {
            testDistance = Vector3.Distance(nearest.position, transform.position);
            if (testDistance < distance)
            {
                distance = testDistance;
                nearest = g;
            }
        }
        transform.position = nearest.position;
        transform.rotation = nearest.rotation;
    }

}
