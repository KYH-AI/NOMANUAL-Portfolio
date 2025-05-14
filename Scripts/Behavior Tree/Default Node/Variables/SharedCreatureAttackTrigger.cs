using BehaviorDesigner.Runtime;
using NoManual.Creature;

public class SharedCreatureAttackTrigger : SharedVariable<CreatureAttackTriggerHandler>
{
    public static implicit operator SharedCreatureAttackTrigger(CreatureAttackTriggerHandler value) { return new SharedCreatureAttackTrigger { mValue = value }; }
}
