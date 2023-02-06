using UnityEngine;

public class PlayerManager : MonoBehaviour
{
  [SerializeField] GameManager gameManager;
  [SerializeField] LayerMask blockLayer;
  enum DIRECTION_TYPE
  {
    STOP,
    RIGHT,
    LEFT,
  }

  DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;

  Rigidbody2D rigidbody2D;

  float speed;

  float jumpPower = 575;

  void Start()
  {
    rigidbody2D = GetComponent<Rigidbody2D>();
  }

  void Update()
  {
    float x = Input.GetAxis("Horizontal");

    if (x == 0)
    {
      // 止まっている
      direction = DIRECTION_TYPE.STOP;
    }
    else if (x > 0)
    {
      // 右へ
      direction = DIRECTION_TYPE.RIGHT;
    }
    else if (x < 0)
    {
      // 左へ
      direction = DIRECTION_TYPE.LEFT;
    }
    // スペースが押されたらJumpさせる
    if (IsGround() && Input.GetKeyDown("space"))
    {
      Jump();
    }
  }

  void FixedUpdate()
  {
    switch (direction)
    {
      case DIRECTION_TYPE.STOP:
        speed = 0;
        break;
      case DIRECTION_TYPE.RIGHT:
        speed = 3;
        transform.localScale = new Vector3(1, 1, 1);
        break;
      case DIRECTION_TYPE.LEFT:
        speed = -3;
        transform.localScale = new Vector3(-1, 1, 1);
        break;
    }
    rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
  }

  void Jump()
  {
    // 上に力を加える
    rigidbody2D.AddForce(Vector2.up * jumpPower);
  }

  bool IsGround()
  {
    // 始点と終点を作成
    Vector3 leftStartPoint = transform.position - Vector3.right * 0.2f + Vector3.up * 0.01f;
    Vector3 rightStartPoint = transform.position + Vector3.right * 0.2f + Vector3.up * 0.01f;
    Vector3 endStartPoint = transform.position - Vector3.up * 0.1f;
    Debug.DrawLine(leftStartPoint, endStartPoint);
    Debug.DrawLine(rightStartPoint, endStartPoint);
    return Physics2D.Linecast(leftStartPoint, endStartPoint, blockLayer) || Physics2D.Linecast(rightStartPoint, endStartPoint, blockLayer);
  }

  void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.tag == "Trap")
    {
      Debug.Log("ゲームオーバー");
      gameManager.GameOver();
    }
    else if (collision.gameObject.tag == "Finish")
    {
      Debug.Log("クリア");
      gameManager.GameClear();
    }
    else if (collision.gameObject.tag == "Item")
    {
      // アイテム取得
      collision.gameObject.GetComponent<ItemManager>().GetItem();
    }
    else if (collision.gameObject.tag == "Enemy")
    {
      EnemyManager enemy = collision.gameObject.Getcomponent<EnemyManager>();
      if (this.transform.position.y + 0.2f > enemy.transform.position.y)
      {
        // 上から踏んだら敵を削除

      }
      else
      {
        // 横からぶつかったらプレイヤー破壊
        Destroy(this.gameObject);
        gameManager.GameOver();
      }
    }
  }
}
