// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open SqaleManager
open System.IO

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let manager = new SqaleManager()
    
    if argv.Length = 1 then
        let model = manager.ImportSqaleProjectFromFile(argv.[0])
        manager.WriteSqaleModelToFile(model, "cxx-model.xml")
    else
        let model = manager.ImportSqaleProjectFromFile(argv.[0])
        let modelToMerge = manager.ImportSqaleProjectFromFile(argv.[1])
        manager.MergeSqaleDataModels(model, modelToMerge)
        manager.WriteSqaleModelToFile(model, "cxx-model-combined.xml")
        manager.SaveSqaleModelAsXmlProject(model, "cxx-model-project-combined.xml")

    0 // return an integer exit code
