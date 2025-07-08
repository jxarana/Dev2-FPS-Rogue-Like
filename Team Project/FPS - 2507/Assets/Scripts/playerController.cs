using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] int Hp;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpVel;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] int shootdamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    [SerializeField] int magMax;
    [SerializeField] int maxAmmo;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;

    float shootTimer;

    bool hasSlam;

    bool hasDashUnlocked;
    int dashMax;
    int dashCount;
    int magCurrent;
    int currentAmmo;
    int hpOrig;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpOrig = Hp;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        sprint();

        movement();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer > shootRate)
        {
            if (magCurrent > 0)
                shoot();
            else
                reload();
        }

        if (Input.GetButton("Reload") && magCurrent != magMax)
        {
            reload();
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

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            //Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.gameObject.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootdamage);
            }
        }
    }

    void reload()
    {
        magCurrent = magMax;
        currentAmmo -= magMax;
    }

    public void updatePlayerUi()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)Hp / hpOrig;
        gameManager.instance.ammoBar.fillAmount = (float)magCurrent / magMax;
    }

    void slam()
    {

    }

    void dash()
    {
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward) * 3;
    }
}
