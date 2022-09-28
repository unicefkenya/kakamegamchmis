using System;
using System.Collections;

namespace MCHMIS.Mobile.Interface
{
    public interface ILogger
    {
        void Report(Exception exception = null, Severity warningLevel = Severity.Warning);

        void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning);

        void Report(Exception exception, string key, string value, Severity warningLevel = Severity.Warning);

        void Track(string trackIdentifier);

        void Track(string trackIdentifier, string key, string value);

        void TrackPage(string page, string id = null);
    }
}