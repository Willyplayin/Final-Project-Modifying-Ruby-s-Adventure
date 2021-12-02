using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    int isInfinite = 0;

    public int maxHealth = 5;

    public GameObject projectilePrefab;
    public GameObject hurtPrefab;
    public GameObject healPrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip ribbitSound;

    public AudioSource winMusic;
    public AudioSource loseMusic;
    public AudioSource backgroundMusic;

    public int health { get { return currentHealth; } }
    public int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    public int scoreValue = 0;
    public Text scoreText;

    public Text winText;
    public Text loseText;

    public Text cogText;

    public Text cogTimerText;

    public bool gameOver = false;
    public bool isMoveable = true;

    public bool beatLevel1 = false;

    private RubyController rubyController;

    public int ammo { get { return currentAmmo; } }
    public int currentAmmo;

    public float displayTime = 6.0f;
    float timerDisplay;

    // Start is called before the first frame update
    void Start()
    {
        timerDisplay = -1.0f;

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentAmmo = 4;
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        scoreText.text = "Robots Fixed: " + scoreValue.ToString();
        winText.text = "";
        loseText.text = "";
        cogTimerText.text = "";
        cogText.text = "Cog Ammo: " + currentAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerDisplay >= 0)
        {
            cogTimerText.text = "UNLIMITED AMMO OVER IN:\n" + (int)(timerDisplay + 1);
            timerDisplay -= Time.deltaTime;

            if (timerDisplay < 0)
            {
                isInfinite = 0;
                cogText.text = "Cog Ammo: " + currentAmmo;
                cogTimerText.text = "";
            }
        }

        if (isMoveable == true)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Vector2 move = new Vector2(horizontal, vertical);

            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);

            if (isInvincible)
            {
                invincibleTimer -= Time.deltaTime;
                if (invincibleTimer < 0)
                    isInvincible = false;
            }

            if (Input.GetKeyDown(KeyCode.C) && currentAmmo > 0 && isInfinite == 0)
            {
                Launch();
                currentAmmo -= 1;
                cogText.text = "Cog Ammo: " + currentAmmo;
            }
            else if (Input.GetKeyDown(KeyCode.C) && isInfinite == 1)
            {
                Launch();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
                if (hit.collider != null)
                {
                    PlaySound(ribbitSound);
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        if (beatLevel1 == false)
                            character.DisplayDialog();
                        else
                        {
                            SceneManager.LoadScene("Level2");
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKey(KeyCode.R))

        {

            if (gameOver == true)

            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

            }

        }
    }

    void FixedUpdate()
    {
        if (isMoveable == true)
        {
            Vector2 position = rigidbody2d.position;
            position.x = position.x + speed * horizontal * Time.deltaTime;
            position.y = position.y + speed * vertical * Time.deltaTime;

            rigidbody2d.MovePosition(position);
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject hurtAnimation = Instantiate(hurtPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }

        if (amount > 0)
        {
            GameObject healAnimation = Instantiate(healPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth == 0)
        {
            gameOver = true;
            winText.text = "";
            winMusic.Stop();
            loseText.text = "You lose! Press 'R' to restart!";
            backgroundMusic.Stop();
            loseMusic.Play();
            isMoveable = false;

            animator.enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ChangeScore(int Score)
    {
        scoreValue += Score;
        scoreText.text = "Robots Fixed: " + scoreValue.ToString();

        if (scoreValue == 4)
        {
            winText.text = "Talk to Jambi to visit stage two!";
            beatLevel1 = true;
        }
        if (scoreValue == 10)
        {
            winText.text = "You win! Game created by William Burke.";
            backgroundMusic.Stop();
            winMusic.Play();
            gameOver = true;
        }
    }

    public void ChangeAmmo(int count)
    {
        currentAmmo += count;

        if (isInfinite == 0)
            cogText.text = "Cog Ammo: " + currentAmmo;
        else
            cogText.text = "Cog Ammo: UNLIMITED!";
    }

    public void SetInfinite()
    {
        isInfinite = 1;
        cogText.text = "Cog Ammo: UNLIMITED!";
        timerDisplay = displayTime;
        cogTimerText.text = "UNLIMITED AMMO OVER IN:\n" + (int)(timerDisplay + 1);
    }

    public void TeleportPlayer(float locationX, float locationY)
    {
        transform.position = new Vector2(locationX, locationY);
    }
}
