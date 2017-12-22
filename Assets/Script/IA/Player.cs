﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Humanoid {
    
    public List<Transform> destination;
    private const int coverPos = 5;
    private KeyCode btnTir = KeyCode.T;


    
    //Enemy
    bool EnemiesFind = false;
    List<GameObject> Enemies = new List<GameObject>();
    int indexEnemies = 0;

    //Tuto
    public TutoManager TutoMngr;

	// Use this for initialization
	void Start ()
    {
        Init();
        //Set Player Destination
        GoToNextPosition();
    }

    // Update is called once per frame
    void Update() {

        Debug.Log("update player");

        if (!IsAlive())
        {
            return;
        }

        switch (HumanState)
        {
            // Si le joueur arrive à destination, on passe dans l'étape "Arrived"
            case Etape.Moving:

                // Lorsque le joueur arrive, on enlève sa position dans la list et on passe dans l'étape arrivée
                if (HasArrived())
                {
                    destination.RemoveAt(0);
                    SwitchState(Etape.Arrived);
                }

                break;

            // Si le joueur est arrivé, on fait spawn les ennemis et on passe dans l'étape "Covered"
            case Etape.Arrived:

                SwitchState(Etape.GoCovered);
                if (!EnemiesFind) FindEnemies();


                break;

            case Etape.GoCovered:
                transform.rotation = Quaternion.Slerp(transform.rotation, GetDestination().rotation, 10 * Time.deltaTime);
                if (Mathf.Approximately(transform.rotation.y, GetDestination().rotation.y))
                    SwitchState(Etape.Covered);
                    
                break;
            // Si le joueur est à couvert, un appuie sur le bouton haut nous fait passer dans l'étape "Uncovered"
            case Etape.Covered:


                //switch cam position

                //Go to Undercovered State
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    SwitchState(Etape.GoUncovered);
                }

                break;

            case Etape.GoUncovered:
                //switch cam position

                break;
            // Si le joueur est à découvert, un appuie sur le bouton bas nous fait passer dans l'étape "Covered". Il peut également tirer
            case Etape.Uncovered:

                //Look enemy and shoot
                if (target)
                {
                    
                    LookToTarget();

                    if (Input.GetKeyDown(btnTir))
                    {
                        Fire();
                    }
                }
                //
                else
                {

                }

                //Choose Enemy
                 if (Input.GetKeyDown(KeyCode.LeftArrow))
                 {
                    target = ChooseTarget(-1);
                 }

                 if (Input.GetKeyDown(KeyCode.RightArrow))
                 {
                    target = ChooseTarget(1);
                 }

               
                //Go to Covered State
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    col.enabled = false;
                    SwitchState(Etape.Covered);
                }


                break;
        }
    }

    public void GoToNextPosition()
    {
        Debug.Log("Gotonextpos");
        EnemiesFind = false;
        
        if (destination[0])
        {
            SetDestination(destination[0]);
            MoveToThisPoint();
            
        }
    }


    public void FindEnemies()
    {
        GameObject[] _enemies;
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");

        
        for (int i = 0; i< _enemies.Length; i++)
        {
            Enemies.Add(_enemies[i]);
        }
        EnemiesFind = true;

        Debug.Log(Enemies.Count);

        //Player targeting enemies
        for (int i = 0; i < Enemies.Count; i++)
        {
            Enemies[i].GetComponent<TargetManager>().Targeting();
        }
    }

    //return a target
    GameObject ChooseTarget(int _direction)
    {
        GameObject _enemyCloser = null;
        Vector3 _relativeCloser = Vector3.zero;
        for (int i = 0; i < Enemies.Count; i++)
        {
            //si l'enemi est mort on l'enleve de la liste
           if (!Enemies[i])
            {
                Enemies.Remove(Enemies[i]);
                
            }

            //si il est different de la cible on test ou il est 
            else if (Enemies[i] != target)
            {
                Vector3 relativePoint = transform.InverseTransformPoint(Enemies[i].transform.position);
                //si le joueur a choisi gauche
                if (relativePoint.x < 0.0f && _direction == -1)
                {
                    //on prend l'ennemi le plus proche a gauche
                    if (_relativeCloser.x < relativePoint.x || _relativeCloser.x == 0.0f)
                    {
                        _relativeCloser = relativePoint;
                        _enemyCloser = Enemies[i];
                    }

                }

                //si le joueur a choisi droite
                if (relativePoint.x > 0.0f && _direction == 1)
                {
                    //on prend l'ennemi le plus proche a droite
                    if (_relativeCloser.x > relativePoint.x || _relativeCloser.x == 0.0f)
                    {
                        _relativeCloser = relativePoint;
                        _enemyCloser = Enemies[i];
                    }
                }
            }
        }

        if (!_enemyCloser) Debug.Log("Mauvaise direction");
        return _enemyCloser;
    }
}

