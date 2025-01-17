// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.HttpRepl.FileSystem;

namespace Microsoft.HttpRepl.Commands
{
    public class HeadCommand : BaseHttpCommand
    {
        public HeadCommand(IFileSystem fileSystem) : base(fileSystem) { }

        protected override string Verb => "head";

        protected override bool RequiresBody => false;
    }
}
