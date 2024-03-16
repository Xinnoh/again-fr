using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MinimapControl : MonoBehaviour
{
    private PlayerInputActions playerControls;
    private InputAction mapToggle, moveMap;


    private PlayerManager playerManager;

    private bool mapOpen;
    private Vector2 moveDirection;

    [SerializeField] private float cameraMoveSpeed;

    [SerializeField] private Image gameCover;
    [SerializeField] private RawImage mapSprite;
    [SerializeField] private GameObject mapCamera;

    private Vector3 cameraOriginalPosition;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        mapToggle = playerControls.Player.Map;
        mapToggle.Enable();
        mapToggle.performed += Map;

        moveMap = playerControls.Player.Move;


    }

    private void OnDisable()
    {
        mapToggle.Disable();
        mapToggle.performed -= Map;
    }

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        mapOpen = false;
    }

    private void Update()
    {
        if (mapOpen)
        {
            MapControls();
        }
    }

    private void MapControls()
    {
        moveDirection = moveMap.ReadValue<Vector2>();

        Vector3 move = new Vector3(moveDirection.x, moveDirection.y, 0) * cameraMoveSpeed * Time.deltaTime;

        mapCamera.transform.position += move;
    }

    private void Map(InputAction.CallbackContext context)
    {
        if (mapSprite == null || gameCover == null)
        {
            return;
        }

        if (!mapOpen)
        {
            OpenMap();
        }

        else
        {
            CloseMap();
        }

        mapOpen = !mapOpen;
    }

    private void OpenMap()
    {
        // pause and unpause game
        cameraOriginalPosition = transform.position;

        mapSprite.color = new Color(mapSprite.color.r, mapSprite.color.g, mapSprite.color.b, 1f); 
        gameCover.color = new Color(gameCover.color.r, gameCover.color.g, gameCover.color.b, 1f);
        moveMap.Enable();

        playerManager.playerActive = false;
    }

    private void CloseMap()
    {
        mapSprite.color = new Color(mapSprite.color.r, mapSprite.color.g, mapSprite.color.b, 0f);
        gameCover.color = new Color(gameCover.color.r, gameCover.color.g, gameCover.color.b, 0f);
        moveMap.Disable();

        mapCamera.transform.position = cameraOriginalPosition;
        playerManager.playerActive = true;
    }

}
