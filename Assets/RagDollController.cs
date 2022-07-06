using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollController : MonoBehaviour
{
    [SerializeField] List<GameObject> RagDolls;
    [SerializeField] IKController Player;

    private readonly List<Animator> _animators  =new List<Animator>();
    void Start()
    {
        foreach (var ragDoll in RagDolls)
        {
            _animators.Add(ragDoll.GetComponent<Animator>());
        }
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StartCoroutine(DoFall());
        }
    }

    IEnumerator DoFall()
    {
        var i = RagDolls.Count;
        while (i-- > 0)
        {
            var index = Random.Range(0, _animators.Count);
            var target = _animators[index];
            var targetTransform = target.transform;
            Player.lookObj = targetTransform;
            target.enabled = false;
            _animators.RemoveAt(index);
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
