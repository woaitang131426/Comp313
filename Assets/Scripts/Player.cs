﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour {

    private Rigidbody2D myRigidbody;

    [SerializeField]
    private float movementSpeed;
    private float storedSpeed;

    private bool facingRight;

    private bool attack;

    private Animator myAnimator;

    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    private bool isGrounded;

    private bool jump;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    public Vector3 respawnPoint;

    [SerializeField]
    public bool OnTree;

    [SerializeField]
    public int Lives;

    public bool LadderVisible;


    //GameObject lefttree;
    GameObject[] ladders;

    GameObject[] stones;

	// Use this for initialization
	void Start () {
        storedSpeed = movementSpeed;
        facingRight = true;
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        LadderVisible = true;
        //lefttree = GameObject.Find("Ladder");
        ladders = GameObject.FindGameObjectsWithTag("Ladder");
        stones = GameObject.FindGameObjectsWithTag("Stone");

	}


    void Update()
    {
        HandleInput();
    }


    // Update is called once per frame
    void FixedUpdate () {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        //Debug.Log(horizontal);

        isGrounded =  IsGrounded();

        HandleMovement(horizontal,vertical);

        //Debug.Log(jump);

        Flip(horizontal);

        HandleAttacks();

        ResetValue();
	}


    private void HandleMovement(float horizontal, float vertical){

        if (isGrounded && jump)
        {
            isGrounded = false;
            myRigidbody.AddForce(new Vector2(0, 400));
        }

        if(!myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")){
            myRigidbody.velocity = new Vector2(horizontal * movementSpeed, myRigidbody.velocity.y);
        }

        if(OnTree){
            myRigidbody.velocity = new Vector2(0, vertical * movementSpeed);
        }


        if(LadderVisible){
            //ladders.SetActive(true);
            setLadders(ladders, true);
            setLadders(stones, false);
        }else{
            //lefttree.SetActive(false);
            setLadders(ladders, false);
            setLadders(stones, true);
        }


        myAnimator.SetFloat("speed", Mathf.Abs(horizontal));

    }

    private void setLadders(GameObject[] Ladders, bool b){
        foreach(GameObject ladder in Ladders){
            ladder.SetActive(b);
        }
    }

    private void Flip( float horizontal){
        if(horizontal > 0 && !facingRight || horizontal <0 && facingRight ){
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void HandleAttacks(){
        
            
        if(attack && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")){
            myAnimator.SetTrigger("attack");
        }
    }



    private void HandleInput(){
        if(Input.GetKeyDown(KeyCode.Space)){
            jump = true;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)){
            attack = true;
        }

        if(Input.GetKeyDown(KeyCode.V)){
            LadderVisible = !LadderVisible;
        }


    }

    private void ResetValue(){
        attack = false;
        jump = false;
    }


    private bool IsGrounded(){
        if(myRigidbody.velocity.y<=0){
            foreach(Transform point in groundPoints){
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++){
                    if(colliders[i].gameObject != gameObject){
                        return true;
                    }
                }
            }

            //return true;
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "FallDetector"){
            transform.position = respawnPoint;
        }

        if (collision.tag == "chasingEnemy")
        {
            //transform.position = respawnPoint;
            Lives--;
            handledestroy();
        }


        if(collision.tag == "Ladder"){
            OnTree = true;
            ignoreLaddersColission(ladders, true);
        }

        if (collision.tag == "Brige")
        {
            GameObject bridge = GameObject.Find("Bridge");
            bridge.transform.position = new Vector3(9,-3,0);
            Quaternion newr = Quaternion.Euler(0, 0, -90f);
            bridge.transform.rotation = Quaternion.Slerp(bridge.transform.rotation, newr, 1f);
        }

        if (collision.tag == "Bush")
        {
            movementSpeed = 1;
            //Debug.Log(movementSpeed);
           
        }
    }

    void handledestroy(){
        if(Lives<=0){
            //Destroy(GameObject.FindGameObjectWithTag("Player"));
            GameObject.Find("Player").SendMessage("Finish");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Game Over");
            PauseGame();
        }
        
    }

    public void restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PauseGame(){
        Time.timeScale = 0;

    }

    private void ignoreLaddersColission(GameObject[] Ladders, bool b)
    {
        foreach (GameObject ladder in Ladders)
        {
            Physics2D.IgnoreCollision(ladder.GetComponent<BoxCollider2D>(), GameObject.Find("Player").GetComponent<BoxCollider2D>(), b);
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
            OnTree = false;
            ignoreLaddersColission(ladders, false);
        }

        if (collision.tag == "Bush")
        {
            movementSpeed = storedSpeed;

        }
    }


}
