using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Player {
    Vector3 moveDirection = Vector3.zero;
    CharacterController control;
    public float speed, JumpStrength, Gravity, sensitivity;
    private int CharacterIndex;
    private bool female = false;
    private int health, MaxHealth, healthRegen;
    private int mana, MaxMana, manaRegen;
    private double exp = 0.00, nextLevel = 100.00;
    private Class Class;

    private double attackSpeed, regenRate;
    // Update is called once per frame

    void Update() {
        control = GetComponent<CharacterController>();
        if (control.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = JumpStrength;
            }
        }
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * sensitivity, 0));
        moveDirection.y -= Gravity * Time.deltaTime;
        control.Move(moveDirection * Time.deltaTime);
	}

    public void setIndex(int i)
    {
        this.CharacterIndex = i;
    }

    public int getIndex()
    {
        return this.CharacterIndex;
    }
    
    public void chooseClass(Class c)
    {
        this.Class = c;
    }
}
