using System;
using UnityEngine;
using UnityEngine.AI;

public class CharacterDefeatHandler : MonoBehaviour
{
    NavMeshAgent agent;
    AIEnemy aiEnemy;
    Collider objectCollider;

    AttackInput attackInput;
    InteractInput interactInput;
    PlayerCharacterInput playerCharacterInput;
    CharacterMovementInput movementInput;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        aiEnemy = GetComponent<AIEnemy>();
        objectCollider = GetComponent<Collider>();
    
        attackInput  = GetComponent<AttackInput>();
        interactInput  = GetComponent<InteractInput>();
        playerCharacterInput  = GetComponent<PlayerCharacterInput>();
        movementInput  = GetComponent<CharacterMovementInput>();
    }

    public void Defeated()
    {
        agent.isStopped = true;
        agent.enabled = false;

        //AI part

        if(aiEnemy != null)
        {
            aiEnemy.enabled = false;
        }

        objectCollider.enabled = false;

        //player part
        
        if(attackInput != null)
        {
            attackInput.enabled = false;
        }  

        if(interactInput != null)
        {
            interactInput.enabled = false;
        }    

        if(playerCharacterInput != null)
        {
            playerCharacterInput.enabled = false;
        }   

        if(movementInput != null)
        {
            movementInput.enabled = false;
        }  

    }

    internal void Respawn()
    {
        

        agent.isStopped = false;
        agent.enabled = true;

        //AI part

        if(aiEnemy != null)
        {
            aiEnemy.enabled = true;
        }

        objectCollider.enabled = true;

        //player part
        
        if(attackInput != null)
        {
            attackInput.enabled = true;
        }  

        if(interactInput != null)
        {
            interactInput.enabled = true;
        }    

        if(playerCharacterInput != null)
        {
            playerCharacterInput.enabled = true;
        }   

        if(movementInput != null)
        {
            movementInput.enabled = true;
        }  
    }
}
