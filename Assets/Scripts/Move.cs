using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Move : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    bool isPressed = false;
    Animator animator;
    public event Action<bool> MoveHappened;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DoMove();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false; ;
    }

    public void DoMove()
    {
        MoveHappened?.Invoke(isPressed);
        animator.SetBool("Pressed", isPressed);
    }

}
