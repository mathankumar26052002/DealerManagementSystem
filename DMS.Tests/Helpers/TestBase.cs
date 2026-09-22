using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Tests.Helpers
{
    public abstract class TestBase
    {
        protected async Task InitializeDatabaseAsync()
        {
            await TestDatabaseSetup.InitializeAsync();
        }
    }
}
