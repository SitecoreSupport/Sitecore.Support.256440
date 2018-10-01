using Sitecore.DataExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.DataExchange.Providers.Sc.Plugins
{
  public class ItemSearchSettingsPlugin: IPlugin
  {
    public int PageSize { get; set; }
  }
}