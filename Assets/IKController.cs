using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKController : MonoBehaviour
{
    public bool ikActive = false;
    public Transform rightHandObj = null;
    public Transform leftHandObj = null;
    public Transform lookObj = null;
    public LayerMask LayerMask;
    
    private Animator _animator;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    void OnAnimatorIK(int layerIndex)
    {
        if(_animator)
        {
            //Если, мы включили IK, устанавливаем позицию и вращение
            if(ikActive)
            {
                //Ищем интерактивные объекты
                var hints = Physics.SphereCastAll(transform.position, 2f, transform.forward, 5, LayerMask);
                if (hints.Length != 0)
                {
                    var minDist = float.MaxValue;
                    //Ищем ближайший объект
                    foreach (var hint in hints)
                    {
                        var dist = Vector3.Distance(transform.position, hint.transform.position);
                        if (dist <= minDist)
                        {
                            lookObj = hint.transform;
                            minDist = dist;
                        }
                    }
                }
                else
                {
                    lookObj = null;
                }
                // Устанавливаем цель взгляда для головы
                if(lookObj != null)
                {
                    _animator.SetLookAtWeight(1);
                    _animator.SetLookAtPosition(lookObj.position);
                }
                // Устанавливаем цель для правой руки и выставляем её в позицию
                if(rightHandObj != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
                }
                // Устанавливаем цель для правой руки и выставляем её в позицию
                if(leftHandObj != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
            }
            // Если IK неактивен, ставим позицию и вращение рук и головы визначальное положение
            else 
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);
                _animator.SetLookAtWeight(0);
            }
        }
    }

}
