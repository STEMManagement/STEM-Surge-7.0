using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace STEM.Surge.NuGet
{
    /// <summary>
    /// Push a NuGet package to a repository. Based on this example in the MSDN
    /// docs: https://docs.microsoft.com/en-us/nuget/reference/nuget-client-sdk#push-a-package.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Push")]
    [Description("Push a NuGet package to a 'Source Repository'.")]
    public class Push : Instruction
    {
        [Category("NuGet")]
        [DisplayName("Source Repository")]
        [Description("Source repository URI. This may be either an HTTP server or file system.")]
        public string SourceRepository { get; set; } = "https://api.nuget.org/v3/index.json";

        [Category("NuGet")]
        [DisplayName("API Key")]
        [Description("API key to use when pushing to the repository.")]
        [PasswordPropertyText(true)]
        [XmlIgnore]
        public string ApiKey { get; set; } = string.Empty;

        [Category("NuGet")]
        [DisplayName("Package Path")]
        [Description("Path to the package to push.")]
        public string PackagePath { get; set; } = @"[TargetPath]\[TargetName]";

        protected override void _Rollback()
        {
            // Do nothing.
        }

        protected override bool _Run()
        {
            try
            {
                var repository = Repository.Factory.GetCoreV3(SourceRepository);
                var resource = repository.GetResource<PackageUpdateResource>();

                resource.Push(
                    new List<string> { PackagePath },
                    symbolSource: null,
                    timeoutInSecond: 5 * 60,
                    disableBuffering: false,
                    getApiKey: _ => ApiKey,
                    getSymbolApiKey: _ => null,
                    noServiceEndpoint: false,
                    skipDuplicate: false,
                    symbolPackageUpdateResource: null,
                    NullLogger.Instance)
                    .Wait();
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.InnerExceptions)
                {
                    AppendToMessage(e.Message);
                    Exceptions.Add(e);
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }
    }
}
