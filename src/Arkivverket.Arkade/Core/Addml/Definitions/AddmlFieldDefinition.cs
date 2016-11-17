﻿using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlFieldDefinition
    {
        public AddmlRecordDefinition AddmlRecordDefinition { get; }

        public string Name { get; }
        //public string Description { get;  }
        public int? StartPosition { get; }
        public int? FixedLength { get; }
        public DataType Type { get; }
        public bool IsUnique { get; }
        public bool IsNullable { get; }
        public int? MinLength { get; }
        public int? MaxLength { get; }
        public AddmlFieldDefinition ForeignKey { get; }
        public List<string> Processes { get; }
        public List<AddmlCode> Codes { get; }

        public AddmlFieldDefinition(string name,
            int? startPosition,
            int? fixedLength,
            DataType type,
            bool isUnique,
            bool isNullable,
            int? minLength,
            int? maxLength,
            AddmlFieldDefinition foreignKey,
            AddmlRecordDefinition addmlRecordDefinition,
            List<string> processes,
            List<AddmlCode> codes)
        {
            Name = name;
            StartPosition = startPosition;
            FixedLength = fixedLength;
            Type = type;
            IsUnique = isUnique;
            IsNullable = isNullable;
            MinLength = minLength;
            MaxLength = maxLength;
            ForeignKey = foreignKey;
            AddmlRecordDefinition = addmlRecordDefinition;
            Processes = processes;
            Codes = codes;
        }

        public AddmlFlatFileDefinition GetAddmlFlatFileDefinition()
        {
            return AddmlRecordDefinition.AddmlFlatFileDefinition;
        }

        public bool IsPartOfPrimaryKey()
        {
            return AddmlRecordDefinition.PrimaryKey.Contains(this);
        }

        public string Key()
        {
            return AddmlRecordDefinition.Key() + "_" + Name;
        }

        public FieldIndex GetIndex()
        {
            return new FieldIndex(AddmlRecordDefinition.AddmlFlatFileDefinition.Name, AddmlRecordDefinition.Name, this.Name);
        }
    }
}
