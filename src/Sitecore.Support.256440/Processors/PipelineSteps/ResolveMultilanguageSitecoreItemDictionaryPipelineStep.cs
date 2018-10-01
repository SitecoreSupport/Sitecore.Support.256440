using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.DataAccess;
using Sitecore.DataExchange.Extensions;
using Sitecore.DataExchange.Providers.Sc.DataAccess.Readers;
using Sitecore.DataExchange.Providers.Sc.Plugins;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Diagnostics;
using Sitecore.Services.Core.Model;
using Sitecore.Support.DataExchange.Providers.Sc.Plugins;

namespace Sitecore.Support.DataExchange.Providers.Sc.Processors.PipelineSteps
{
  public class ResolveMultilanguageSitecoreItemDictionaryPipelineStep: Sitecore.DataExchange.Providers.Sc.Processors.PipelineSteps.ResolveMultilanguageSitecoreItemDictionaryPipelineStep
  {
    protected override ItemModel FindItemModel(object identifierValue, ResolveSitecoreItemSettings resolveItemSettings, IItemModelRepository repository, PipelineContext pipelineContext, ILogger logger, string language = null, int version = 0)
    {
      if (resolveItemSettings == null || repository == null)
      {
        throw new ArgumentNullException();
      }

      var reader = this.GetValueReader(resolveItemSettings.MatchingFieldValueAccessor) as SitecoreItemFieldReader;
      if (reader == null)
      {
        return null;
      }
      if (reader.FieldName.Equals(ItemModel.ItemID))
      {
        return GetItemModelById(identifierValue, resolveItemSettings, repository, pipelineContext, logger, language, version);
      }
      var searchSettings = this.ResolveItemSearchSettings(pipelineContext);
      searchSettings.SearchFilters.Add(new SearchFilter { FieldName = reader.FieldName, Value = identifierValue.ToString() });
      var items = repository.Search(searchSettings);

      return items?.FirstOrDefault(m => m[ItemModel.ItemLanguage].ToString() == language);
    }

    protected virtual ItemSearchSettings ResolveItemSearchSettings(PipelineContext pipelineContext)
    {
      ItemSearchSettings settings = new ItemSearchSettings();
      var plugin = pipelineContext.CurrentPipelineStep.GetPlugin<ItemSearchSettingsPlugin>();
      if (plugin != null && plugin.PageSize > 0)
      {
        settings.PageSize = plugin.PageSize;
      }
      return settings;
    }

    private IValueReader GetValueReader(IValueAccessor config)
    {
      return config?.ValueReader;
    }

    protected override ItemModel CreateItemModel(object identifierObject, IItemModelRepository repository, ResolveSitecoreItemSettings settings, PipelineContext pipelineContext, ILogger logger, string language = null, int version = 0)
    {
      var parentItemId = this.GetParentItemIdForNewItem(repository, settings, pipelineContext, logger);
      var itemModel = base.CreateItemModel(identifierObject, repository, settings, pipelineContext, logger, language, version);
      if (itemModel != null)
      {
        itemModel[ItemModel.ParentID] = parentItemId;
      }

      return itemModel;
    }

    protected override Guid GetParentItemIdForNewItem(IItemModelRepository repository, ResolveSitecoreItemSettings settings, PipelineContext pipelineContext, ILogger logger)
    {
      if (settings.ParentForItemLocation != null && settings.ParentForItemLocation != Guid.Empty)
      {
        var parentItems = this.GetObjectFromPipelineContext(settings.ParentForItemLocation, pipelineContext, logger) as Dictionary<string, ItemModel>;
        if (parentItems != null && parentItems.Count > 0)
        {
          var parentItem = parentItems.FirstOrDefault().Value as ItemModel;
          if (parentItem != null)
          {
            var parentItemId = parentItem.GetItemId();
            if (parentItemId != Guid.Empty)
            {
              return parentItemId;
            }
          }
        }
      }
      return settings.ParentItemIdItem;
    }
  }
}