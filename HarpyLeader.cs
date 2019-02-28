using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpyLeader : MonoBehaviour {

    private float hp = 40;
    public float speed;
    private bool dead = false;
    private float distToGround;
    private Rigidbody rb;
    public Animator animator;

    public GameObject playerObject;
    private float distToPlayer;
    private bool inCombat = false;
    private Vector3 target;

    [Header("Squad Variables")]
    public Transform returnPosition;

    //Enemy shot variables
    public GameObject shotObject;
    public Transform shotSpawn;
    private float aggroRadius = 40f;
    private float nextShot=2f;
    private float shootTimer = 2f;

    //Enemy dodge variables
    private float nextDodge;
    private float dodgeTimer = 7f;


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);
        if (!dead)
        {
            if (distToPlayer < aggroRadius) //If player is close enough, else return to position
            {
                target = playerObject.transform.position;
                inCombat = true;
                if (distToPlayer < 20 && shootTimer >= nextShot)
                {
                    animator.SetBool("rangedAttack", true);
                    shootTimer = 0f;
                    Instantiate(shotObject, shotSpawn);
                }
                transform.LookAt(target);
            }
            else
            {
                if (Vector3.Distance(transform.position, target) > 5f)
                {
                    target = returnPosition.transform.position;
                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, target, step);
                    transform.LookAt(target);
                }
            }

        }
        else
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector3(0, 0, 0);
                animator.SetBool("isGrounded", true);
                Destroy(gameObject, 5f);
            }
            else
            {
                float step = speed * 5 * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x,-200,transform.position.z), step);
            }
        }
        if (shootTimer <= 2f)
        {
            shootTimer += Time.deltaTime;
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.4f);
        if (hit.collider != null && hit.collider.gameObject.tag == "tetherAble")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Playershot")
        {
            hp -= 10;
            if (hp <= 0)
            {
                Debug.Log("Dead");
                dead = true;
                animator.SetBool("dead", true);
                
            }
        }
        if (other.gameObject.tag == "lowerBound")
        {
            Destroy(gameObject, 5f);
        }
    }

    public bool CheckAlive()
    {
        if(hp > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
