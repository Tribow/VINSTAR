using UnityEngine;

//Even more aggressive than the fighter. Does a better job of chasing and decelerates better as well. However, it needs to be closer to the player to detect it
//Shoots a bullet with an arc. It's 4 bullets instead of just 1.
//The bullets generally move slower than the fighter bullets
//Mega health


public class red_warrior : Base_Enemy_Script
{

    public override void Attack_Method()
    {
        for(int i = 0; i < 4; i++)
        {
            GameObject new_bullet = Instantiate(my_bullet, gameObject.transform.position, Quaternion.Euler(0f, 0f, gameObject.transform.rotation.eulerAngles.z + 30 - (20 * i)));
            new_bullet.GetComponent<enemy_bullet_script>().speed = 20;
            mybullets.Add(new_bullet);
        }
        audiomanager.Play_Sound(audio_manager.Sound.shoot_01, transform.position, 1.8f);
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("player");
        }
        //int amount_nearby = Physics2D.OverlapCircleNonAlloc(new Vector2(transform.position.x, transform.position.y), mineral_radius, nearby_minerals, 1 << 8);
        //DrawThis.Polygon(gameObject, 50, 60f, new Vector3(transform.position.x, transform.position.y, -5f), .2f, .2f);

        switch (AI)
        {
            case State.Idle:
                if (transform.position.x > 160f || transform.position.x < -160f || transform.position.y > 137.5f || transform.position.y < -137.5f)
                { //Switch to the Edge AI if it gets too close to the edge of the arena
                    StopAllCoroutines();
                    StartCoroutine(Edge_Movement(30f, new FloatRange(2f, 4f)));
                    AI = State.Edge;
                }
                if (player != null)
                { //Switch to Attack AI once the player is close enough
                    if (Vector2.Distance(transform.position, player.transform.position) <= 30f)
                    {
                        StopAllCoroutines();
                        StartCoroutine(Attack_Movement(25f + p_speed, 5f, 25f, 20f, new Stopwatch(.25f), new FloatRange(2f, 4f), new FloatRange(.85f, 1f), .5f, 2f, 4f));
                        AI = State.Attack;
                    }
                }
                break;

            case State.Edge:
                if (boundary.bounds.Contains(new Vector2(Mathf.Abs(transform.position.x), Mathf.Abs(transform.position.y))))
                { //Switch to the Idle if you made it back
                    StopAllCoroutines();
                    StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 6f), 2));
                    AI = State.Idle;
                }
                break;

            case State.Attack:
                if (player != null)
                {
                    if (Vector2.Distance(transform.position, player.transform.position) > 30f)
                    {
                        StopAllCoroutines();
                        StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 6f), 2));
                        mybullets.Clear(); //Clear the list since you're not attacking anymore
                        maxspeed = ogspeed;
                        decelerating = false; //Just in case
                        AI = State.Idle;
                    }
                }
                break;
        }
        velocity = Speed_Management(transform, maxspeed);
        Transform_Management(transform, turning_speed, velocity, 1.6f);
    }
}
