using NoManual.ANO;
using NoManual.Interaction;
using NoManual.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANO_DropItem_Component : MonoBehaviour
{
    // ANO Collider가 특정 아이템을 인식하게 만듬

    ANO_Component anoComponent;

    private void Awake()
    {
        anoComponent = GetComponentInParent<ANO_Component>();
    }

    // Enter? Stay? 
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == (int)NoManual.Utils.Layer.LayerIndex.Interact)
        {
            ItemComponent itemComponent = other.gameObject.GetComponentInParent<ItemComponent>();

            if(itemComponent != null )
            {
                
                if(anoComponent.DropItemCheck(itemComponent.InventoryItemId))
                {
                    
                }
            }

        }
    }


}