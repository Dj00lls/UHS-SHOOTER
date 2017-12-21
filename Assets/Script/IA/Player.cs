﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Humanoid {
    
    public List<Transform> destination;
    private const int coverPos = 5;
    private KeyCode btnTir = KeyCode.T;

    //cam
    public Camera MainCam;
    Vector3 PosFPS;
    Vector3 PosTPS;
    
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
        PosFPS = MainCam.transform.position;
        //Set Player Destination
        GoToNextPosition();
    }

    // Update is called once per frame
    public override void Update() {

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

<<<<<<< HEAD
                
                SwitchState(Etape.GoCovered);
                if (!EnemiesFind) FindEnemies();
=======
                SwitchCam(false);
                SwitchState(Etape.Covered);
>>>>>>> a1b42543b85feef44f83f2dd813c3f2a24df9e66

                break;

            case Etape.GoCovered:
                transform.rotation = Quaternion.Slerp(transform.rotation, GetDestination().rotation, 10 * Time.deltaTime);
                if (Mathf.Approximately(transform.rotation.y, GetDestination().rotation.y))
                    SwitchState(Etape.Covered);
                    
                break;
            // Si le joueur est à couvert, un appuie sur le bouton haut nous fait passer dans l'étape "Uncovered"
            case Etape.Covered:

<<<<<<< HEAD
                //switch cam position
                SwitchPosCam(PosTPS);
=======
                //Rotate player and animation
                transform.rotation = Destination.rotation;

                //transform.rotation = Quaternion.Slerp(transform.rotation, Destination.rotation, 10 * Time.deltaTime);

                //Find enemies aprés la cam transition 
                if (!EnemiesFind) FindEnemies();
>>>>>>> a1b42543b85feef44f83f2dd813c3f2a24df9e66

                //Go to Undercovered State
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    SwitchState(Etape.GoUncovered);
                }

                break;

            case Etape.GoUncovered:
                //switch cam position
                SwitchPosCam(PosFPS);

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

    public void GetTacticalPos(Transform _campos)
    {
        PosTPS = _campos.position;
        //CamAnimator = TacticalCam.gameObject.GetComponent<Animator>();
    }
    void SwitchPosCam(Vector3 _PosDestination)
    {
        MainCam.transform.rotation = Quaternion.Slerp(transform.rotation, GetDestination().rotation, 10 * Time.deltaTime);
        MainCam.transform.position = Vector3.MoveTowards(transform.position, _PosDestination, 10 * Time.deltaTime);
        //CamAnimator.SetBool("IsMain", _IsMain);
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

    GameObject ChooseTarget(int _direction)
    {
        GameObject _enemyCloser = null;
        Vector3 _relativeCloser = Vector3.zero;
        for (int i = 0; i < Enemies.Count; i++)
        {
           if (!Enemies[i])
            {
                Enemies.Remove(Enemies[i]);
                
            }

            
            else if (Enemies[i] != target)
            {
                Vector3 relativePoint = transform.InverseTransformPoint(Enemies[i].transform.position);
                //si c'est gauche
                if (relativePoint.x < 0.0f && _direction == -1)
                {

                    if (_relativeCloser.x < relativePoint.x || _relativeCloser.x == 0.0f)
                    {
                        _relativeCloser = relativePoint;
                        _enemyCloser = Enemies[i];
                    }

                }

                //Si c'est droite
                if (relativePoint.x > 0.0f && _direction == 1)
                {
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

