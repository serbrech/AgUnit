﻿extern alias mstest10;
extern alias mstestlegacy;

using System;
using System.Linq;
using AgUnit.Runner.Resharper60.Util;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using MsTest10MetadataExplorer = mstest10::JetBrains.ReSharper.UnitTestProvider.MSTest.MsTestMetadataExplorer;
using MsTestLegacyMetadataExplorer = mstestlegacy::JetBrains.ReSharper.UnitTestProvider.MSTest.MsTestMetadataExplorer;
using MsTest10Provider = mstest10::JetBrains.ReSharper.UnitTestProvider.MSTest.MsTestProvider;
using MsTestLegacyProvider = mstestlegacy::JetBrains.ReSharper.UnitTestProvider.MSTest.MsTestProvider;

namespace AgUnit.Runner.Resharper60.UnitTestProvider.MSTest
{
    [MetadataUnitTestExplorer]
    public class SilverlightMsTestMetadataExplorer : IUnitTestMetadataExplorer
    {
        private const string DotNetMsTestAssemblyName = "Microsoft.VisualStudio.QualityTools.UnitTestFramework";
        private const string SilverlightMsTestAssemblyName = "Microsoft.VisualStudio.QualityTools.UnitTesting.Silverlight";

        private readonly MsTest10Provider msTest10Provider;
        private readonly MsTestLegacyProvider msTestLegacyProvider;

        public IUnitTestProvider Provider
        {
            get { return msTest10Provider as IUnitTestProvider ?? msTestLegacyProvider; }
        }

        public SilverlightMsTestMetadataExplorer(MsTest10Provider msTest10Provider = null, MsTestLegacyProvider msTestLegacyProvider = null)
        {
            this.msTest10Provider = msTest10Provider;
            this.msTestLegacyProvider = msTestLegacyProvider;
        }

        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
        {
            if (assembly.ReferencedAssembliesNames.Any(n => n.Name == SilverlightMsTestAssemblyName))
            {
                var originalReferencedAssemblies = InjectMsTestIntoReferencedAssemblyNames(assembly);

                try
                {
                    if (msTest10Provider != null)
                    {
                        new MsTest10MetadataExplorer(msTest10Provider, project, consumer).ExploreAssembly(assembly);
                    }   
                    else if (msTestLegacyProvider != null)
                    {
                        new MsTestLegacyMetadataExplorer(msTestLegacyProvider, project, consumer).ExploreAssembly(assembly);
                    }
                }
                finally
                {
                    SetReferencedAssemblyNames(assembly, originalReferencedAssemblies);
                }
            }
        }

        private AssemblyNameInfo[] InjectMsTestIntoReferencedAssemblyNames(IMetadataAssembly assembly)
        {
            var referencedAssemblyNames = assembly.ReferencedAssembliesNames;
            var updatedReferencedAssemblyNames = new AssemblyNameInfo[referencedAssemblyNames.Length + 1];

            Array.Copy(referencedAssemblyNames, updatedReferencedAssemblyNames, referencedAssemblyNames.Length);
            updatedReferencedAssemblyNames[referencedAssemblyNames.Length] = new AssemblyNameInfo(DotNetMsTestAssemblyName);

            SetReferencedAssemblyNames(assembly, updatedReferencedAssemblyNames);

            return referencedAssemblyNames;
        }

        private void SetReferencedAssemblyNames(IMetadataAssembly assembly, AssemblyNameInfo[] referencedAssemblyNames)
        {
            assembly.SetField("myReferencedAssembliesNames", referencedAssemblyNames);
        }
    }
}