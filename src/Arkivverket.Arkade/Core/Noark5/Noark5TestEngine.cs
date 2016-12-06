using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Noark5
{
    public class Noark5TestEngine : ITestEngine
    {
        private readonly IArchiveContentReader _archiveContentReader;
        private readonly ITestProvider _testProvider;
        private readonly IStatusEventHandler _statusEventHandler;

        public Noark5TestEngine(IArchiveContentReader archiveContentReader, ITestProvider testProvider, IStatusEventHandler statusEventHandler)
        {
            _archiveContentReader = archiveContentReader;
            _testProvider = testProvider;
            _statusEventHandler = statusEventHandler;
        }

        public event EventHandler<ReadElementEventArgs> ReadStartElementEvent;
        public event EventHandler<ReadElementEventArgs> ReadElementValueEvent;
        public event EventHandler<ReadElementEventArgs> ReadEndElementEvent;


        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
            List<IArkadeStructureTest> structureTests = RunStructureTests(testSession.Archive);

            List<INoark5Test> contentTests = RunContentTests(testSession.Archive);

            var testSuite = new TestSuite();
            AddTestToTestSuite(contentTests, testSuite);
            AddTestToTestSuite(structureTests, testSuite);
            return testSuite;
        }

        private static void AddTestToTestSuite(IEnumerable<IArkadeTest> tests, TestSuite testSuite)
        {
            foreach (var test in tests)
                testSuite.AddTestRun(test.GetTestRun());
        }

        private List<INoark5Test> RunContentTests(Archive archive)
        {
            List<INoark5Test> contentTests = _testProvider.GetContentTests(archive);

            SubscribeTestsToReadElementEvent(contentTests);

            using (var reader = XmlReader.Create(_archiveContentReader.GetContentAsStream(archive)))
            {
                RaiseEventStartParsingFile();

                var path = new Stack<string>();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            path.Push(reader.LocalName);
                            RaiseReadStartElementEvent(CreateReadElementEventArgs(reader, path));
                            break;
                        case XmlNodeType.Text:
                            RaiseReadElementValueEvent(CreateReadElementEventArgs(reader, path));
                            break;
                        case XmlNodeType.EndElement:
                            path.Pop();
                            RaiseReadEndElementEvent(CreateReadElementEventArgs(reader, path));
                            _statusEventHandler.RaiseEventRecordProcessingStopped();
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Comment:
                        case XmlNodeType.Whitespace:
                            break;
                    }
                }
                RaiseEventFinishedParsingFile();
            }
            return contentTests;
        }

        private List<IArkadeStructureTest> RunStructureTests(Archive archive)
        {
            List<IArkadeStructureTest> structureTests = _testProvider.GetStructureTests();
            foreach (var test in structureTests)
            {
                test.Test(archive);
            }
            return structureTests;
        }

        private void RaiseEventStartParsingFile()
        {
            _statusEventHandler.RaiseEventFileProcessingStarted(
                new FileProcessingStatusEventArgs(Archive.ContentDescriptionFileNameNoark5, Archive.ContentDescriptionFileNameNoark5));
        }

        private void RaiseEventFinishedParsingFile()
        {
            _statusEventHandler.RaiseEventFileProcessingFinished(
                new FileProcessingStatusEventArgs(Archive.ContentDescriptionFileNameNoark5, Archive.ContentDescriptionFileNameNoark5, true));
        }

        private static ReadElementEventArgs CreateReadElementEventArgs(XmlReader reader, Stack<string> path)
        {
            return new ReadElementEventArgs(reader.Name, reader.Value, new ElementPath(path.ToList()));
        }

        private void SubscribeTestsToReadElementEvent(List<INoark5Test> testsForArchive)
        {
            foreach (var test in testsForArchive)
            {
                ReadStartElementEvent += test.OnReadStartElementEvent;
                ReadElementValueEvent += test.OnReadElementValueEvent;
                ReadEndElementEvent += test.OnReadEndElementEvent;
            }
        }

        private void RaiseReadStartElementEvent(ReadElementEventArgs readElementEventArgs)
        {
            var handler = ReadStartElementEvent;
            handler?.Invoke(this, readElementEventArgs);
        }
        private void RaiseReadElementValueEvent(ReadElementEventArgs readElementEventArgs)
        {
            var handler = ReadElementValueEvent;
            handler?.Invoke(this, readElementEventArgs);
        }

        private void RaiseReadEndElementEvent(ReadElementEventArgs readElementEventArgs)
        {
            var handler = ReadEndElementEvent;
            handler?.Invoke(this, readElementEventArgs);
        }
    }
}
