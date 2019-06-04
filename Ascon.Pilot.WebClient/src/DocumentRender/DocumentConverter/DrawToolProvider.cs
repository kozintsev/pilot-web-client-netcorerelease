using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;

namespace DocumentRender.DocumentConverter
{
    public interface IDrawToolProvider
    {
        ToolProperties GetDrawTool();
    }

    class DrawToolProvider : IDrawToolProvider
    {
        private readonly List<ToolProperties> _tools = new List<ToolProperties>();
        private readonly ToolProperties _defaultTool;

        public DrawToolProvider(ILog logger)
        {
            _tools.Add(new ToolProperties("/usr/bin/mutool", "draw -o {0} {1} {2}"));
            _tools.Add(new ToolProperties("/usr/bin/mudraw", "-o {0} {1} {2}"));

            foreach (var tool in _tools)
            {
                if (!File.Exists(tool.ToolName))
                    continue;

                _defaultTool = tool;
                logger.Info("Draw tool set to " + _defaultTool.ToolName);
            }
        }

        public ToolProperties GetDrawTool()
        {
            if (_defaultTool == null)
                throw new RenderToolNotFoundException();

            return _defaultTool;
        }
    }

    public class ToolProperties
    {
        private readonly string _argumentsFormat;

        public ToolProperties(string toolName, string argumentsFormat)
        {
            _argumentsFormat = argumentsFormat;
            ToolName = toolName;
        }

        public string ToolName { get; }

        public string GetArguments(string filename, string outputDir, int? page)
        {
            var pageName = "page_%d.png";
            if (page != null)
                pageName = $"page_{page}.png";

            var drawResultPath = Path.Combine(outputDir, pageName);

            var arguments = string.Format(_argumentsFormat, drawResultPath, filename, page);
            return arguments;
        }
    }
}
