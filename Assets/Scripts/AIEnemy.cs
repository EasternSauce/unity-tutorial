using CharacterCommand;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    AttackHandler attackHandler;
    [SerializeField] float attackRange = 5f;

    private void Awake()
    {
        attackHandler = GetComponent<AttackHandler>();
    }

    [SerializeField] Character target;
    float timer = 0.2f;

    private void Start()
    {
        target = GameManager.instance.playerObject.GetComponent<Character>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (target.lifePool.currentValue > 0 && timer < 0f && distanceToTarget <= attackRange)
        {
            timer = 0.2f;

            attackHandler.ProcessCommand(new Command(CommandType.Attack, target.gameObject));
        }
    }
}
