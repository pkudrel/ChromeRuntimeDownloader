using System;
using System.CommandLine;

namespace ChromeRuntimeDownloader.Feature.Arguments
{
    public static class Extensions
    {



        public static Argument Decorate(this Argument argument, Action<Argument> action)
        {
            action(argument);
            return argument;
        }
    }
}