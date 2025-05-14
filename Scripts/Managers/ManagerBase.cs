namespace NoManual.Managers
{
    public abstract class ManagerBase 
    {
        public virtual void Initialize()
        {
        }
        
        public virtual void PostInitialize()
        {
        }

        public virtual void Tick()
        {
        }

        public virtual void LateTick()
        {
        }
    }
}


