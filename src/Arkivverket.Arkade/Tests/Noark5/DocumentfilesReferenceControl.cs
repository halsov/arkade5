﻿using System.Collections.Generic;
using System.Collections;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #47
    /// </summary>
    public class DocumentfilesReferenceControl : Noark5XmlReaderBaseTest
    {
        private static Hashtable _documentFileNames;

        public DocumentfilesReferenceControl(Archive archive)
        {
            _documentFileNames = GetNamesActualFiles(archive);
        }

        public override string GetName()
        {
            return Noark5Messages.DocumentfilesReferenceControl;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            foreach (DictionaryEntry fileNameEntry in _documentFileNames)
                testResults.Add(new TestResult(ResultType.Error,
                    new Location(ArkadeConstants.DirectoryNameDocuments),
                    string.Format(Noark5Messages.DocumentfilesReferenceControlMessage, fileNameEntry.Key)));

            return testResults;
        }

        private static Hashtable GetNamesActualFiles(Archive archive)
        {
            DirectoryInfo documentsDirectory = archive.WorkingDirectory.Content()
                .WithSubDirectory(ArkadeConstants.DirectoryNameDocuments).DirectoryInfo();

            var filenames = new Hashtable();

            if (documentsDirectory.Exists)
                FindFilenames(documentsDirectory, filenames, documentsDirectory.Name);

            return filenames;
        }

        private static void FindFilenames(DirectoryInfo directoryInfo, Hashtable filenames, string relativePath)
        {
            foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles())
                filenames.Add($"{relativePath}/{fileInfo.Name}", null);

            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.EnumerateDirectories())
                FindFilenames(subDirectoryInfo, filenames, $"{relativePath}/{subDirectoryInfo.Name}");
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("referanseDokumentfil", "dokumentobjekt"))
                _documentFileNames.Remove(eventArgs.Value.Replace("\\", "/"));
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
