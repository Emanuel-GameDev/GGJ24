using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Patroling,
    Charging,
    OmegaRun,
    Stunned
}

public class Mushroom : BaseEnemy
{
    public override void Start()
    {
        base.Start();

        if (!CheckPoints())
            return;

        AlignPoints();

        state = State.Patroling;
    }

    void Update()
    {
        if (!CheckPoints()) return;

        switch (state)
        {
            case State.Patroling:

                Patrol();
                break;
        }

        CheckFacing();
    }

}