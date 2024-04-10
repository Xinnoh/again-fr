using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TreasureManager : MonoBehaviour
{
    public TMP_Text reward1, reward2, reward3;
    public TMP_Text desc1, desc2, desc3;
    private Weapon[] selectedWeapons = new Weapon[3];

    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private GameObject highlightField;


    private PlayerInputActions playerControls;
    private InputAction button1, button2, button3;

    public GameObject treasureUI;
    private PlayerManager playerManager;
    private WeaponManager weaponManager;
    private GameObject player;
    public bool hasInteracted = false;
    private CanvasGroup canvasGroup;
    private CanvasScaler canvasScaler;
    public AnimationCurve animationCurve;

    [SerializeField] private float fadeInTime = .4f, fadeOutTime = .4f, initialScale = 3f;

    private bool isFadingOut = false;


    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        button1 = playerControls.Player.Button1;
        button1.Enable();
        button1.performed += Button1;
    }


    private void Button1(InputAction.CallbackContext context)
    {
        if (hasInteracted)
        {
            ButtonInput(1);
        }

        else if (PlayerNearby() && !hasInteracted)
        {
            OnTreasureInteracted();
        }

    }
    private void Button2(InputAction.CallbackContext context)
    {
        ButtonInput(2);
    }
    private void Button3(InputAction.CallbackContext context)
    {
        ButtonInput(3);
    }


    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player"); 
        playerManager = player.GetComponent<PlayerManager>();
        weaponManager = player.GetComponent<WeaponManager>();

        AddWeapons(); // Put random weapons in the chest


        if (treasureUI != null)
        {
            canvasGroup = treasureUI.GetComponent<CanvasGroup>();
            canvasScaler = treasureUI.transform.GetComponent<CanvasScaler>();
            canvasGroup.alpha = 0f;
            canvasScaler.scaleFactor = initialScale;
            treasureUI.SetActive(false);
        }
        if (highlightField!= null) highlightField.SetActive(false);
        hasInteracted = false;
    }


    private void Update()
    {
        CheckDistancePlayer();
    }




    private void CheckDistancePlayer()
    {
        if (PlayerNearby())
        {
            if (!hasInteracted)
            {
                if (highlightField != null)
                {
                    weaponManager.nearTreasure = true;
                    highlightField.SetActive(true);
                }

            }
        }

        else
        {
            if (highlightField != null)
            {
                weaponManager.nearTreasure = false;
                highlightField.SetActive(false);
            }
        }
    }

    private bool PlayerNearby()
    {
        return Vector2.Distance(player.transform.position, transform.position) <= interactionDistance;
    }

    public void OnTreasureInteracted()
    {
        hasInteracted = true;
        if (treasureUI != null) treasureUI.SetActive(true);
        if (highlightField != null) highlightField.SetActive(false);
        playerManager.playerActive = false;

        weaponManager.nearTreasure = false;
        StartCoroutine(AnimateCanvasOpen());


        button2 = playerControls.Player.Button2;
        button2.Enable();
        button2.performed += Button2;
        button3 = playerControls.Player.Button3;
        button3.Enable();
        button3.performed += Button3;
    }



    private IEnumerator AnimateCanvasOpen()
    {
        float elapsedTime = 0f;


        while (elapsedTime < fadeInTime)
        {

            float curveFraction = animationCurve.Evaluate(elapsedTime / fadeInTime);

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, curveFraction);

            canvasScaler.scaleFactor = Mathf.Lerp(initialScale, 1f, curveFraction);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        canvasGroup.alpha = 1f;
        treasureUI.transform.localScale = Vector3.one;
    }


    private void AddWeapons()
    {
        List<string> weaponNames = new List<string>();
        List<string> weaponDescs = new List<string>();

        Weapon newWeapon = GetRandomWeaponFromAnyCategory();

        for (int i = 0; i < 3; i++)
        {
            newWeapon = GetRandomWeaponFromAnyCategory();
            while (IsDuplicate(newWeapon))
            {
                newWeapon = GetRandomWeaponFromAnyCategory();
            }

            selectedWeapons[i] = newWeapon;
            weaponNames.Add(newWeapon.name);
            weaponDescs.Add(newWeapon.description);
        }
        DisplayWeaponNames(weaponNames, weaponDescs);
    }

    private Weapon GetRandomWeaponFromAnyCategory()
    {
        // Randomly select a category
        InventorySlot[] slots = { InventorySlot.Light, InventorySlot.Heavy, InventorySlot.Ranged };
        InventorySlot selectedSlot = slots[Random.Range(0, slots.Length)];

        // Get a random weapon from the selected category
        return WeaponList.GetRandomWeapon(selectedSlot);
    }

    private bool IsDuplicate(Weapon weapon)
    {
        // Check if the weapon parameter is null to avoid comparing null values
        if (weapon == null) return false;

        for (int i = 0; i < selectedWeapons.Length; i++)
        {
            // Check for null to avoid comparing with unassigned array slots
            if (selectedWeapons[i] != null && selectedWeapons[i].name == weapon.name)
            {
                return true;
            }
        }
        return false;
    }

    private void DisplayWeaponNames(List<string> names, List<string> descs)
    {
        if (reward1 == null || reward2 == null || reward3 == null)
        {
            return; // WHY DOES THE CODE TRY TO DO IT TWICE I HAVE NO IDEA
        }

        if (names.Count >= 3)
        {
            reward1.text = names[0] ?? "Name Missing";
            reward2.text = names[1] ?? "Name Missing";
            reward3.text = names[2] ?? "Name Missing";

            desc1.text = descs[0] ?? "Description Missing";
            desc2.text = descs[1] ?? "Description Missing";
            desc3.text = descs[2] ?? "Description Missing";

        }
    }


    // Called when the player selects an item
    public void ButtonInput(int button)
    {

        if(hasInteracted == false)
        {
            OnTreasureInteracted();
            return;
        }


        if (isFadingOut) return;
        isFadingOut = true;

        if (button >= 1 && button <= 3)
        {
            Inventory inventory = FindObjectOfType<Inventory>();
            if (inventory != null)
            {
                inventory.AddWeaponToInventory(selectedWeapons[button - 1]);
            }
        }

        playerManager.playerActive = true;

        button1.Disable();
        button1.performed -= Button1;
        button2.Disable();
        button2.performed -= Button2;
        button3.Disable();
        button3.performed -= Button3;

        StartCoroutine(AnimateCanvasClose());
    }



    private IEnumerator AnimateCanvasClose()
    {
        float elapsedTime = 0f;

        // Optional: Make the GameObject's SpriteRenderer disappear instantly
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        while (elapsedTime < fadeOutTime)
        {
            float curveFraction = animationCurve.Evaluate(elapsedTime / fadeOutTime);

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, curveFraction);
            canvasScaler.scaleFactor = Mathf.Lerp(1f, initialScale, curveFraction);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }




}
