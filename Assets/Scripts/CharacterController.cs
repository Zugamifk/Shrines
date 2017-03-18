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

    public bool facingLeft { get; protected set; }
    public float normalizedSpeed { get; protected set; }
    public float speed { get; protected set; }
    public bool grounded { get; protected set; }

    public delegate void InputEvent();
    public InputEvent OnJump;

    protected FootTrigger footTrigger;

	// Use this for initialization
	protected virtual void Start () {
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

    protected void Move(float value)
    {
        facingLeft = value < 0;
        speed = value * maxSpeed;
        normalizedSpeed = Mathf.Abs(value);
    }

    protected void Jump()
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

    protected void OnLand(bool landed)
    {
        grounded = landed;
    }

}
