using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberMan : MonoBehaviour
{
    private bool ButtonLeft;
    private bool ButtonRight;
    private bool ButtonUp;
    private bool ButtonDown;
    private bool ButtonBomb;
    private bool ButtonDetonate;

    private int BombsAllowed;
    private int FireLength;
    //private int SpeedBoosts;
    private bool NoclipWalls;
    private bool NoclipBombs;
    private bool NoclipFire;
    private bool HasDetonator;


    private bool CanMove;
    private bool IsMoving;
    private bool InsideBomb;
    private bool InsideWall;
    private bool InsideFire;



    public int Direction; // <4 8^ 6> 2v

    public Transform Sensor;
    public float SensorSize = 0.7f;
    public float SensorRange = 0.4f;

    public float MoveSpeed = 2;
    public float SpeedBoostPower = 0.5f;

    public LayerMask StoneLayer;
    public LayerMask BombLayer;
    public LayerMask BrickLayer;
    public LayerMask FireLayer;

    public GameObject Bomb;
    



    // Start is called before the first frame update
    void Start()
    {
        BombsAllowed = 1;
        FireLength = 1;
    }
    // Update is called once per frame
    void Update()
    {
        GetInput();
        GetDirection();
        HandleSensor();
        HandleBombs();
        Move();

        Animate();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PowerUp")
        {
            switch(other.GetComponent<PowerUp>().Type)
            {
                // 0 - extra bomb
                // 1 - fire
                // 2 - speed
                // 3 - noclip wall
                // 4 - noclip fire
                // 5 - noclip bomb
                // 6 - detonator
                case 0:
                    GetExtraBomb();
                    break;
                case 1:
                    GetExtraFire();
                    break;
                case 2:
                    GetExtraSpeed();
                    break;
                case 3:
                    GetNoclipWalls();
                    break;
                case 4:
                    GetNoclipFire();
                    break;
                case 5:
                    GetNoclipBombs();
                    break;
                case 6:
                    GetDetonator();
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    void GetDetonator()
    {
        HasDetonator = true;
    }

    void GetNoclipWalls()
    {
        NoclipWalls = true;
    }

    void GetNoclipBombs()
    {
        NoclipBombs = true;
    }

    void GetNoclipFire()
    {
        NoclipFire = true;
    }

    void GetExtraSpeed()
    {
        //SpeedBoosts++;
        MoveSpeed = MoveSpeed += SpeedBoostPower;
    }

    void GetExtraFire()
    {
        FireLength++;
    }

    void GetExtraBomb()
    {
        BombsAllowed++;
    }

    private void HandleBombs()
    {
        if(ButtonBomb && GameObject.FindGameObjectsWithTag("Bomb").Length < BombsAllowed && !InsideBomb && !InsideFire && !InsideWall)
        {
            Instantiate(Bomb, new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), transform.rotation);
        }
        if(ButtonDetonate)
        {
            print("det");
            var bombs = FindObjectsOfType<Bomb>();
            foreach (var bomb in bombs)
            {
                bomb.Blow();
            }
        }
    }

    private void Move()
    {
        if (!CanMove)
        {
            IsMoving = false;
            return;
        }
        IsMoving = true;
        switch (Direction)
        {
            case 2:
                transform.position = new Vector2(Mathf.Round(transform.position.x), transform.position.y - MoveSpeed * Time.deltaTime);
                break;
            case 4:
                transform.position = new Vector2(transform.position.x - MoveSpeed * Time.deltaTime, Mathf.Round(transform.position.y));
                GetComponent<SpriteRenderer>().flipX = false;
                break;
            case 6:
                transform.position = new Vector2(transform.position.x + MoveSpeed * Time.deltaTime, Mathf.Round(transform.position.y));
                GetComponent<SpriteRenderer>().flipX = true;
                break;
            case 8:
                transform.position = new Vector2(Mathf.Round(transform.position.x), transform.position.y + MoveSpeed * Time.deltaTime);
                break;
            case 5:
                IsMoving = false;
                break;
        }
    }

    void HandleSensor()
    {
        Sensor.transform.localPosition = new Vector2(0, 0);
        InsideWall = Physics2D.OverlapBox(Sensor.position, new Vector2(SensorSize, SensorSize), 0, BrickLayer);
        InsideBomb = Physics2D.OverlapBox(Sensor.position, new Vector2(SensorSize, SensorSize), 0, BombLayer);
        InsideFire = Physics2D.OverlapBox(Sensor.position, new Vector2(SensorSize, SensorSize), 0, FireLayer);
        switch (Direction)
        {            
            case 2:
                Sensor.transform.localPosition = new Vector2(0, -SensorRange);
                break;
            case 4:
                Sensor.transform.localPosition = new Vector2(-SensorRange, 0);
                break;
            case 6:
                Sensor.transform.localPosition = new Vector2(SensorRange, 0);
                break;
            case 8:
                Sensor.transform.localPosition = new Vector2(0, SensorRange);
                break;
        }
        CanMove = !Physics2D.OverlapBox(Sensor.position, new Vector2(SensorSize, SensorSize), 0, StoneLayer);
        if (CanMove)
        {
            if(!NoclipWalls)
                CanMove = !Physics2D.OverlapBox(Sensor.position, new Vector2(SensorSize, SensorSize), 0, BrickLayer);
        }
        if (CanMove && !InsideBomb)
        {
            if (!NoclipBombs)
                CanMove = !Physics2D.OverlapBox(Sensor.position, new Vector2(SensorSize, SensorSize), 0, BombLayer);
        }
            
    }

    void GetDirection()
    {
        Direction = 5;
        if (ButtonLeft) Direction = 4;
        if (ButtonRight) Direction = 6;
        if (ButtonUp) Direction = 8;
        if (ButtonDown) Direction = 2;
    }

    public bool CheckDetonator()
    {
        return HasDetonator;
    }
    
    void GetInput()
    {
        ButtonLeft = Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow);
        ButtonRight = !Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow);
        ButtonUp = !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow);
        ButtonDown = !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow);

        ButtonBomb = Input.GetKeyDown(KeyCode.Z);
        ButtonDetonate = Input.GetKeyDown(KeyCode.X);
    }

    public void AddBomb()
    {
        BombsAllowed++;
    }

    public void AddFireLenght()
    {
        FireLength++;
    }

    public int GetFireLength()
    {
        return FireLength;
    }

    void Animate()
    {
        var animator = GetComponent<Animator>();
        animator.SetInteger("Direction", Direction);
        animator.SetBool("Moving", IsMoving);
    }
}
