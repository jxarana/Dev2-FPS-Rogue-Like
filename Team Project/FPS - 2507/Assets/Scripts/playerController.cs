using System.Collections;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] int HPOrig;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpVel;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int dashMax;
    [SerializeField] Transform camPivot;
    [SerializeField] float mouseSensitivity = 3f;

    public int goldCount;
    public int upgradePoints;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int magMax;
    [SerializeField] int maxAmmo;

  
    int dashCount;
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;
    float dashCooldownTimer;
    bool isDashing;
    float dashTimeLeft;
    private Vector3 dashDirection;



    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HP;
    int speedOrig;

    float shootTimer;
    float xRotation = 0f;

    bool hasSlamunlocked = false;
    bool hasDashUnlocked = false;
    bool hasGrappleUnlocked = false;

    
    int magCurrent;
    int currentAmmo;
   
    public int dmgUp;

   
    public bool isGrappling;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HP = HPOrig;
        speedOrig = speed;
        isGrappling = false;
        magCurrent = magMax;
        currentAmmo = maxAmmo;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        sprint();
        movement();
        
        handleCamera();

    }

    void movement()
    {

        shootTimer += Time.deltaTime;

        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
            dashCount = 0;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * Time.deltaTime);

        if (!isGrappling)
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isGrappling && shootTimer > shootRate && magCurrent < 0)
            {
                shoot();
                updatePlayerUI();
            }
            else if(!isGrappling && shootTimer > shootRate && magCurrent == 0)
            {
                reload();
                updatePlayerUI();
            }
            else if (isGrappling)
            {
                GetComponent<GrappleHook>()?.ApplySwing();
            }
        }

        /*
         if(Input.GetButtonDown("Grapple"))
        {
            if(!isGrappling)    
            {
             GetComponent<GrappleHook>()?.ApplySwing();     If dont want the player to be able to grapple while grappling  
            }
            

            GetComponent<GrappleHook>()?.ApplySwing();         If it doesnt matter




        }
         
         
         
         
         
         
         */


        if (Input.GetButtonDown("Fire1"))
        {
            if(shootTimer > shootRate && magCurrent > 0)
            {
                shoot();
                updatePlayerUI();
            }
            else if(shootTimer > shootRate && magCurrent == 0)
            {
                reload();
                updatePlayerUI();
            }
        }

        if(Input.GetButtonDown("Reload"))
        {
            if(magCurrent != magMax)
            {
                reload();
                updatePlayerUI();
            }
        }

    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpVel;
            jumpCount++;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        magCurrent--;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage + dmgUp);
            }
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        updatePlayerUI();

        StartCoroutine(damageFlashScreen());

        if (HP <= 0)
        {
            //you dead!
            gameManager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.ammoBar.fillAmount = (float)magCurrent / magMax;
        gameManager.instance.inMagCount.text = magCurrent.ToString();
        gameManager.instance.currAmmoCount.text = currentAmmo.ToString();


    }

    IEnumerator damageFlashScreen()
    {
        gameManager.instance.playerDamagePanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamagePanel.SetActive(false);
    }


    void reload()
    {
        if (currentAmmo > 0)
        {
            magCurrent = magMax;
            currentAmmo -= magMax;
        }
    }

    public void replenishAmmo()
    {
        currentAmmo = maxAmmo;
        magCurrent = magMax;
        updatePlayerUI();   
    }

    void dash()
    {
        
    }

    public void  healhp(int ammount)
    {
        HP = Mathf.Min(HP + ammount, HPOrig);
        updatePlayerUI();
    }

    public void dashUnlock()
    {
        hasDashUnlocked = true;
    }

    public void grappleUnlock()
    {
        hasGrappleUnlocked = true;
    }

    public void slamUnlock()
    {
        hasSlamunlocked = true;
    }

    public void dashCountUp()
    {
        dashMax += 1;
    }

    public void jumpCountUp()
    {
        jumpMax += 1;
    }

    public void speedUp()
    {
        speed += 1;
        speedOrig = speed;
    }

    public bool dashReturn()
    {
        return hasDashUnlocked;
    }

    public bool grappleReturn()
    {
        return hasGrappleUnlocked;
    }

    public bool slamReturn() {

        return hasSlamunlocked;
    }
    void handleCamera()
    {
        if (Time.timeScale <= 0f) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
        camPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

}
