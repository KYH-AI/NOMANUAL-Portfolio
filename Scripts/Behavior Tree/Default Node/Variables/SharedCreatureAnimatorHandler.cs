using NoManual.Creature;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]

    public class SharedCreatureAnimatorHandler : SharedVariable<CreatureAnimatorHandler>
    {
        public static implicit operator SharedCreatureAnimatorHandler(CreatureAnimatorHandler value) { return new SharedCreatureAnimatorHandler { mValue = value }; }
    }
}
