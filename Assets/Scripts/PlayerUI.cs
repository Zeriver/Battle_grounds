using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    private Text turn = null;


    private Animator _animator;

    public bool IsOpen
    {
        get { return _animator.GetBool("IsOpen"); }
        set { _animator.SetBool("IsOpen", value); }
    }

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        IsOpen = true;

        //var rect = GetComponent<RectTransform>();
        //rect.offsetMax = rect.offsetMin = new Vector2(0, 0);
    }

    public void Update()
    {

    }

    
    public void updateTurn(int turnNumber)
    {
        turn.text = "Turn: " + turnNumber;
    }

}
