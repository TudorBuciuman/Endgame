using System;
using UnityEngine;

public class AttackSpawner : UnityEngine.Object
{
    private static Type[] attackIndex = new Type[2]
    {
        typeof(KnightSwing),
        typeof(BladeKnightBlasters),
        
    };

    public static AttackBase GetAttack(int index)
    {
        if (index > -1 && index < attackIndex.Length)
        {
            return new GameObject(attackIndex[index].Name, attackIndex[index]).GetComponent<AttackBase>();
        }
        return new GameObject("BlankAttack", typeof(AttackBase)).GetComponent<AttackBase>();
    }
}
