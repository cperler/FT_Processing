using System.Reflection;
using System.Runtime.InteropServices;
using FT.Input.Data;
using FT.Output.Logging;
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("FT")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("FT")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("9d705faf-9405-4ea9-83fc-a3a032b74c43")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: TransactionAspect(AttributeTargetAssemblies = "FT", AttributeTargetTypes = "*Transaction")]
#if DEBUG
[assembly: LoggingAspect(AttributeTargetAssemblies = "FT", AttributeTargetTypes = "*", AttributePriority = 1)]
[assembly: LoggingAspect(AttributeTargetAssemblies = "FT", AttributeExclude = true, AttributeTargetTypes = "*Aspect*", AttributePriority = 2)]
[assembly: LoggingAspect(AttributeTargetAssemblies = "FT", AttributeExclude = true, AttributeTargetTypes = "*Program*", AttributePriority = 2)]
[assembly: LoggingAspect(AttributeTargetAssemblies = "FT", AttributeExclude = true, AttributeTargetMembers = "*get*", AttributePriority = 2)]
[assembly: LoggingAspect(AttributeTargetAssemblies = "FT", AttributeExclude = true, AttributeTargetMembers = "*set*", AttributePriority = 2)]
[assembly: LoggingAspect(AttributeTargetAssemblies = "FT", AttributeExclude = true, AttributeTargetMembers = "*add*", AttributePriority = 2)]
[assembly: LoggingAspect(AttributeTargetAssemblies = "FT", AttributeExclude = true, AttributeTargetMembers = "*remove*", AttributePriority = 2)]
#endif