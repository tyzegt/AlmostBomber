﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private List<Vector2> PathToBomberMan = new List<Vector2>();
    private List<Vector2> RandomPath = new List<Vector2>();
    private List<Vector2> CurrentPath = new List<Vector2>();
    private PathFinder PathFinder;
    private bool isMoving;
    private bool SeeBomber;

    public GameObject BomberMan;
    public GameObject DeathEffect;
    public float MoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        if(BomberMan != null)
        {
            PathFinder = GetComponent<PathFinder>();
            ReCalculatePath();
            isMoving = true;

        }
    }

    public void ReCalculatePath()
    {
        PathToBomberMan = PathFinder.GetPath(BomberMan.transform.position);
        
        if (PathToBomberMan.Count == 0)
        {
            SeeBomber = false;
            if (!SeeBomber)
            {                
                var r = Random.Range(0, PathFinder.FreeNodes.Count);                
                RandomPath = PathFinder.GetPath(PathFinder.FreeNodes[r].Position);
                CurrentPath = RandomPath;
                print(CurrentPath.Count);
            }
            
        }
        else
        {
            CurrentPath = PathToBomberMan;
            SeeBomber = true;
        }
    }

    public void Damage(int source)
    {
        if (source == 1)
        {
            Instantiate(DeathEffect, transform.position, transform.rotation);
            Destroy (gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (BomberMan == null) return;

        if(CurrentPath.Count == 0 && Vector2.Distance(transform.position, BomberMan.transform.position) > 0.5f)
        {
            ReCalculatePath();
            isMoving = true;
        }
        if (CurrentPath.Count == 0)
        {
            return;
        }
        

        if (isMoving)
        {
            if(Vector2.Distance(transform.position, CurrentPath[CurrentPath.Count - 1]) > 0.1f)
            {
                
                transform.position = Vector2.MoveTowards(transform.position, CurrentPath[CurrentPath.Count - 1], MoveSpeed * Time.deltaTime);
                
            }
            if(Vector2.Distance(transform.position, CurrentPath[CurrentPath.Count - 1]) <= 0.1f)
            {
                isMoving = false;
            }
        } else
        {

            ReCalculatePath();
            isMoving = true;
        }
    }
}
