﻿using System.Collections.Generic;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseCountRecords : AddmlProcess
    {
        public const string Name = "Analyse_CountRecords";
        private readonly List<TestResult> _testResults = new List<TestResult>();
        private FlatFile _currentFlatFile;
        private int _numberRecords;

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.AnalyseCountRecordsDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override void DoRun(FlatFile flatFile)
        {
            _numberRecords = 0;
            _currentFlatFile = flatFile;
        }

        protected override void DoRun(Record record)
        {
            _numberRecords++;
        }

        protected override void DoRun(Field field)
        {
        }

        protected override void DoEndOfFile()
        {
            _testResults.Add(new TestResult(ResultType.Success, new Location(_currentFlatFile.Definition.FileName),
                string.Format(Messages.AnalyseCountRecordsMessage, _numberRecords)));
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }
    }
}