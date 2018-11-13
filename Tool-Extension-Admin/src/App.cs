// Copyright 2005-2006 University of Wisconsin
// Copyright 2011 Portland State University
// All rights reserved. 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Contributors:
//   James Domingo, UW-Madison, Forest Landscape Ecology Lab
//   Srinivas S., Portland State University

using Landis.Utilities;
using System;
using System.Reflection;
using System.Text;
using Landis.Extensions;

namespace Landis.Extensions.Admin
{
    public static class App
    {
        public static bool InstallingExtension = false;

        //---------------------------------------------------------------------

        public static int Main(string[] args)
        {
            try {
                Assembly coreAssembly = Assembly.GetAssembly(typeof(Landis.Core.ICore));
                Version coreVersion = coreAssembly.GetName().Version;
                Console.WriteLine("LANDIS-II {0}.{1}", coreVersion.Major, coreVersion.Minor);
                
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                Version toolVersion = myAssembly.GetName().Version;
                Console.WriteLine("Extensions Administration Tool {0}.{1}", toolVersion.Major, toolVersion.Minor);

                Console.WriteLine("Copyright 2005-2006 University of Wisconsin");
                Console.WriteLine("Copyright 2011 Portland State University");
                Console.WriteLine();

                ICommand command = ParseArgs(args);
                InstallingExtension = command is InstallCommand;
                if (command != null)
                    command.Execute();
                return 0;
            }
            catch (ApplicationException exc) {
                Console.WriteLine(exc.Message);
                return 1;
            }
            catch (Exception exc) {
                Console.WriteLine("Internal error occurred within the program:");
                Console.WriteLine("  {0}", exc.Message);
                if (exc.InnerException != null) {
                    Console.WriteLine("  {0}", exc.InnerException.Message);
                }
                Console.WriteLine();
                Console.WriteLine("Stack trace:");
                Console.WriteLine(exc.StackTrace);
                return 1;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Parses the arguments passed to the program on the command line.
        /// </summary>
        /// <returns>
        /// A ICommand object which will perform the command (action) specified
        /// by the arguments.
        /// </returns>
        private static ICommand ParseArgs(string[] args)
        {
            if (args.Length == 0)
                return new ListBriefCommand();

            if (args[0] == "list") {
                if (args.Length > 1)
                    throw ExtraArgsException(args, 1);
                return new ListCommand();
            }

            if (args[0] == "add") {
                if (args.Length == 1)
                    throw UsageException("No filename for \"add\" command");
                if (args.Length > 2)
                    throw ExtraArgsException(args, 2);
                return new AddCommand(args[1]);
            }

            if (args[0] == "remove") {
                if (args.Length == 1)
                    throw UsageException("No extension name for \"remove\" command");
                if (args.Length > 2)
                    throw ExtraArgsException(args, 2);
                return new RemoveCommand(args[1]);
            }

            throw UsageException("Unknown argument: {0} -- expected one of these: list, add, remove", args[0]);
        }

        //---------------------------------------------------------------------

        private static MultiLineException UsageException(string          message,
                                                         params object[] mesgArgs)
        {
            string[] lines = { 
                "Error with arguments on command line:",
                "  " + string.Format(message, mesgArgs),
                "",
                "Usage:",
                "  landis-extensions", 
                "  landis-extensions list", 
                "  landis-extensions add {extension-info-file}",
                "  landis-extensions remove {extension-name}"
            };
            return new MultiLineException(lines);
        }

        //---------------------------------------------------------------------

        private static MultiLineException ExtraArgsException(string[] args,
                                                             int      firstExtraArgIndex)
        {
            StringBuilder message = new StringBuilder();
            if (firstExtraArgIndex == args.Length - 1)
                message.Append("Extra argument:");
            else
                message.Append("Extra arguments:");
            for (int i = firstExtraArgIndex; i < args.Length; ++i)
                message.AppendFormat(" {0}", args[i]);
            return UsageException(message.ToString());
        }
    }
}
