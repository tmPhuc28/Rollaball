using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private bool isOnGround = true;
    Rigidbody rb;
    [SerializeField] float movementSpeed = 2.5f;
    [SerializeField] float jumpForce = 2.5f;
    // Double jump: Số lần nhảy
    private const int MAX_JUMP = 2;
    //Số bước nhảy hiện tại
    private int currentJump = 0;
    // Tính điểm
    public Text score;
    public AudioSource gameOver, winGame, jump, eatStar, soundTrack, soundReset;
    private int point = 0;

    //Tính điểm trên màn hình Over Game
    public Text pointsOverGame;
    public Text pointsWinGame;
    public GameObject overMenu;
    // Reset Game: khi rơi khỏi bề mặt
    private float y = -2f;
    public GameObject winMenu;
    public GameObject win;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        point = 0;
        Show();
        Time.timeScale = 1f;
        win.GetComponent<Renderer>().enabled = false;
        overMenu.SetActive(false);
        winMenu.SetActive(false);
    }
    void Update()
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);
        Jump();
        ResetGames();
        Show();
        MenuPaused();
    }
    // Dùng để hiển thị khi tạm dừng game
    public void MenuPaused()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //soundTrack.Pause();
            if (FindObjectOfType<PauseGame>().isPaused)
            {
                FindObjectOfType<PauseGame>().ResumeGame();
            }
            else
            {
                FindObjectOfType<PauseGame>().PauseGames();
            }
        }
    }
    // Hàm dùng khi bị thua
    public void GameOver()
    {
        Time.timeScale = 0f; // *Note:...
        overMenu.SetActive(true);
        pointsOverGame.text = point.ToString();
    }
    private bool IsPausedSound = false;
    // Hàm dùng khi muốn reset game
    public void ResetGames()
    {

        if (transform.position.y <= y && IsPausedSound == false)
        {
            IsPausedSound = true;
            soundReset.Play();
            GameOver();
        }
    }
    // Hàm dùng để quả bóng nahyr lên
    public void Jump()
    {
        if (transform.position.y >= 0)
        {
            if (Input.GetButtonDown("Jump") && (isOnGround || MAX_JUMP > currentJump))
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                isOnGround = false;
                currentJump++;
                jump.Play();
            }
        }

    }
    private void FixedUpdate()
    {
        Show();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            eatStar.Play();
            point += 1;
            WinPoint();
        }
        if (other.gameObject.CompareTag("Finish"))
        {
            if (point >= 13)
            {
                WinGame();
                winGame.Play();
            }
        }

    }
    public void WinPoint()
    {
        if (point >= 13) // Số điểm sẽ win game
        {
            win.GetComponent<Renderer>().enabled = true;
        }
    }
    public void WinGame()
    {
        Time.timeScale = 0f; // *Note:...
        winMenu.SetActive(true);
        pointsWinGame.text = point.ToString() + " Stars";
    }
    // nhận những vật thể KHÔNG đánh dấu Is Trigger
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            this.enabled = false;
            gameOver.Play();
            GameOver();
        }
        if (collision.gameObject.CompareTag("Ground"))
        {

            isOnGround = true;
            currentJump = 0;

        }
    }
    // Hiển thị điểm số
    private void Show()
    {
        score.text = point.ToString();
    }
}
