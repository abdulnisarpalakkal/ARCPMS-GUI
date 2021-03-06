//----------------------------------------------------------------
// OPCDA.NET Client Application
// ----------------------------
// Uses the OPCDA.NET Wrapper Assembly to access the OPC DA Server
// This smaple application shows how an application can handle all OPC access in
// a background thread.
// This ensures that the application doesn't freeze, even with the OPC access
// hanging in a DCOM timout due to communication failure.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//
// Copyright (C) 2003-04 Advosol Inc.    (www.advosol.com)
// All rights reserved.
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using System.Threading ;
using System.Collections ;

using OPC;
using OPCDA;
using OPCDA.NET;



namespace VCSOpcThread
{
	/// <summary>
	/// Background thread that handles the OPC server access.
	/// 
	/// </summary>
	public class OpcThread
	{

      OpcServer      OpcSrv ;
      
      RequestQueue   Requests ;
      Thread         AccessThread ;
      ManualResetEvent    StopThread = null ;
      SyncIOGroup    SioGrp = null;


      //--------------------------------------------------
      // Constructor
		public OpcThread(  OpcServer srv )
		{
			
         OpcSrv = srv ;
         SioGrp = OpcSrv.AddSyncIOGroup();
         Requests = new RequestQueue() ;
         AccessThread = new Thread( new ThreadStart(OpcAccessThread)  );
         AccessThread.Start();
		}


      //---------------------------------------------------------------
      // Queue OPC server access request
      public void Request( OpcRequest req )
      {
         if( SioGrp == null )
            return ;
         Requests.Add( req );
      }


      //---------------------------------------------------------------
      // Queue OPC server access request
      public void Stop()
      {
         if( SioGrp == null )
            return ;

         // terminate the server access thread
         StopThread = new ManualResetEvent( false );
         StopThread.WaitOne( 5000, true );
         StopThread.Close();
         StopThread = null;

         SioGrp.Dispose();
         SioGrp = null;
         OpcSrv.Disconnect();
      }




      //-----------------------------------------------------
      // Thread that dequeues the requests and makes the OPC server access
      private void OpcAccessThread()
      {
         for(;;)        // Thread loop
         {
            if( Requests.Count() >  0 )
            {
               OpcRequest req = Requests.Remove();

               // An item value is read and displayed in the defined TextBox.
               if( req.Cmd == Command.Read )
               {
                  OPCItemState val ;
                  int rtc = SioGrp.Read( OPCDATASOURCE.OPC_DS_CACHE, req.ItemID, out val );
                  if (!HRESULTS.Failed(rtc) && !HRESULTS.Failed(val.Error))
                  {
                      req.res = true;
                      req.Val = val.DataValue.ToString();
                  }
               
               }

               // A value is written to the OPC server item
               else if( req.Cmd == Command.Write )
               {
                  int rtc = SioGrp.Write( req.ItemID, req.Val );
                  if( !HRESULTS.Failed(rtc) )
                      req.res = true;
               }

            }

            Thread.Sleep( 100 ) ;   // ms

            if( StopThread != null )      // Thread kill request
            {
               StopThread.Set();
               return;               // terminate the thread
            }

         }
      }


	}


   //===============================================================
   // Requests to the
   public class OpcRequest
   {
      public Command    Cmd ;          // command type
      public string     ItemID ;       // OPC server item ID
      public object     Val ;          // value for write requests
      public bool    res ;       // textbox for read result display


      public OpcRequest( Command c, string id )
      {
         Cmd = c ;
         ItemID = id ;
        
      }

      public OpcRequest( Command c, string id, string v )
      {
         Cmd = c ;
         ItemID = id ;
         Val = v ;
      }
   }


   public enum Command
   {
      Read,
      Write
   }



   //===============================================================
   // FIFO queue for OPC server access requests 
   public class RequestQueue
   {
      private Queue reqQueue ;
      private Mutex mtx ;

      public RequestQueue()
      {
         reqQueue = new Queue() ;
         mtx = new Mutex() ;
      }

      public int Count()
      {
         return reqQueue.Count;
      }

      public void Add( OpcRequest req )
      {
         mtx.WaitOne() ;
         reqQueue.Enqueue( req );
         mtx.ReleaseMutex();
      }

      public OpcRequest Remove()
      {
         mtx.WaitOne() ;
         OpcRequest req = (OpcRequest)reqQueue.Dequeue();
         mtx.ReleaseMutex();
         return req ;
      }

   }
}
