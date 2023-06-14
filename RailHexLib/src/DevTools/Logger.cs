using RailHexLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace RailHexLib.DevTools
{
    public abstract class BaseLogger : ILogger
    {
        string category;
        static TraceSource debug = new TraceSource("debug logger");
        static Dictionary<string, bool> rules;

        public string Category => category;

        public bool Enabled(string category)
        {
            if (category == null || this.category == null)
            {
                return false;
            }

            foreach (var (key, value) in rules)
            {
                debug.TraceInformation($"compare {key} and {category}");
                if (Regex.IsMatch(category, $"^{key}$"))
                {
                    debug.TraceInformation($"match {key} and {category} - {value}");
                    return value;
                }
            }
            return false;
        }

        public BaseLogger(string category = null)
        {
            if (category != null)
            {
                this.category = category;
                var listener = new ConsoleTraceListener();
                debug.Listeners.Add(listener);
                if (Environment.GetEnvironmentVariable("DEBUG_LOG") != null)
                    debug.Switch.Level = SourceLevels.All;
                else
                    debug.Switch.Level = SourceLevels.Off;
            }

            parseRules();

        }
        protected abstract void logImpl(string msg);
        public void Log(string msg, string category = null)
        {
            debug.TraceInformation($"log in '{category}' and logger category '{this.category}'");

            string testCategory = null;
            if (category != null && this.category != null)
            {
                testCategory = string.Join("-", new List<string>() { this.category, category });
            }
            else
            {
                testCategory = category ?? this.category;
            }

            if (testCategory != null && Enabled(testCategory))
            {
                //debug.TraceInformation($"msg '{msg}'");
                logImpl(msg);
                
            }
        }
        void parseRules()
        {
            rules = new Dictionary<string, bool>();

            var log_rules = Environment.GetEnvironmentVariable("HEXRAIL_LOG_RULES");
            if (log_rules != null)
            {
                // parse srting like '*catOne*=true;my.other.*=fale' to a dictionary 
                // where the key is the category substring regexp and the value is a bool
                foreach (var rule in log_rules.Split(';'))
                {
                    var parts = rule.Split('=');
                    var key = parts[0];
                    var value = bool.Parse(parts[1]);
                    rules[key] = value;
                }
            }
        }

    }

    public class Logger : BaseLogger
    {
        TraceSource source;
        public Logger(string category=null) : base(category)
        {
            var listener = new ConsoleTraceListener();
            source = new TraceSource(category);
            source.Listeners.Add(listener);
            source.Switch.Level = SourceLevels.All;
        }

        protected override void logImpl(string msg)
        {
            source.TraceInformation(msg);
        }
    }

    public class DefaultSilentLogger : ILogger
    {
        public string Category => "silent";

        public void Log(string msg, string category = null) { }


    }
}