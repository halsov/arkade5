﻿using Arkivverket.Arkade.Core.Addml.Definitions;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class RecordSeparatorTest
    {
        [Fact]
        public void ShouldConvertCrlfRecordSeparator()
        {
            Separator separator = new Separator("CRLF");
            separator.ToString().Should().Be("CRLF");
            separator.Get().Should().Be("\r\n");
        }

        [Fact]
        public void ShouldNotConvertOtherRecordSeparator()
        {
            Separator separator = new Separator("X");
            separator.ToString().Should().Be("X");
            separator.Get().Should().Be("X");
        }
    }
}