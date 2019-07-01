// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.HttpRepl.FileSystem;

namespace Microsoft.HttpRepl.Commands
{
    public class GetCommand : BaseHttpCommand
    {
        public GetCommand(IFileSystem fileSystem) : base(fileSystem) { }

        protected override string Verb => "get";

        protected override bool RequiresBody => false;
    }
}
