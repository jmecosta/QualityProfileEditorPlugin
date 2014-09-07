namespace SqaleManager.Test

open NUnit.Framework
open FsUnit
open SqaleManager
open Foq
open FSharp.Data
open System.Xml.Linq
open System.IO
open ExtensionTypes


type RootConfigurationPropsChecksTests() = 
    let rulesinFile = "fxcop-profile.xml"

    [<SetUp>]
    member test.``SetUp`` () = 
        if File.Exists(rulesinFile) then
            File.Delete(rulesinFile)

    [<TearDown>]
    member test.``tearDown`` () = 
        if File.Exists(rulesinFile) then
            File.Delete(rulesinFile)
            
    [<Test>]
    member test.``It Creates a Default Model`` () = 
        let manager = new SqaleManager()
        let def = manager.GetDefaultSqaleModel()
        def.GetCharacteristics().Length |> should equal 8
        def.GetProfile().Rules.Count |> should equal 0

    [<Test>]
    member test.``Should Load Profile into Model With New Format`` () = 
        let manager = new SqaleManager()
        let def = manager.GetDefaultSqaleModel()
        manager.AddAProfileFromFileToSqaleModel("intel", def, "samples/intel-profile.xml")
        def.GetProfile().Rules.Count |> should equal 22
        def.GetProfile().Rules.[2].Key |> should equal "intel:intelXe.CrossThreadStackAccess"
        def.GetProfile().Rules.[2].Name |> should equal "Cross-thread Stack Access"
        def.GetProfile().Rules.[2].Repo |> should equal "intel"
        def.GetProfile().Rules.[2].Category |> should equal Category.RELIABILITY
        def.GetProfile().Rules.[2].ConfigKey |> should equal "intelXe.CrossThreadStackAccess@INTEL"
        def.GetProfile().Rules.[2].Description |> should equal "Occurs when a thread accesses a different thread's stack."

    [<Test>]
    member test.``Should Load Profile into Model With Old Format`` () = 
        let manager = new SqaleManager()
        let def = manager.GetDefaultSqaleModel()
        manager.AddAProfileFromFileToSqaleModel("cppcheck", def, "samples/cppcheck.xml")
        def.GetProfile().Rules.Count |> should equal 305

    [<Test>]
    member test.``Should Load Model From CSharp Xml File`` () = 
        let manager = new SqaleManager()
        let model = manager.ParseSqaleModelFromXmlFile("samples/CSharpSqaleModel.xml")
        model.GetCharacteristics().Length |> should equal 8
        model.GetProfile().Rules.Count |> should equal 617
        model.GetProfile().Rules.[0].Category |> should equal Category.PORTABILITY
        model.GetProfile().Rules.[0].Subcategory |> should equal SubCategory.COMPILER_RELATED_PORTABILITY
        model.GetProfile().Rules.[0].Repo |> should equal "common-c++"
        model.GetProfile().Rules.[0].Key |> should equal "common-c++:InsufficientBranchCoverage"
        model.GetProfile().Rules.[0].RemediationFunction |> should equal RemediationFunction.LINEAR
        model.GetProfile().Rules.[0].RemediationFactorTxt |> should equal RemediationUnit.D
        model.GetProfile().Rules.[0].RemediationFactorVal |> should equal 0


    [<Test>]
    member test.``Should Get Correct Number Of Repositories From Model`` () = 
        let manager = new SqaleManager()
        let model = manager.ParseSqaleModelFromXmlFile("samples/CSharpSqaleModel.xml")
        manager.GetRepositoriesInModel(model).Length |> should equal 6
    
    [<Test>]
    member test.``Should Create Xml Profile`` () = 
        let manager = new SqaleManager()
        let model = manager.ParseSqaleModelFromXmlFile("samples/CSharpSqaleModel.xml")

        manager.WriteProfileToFile(model, "fxcop", rulesinFile)
        let managerNew = new SqaleManager()
        let newModel = managerNew.GetDefaultSqaleModel()
        managerNew.AddAProfileFromFileToSqaleModel("fxcop", newModel, rulesinFile)
        newModel.GetProfile().Rules.Count |> should equal 240
        newModel.GetProfile().Rules.[2].Key |> should equal "fxcop:EnumStorageShouldBeInt32"
        newModel.GetProfile().Rules.[2].Name |> should equal ""
        newModel.GetProfile().Rules.[2].Repo |> should equal "fxcop"
        newModel.GetProfile().Rules.[2].Category |> should equal Category.PORTABILITY
        newModel.GetProfile().Rules.[2].ConfigKey |> should equal "EnumStorageShouldBeInt32@fxcop"
        newModel.GetProfile().Rules.[2].Description |> should equal ""

    [<Test>]
    member test.``Should Create Write A Sqale Model To Xml Correctly And Read It`` () = 
        let manager = new SqaleManager()
        let def = manager.GetDefaultSqaleModel()

        let rule = new Rule()
        rule.Key <- "Example:RuleKey"
        rule.Name <- "Rule Name"
        rule.ConfigKey <- "Rule Name@Example"
        rule.Description <- "this is description"
        rule.Category <- Category.MAINTAINABILITY
        rule.Subcategory <- SubCategory.READABILITY
        rule.RemediationFactorVal <- 10
        rule.RemediationFactorTxt <- RemediationUnit.MN
        rule.RemediationFunction <- RemediationFunction.LINEAR
        rule.Severity <- Severity.MINOR
        rule.Repo <- "Example"
        
        def.CreateRuleInProfile(rule) |> ignore
        manager.WriteSqaleModelToFile(def, rulesinFile)

        let model = manager.ParseSqaleModelFromXmlFile(rulesinFile)
        model.GetProfile().Rules.Count |> should equal 1
        model.GetProfile().Rules.[0].Key |> should equal "Example:RuleKey"
        model.GetProfile().Rules.[0].ConfigKey |> should equal "RuleKey@Example"
        model.GetProfile().Rules.[0].Category |> should equal Category.MAINTAINABILITY
        model.GetProfile().Rules.[0].Subcategory |> should equal SubCategory.READABILITY
        model.GetProfile().Rules.[0].RemediationFactorVal |> should equal 10
        model.GetProfile().Rules.[0].RemediationFactorTxt |> should equal RemediationUnit.MN
        model.GetProfile().Rules.[0].RemediationFunction |> should equal RemediationFunction.LINEAR
        model.GetProfile().Rules.[0].Severity |> should equal Severity.UNDEFINED
        model.GetProfile().Rules.[0].Repo |> should equal "Example"

    [<Test>]
    member test.``Should Serialize the model Correctly And Read It`` () = 
        let manager = new SqaleManager()
        let def = manager.GetDefaultSqaleModel()

        let rule = new Rule()
        rule.Key <- "RuleKey"
        rule.Name <- "Rule Name"
        rule.ConfigKey <- "Rule Name@Example"
        rule.Description <- "this is description"
        rule.Category <- Category.MAINTAINABILITY
        rule.Subcategory <- SubCategory.READABILITY
        rule.RemediationFactorVal <- 10
        rule.RemediationFactorTxt <- RemediationUnit.MN
        rule.RemediationFunction <- RemediationFunction.LINEAR
        rule.Severity <- Severity.MINOR
        rule.Repo <- "Example"
        
        def.CreateRuleInProfile(rule) |> ignore
        manager.SaveSqaleModelToDsk(def, rulesinFile) |> ignore

        let model = manager.LoadSqaleModelFromDsk(rulesinFile)
        model.GetProfile().Rules.Count |> should equal 1
        model.GetProfile().Rules.[0].Key |> should equal "RuleKey"
        model.GetProfile().Rules.[0].Name |> should equal "Rule Name"
        model.GetProfile().Rules.[0].ConfigKey |> should equal "Rule Name@Example"
        model.GetProfile().Rules.[0].Description |> should equal "this is description"
        model.GetProfile().Rules.[0].Category |> should equal Category.MAINTAINABILITY
        model.GetProfile().Rules.[0].Subcategory |> should equal SubCategory.READABILITY
        model.GetProfile().Rules.[0].RemediationFactorVal |> should equal 10
        model.GetProfile().Rules.[0].RemediationFactorTxt |> should equal RemediationUnit.MN
        model.GetProfile().Rules.[0].RemediationFunction |> should equal RemediationFunction.LINEAR
        model.GetProfile().Rules.[0].Severity |> should equal Severity.MINOR
        model.GetProfile().Rules.[0].Repo |> should equal "Example"

    [<Test>]
    member test.``Read A ProfileDefinition`` () = 
        let manager = new SqaleManager()
        let model = manager.GetDefaultSqaleModel()
        manager.AddAProfileFromFileToSqaleModel("cppcheck", model, "samples/cppcheck.xml")
        manager.CombineWithDefaultProfileDefinition(model, "samples/default-profile.xml")

        model.GetProfile().Rules.Count |> should equal 305
        model.GetProfile().Rules.[0].Severity |> should equal Severity.MINOR
        

    [<Test>]
    member test.``Should Save Model As XML`` () = 
        let manager = new SqaleManager()
        let model = manager.GetDefaultSqaleModel()
        manager.AddAProfileFromFileToSqaleModel("cppcheck", model, "samples/cppcheck.xml")
        manager.AddAProfileFromFileToSqaleModel("pclint", model, "samples/pclint.xml")
        manager.AddAProfileFromFileToSqaleModel("rats", model, "samples/rats.xml")
        manager.AddAProfileFromFileToSqaleModel("vera++", model, "samples/vera++.xml")
        manager.AddAProfileFromFileToSqaleModel("valgrind", model, "samples/valgrind.xml")
        manager.AddAProfileFromFileToSqaleModel("compiler", model, "samples/compiler.xml")
        manager.CombineWithDefaultProfileDefinition(model, "samples/default-profile.xml")

        manager.SaveSqaleModelAsXmlProject(model, "cxx-model-project.xml")
        manager.WriteSqaleModelToFile(model, "cxx-model.xml")

    //[<Test>]
    member test.``Read Cxx Project`` () = 
        let manager = new SqaleManager()
        let model = manager.ImportSqaleProjectFromFile("cxx-model-project.xml")
        model.GetCharacteristics().Length |> should equal 8
        manager.WriteSqaleModelToFile(model, "cxx-model.xml")

    //[<Test>]
    member test.``Get C++ Profile`` () = 
        let manager = new SqaleManager()
        let model = manager.GetDefaultSqaleModel()
        manager.AddProfileDefinitionFromServerToModel(model, "c++", "DefaultTeklaC++", new ExtensionTypes.ConnectionConfiguration("http://sonar", "jocs1", "jocs1"))
        manager.SaveSqaleModelAsXmlProject(model, "cxx-model-project-updated.xml")
        ()

    //[<Test>]
    member test.``Read A Project and Merge Info From Another Project`` () = 
        let manager = new SqaleManager()
        let model = manager.ImportSqaleProjectFromFile("cxx-model-project.xml")
        let modelToMerge = manager.ImportSqaleProjectFromFile("cppcheck-model-project.xml")
        manager.MergeSqaleDataModels(model, modelToMerge)
        manager.WriteSqaleModelToFile(model, "cxx-model-combined.xml")
        manager.SaveSqaleModelAsXmlProject(model, "cxx-model-project-combined.xml")
        ()




