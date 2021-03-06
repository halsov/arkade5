﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml
{
    public class DelimiterFileFormatReader : FileFormatReader
    {
        private readonly string _fieldDelimiter;
        private readonly IEnumerator<string> _lines;

        // Zero based position of field used to identify recordDefinition
        private readonly int? _recordIdentifierPosition;

        public override Record Current => GetCurrentRecord();

        public DelimiterFileFormatReader(FlatFile flatFile) : base(flatFile.Definition)
        {
            StreamReader stream = GetStream(flatFile);
            string recordDelimiter = GetRecordDelimiter(flatFile);
            _fieldDelimiter = GetFieldDelimiter(flatFile);
            _recordIdentifierPosition = GetRecordIdentifierPosition(flatFile);
            _lines = stream.ReadUntil(recordDelimiter).GetEnumerator();
        }

        private int? GetRecordIdentifierPosition(FlatFile flatFile)
        {
            // TODO jostein: Støtte for flere recordDefinitions

            return null;
        }

        private string GetFieldDelimiter(FlatFile flatFile)
        {
            return flatFile.Definition.FieldSeparator.Get();
        }

        private string GetRecordDelimiter(FlatFile flatFile)
        {
            return flatFile.Definition.RecordSeparator.Get();
        }

        private static StreamReader GetStream(FlatFile flatFile)
        {
            FileStream fileStream = flatFile.Definition.FileInfo.OpenRead();
            Encoding encoding = flatFile.Definition.Encoding;
            StreamReader streamReader = new StreamReader(fileStream, encoding);
            return streamReader;
        }


        private Record GetCurrentRecord()
        {
            List<Field> fields = new List<Field>();

            string currentLine = _lines.Current;

            string[] strings = currentLine.Split(new[] {_fieldDelimiter}, StringSplitOptions.None);

            string recordIdentifier = null;
            if (_recordIdentifierPosition.HasValue)
            {
                recordIdentifier = strings[_recordIdentifierPosition.Value];
            }
            AddmlRecordDefinition recordDefinition = GetAddmlRecordDefinition(recordIdentifier);
            List<AddmlFieldDefinition> fieldDefinitions = recordDefinition.AddmlFieldDefinitions;

            if (fieldDefinitions.Count != strings.Length)
            {
                int maxFields = 40;
                string fielddata = currentLine.Length <= maxFields ? currentLine : currentLine.Substring(0, maxFields-1);
                throw new ArkadeAddmlDelimiterException(
                    $"{Resources.AddmlMessages.UnexpectedNumberOfFields}: {strings.Length}/{fieldDefinitions.Count}", 
                    recordDefinition.Name, 
                    fielddata);
            }

            for (int i = 0; i < strings.Length; i++)
            {
                string s = strings[i];
                AddmlFieldDefinition fieldDefinition = fieldDefinitions[i];
                fields.Add(new Field(fieldDefinition, s));
            }

            return new Record(recordDefinition, fields);
        }

        public override void Dispose()
        {
            _lines.Dispose();
        }

        public override bool MoveNext()
        {
            return _lines.MoveNext();
        }

        public override void Reset()
        {
            _lines.Reset();
        }
    }
}