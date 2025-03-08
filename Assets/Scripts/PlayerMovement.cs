using System.Linq.Expressions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Creates a variable for player speed. Serializedfield makes the float directly editable from within unity
    [SerializeField] private float speed;
    //Reference for player jump power
    [SerializeField] private float jumpPower;
    //references the Layers. 
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    //Reference the physics we applied to the player 
    private Rigidbody2D body;
    private Animator anim;
    //References the box collider 
    private BoxCollider2D boxCollider;
    //variable for creating delays between jumps
    private float wallJumpCooldown;
    private float horizontalInput;
   
    //calls everytime you start the game 
    private void Awake()
    {
        //checks the player object for Rigidbody2D and stores it in the body variable 
        body = GetComponent<Rigidbody2D>();
        //Reference the animator for the object 
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    // runs on player input 
    private void Update()
    {
        //creates and defines the variable for horizontal input 
        horizontalInput =  Input.GetAxis("Horizontal");

        //flips the player when moving left-right
        if (horizontalInput > 0.01f)
            transform.localScale = Vector2.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector2(-1, 1);

        //sets Animator parameters
        anim.SetBool("Walk", horizontalInput != 0);
        anim.SetBool("Grounded", isGrounded());

        if (wallJumpCooldown > 0.2f)
        {

            //registers horizontal input form the player 
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
            else
                body.gravityScale = 3;
            //checks if the spacebar has been pressed. Input.GetKey can only have 2 values: true when the key is pressed and false when its not
            if (Input.GetKey(KeyCode.Space))
                //References the Jump void
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }
    private void Jump()
    {
        if (isGrounded())
        {
            //defines what happens when the space key is pressed
            //when space is pressed  this code will maintain the current velocity on the x axis and apply a velocity of the speed variable on the Y axis.
            //you can swap the speed variable with a number to tweak the jump
            body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
            //sets the value of the trigger
            anim.SetTrigger("Jump");
        }
        else if (onWall() && !isGrounded())
        {
            if(horizontalInput == 0)
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            wallJumpCooldown = 0;
            body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);  
        }
    }
   

    //checks the object the player is colliding with
    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
