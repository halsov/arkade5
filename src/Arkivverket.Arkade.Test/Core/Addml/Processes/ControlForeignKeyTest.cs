using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using Arkivverket.Arkade.Test.Util;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlForeignKeyTest : IDisposable
    {
        private readonly IDisposable _logCapture;
        
        public ControlForeignKeyTest(ITestOutputHelper outputHelper)
        {
            _logCapture = LoggingHelper.Capture(outputHelper);

        }

        public void Dispose()
        {
            _logCapture.Dispose();
        }

        [Fact]
        public void ShouldReturnNoErrorsWhenAllForeignKeysExists()
        {
            var addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder().Build();
            var recordDef = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addmlFlatFileDefinition)
                .Build();

            AddmlFieldDefinition primaryKeyFieldDef = recordDef.AddAddmlFieldDefinition("id", null, null, new IntegerDataType(), true, false,
                null, null, null, null, null, true);
            var primaryKeyField = new Field(primaryKeyFieldDef, "1001");

            AddmlFieldDefinition foreignKeyFieldDef = recordDef.AddAddmlFieldDefinition("foreignKeyId", null, null, new IntegerDataType(), true, false,
                null, null, null, null, null, false);
            var foreignKeyField = new Field(foreignKeyFieldDef, "1001");

            var controlForeignKey = new ControlForeignKey();
            controlForeignKey.Run(primaryKeyField);
            controlForeignKey.Run(foreignKeyField);

            controlForeignKey.EndOfFile();
            TestRun testRun = controlForeignKey.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnErrorWhenForeignKeyReferencesNonExistingPrimaryKeyValue()
        {
            var addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder().Build();
            var recordDef = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addmlFlatFileDefinition)
                .Build();

            AddmlFieldDefinition primaryKeyFieldDef = recordDef.AddAddmlFieldDefinition("id", null, null, new IntegerDataType(), true, false,
                null, null, null, null, null, true);
            var primaryKeyField = new Field(primaryKeyFieldDef, "1001");

            AddmlFieldDefinition foreignKeyFieldDef = new AddmlFieldDefinitionBuilder()
                .WithForeignKey(primaryKeyFieldDef)
                .Build();

            var foreignKeyField = new Field(foreignKeyFieldDef, "25");

            var controlForeignKey = new ControlForeignKey();
            controlForeignKey.Run(primaryKeyField);
            controlForeignKey.Run(foreignKeyField);

            controlForeignKey.EndOfFile();
            TestRun testRun = controlForeignKey.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
        }

    }
}