using System.IO;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Metadata;
using Arkivverket.Arkade.Report;

namespace Arkivverket.Arkade.Core
{

    /// <summary>
    /// Use this class for interacting with the Arkade Api when you are using Autofac. If you don't use Autofac, please use the Arkade class instead.
    /// </summary>
    public class ArkadeApi
    {
        private readonly TestSessionFactory _testSessionFactory;
        private readonly TestEngineFactory _testEngineFactory;
        private readonly MetadataFilesCreator _metadataFilesCreator;
        private readonly InformationPackageCreator _informationPackageCreator;
        private readonly TestSessionXmlGenerator _testSessionXmlGenerator;

        public ArkadeApi(TestSessionFactory testSessionFactory, TestEngineFactory testEngineFactory, MetadataFilesCreator metadataFilesCreator, InformationPackageCreator informationPackageCreator, TestSessionXmlGenerator testSessionXmlGenerator)
        {
            _testSessionFactory = testSessionFactory;
            _testEngineFactory = testEngineFactory;
            _metadataFilesCreator = metadataFilesCreator;
            _informationPackageCreator = informationPackageCreator;
            _testSessionXmlGenerator = testSessionXmlGenerator;
        }

        public TestSession RunTests(ArchiveDirectory archiveDirectory)
        {
            TestSession testSession = _testSessionFactory.NewSession(archiveDirectory);
            RunTests(testSession);
            return testSession;
        }

        public TestSession RunTests(ArchiveFile archive)
        {
            TestSession testSession = _testSessionFactory.NewSession(archive);
            RunTests(testSession);
            return testSession;
        }

        private void RunTests(TestSession testSession)
        {
            ITestEngine testEngine = _testEngineFactory.GetTestEngine(testSession);
            testSession.TestSuite = testEngine.RunTestsOnArchive(testSession);
            _testSessionXmlGenerator.GenerateXmlAndSaveToFile(testSession);
        }

        public void CreatePackage(TestSession testSession, PackageType packageType)
        {
            _metadataFilesCreator.Create(testSession.Archive, testSession.ArchiveMetadata);

            if (packageType == PackageType.SubmissionInformationPackage)
            {
                _informationPackageCreator.CreateSip(testSession.Archive);
            }
            else
            {
                _informationPackageCreator.CreateAip(testSession.Archive);
            }

            new InfoXmlCreator().CreateAndSaveFile(testSession.Archive, testSession.ArchiveMetadata);
        }

        public void SaveReport(TestSession testSession, FileInfo file)
        {
            using (FileStream fs = file.OpenWrite())
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    IReportGenerator reportGenerator = new HtmlReportGenerator(sw);
                    reportGenerator.Generate(testSession);
                }
            }
        }

    }
}
