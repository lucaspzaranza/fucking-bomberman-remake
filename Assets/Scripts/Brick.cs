using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class Brick : MonoBehaviour
{
    public AnimatedTile explosionTile;
    public Tilemap terrainMap;

    public Vector2 coordinates;
    public float timeToDestroyBrick;

    private BombermanInput input;
    private InputAction bomb;

    void Awake()
    {
        input = new BombermanInput();
    }

    private void OnEnable()
    {
        //bomb = input.PlayerControls.Bomb;
        //bomb.performed += ExplodeTile;
        //bomb.Enable();
    }

    private void OnDisable()
    {
        //bomb.performed -= ExplodeTile;
        //bomb.Disable();
    }

    void Update()
    {
        
    }
}
