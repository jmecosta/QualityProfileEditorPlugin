namespace SqaleManager

open System
open System.ComponentModel 

type RemediationFunction =
   | [<Description("linear")>] LINEAR = 0
   | [<Description("linear_offset")>] LINEAR_OFFSET = 1
   | [<Description("constant_issue")>] CONSTANT_ISSUE = 2
   | [<Description("UNDEFINED")>] UNDEFINED = 3

type Severity =
   | [<Description("BLOCKER")>] BLOCKER = 0
   | [<Description("CRITICAL")>] CRITICAL = 1
   | [<Description("MAJOR")>] MAJOR = 2
   | [<Description("MINOR")>] MINOR = 3
   | [<Description("INFO")>] INFO = 4
   | [<Description("UNDEFINED")>] UNDEFINED = 5



type RemediationUnit =
   | [<Description("mn")>] MN = 0
   | [<Description("h")>] H = 1
   | [<Description("d")>] D = 2
   | [<Description("UNDEFINED")>] UNDEFINED = 3

type Category =
   | [<Description("PORTABILITY")>] PORTABILITY = 0
   | [<Description("MAINTAINABILITY")>] MAINTAINABILITY = 1
   | [<Description("SECURITY")>] SECURITY = 2
   | [<Description("EFFICIENCY")>] EFFICIENCY = 3
   | [<Description("CHANGEABILITY")>] CHANGEABILITY = 4
   | [<Description("RELIABILITY")>] RELIABILITY = 5
   | [<Description("TESTABILITY")>] TESTABILITY = 6
   | [<Description("REUSABILITY")>] REUSABILITY = 7
   | [<Description("UNDEFINED")>] UNDEFINED = 8

type SubCategory =
   | [<Description("MODULARITY")>] MODULARITY = 0
   | [<Description("TRANSPORTABILITY")>] TRANSPORTABILITY = 1
   | [<Description("UNIT_TESTABILITY")>] UNIT_TESTABILITY = 2
   | [<Description("UNIT_TESTS")>] UNIT_TESTS = 3
   | [<Description("SYNCHRONIZATION_RELIABILITY")>] SYNCHRONIZATION_RELIABILITY = 4
   | [<Description("INSTRUCTION_RELIABILITY")>] INSTRUCTION_RELIABILITY = 5
   | [<Description("FAULT_TOLERANCE")>] FAULT_TOLERANCE = 6
   | [<Description("EXCEPTION_HANDLING")>] EXCEPTION_HANDLING = 7
   | [<Description("DATA_RELIABILITY")>] DATA_RELIABILITY = 8
   | [<Description("ARCHITECTURE_RELIABILITY")>] ARCHITECTURE_RELIABILITY = 9
   | [<Description("LOGIC_CHANGEABILITY")>] LOGIC_CHANGEABILITY = 10
   | [<Description("DATA_CHANGEABILITY")>] DATA_CHANGEABILITY = 11
   | [<Description("ARCHITECTURE_CHANGEABILITY")>] ARCHITECTURE_CHANGEABILITY = 12
   | [<Description("CPU_EFFICIENCY")>] CPU_EFFICIENCY = 13
   | [<Description("MEMORY_EFFICIENCY")>] MEMORY_EFFICIENCY = 14
   | [<Description("SECURITY_FEATURES")>] SECURITY_FEATURES = 15
   | [<Description("INPUT_VALIDATION_AND_REPRESENTATION")>] INPUT_VALIDATION_AND_REPRESENTATION = 16
   | [<Description("ERRORS")>] ERRORS = 17
   | [<Description("API_ABUSE")>] API_ABUSE = 18
   | [<Description("UNDERSTANDABILITY")>] UNDERSTANDABILITY = 19
   | [<Description("READABILITY")>] READABILITY = 20
   | [<Description("TIME_ZONE_RELATED_PORTABILITY")>] TIME_ZONE_RELATED_PORTABILITY = 21
   | [<Description("SOFTWARE_RELATED_PORTABILITY")>] SOFTWARE_RELATED_PORTABILITY = 22
   | [<Description("OS_RELATED_PORTABILITY")>] OS_RELATED_PORTABILITY = 23
   | [<Description("LANGUAGE_RELATED_PORTABILITY")>] LANGUAGE_RELATED_PORTABILITY = 24
   | [<Description("HARDWARE_RELATED_PORTABILITY")>] HARDWARE_RELATED_PORTABILITY = 25
   | [<Description("COMPILER_RELATED_PORTABILITY")>] COMPILER_RELATED_PORTABILITY = 26
   | [<Description("LOGIC_RELIABILITY")>] LOGIC_RELIABILITY = 27
   | [<Description("INTEGRATION_TESTABILITY")>] INTEGRATION_TESTABILITY = 28
   | [<Description("UNDEFINED")>] UNDEFINED = 29

module EnumHelper = 
    let asEnum<'T 
      when 'T : enum<int>
      and 'T : struct
      and 'T :> ValueType
      and 'T : (new : unit -> 'T)> text =
      match Enum.TryParse<'T>(text, true) with
      | true, value -> Some value
      | _ -> None

    let getEnumDescription (value : Enum) =
       let typ = value.GetType()
       let name = value.ToString();
       let attrs = typ.GetField(name).GetCustomAttributes(typedefof<DescriptionAttribute>, false)
       if (attrs.Length > 0) then (attrs.[0] :?> DescriptionAttribute).Description
       else name

