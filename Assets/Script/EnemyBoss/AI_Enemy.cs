using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using AI.BehaviourTree;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class AI_Enemy : MonoBehaviour, IDamagable
{
    [Header("BehaviourTree")]
    [SerializeField] Transform player;
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float attackRange = 1.5F, attackWaitTime = 0.8F;
    [SerializeField] float followrange = 5F, followWaitTime = 5F;
    [SerializeField] float patrolWaitDuration = 3F;

    [Header("health")]
    [SerializeField] float health = 200;

    [Header("GamePlayMode")]
    [SerializeField] GamePlayMode gamePlayMode;

    public Action<AI_Enemy> OnDestroy;

    [Header("Ragdoll")]
    [SerializeField] Collider[] ragdollColliders;
    Rigidbody[] ragdollBodies;



    Animator animator;
    Transform myTransform;
    NavMeshAgent navMeshAgent;
    CharacterController characterController;
    GameObject handWeapon;
    BehaviourTree BTTree = new BehaviourTree();

    private void Start()
    {
        InitializeBehaviourTree();
        InitializeRagdoll();
    }

    private void InitializeRagdoll()
    {
        ragdollBodies = gameObject.GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }

    private void Update()
    {
        if(health > 0) BTTree.UpdateTree();
    }

    private async void InitializeBehaviourTree()
    {
        Task waitForPlayer = GameManager.Instance.WaitForPlayer();
        await waitForPlayer;

        player = GameManager.Instance.player.transform;
        if (player == null) Debug.Log("Player transform not found in Enemy");
        
        BTTree.AddData("PlayerTransform", player);
        animator = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        handWeapon = GetComponentInChildren<HandWepon>().gameObject;

        BTTree.AddData("GamePlayMode", gamePlayMode);
        BTTree.AddData("Animator", animator);
        BTTree.AddData("Transform", myTransform);
        BTTree.AddData("NavMeshAgent", navMeshAgent);
        BTTree.AddData("Waypoints", waypoints);
        BTTree.AddData("IsPreviouslyFollowing", false);
        BTTree.AddData("HandWeapon", handWeapon);

        IsInRange isInAttackRange = new IsInRange(BTTree, attackRange);
        Attack attackNode = new Attack(BTTree);
        Wait attackWaitNode = new Wait(BTTree, attackWaitTime, 2F);
        SequenceNode attackSequenceNode = new SequenceNode(BTTree);

        attackSequenceNode.childrens.Add(isInAttackRange);
        attackSequenceNode.childrens.Add(attackNode);
        attackSequenceNode.childrens.Add(attackWaitNode);

        IsInRange isInFollowRange = new IsInRange(BTTree, followrange);
        Follow followNode = new Follow(BTTree, 2F);
        FollowWait followWaitNode = new FollowWait(BTTree, followWaitTime, 2);
        SequenceNode followFollowSequencerNode = new SequenceNode(BTTree);
        FallbackNode followFallBackNode = new FallbackNode(BTTree);


        followFollowSequencerNode.childrens.Add(isInFollowRange);
        followFollowSequencerNode.childrens.Add(followNode);

        followFallBackNode.childrens.Add(followFollowSequencerNode);
        followFallBackNode.childrens.Add(followWaitNode);

        Patrol patrolNode = new Patrol(BTTree, 2F);
        Wait patrolWaitNode = new Wait(BTTree, patrolWaitDuration, 2F);
        FallbackNode patrolFallBackNode = new FallbackNode(BTTree);
        FallbackNode patrolWaitFallBackNode = new FallbackNode(BTTree);
        SequenceNode patrolSequenceNode = new SequenceNode(BTTree);

        patrolFallBackNode.childrens.Add(isInFollowRange);
        patrolFallBackNode.childrens.Add(patrolNode);

        patrolWaitFallBackNode.childrens.Add(isInFollowRange);
        patrolWaitFallBackNode.childrens.Add(patrolWaitNode);

        patrolSequenceNode.childrens.Add(patrolFallBackNode);
        patrolSequenceNode.childrens.Add(patrolWaitFallBackNode);

        IsPlayingOrNot isPlayingNode = new IsPlayingOrNot(BTTree);

        FallbackNode rootNode = new FallbackNode(BTTree);

        rootNode.childrens.Add(isPlayingNode);
        rootNode.childrens.Add(attackSequenceNode);
        rootNode.childrens.Add(followFallBackNode);
        rootNode.childrens.Add(patrolSequenceNode);

        BTTree.rootNode = rootNode;


        isInFollowRange.IfInRange += patrolWaitNode.ResetNode;
        isInFollowRange.IfInRange += patrolNode.ResetNode;
        isInFollowRange.IfInRange += followWaitNode.ResetNode;

        isInAttackRange.IfInRange += patrolWaitNode.ResetNode;
        isInAttackRange.IfInRange += patrolNode.ResetNode;
        isInAttackRange.IfInRange += followWaitNode.ResetNode;

    }

    public void AddDamage(float damage, Vector3 force)
    {
        DecreaseHealth(damage);
    }

    public void AddDamage(float damage)
    {
        DecreaseHealth(damage);
    }

    public void Destroy()
    {
        EnableRagdoll();
        if (OnDestroy != null) OnDestroy(this);
        Destroy(gameObject, 5F);
    }

    public void DecreaseHealth(float damage)
    {
        if (health <= 0) return;

        health -= damage;
        if (health <= 0) Destroy();
    }
    private void EnableRagdoll()
    {
        if (animator) animator.enabled = false;
        foreach (Collider col in ragdollColliders) col.enabled = true;
        foreach (Rigidbody rigidbody in ragdollBodies) rigidbody.isKinematic = false;

        if(characterController) characterController.enabled = false;
        if(navMeshAgent) navMeshAgent.enabled = false;
    }
    private void DisableRagdoll()
    {
        if (animator) animator.enabled = true;
        foreach (Collider col in ragdollColliders) col.enabled = false;
        foreach (Rigidbody rigidbody in ragdollBodies) rigidbody.isKinematic = true;

        if (characterController) characterController.enabled = true;
        if (navMeshAgent) navMeshAgent.enabled = true;
    }
}
