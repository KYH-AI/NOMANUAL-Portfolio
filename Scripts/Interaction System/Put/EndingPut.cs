using NoManual.Interaction;
using NoManual.Managers;
using NoManual.Task;
using UnityEngine;

public class EndingPut : PutRelayComponent
{
    [Header("엔딩 목적지")]
    [SerializeField] private GameManager.SceneName endingRoot;

    [Header("JSON이랑 같은 taskId")]
    [SerializeField] private TaskHandler.TaskID taskId;
    
    public override void PutInteraction()
    {
        PutCallBackMapper putCallBack = new PutCallBackMapper(taskId.ToString(), 
            TaskTargetId, 
            RequestItemId, 
            EndingScene,
            this);
        NoManualHotelManager.Instance.InventoryManager.ShowPutInventory(putCallBack);
    }

    private void EndingScene()
    {
        // 독백 씬에서 세이브 데이터 일차 판단 후 로딩 씬 진행
        HotelManager.Instance.ExitHotel(true, false, GameManager.SceneName.Monologue);
    }
}
