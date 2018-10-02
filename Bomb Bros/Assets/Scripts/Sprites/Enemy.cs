using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    //Physics var
    Vector2 grav;

    //Script var
    Bro bro;

    //Enemy vars
    float shootTime;
    GameObject[] goals;
    float giveUpTime;
    Vector2 goal;
    float goalRad;
    float lastX;
    float checkXTime;
    float checkSafeTime;
    float safeDis;

    //Script vars
    Game game;
    
    //Player vars
    GameObject playerObj;
    Rigidbody2D playerRb;


    void Awake() {
        //Physics init
        grav = Physics2D.gravity;

        //Script init
        bro = GetComponent<Bro>();

        //Enemy inits
        if (PracticeDifficulty.difficulty == 0) {
            shootTime = 4f;
        }

        else if (PracticeDifficulty.difficulty == 1) {
            shootTime = 2.5f;
        }

        else if (PracticeDifficulty.difficulty == 2) {
            shootTime = 1f;
        }

        else {
            shootTime = 0.5f;
        }
        goals = GameObject.FindGameObjectsWithTag("Goal");
        giveUpTime = 3f;
        goalRad = 0.5f;
        lastX = 0;
        checkXTime = 0.1f;
        checkSafeTime = 0.1f;
        safeDis = 2.5f;
    }



    void Start() {
        //Script init
        game = GameObject.Find("Main Camera").GetComponent<Game>();

        //Player init
        playerObj = game.player;
        playerRb = playerObj.GetComponent<Rigidbody2D>();


        GetGoal();

        StartCoroutine(GiveUp());
        StartCoroutine(Shoot());
        StartCoroutine(CheckX());
        StartCoroutine(CheckSafe());
    }



    void Update() {
        if (!bro.isDead) {
            Move();
        } 
    }



    IEnumerator CheckSafe() {
        while (true) {
            if (!bro.isDead) {
                Vector2 newPos = bro.rb.position + bro.rb.velocity * checkSafeTime;
                if (!IsSafe(newPos, checkSafeTime, safeDis)) {
                    GetGoal();
                }
            }
            yield return new WaitForSeconds(checkSafeTime);
        } 
    }



    IEnumerator CheckX() {
        while (true) {
            if (!bro.isDead) {
                if (Mathf.Abs(bro.rb.position.x - lastX) < checkXTime * bro.moveSpeed && bro.onGround) {
                    bro.rb.velocity = new Vector2(bro.rb.velocity.x, bro.jumpSpeed);
                }
                lastX = bro.rb.position.x;
            }
            yield return new WaitForSeconds(checkXTime);
        }  
    }



    IEnumerator Shoot() {
        while (true) {
            if (!bro.isDead) {
                CanShoot(bro.rb.position, playerRb.position, 0.1f, playerObj);
            }
            yield return new WaitForSeconds(shootTime);
        }
    }



    IEnumerator GiveUp() {
        while (true) {
            if (!bro.isDead) {
                GetGoal();
            }
            yield return new WaitForSeconds(giveUpTime);
        } 
    }



    void Move() {
        if (!AtGoal()) {
            if (goal.x > bro.rb.position.x) {
                bro.Move(1);
            }

            if (goal.x < bro.rb.position.x) {
                bro.Move(-1);
            }
        }
        else {
            GetGoal();
        }
    }



    void GetGoal() {
        int ran = Random.Range(0, goals.Length - 1);
        goal = goals[ran].transform.position;
    }



    bool AtGoal() {
        if (Mathf.Abs(goal.x - bro.rb.position.x) <= goalRad) {
            return true;
        }
        else {
            return false;
        }
    }



    bool IsSafe(Vector2 pos, float time, float safedis) {
        /*
         Checks whether a position will have any projectiles within
         a certain radius of it in a given amount of time.
         
        Args:
            pos - The position to check at
            time - The time to check for
            safedis - The radius where anything outside it is considered
                      safe
        */

        GameObject[] projs = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject proj in projs) {
            Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
            Vector2 newPos = projRb.position + projRb.velocity * time + 0.5f * grav * time * time;
            if((newPos - pos).magnitude < safedis) {
                return false;
            }
        }
        return true;
    }



    Vector2 AngleRange(Vector2 fromPos, Vector2 toPos) {
        /*
         (2D only)
         Assuming that the normalized gravity vector is <0, -1>, this
         function will find the range of angles in standard position where
         it would be physically possible to shoot a projectile from one
         point to another.

        Args:
            fromPos - The point where the projectile would be shot
            toPos - The projectile's target

        Returns:
            The range of possible angles (in radians) in standard position 
            as a 2D vector. Where a = closest to horizontal and b = furthest
            from horizontal in the vector <a, b>.
        */

        Vector2 rel = toPos - fromPos;
        float relTan = Mathf.Atan2(rel.y, rel.x);
        if(rel.x >= 0) {
            return new Vector2(relTan + 2 * Mathf.PI, 5 * Mathf.PI / 2);
        }
        else {
            if (rel.y >= 0) {
                return new Vector2(relTan, Mathf.PI / 2);
            }
            else {
                return new Vector2(relTan + 2 * Mathf.PI, Mathf.PI / 2);
            }
        }
    }



    Vector2 ArcVelocity(Vector2 fromPos, Vector2 toPos, Vector2 dir) {
        /*
         (2D only)
         This function finds the velocity vector needed to go from one position
         to another in a certain direction. Assuming that that direction is physically 
         possible.

        Args:
            fromPos - Starting position
            toPos - Ending position
            dir - The desired normalized direction

        Returns:
            The required 2D velocity vector.
        */
        if (fromPos.x - toPos.x == 0) {
            if (toPos.y >= fromPos.y) {
                return dir * Mathf.Sqrt(-2 * grav.y * (toPos.y - fromPos.y));
            }
            else {
                return Vector2.zero;
            }
        }
        else {
            Vector2 rel = toPos - fromPos;
            float numerator = 0.5f * grav.y * Mathf.Pow(rel.x, 2);
            float denominator = Mathf.Pow(dir.x, 2) * (rel.y - dir.y * rel.x / dir.x);
            return dir * Mathf.Sqrt(numerator / denominator);
        }  
    }



    Vector3 PathFunc(Vector2 pos, Vector2 vel) {
        Vector3 result = new Vector3();
        result.x = 0.5f * grav.y / Mathf.Pow(vel.x, 2f);
        result.y = (vel.y / vel.x) - (grav.y * pos.x / Mathf.Pow(vel.x, 2f));
        result.z = (0.5f * grav.y * Mathf.Pow(pos.x, 2f) / Mathf.Pow(vel.x, 2f)) - (vel.y * pos.x / vel.x) + pos.y;
        return result;
    }



    bool CurveCast(Vector3 func, float precision, Vector2 domain, GameObject target) {
        /*
            This function checks whether a projectile that follows the quadratic 
            path, func, will hit the player or collide with something else.

            Arguments:
                func - <a, b, c> where y = ax^2 + bx + c is the path function
                precision - The delta x of each chunk
                domain - The domain of the function to check
                target - The desired target to check for

            Returns:
                True if the cast hit the target before anything else, false otherwise.
        */

        float numRays = Mathf.Abs(domain.y - domain.x) / precision;

        Vector2 prev = new Vector2();
        prev.x = domain.x;
        prev.y = func.x * Mathf.Pow(prev.x, 2) + func.y * prev.x + func.z - 1f;

        Vector2 curr = new Vector2();
        RaycastHit2D result;
        

        for (int i = 1; i < numRays; i++) {
            if (domain.x < domain.y) {
                curr.x = domain.x + i * precision;
            }
            if (domain.x > domain.y) {
                curr.x = domain.x - i * precision;
            }
            curr.y = func.x * Mathf.Pow(curr.x, 2) + func.y * curr.x + func.z - 1f;
            result = Physics2D.Linecast(prev, curr);
            Debug.DrawLine(new Vector3(prev.x, prev.y, 0), new Vector3(curr.x, curr.y, 0), Color.white, 5f);

            if(result.collider != null) {
                if (result.collider.gameObject == target) {
                    return true;
                }
                else {
                    return false;
                }
            }
            
            prev.x = curr.x;
            prev.y = curr.y;
        }
        return false;
    }



    bool CanShoot(Vector2 fromPos, Vector2 toPos, float precision, GameObject target) {
        Vector2 range = AngleRange(fromPos, toPos);
        if (range.y - range.x >= 2 * Mathf.Deg2Rad) {    
            for (float i = range.x + Mathf.Deg2Rad; i < range.y; i+=Mathf.Deg2Rad) {
                Vector2 dir = new Vector2(Mathf.Cos(i), Mathf.Sin(i)).normalized;
                Vector2 newFromPos = fromPos + dir * bro.fireRad;
                Vector2 vel = ArcVelocity(newFromPos, toPos, dir);
                Vector3 func = PathFunc(newFromPos, vel);
                Vector2 domain = new Vector2(newFromPos.x, toPos.x);
                bool cast = CurveCast(func, precision, domain, target);
                if (cast) {
                    return bro.Fire(newFromPos, vel);
                }
            }
            return false;
        }
        else if (range.x - range.y >= 2 * Mathf.Deg2Rad) {
            for (float i = range.x - Mathf.Deg2Rad; i > range.y; i-=Mathf.Deg2Rad) {
                Vector2 dir = new Vector2(Mathf.Cos(i), Mathf.Sin(i)).normalized;
                Vector2 newFromPos = fromPos + dir * bro.fireRad;
                Vector2 vel = ArcVelocity(newFromPos, toPos, dir);
                Vector3 func = PathFunc(newFromPos, vel);
                Vector2 domain = new Vector2(newFromPos.x, toPos.x);
                bool cast = CurveCast(func, precision, domain, target);
                if (cast) {
                    return bro.Fire(newFromPos, vel);
                }
            }
            return false;
        }
        else if(range.x == range.y || Mathf.Abs(range.x - range.y) == Mathf.PI) {
            Vector2 dir = new Vector2(0, 1);
            Vector2 newFromPos = fromPos + dir * bro.fireRad;
            Vector2 vel = ArcVelocity(newFromPos, toPos, dir);
            //Ray Cast
            bool cast = true;
            if (cast) {
                return bro.Fire(newFromPos, vel);
            }
            return false;
        }
        else {
            return false; //Less than 2 degrees from <0, 1> (Impossible)
        }  
    }
}

