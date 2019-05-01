using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ARCPSGUI.DB;

namespace ARCPSGUI.ProcessManager
{
    class SlotProcess
    {
        SlotDba objSlotDba = null;
        public void DoTransferCar(int fromFloor, int fromAisle, int fromRow, int toFloor, int toAisle, int toRow)
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            if (toAisle > 0 && toRow > 0 && fromAisle > 0 && fromRow > 0)
            {
                if (!(toAisle == fromAisle && toRow == fromRow && fromFloor == toFloor))
                {
                    if (MessageBox.Show("Transfer from " + fromFloor + "/" + fromAisle + "/" + fromRow + " to " + toFloor + "/" + toAisle + "/" + toRow + "?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        int clickTransferQueueId = 0;
                        clickTransferQueueId = objSlotDba.GetInsertQueueClickTransferId(fromFloor, fromAisle, fromRow, toFloor, toAisle, toRow);
                        if (clickTransferQueueId > 0)
                            MessageBox.Show("TransferId =" + clickTransferQueueId, "Information", MessageBoxButton.OK);
                        else
                            MessageBox.Show("Not valid slot.", "Information", MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("Not valid data", "Information", MessageBoxButton.OK);
            }
        }

    }
}
