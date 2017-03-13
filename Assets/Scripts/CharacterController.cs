using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    public class FootTrigger : MonoBehaviour
    {
        public System.Action<bool> OnLand;
        void OnTriggerEnter2D(Collider2D other)
        {
            if (OnLand != null)
            {
                OnLand.Invoke(true);
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (OnLand != null)
            {
                OnLand.Invoke(false);
            }
        }
    }

    public float maxSpeed = 1;
    public Collider2D footCollider;
    public Rigidbody2D rigidbody;
    public float jumpPower;

    public bool facingLeft { get; private set; }
    public float normalizedSpeed { get; private set; }
    public float speed {get; private set;}
    public bool grounded { get; private set; }

    public delegate void InputEvent();
    public InputEvent OnJump;

    FootTrigger footTrigger;

	// Use this for initialization
	void Start () {
        InputManager.RegisterAxisUpdateCallback(InputKey.MOVE_HORIZONTAL, Move);
        InputManager.RegisterButtonDownCallback(InputKey.BUTTON0, Jump);
        footTrigger = footCollider.gameObject.AddComponent<FootTrigger>();
        if (footTrigger != null)
        {
            footTrigger.OnLand = OnLand;
        }
	}

    void FixedUpdate()
    {
        var v = rigidbody.velocity;
        rigidbody.velocity = new Vector2(speed * Time.fixedDeltaTime, v.y);
    }

    void Move(float value)
    {
        facingLeft = value < 0;
        speed = value * maxSpeed;
        normalizedSpeed = Mathf.Abs(value);
    }

    void Jump()
    {
        if (grounded)
        {
            if (OnJump != null)
            {
                OnJump.Invoke();
            }
            rigidbody.AddForce(jumpPower*Vector2.up, ForceMode2D.Impulse);
        }
    }

    void OnLand(bool landed)
    {
        grounded = landed;
        Debug.Log(landed);
    }
}
