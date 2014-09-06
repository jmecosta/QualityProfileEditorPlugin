namespace SqaleManager

open FSharp.Data
open System.Xml.Linq
open System.IO
open System.Reflection
open System.ComponentModel
open ExtensionTypes

type ImportLogEntry() = 
    member val line = -1 with get, set
    member val message = "" with get, set
    member val exceptionMessage = "" with get, set
        
