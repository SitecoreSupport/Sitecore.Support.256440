using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using Sitecore.Support.DataExchange.Providers.Sc.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.DataExchange.Providers.Sc.Converters.PipelineSteps
{
  public class ResolveSitecoreItemStepConverter: Sitecore.DataExchange.Providers.Sc.Converters.PipelineSteps.ResolveSitecoreItemStepConverter
  {
    public ResolveSitecoreItemStepConverter(IItemModelRepository repository) : base(repository)
    {
    }

    protected override void AddPlugins(ItemModel source, PipelineStep pipelineStep)
    {
      base.AddPlugins(source, pipelineStep);
      this.AddItemSearchSettings(source, pipelineStep);
    }

    private void AddItemSearchSettings(ItemModel source, PipelineStep pipelineStep)
    {
      ItemSearchSettingsPlugin plugin = new ItemSearchSettingsPlugin
      {
        PageSize = base.GetIntValue(source, "PageSize")
      };
      pipelineStep.AddPlugin<ItemSearchSettingsPlugin>(plugin);
    }
  }
}