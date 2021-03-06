﻿using AgUnit.Runner.Resharper60.TaskRunner.UnitTestRunner.Silverlight.Providers;
using AgUnit.Runner.Resharper60.Util;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace AgUnit.Runner.Resharper60.TaskRunner.UnitTestProvider.XUnit
{
    public class XUnitAssemblyTaskProvider : IAssemblyTaskProvider
    {
        public bool IsAssemblyTask(RemoteTask task)
        {
            return task.GetType().FullName == "XunitContrib.Runner.ReSharper.RemoteRunner.XunitTestAssemblyTask";
        }

        public string GetXapPath(RemoteTask task)
        {
            return task.GetProperty<string>("AssemblyLocation").Replace(".dll", ".xap"); // TODO: Find a way to get this from the project settings.
        }
    }
}