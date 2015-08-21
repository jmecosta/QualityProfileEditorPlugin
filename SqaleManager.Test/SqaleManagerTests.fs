// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqaleManagerTests.fs" company="Copyright © 2014 Tekla Corporation. Tekla is a Trimble Company">
//     Copyright (C) 2013 [Jorge Costa, Jorge.Costa@tekla.com]
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
// You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// --------------------------------------------------------------------------------------------------------------------
namespace SqaleManager.Test

open NUnit.Framework
open FsUnit
open SqaleManager
open Foq
open FSharp.Data
open System.Xml.Linq
open System.IO
open VSSonarPlugins
open VSSonarPlugins.Types


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
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let def = manager.GetDefaultSqaleModel()
        def.GetCharacteristics().Length |> should equal 8
        def.GetProfile().GetAllRules().Count |> should equal 0

    [<Test>]
    member test.``Should Load Profile into Model With New Format`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let def = manager.GetDefaultSqaleModel()
        manager.AddAProfileFromFileToSqaleModel("intel", def, "samples/intel-profile.xml")
        let rules = def.GetProfile().GetAllRules()
        rules.Count |> should equal 22
        rules.[2].Key |> should equal "intel:intelXe.CrossThreadStackAccess"
        rules.[2].Name |> should equal "Cross-thread Stack Access"
        rules.[2].Repo |> should equal "intel"
        rules.[2].Category |> should equal Category.RELIABILITY
        rules.[2].ConfigKey |> should equal "intelXe.CrossThreadStackAccess@INTEL"
        rules.[2].Description |> should equal "Occurs when a thread accesses a different thread's stack."

    [<Test>]
    member test.``Should Load Profile into Model With Old Format`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let def = manager.GetDefaultSqaleModel()
        manager.AddAProfileFromFileToSqaleModel("cppcheck", def, "samples/cppcheck.xml")
        def.GetProfile().GetAllRules().Count |> should equal 305

    [<Test>]
    member test.``Should Load Model From CSharp Xml File`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let model = manager.ParseSqaleModelFromXmlFile("samples/CSharpSqaleModel.xml")
        model.GetCharacteristics().Length |> should equal 8
        let rules = model.GetProfile().GetAllRules()
        rules.Count |> should equal 617
        rules.[0].Category |> should equal Category.PORTABILITY
        rules.[0].Subcategory |> should equal SubCategory.COMPILER_RELATED_PORTABILITY
        rules.[0].Repo |> should equal "common-c++"
        rules.[0].Key |> should equal "common-c++:InsufficientBranchCoverage"
        rules.[0].RemediationFunction |> should equal RemediationFunction.LINEAR
        rules.[0].RemediationFactorTxt |> should equal RemediationUnit.D
        rules.[0].RemediationFactorVal |> should equal 0


    [<Test>]
    member test.``Should Get Correct Number Of Repositories From Model`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let model = manager.ParseSqaleModelFromXmlFile("samples/CSharpSqaleModel.xml")
        manager.GetRepositoriesInModel(model).Length |> should equal 6
    
    [<Test>]
    member test.``Should Create Xml Profile`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let model = manager.ParseSqaleModelFromXmlFile("samples/CSharpSqaleModel.xml")

        manager.WriteProfileToFile(model, "fxcop", rulesinFile)
        let managerNew = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let newModel = managerNew.GetDefaultSqaleModel()
        managerNew.AddAProfileFromFileToSqaleModel("fxcop", newModel, rulesinFile)
        let newrules = model.GetProfile().GetAllRules()
        newrules.Count |> should equal 617

    [<Test>]
    member test.``Should Create Write A Sqale Model To Xml Correctly And Read It`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
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
        let rules = model.GetProfile().GetAllRules()
        rules.Count |> should equal 1
        rules.[0].Key |> should equal "Example:RuleKey"
        rules.[0].ConfigKey |> should equal "RuleKey@Example"
        rules.[0].Category |> should equal Category.MAINTAINABILITY
        rules.[0].Subcategory |> should equal SubCategory.READABILITY
        rules.[0].RemediationFactorVal |> should equal 10
        rules.[0].RemediationFactorTxt |> should equal RemediationUnit.MN
        rules.[0].RemediationFunction |> should equal RemediationFunction.LINEAR
        rules.[0].Severity |> should equal Severity.UNDEFINED
        rules.[0].Repo |> should equal "Example"

    [<Test>]
    member test.``Should Serialize the model Correctly And Read It`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
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
        let rules = model.GetProfile().GetAllRules()
        rules.Count |> should equal 1
        rules.[0].Key |> should equal "RuleKey"
        rules.[0].Name |> should equal "Rule Name"
        rules.[0].ConfigKey |> should equal "Rule Name@Example"
        rules.[0].Description |> should equal "this is description"
        rules.[0].Category |> should equal Category.MAINTAINABILITY
        rules.[0].Subcategory |> should equal SubCategory.READABILITY
        rules.[0].RemediationFactorVal |> should equal 10
        rules.[0].RemediationFactorTxt |> should equal RemediationUnit.MN
        rules.[0].RemediationFunction |> should equal RemediationFunction.LINEAR
        rules.[0].Severity |> should equal Severity.MINOR
        rules.[0].Repo |> should equal "Example"

    [<Test>]
    member test.``Read A ProfileDefinition`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let model = manager.GetDefaultSqaleModel()
        manager.AddAProfileFromFileToSqaleModel("cppcheck", model, "samples/cppcheck.xml")
        manager.CombineWithDefaultProfileDefinition(model, "samples/default-profile.xml")
        let rules = model.GetProfile().GetAllRules()
        rules.Count |> should equal 305
        rules.[0].Severity |> should equal Severity.UNDEFINED
        

    [<Test>]
    member test.``Should Save Model As XML`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
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
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let model = manager.ImportSqaleProjectFromFile("cxx-model-project.xml")
        model.GetCharacteristics().Length |> should equal 8
        manager.WriteSqaleModelToFile(model, "cxx-model.xml")

    //[<Test>]
    member test.``Get C++ Profile`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let model = manager.GetDefaultSqaleModel()
        manager.AddProfileDefinitionFromServerToModel(model, "c++", "DefaultTeklaC++", new ConnectionConfiguration("http://sonar", "jocs1", "jocs1", 4.5))
        manager.SaveSqaleModelAsXmlProject(model, "cxx-model-project-updated.xml")
        ()

    //[<Test>]
    member test.``Read A Project and Merge Info From Another Project`` () = 
        let manager = new SqaleManager(Mock<ISonarRestService>().Create(), Mock<ISonarConfiguration>().Create())
        let model = manager.ImportSqaleProjectFromFile("cxx-model-project.xml")
        let modelToMerge = manager.ImportSqaleProjectFromFile("cppcheck-model-project.xml")
        manager.MergeSqaleDataModels(model, modelToMerge)
        manager.WriteSqaleModelToFile(model, "cxx-model-combined.xml")
        manager.SaveSqaleModelAsXmlProject(model, "cxx-model-project-combined.xml")
        ()




