using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class HarpySmall : MonoBehaviour {
    private float hp = 10;
    public float speed;
    private bool dead = false;
    private float distToGround;
    private Rigidbody rb;
    public Animator animator;

    public GameObject playerObject;
    private float distToPlayer;
    private Vector3 target;

    [Header ("Squad Variables")]
    public bool squad = false;
    public bool squadLeaderAlive = true;
    public GameObject squadLeader;
    public Transform returnPosition;

    //Enemy shot variables
    public Collider hitBox;
    public float aggroRadius;
    private float nextShot;
    private float shootTimer = 2f;
    private float hitboxTimer = 0f;

    //Enemy dodge variables
    private float nextDodge;
    private float dodgeTimer = 7f;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
	}
	
	// Update is called once per frame
	void Update () {
        distToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);
        if(squad == true)
        {
            if(squadLeader != null)
            {
                if (squadLeader.GetComponent<HarpyLeader>().CheckAlive() == false)
                {
                    aggroRadius = 0;
                    squadLeaderAlive = false;
                }
            }
        }
        if(!dead)
        {
            if (distToPlayer < aggroRadius) //If player is close enough, else return to position
            {
                transform.LookAt(playerObject.transform);
                target = playerObject.transform.position;
                if (distToPlayer <= 2 && shootTimer >= 2)
                {
                    animator.SetTrigger("meleeAttack");
                    shootTimer = 0f;
                    hitBox.gameObject.SetActive(true);
                    hitboxTimer = 0;
                }
                else
                {
                    if (Vector3.Distance(transform.position, target) > 2f)
                    {
                        
                        float step = speed * Time.deltaTime;
                        transform.position = Vector3.MoveTowards(transform.position, target, step);
                    }
                }
            }   
            else
            {
                if(returnPosition != null)
                {
                    target = returnPosition.transform.position;
                }
                else
                {
                    target = new Vector3(transform.position.x, transform.position.y - 5f, transform.position.z);
                }
                if (Vector3.Distance(transform.position, target) > 0.5f)
                {
                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, target, step);
                }
                transform.LookAt(target);
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
                float step = speed * 8 * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, -200, transform.position.y), step);
            }
        }
        if (hitboxTimer < .5f)
        {
            hitboxTimer += Time.deltaTime;
        }
        else
        {
            hitBox.gameObject.SetActive(false);
        }
        if(shootTimer <= 2f)
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
        if(other.gameObject.tag == "Playershot")
        {
            hp -= 10;
            if(hp <= 0)
            {
                Debug.Log("Dead");
                dead = true;
                animator.SetBool("dead", true);
                rb.velocity = new Vector3(0, 20000, 0);
            }
        }
        if(other.gameObject.tag == "lowerBound")
        {
            Destroy(gameObject, 5f);
        }
    }
}
