﻿using System.Collections.Generic;
using System.Numerics;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseCountChars : AddmlProcess
    {
        public const string Name = "Analyse_CountChars";

        private readonly List<TestResult> _testResults = new List<TestResult>();

        private FlatFile _currentFlatFile;
        private BigInteger _numberOfChars;

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.AnalyseCountCharsDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override void DoRun(FlatFile flatFile)
        {
            _numberOfChars = 0;
            _currentFlatFile = flatFile;
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoRun(Field field)
        {
            _numberOfChars += field.Value.Length;
        }

        protected override void DoEndOfFile()
        {
            _testResults.Add(new TestResult(ResultType.Success, new Location(_currentFlatFile.Definition.FileName),
                string.Format(Messages.AnalyseCountCharsMessage, _numberOfChars)));
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }
    }
}