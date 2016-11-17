﻿using System.Collections.Generic;
using System.Numerics;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseAllFrequenceList : AddmlProcess
    {
        public const string Name = "Analyse_AllFrequenceList";

        private readonly Dictionary<FieldIndex, FrequencyList> _frequenceListPerField
            = new Dictionary<FieldIndex, FrequencyList>();

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.AnalyseAllFrequenceListDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.Content;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoEndOfFile()
        {
            foreach (KeyValuePair<FieldIndex, FrequencyList> entry in _frequenceListPerField)
            {
                FieldIndex index = entry.Key;
                FrequencyList frequencyList = entry.Value;

                foreach (KeyValuePair<string, BigInteger> e in frequencyList.Get())
                {
                    string word = e.Key;
                    BigInteger count = e.Value;

                    _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(index),
                        string.Format(Messages.AnalyseAllFrequenceListMessage, count, word)));
                }
            }

            _frequenceListPerField.Clear();
        }

        protected override void DoRun(Field field)
        {
            List<AddmlCode> codes = field.Definition.Codes;
            if (codes == null || codes.Count == 0)
            {
                return;
            }

            FieldIndex fieldIndeks = field.Definition.GetIndex();
            if (!_frequenceListPerField.ContainsKey(fieldIndeks))
            {
                _frequenceListPerField.Add(fieldIndeks, new FrequencyList());
            }

            FrequencyList frequencyList = _frequenceListPerField[fieldIndeks];
            frequencyList.Add(field.Value);
        }
    }
}