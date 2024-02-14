﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using CommandLine;
using CommandLine.Text;

namespace NAPS2.Automation
{
    public class CommandLineOptions
    {
        [ParserState]
        [XmlIgnore]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}