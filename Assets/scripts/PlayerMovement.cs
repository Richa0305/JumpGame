using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public Transform platforms;
    public float restartDelay = 1f;

    // for moving left and right
    public float speed;
    public float moveInput;
    private bool facingRight = true;
    public Transform Player;
    public Transform Background;
    public Text scoreText;

    // for jumping
    public float jumpForce;
    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;
    public Transform mycam;

    private Rigidbody2D rb;
    private Rigidbody2D prefabsRb;

    private float startingY = -1.98f;
    private float startingX = -10f;
    private bool direction = false;
    private bool availSecondJump;
    private float fallMultiplier = 4.5f;
    private int score = 0;
    private int prefabsCount = 0;

    public Text finalScoreText;
    public GameObject gameOver;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("NewPlatform", 2f);
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isGrounded = true;

    }

    private void addNewPlatform(bool platformDirection) {

        startingY = startingY + 0.72f;
        if (platformDirection) {
            startingX = 6f;
        }
        else
        {
            startingX = -6f;
        }
        prefabsCount = prefabsCount + 1;
        Transform prefabsPlatforms = Instantiate(platforms, new Vector3(startingX, startingY, 0), Quaternion.identity);

        prefabsRb = prefabsPlatforms.GetComponent<Rigidbody2D>();
        prefabsRb.velocity = new Vector2(-startingX, prefabsRb.velocity.y);

        //Rigidbody2D bgRbs;
        //bgRbs = Background.GetComponent<Rigidbody2D>();
        //bgRbs.AddForce(new Vector2(10f, 10f));
    }
    bool grounded(){
      return Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

    }
private void FixedUpdate()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        //moveInput = Input.GetAxis("Horizontal");
        ////rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        //if (facingRight == false && moveInput > 0) {
        //    Flip();
        //}else if (facingRight == true && moveInput < 0) {
        //    Flip();
        //}

        //rb.freezeRotation = true;
         Touch touch;

        if (Input.touchCount > 0)
        {
        touch = Input.GetTouch(0);

        //if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            if ((touch.phase == TouchPhase.Began) && isGrounded)
            {
                StartCoroutine(JumpUp());

            }

           // if (Input.GetKeyUp(KeyCode.UpArrow) )
            if (touch.phase == TouchPhase.Ended)
            {
                //JumpDown();
               // isGrounded = false;
            }
        }

    }

    IEnumerator JumpUp() {
        rb.AddForce(Vector2.up * jumpForce , ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.05f);

        rb.AddForce(Vector2.down * jumpForce , ForceMode2D.Impulse);
    }

    void JumpDown() {

        Debug.Log("Jump Down Called");
        rb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
    }
    void Flip() { 
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.collider.tag);
        // Detect sides 
        //if (collision.collider.tag.Equals("DiffPlatform"))
        //{
        //    isGrounded = false;
        //}
            if (collision.collider.tag.Equals("Platform"))
        {
            direction = !direction;
            Vector3 hit = collision.contacts[0].normal;
            float angle = Vector3.Angle(hit, Vector3.up);
            //Debug.Log(hit);
            //Debug.Log(angle);
                
            if (Mathf.Approximately(angle, 0) && isGrounded)
            {
                //Down
                // Debug.Log("Down");
                //isGrounded = false;
                score = score + 1;
                scoreText.text = score.ToString();
                //Background.GetComponent<Rigidbody2D>().velocity = Vector2.up * 0.1f;
                     mycam.position = mycam.position + Vector3.up * 0.72f;
              
                Invoke("StopPrefebs", 0.5f);
                Invoke("NewPlatform", 1f);
            }
            if (Mathf.Approximately(angle, 180))
            {
                ////Up
                //Debug.Log("Up");
                //prefabsRb.velocity = Vector2.zero;
                //prefabsRb.isKinematic = true;
                //direction = !direction;
                //Invoke("NewPlatform", 1f);
            }
            if (Mathf.Approximately(angle, 90))
            {
                // Sides
                Vector3 cross = Vector3.Cross(Vector3.forward, hit);
                if (cross.y > 0)
                { // left side of the player
                    //Debug.Log("Left");
                    Invoke("RestartGame", 1f);
                }
                else
                { // right side of the player
                    //Debug.Log("Right");
                    Invoke("RestartGame", 1f);
                }
            }
        }
    }
    public void StopPrefebs()
    {
        prefabsRb.velocity = Vector2.zero;
        prefabsRb.isKinematic = true;
    }
    public void NewPlatform()
    {  
        addNewPlatform(platformDirection: direction);
    }

    public void RestartGame()
    {
        gameOver.SetActive(true);
        finalScoreText.text = "Score : " + score.ToString();
        score = 0;
    }
    public void reloadGame() {

        SceneManager.LoadScene(1);
    }
}
