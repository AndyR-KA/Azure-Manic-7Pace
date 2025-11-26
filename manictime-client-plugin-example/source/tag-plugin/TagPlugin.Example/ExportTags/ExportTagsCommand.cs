using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Finkit.ManicTime.Client.LocalActivityFetching.Messaging;
using Finkit.ManicTime.Client.Main.Logic;
using Finkit.ManicTime.Common;
using Finkit.ManicTime.Common.TagSources;
using Finkit.ManicTime.Common.Timelines;
using Finkit.ManicTime.Plugins.Timelines.Tags;
using Finkit.ManicTime.Shared;
using Finkit.ManicTime.Shared.Logging;
using Finkit.ManicTime.Shared.Plugins.ServiceProviders.PluginCommands;
using Finkit.ManicTime.Shared.Helpers;
using TagPlugin.Settings;
using TagPlugins.Core;

namespace TagPlugin.ExportTags
{
    public class ExportTagsCommand : PluginCommand
    {
        private readonly TagSourceService _tagSourceService;
        private readonly ActivityReaderMessageClient _activityReaderMessageClient;
        private readonly IViewTimelineCache _viewTimelineCache;

        // need a mutex on changes to this
        private static bool processing = false;
        private static bool changesQueued = false;

        public ExportTagsCommand(
            IEventHub eventHub,
            TagSourceService tagSourceService,
            ActivityReaderMessageClient activityReaderMessageClient,
            IViewTimelineCache viewTimelineCache)
        {
            _tagSourceService = tagSourceService;
            _activityReaderMessageClient = activityReaderMessageClient;
            _viewTimelineCache = viewTimelineCache;
            eventHub.Subscribe<TagSourceCacheUpdatedEvent>(OnTagSourceCacheUpdated);
            InvokeOnUiThread(SetCanExecute);
        }

        public override string Name => "Publish Work Logs";

        private static void InvokeOnUiThread(Action action)
        {
            var currentApplication = Application.Current;
            if (currentApplication == null || currentApplication.CheckAccess())
                action();
            else
                currentApplication.Dispatcher.Invoke(action);
        }

        private void OnTagSourceCacheUpdated(TagSourceCacheUpdatedEvent obj)
        {
            InvokeOnUiThread(SetCanExecute);

            if (CanExecute)
            {
                if (processing == false)
                {
                    Execute();
                }
                else
                {
                    changesQueued = true;
                }
            }
        }

        private void SetCanExecute()
        {
            CanExecute =
                TagPluginsHelper.GetTagSourceInstances(_tagSourceService.GetTagSourceInstances(),
                    ClientPlugin.Id).Any();
        }

        public override async void Execute()
        {
            processing = true;

            try
            {
                var tagSourceInstance = TagPluginsHelper.GetTagSourceInstances(
                    _tagSourceService.GetTagSourceInstances(),
                    ClientPlugin.Id)
                    .First();

                var azureDevOpsSettings = (AzureDevOpsWorkItemTagSettings)tagSourceInstance.Settings ?? new AzureDevOpsWorkItemTagSettings();

                int days = int.Parse(azureDevOpsSettings.Days);

                var exporter = new TagsExporter(
                    organizationName: azureDevOpsSettings.Organization,
                    timeTrackerToken: azureDevOpsSettings.TimeTrackerApiSecret,
                    billableActivityId: azureDevOpsSettings.BillableActivityId,
                    nonBillableActivityId: azureDevOpsSettings.NonBillableActivityId,
                    days: days);

                DateRange range = TagsExporter.GetDateRange(days);

                var tagActivities = await GetTagActivitiesAsync(range.From, range.To).ConfigureAwait(false);

                await exporter.Export(tagActivities, range);
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteError(ex);
            }

            processing = false;

            if (changesQueued)
            {
                changesQueued = false;
                Execute();
            }
        }

        private async Task<TagActivity[]> GetTagActivitiesAsync(int dateFrom, int dateTo)
        {
            var timeline = _viewTimelineCache.LocalTagTimeline;

            // Instead of trying to reverse engineer the conversion, let's use DateTime.Today
            // and calculate the number of days difference to get the actual dates
            // Since dateFrom and dateTo are likely representing a date range relative to today,
            // we'll compute the actual dates based on what DateTimeHelper.FromUnshiftedDateTime would produce

            var today = DateTime.Today;
            var todayAsInt = DateTimeHelper.FromUnshiftedDateTime(today);

            // Calculate the offset from today and apply it
            var fromDateOffset = dateFrom - todayAsInt;
            var toDateOffset = dateTo - todayAsInt;

            var fromDateTime = today.AddDays(fromDateOffset);
            var toDateTime = today.AddDays(toDateOffset);

            var activities = await _activityReaderMessageClient.GetActivitiesAsync(
                timeline,
                new Date(fromDateTime),
                new Date(toDateTime),
                false,
                CancellationToken.None).ConfigureAwait(false);

            return activities.Cast<TagActivity>().ToArray();
        }
    }
}