#include "stdafx.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly:AssemblyTitleAttribute("BAPS Presenter")];
[assembly:AssemblyDescriptionAttribute("Broadcasting and Presenting Suite - Presenter Interface")];
[assembly:AssemblyConfigurationAttribute("Private")];
[assembly:AssemblyCompanyAttribute("Uni-Software")];
[assembly:AssemblyProductAttribute("BAPS Presenter - User Interface")];
[assembly:AssemblyCopyrightAttribute("This software remains the property of Matthew Fortune. All rights reserved.\n"
									 "You are not permitted to install, copy, or use this software without explicit "
									 "consent from the copyright holder")];
[assembly:AssemblyTrademarkAttribute("Uni-Software")];
[assembly:AssemblyCultureAttribute("")];
[assembly:AssemblyInformationalVersionAttribute("2.4.0.0")];

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the value or you can default the Revision and Build Numbers
// by using the '*' as shown below:

[assembly:AssemblyVersionAttribute("2.4.*")];

[assembly:ComVisible(false)];

[assembly:CLSCompliantAttribute(true)];

[assembly:SecurityPermission(SecurityAction::RequestMinimum, UnmanagedCode = true)];
