namespace NoManual.Interaction
{
    public interface IPut
    {
        public enum PutMode
        {
            None = 0,
            Put = 1,
        }

        public void InitializationPutMode();

        /// <summary>
        /// Put 상호작용 가능한지 확인
        /// </summary>
        public PutMode GetMode { get; }
        
        /// <summary>
        /// Put 관련 상호작용
        /// </summary>
        public void PutInteraction();

        /// <summary>
        /// Put 모드 교체
        /// </summary>
        public void SwapPutMode();
        
        /// <summary>
        /// Put 진행 시 인벤토리 아이템제거 여부
        /// </summary>
        public bool RemoveInventoryItem();

    }
}