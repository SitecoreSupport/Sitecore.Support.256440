using System;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Extensions;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Providers.Sc.Extensions;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Diagnostics;
using Sitecore.Services.Core.Model;

namespace Sitecore.Support.DataExchange.Providers.Sc.Processors.PipelineSteps
{
  public class UpdateSitecoreItemStepProcessor : Sitecore.DataExchange.Providers.Sc.Processors.PipelineSteps.UpdateSitecoreItemStepProcessor
  {
    protected override void ProcessPipelineStep(PipelineStep pipelineStep, PipelineContext pipelineContext, ILogger logger)
    {
      var repository = GetItemModelRepository(pipelineStep, pipelineContext);
      if (repository == null)
      {
        return;
      }
      var itemModels = GetTargetObjectAsItemModels(pipelineStep, pipelineContext, logger);
      if (itemModels == null)
      {
        return;
      }
      var itemId = Guid.Empty;
      foreach (var itemModel in itemModels)
      {
        FixItemModel(itemModel);
        var language = itemModel.ContainsKey(ItemModel.ItemLanguage) ? itemModel[ItemModel.ItemLanguage].ToString() : string.Empty;
        if (itemId == Guid.Empty)
        {
          itemId = itemModel.GetItemId();
        }
        var wasSaved = false;
        if (itemId == Guid.Empty)
        {
          itemId = repository.Create(itemModel);
          if (itemId != Guid.Empty)
          {
            wasSaved = true;
            itemModel[ItemModel.ItemID] = itemId;
          }
        }
        else
        {
          wasSaved = repository.Update(itemId, itemModel, language);
        }
        if (!wasSaved)
        {
          logger.Error("Item was not saved. (id: {0}, language: {1})", itemId, language);
          return;
        }
        logger.Debug("Item was saved. (id: {0}, language: {1})", itemId, language);
      }
    }

    private IItemModelRepository GetItemModelRepository(PipelineStep pipelineStep, PipelineContext pipelineContext)
    {
      var endpointsSettings = pipelineStep.GetEndpointSettings();
      if (endpointsSettings == null)
      {
        return null;
      }
      var endpoint = endpointsSettings.EndpointTo;
      var repositorySettings = endpoint?.GetItemModelRepositorySettings();
      return repositorySettings?.ItemModelRepository;
    }
  }
}