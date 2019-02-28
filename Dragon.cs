using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{

    private int health = 10;
    private float hitTimer = 5;
    private bool isVulnerable = false;
    private int bossStage = 0; //0-Standard Fight, 1-Destroy Platform, 2-Bomb run -1= Death
    private int standardAttack = 0;
    private int flameBallTotal = 0;
    private float flameSpinAngle = 0;
    public List<GameObject> platformList = new List<GameObject>();
    GameObject player;

    public GameObject PlatformFireball;
    public GameObject Fireball;
    public GameObject FireStorm;

    private bool attacking = false;
    private float attackTimer = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!attacking)
        {
            LookAtPLayer();
            attackTimer += Time.deltaTime;
            if (attackTimer >= 1f)
            {
                attacking = true;
            }
        }
        else
        {
            switch (bossStage)
            {
                case 0:
                    if (standardAttack == 0)
                    {
                        FlameBallAttack();
                    }
                    if (standardAttack == 1)
                    {
                        if (isVulnerable == false)
                        {
                            isVulnerable = true;
                        }
                        FlameSpinAttack();
                    }
                    break;
                case 1:
                    break;

            }

        }
    }

    void LookAtPLayer()
    {

    }

    void FlameBallAttack()
    {
        if (flameBallTotal == -1)
        {
            flameBallTotal = 11 - health;
        }
        transform.LookAt(player.transform);
        //Instantiate fireball if there are fireballs and can attack
        if (attackTimer >= 1 && flameBallTotal > 0)
        {
            Instantiate(Fireball);
            attackTimer = .5f;
        }
        else if (attackTimer < 1 && flameBallTotal > 0)
        {
            attackTimer += Time.deltaTime;
        }
        else if (flameBallTotal == 0)
        {
            attacking = false;
            attackTimer = 0;
            flameBallTotal = -1;
            standardAttack = 1;
        }
    }
    void FlameSpinAttack()
    {
        //Shoot flame attack in circle
        //Turn on flame emitter object and collider
        if (health > 5)
        {
            flameSpinAngle += Time.deltaTime * 75;
            transform.Rotate(Vector3.up * (Time.deltaTime * 75), Space.World);
        }
        else
        {

        }
        if (flameSpinAngle >= 360)
        {
            attacking = false;
            attackTimer = 0;
            //Turn off flame emitter and collider
            standardAttack = 0;
        }
    }

    //Do a * shaped blast run
    void FlameRun()
    {

    }

    void DestroyPlatform()
    {
        GameObject nearest = FindPlatformNearPlayer();
        if (nearest != null)
        {
            transform.LookAt(nearest.transform);

        }

    }

    GameObject FindPlatformNearPlayer()
    {
        float distance = 10000;
        float testDistance = 0;
        GameObject nearest = null;
        if (platformList.Count > 0)
        {
            foreach (GameObject g in platformList)
            {
                testDistance = Vector3.Distance(player.transform.position, g.transform.position);
                if (testDistance < distance)
                {
                    distance = testDistance;
                    nearest = g;
                }
            }
            return nearest;
        }
        else
        {
            return null;
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Sword")
        {
            if (health > 0 && hitTimer > 5 && isVulnerable == true)
            {
                health -= 1;
                isVulnerable = false;
                standardAttack = 0;
                if (health == 3 || health == 7)
                {
                    bossStage = 2;
                }
                else if (health == 2 || health == 5 || health == 9)
                {
                    bossStage = 1;
                }
                else if (health == 0)
                {
                    bossStage = -1;
                }
            }
        }
    }
}
