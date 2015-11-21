#!/bin/bash

set -x

rm -rf bin/
mkdir bin/

cp packages/NUnit.*/lib/nunit.framework.dll bin/

################################################################################

echo 'using System;' > assembly.cs
echo 'using System.Reflection;' >> assembly.cs
echo 'using System.Runtime.CompilerServices;' >> assembly.cs
echo '[assembly:AssemblyTitle ("Json")]' >> assembly.cs
echo '[assembly:AssemblyDescription ("A really simple C# JSON parser.")]' >> assembly.cs
echo '[assembly:AssemblyCopyright ("Alex Parker")]' >> assembly.cs
echo '[assembly:CLSCompliant (true)]' >> assembly.cs
echo '[assembly: AssemblyVersion ("1.0.0")]' >> assembly.cs

mcs \
-unsafe \
-debug \
-define:DEBUG \
-out:bin/TinyJson.dll \
-target:library \
-recurse:assembly.cs \
-recurse:src/*.cs \
/doc:bin/TinyJson.xml \
-lib:bin/

rm assembly.cs

################################################################################

mcs \
-unsafe \
-debug \
-define:DEBUG \
-out:bin/TinyJson.Test.dll \
-target:library \
-reference:TinyJson.dll \
-reference:nunit.framework.dll \
-recurse:test/*.cs \
-lib:bin/
