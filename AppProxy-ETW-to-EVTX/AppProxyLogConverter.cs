using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;

namespace AppProxy_ETW_to_EVTX
{
    public partial class AppProxyLogConverter : ServiceBase
    {

        private static Logger _logger;
        private TraceEventSession _session;

        public AppProxyLogConverter()
        {
            InitializeComponent();

            string logFilePath = "application_log.txt";
            _logger = new Logger(logFilePath);   
        }

        protected override void OnStart(string[] args)
        {
            _logger.Log("Service started.");

            const string sessionName = "AppProxyLogExporter";
            const string providerName = "Microsoft-AadApplicationProxy-Connector";

            _session = new TraceEventSession(sessionName);
            _session.Source.Dynamic.All += HandleEvent;
            _session.EnableProvider(providerName);

            _logger.Log($"Listening to events from provider '{providerName}'.");
            Task.Run(() => _session.Source.Process());
        }

        protected override void OnStop()
        {
            _logger.Log("Service stopped.");

            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
        }

        public class Logger
        {
            private readonly string _logFilePath;

            public Logger(string logFilePath)
            {
                _logFilePath = logFilePath;
            }

            public void Log(string message)
            {
                string logMessage = $"{DateTime.Now}: {message}";
                File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
            }
        }

        private async void HandleEvent(TraceEvent traceEvent)
        {
            const int EventId1 = 24028; // Microsoft AAD Application Proxy Connector received a frontend request.
            const int EventId2 = 24029; // Microsoft AAD Application Proxy Connector sent a request to backend application.
            const int EventId3 = 24030; // Microsoft AAD Application Proxy Connector received a response from backend.

            if (traceEvent.ID == (TraceEventID)EventId1)
            {
                Console.WriteLine($"Event {EventId1} received at: {traceEvent.TimeStamp}");
                string source = "AppProxy-ETW-Event-Extractor";
                string log = "Microsoft-AadApplicationProxy-Connector/Admin";
                EventLogEntryType entryType = EventLogEntryType.Information;
                int eventId = (int)traceEvent.ID;
                string input_message = $"{traceEvent}";
                string formated_message = input_message.Replace("&lt;", "<").Replace("&gt;", ">");
                string prefix = "Microsoft AAD Application Proxy Connector received a frontend request." + Environment.NewLine;
                formated_message = prefix + formated_message;

                await WriteToEventLogAsync(source, log, entryType, eventId, formated_message);
            }

            if (traceEvent.ID == (TraceEventID)EventId2)
            {
                Console.WriteLine($"Event {EventId2} received at: {traceEvent.TimeStamp}");
                string source = "AppProxy-ETW-Event-Extractor";
                string log = "Microsoft-AadApplicationProxy-Connector/Admin";
                EventLogEntryType entryType = EventLogEntryType.Information;
                int eventId = (int)traceEvent.ID;
                string input_message = $"{traceEvent}";
                string formated_message = input_message.Replace("&lt;", "<").Replace("&gt;", ">");
                string prefix = "Microsoft AAD Application Proxy Connector sent a request to backend application." + Environment.NewLine;
                formated_message = prefix + formated_message;

                await WriteToEventLogAsync(source, log, entryType, eventId, formated_message);
            }

            if (traceEvent.ID == (TraceEventID)EventId3)
            {
                Console.WriteLine($"Event {EventId3} received at: {traceEvent.TimeStamp}");
                string source = "AppProxy-ETW-Event-Extractor";
                string log = "Microsoft-AadApplicationProxy-Connector/Admin";
                EventLogEntryType entryType = EventLogEntryType.Information;
                int eventId = (int)traceEvent.ID;
                string input_message = $"{traceEvent}";
                string formated_message = input_message.Replace("&lt;", "<").Replace("&gt;", ">");
                string prefix = "Microsoft AAD Application Proxy Connector received a response from backend." + Environment.NewLine;
                formated_message = prefix + formated_message;

                await WriteToEventLogAsync(source, log, entryType, eventId, formated_message);
            }

        }

        public async Task WriteToEventLogAsync(string source, string log, EventLogEntryType entryType, int eventId, string message)
        {
            // Check if the source exists, if not, create it
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, log);
            }

            // Write the entry to the event log
            using (EventLog eventLog = new EventLog(log))
            {
                eventLog.Source = source;
                eventLog.WriteEntry(message, entryType, eventId);
            }
        }

    }
}
