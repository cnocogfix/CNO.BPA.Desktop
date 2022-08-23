using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emc.InputAccel.IndexPlus.Scripting;
using Emc.InputAccel.QuickModule.ClientScriptingInterface;

namespace CNO.BPA.IAIndex
{
   public class ModuleEvents
      : IModuleEvents
   {
      public Boolean FilterBatchList(IBatchInformation batch)
      {
         return true;
      }

      public void ModuleStart(IApplication arg)
      {
      }

      public void ModuleFinish()
      {
      }

      public void GotServerConnection(IServerInformation serverInformation)
      {
      }

      public void LostServerConnection(String serverHostName)
      {
      }
   }


}
