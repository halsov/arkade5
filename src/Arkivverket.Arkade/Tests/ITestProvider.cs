using System.Collections.Generic;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests
{
    public interface ITestProvider
    {
        List<INoark5Test> GetContentTests(Archive archive);

        List<IArkadeStructureTest> GetStructureTests();
    }
}