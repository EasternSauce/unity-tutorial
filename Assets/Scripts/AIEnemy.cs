using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    AttackHandler attackHandler;

    private void Awake()
    {
        attackHandler = GetComponent<AttackHandler>();
    }

    [SerializeField] Character target;
    float timer = 4f;

    private void Start()
    {
        target = GameManager.instance.playerObject.GetComponent<Character>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            timer = 4f;

            attackHandler.Attack(target);
        }
    }
}
