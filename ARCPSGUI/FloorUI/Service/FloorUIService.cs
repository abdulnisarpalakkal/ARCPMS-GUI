using ARCPSGUI.FloorUI.floorMachines;
using ARCPSGUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.FloorUI.Service
{
    interface FloorUIService
    {
        void Handle_triggerSlotUpdate(object sender, EventArgs e);
        void InitialUpdateAllSlots();
        void UpdateSlotInScreen(SlotData objSlotData);
        
        void InitializeCMSettings();
        void TerminateCMSettings();
        void ucCM_OnPositionChanged(object sender, EventArgs e);
        void SetCMPosition(string cmCode, int position);
        ucFloorCM GetCMObject(string cmCode);
        //void TranslateCM(ucFloorCM objCM);
        void TranslateCMPosition(ucFloorCM objCM, int position);


    }
}
