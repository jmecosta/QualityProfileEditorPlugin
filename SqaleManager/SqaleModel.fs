namespace SqaleManager

open System
open System.IO
open ExtensionTypes
open SonarRestService

type SqaleModel() =
    let profile : Profile = new Profile()
    let mutable characteristics : Characteristic list = []

    member val profileName = "" with get, set
    member val language = "" with get, set
    member x.GetCharacteristics() = characteristics
    member x.GetProfile() = profile

    member x.IsCharPresent(key : Category) =
        let CompareKey(char : Characteristic) =
            char.Key.Equals(key)
                                 
        (List.toArray characteristics) |> Array.exists (fun elem ->  CompareKey(elem))

    member x.GetChar(key : Category) =
        let CompareKey(char : Characteristic) =
            char.Key.Equals(key)
                                 
        (List.toArray characteristics) |> Array.find (fun elem ->  CompareKey(elem))

    member x.CreateAChar(key : Category, name : string) =
        if not(x.IsCharPresent(key)) then
            let newChar = new Characteristic(key, name)
            characteristics <- characteristics @ [newChar]
            newChar
        else
            x.GetChar(key)
    
    member x.CreateRuleInProfile(rule : Rule) =
        profile.CreateRule(rule)
      
    member x.LoadSqaleModelFromString(str : string) =
        let sqale = SqaleModelType.Parse(str)

        for chk in sqale.GetChcs() do
            let char = x.CreateAChar(EnumHelper.asEnum<Category>(chk.Key).Value, chk.Name)            

            for subchk in chk.GetChcs() do
                char.CreateSubChar(EnumHelper.asEnum<SubCategory>(subchk.Key).Value, subchk.Name)

                for chc in subchk.GetChcs() do
                    let rule = new Rule()
                    rule.Repo <- chc.RuleRepo
                    rule.Key <- chc.RuleKey
                    rule.ConfigKey <- chc.RuleKey + "@" + chc.RuleRepo
                    rule.EnableSetDeafaults <- false

                    for prop in chc.GetProps() do
                        if prop.Key.Equals("remediationFactor") then
                            rule.Category <- (EnumHelper.asEnum<Category>(chk.Key)).Value
                            rule.Subcategory <- (EnumHelper.asEnum<SubCategory>(subchk.Key)).Value
                            try
                                rule.RemediationFactorVal <- Int32.Parse(prop.Val)
                            with
                            | ex -> ()
                            try
                                rule.RemediationFactorTxt <- (EnumHelper.asEnum<RemediationUnit>(prop.Txt)).Value
                            with
                            | ex -> ()

                        if prop.Key.Equals("remediationFunction") then
                            try
                                rule.RemediationFunction <- (EnumHelper.asEnum<RemediationFunction>(prop.Txt)).Value
                            with
                            | ex -> ()

                    rule.EnableSetDeafaults <- true
                    profile.CreateRule(rule) |> ignore

    member x.LoadSqaleModelFromFile(path : string) =
        let sqale = SqaleModelType.Parse(File.ReadAllText(path))

        for chk in sqale.GetChcs() do
            let char = x.CreateAChar(EnumHelper.asEnum<Category>(chk.Key).Value, chk.Name)            

            for subchk in chk.GetChcs() do
                char.CreateSubChar(EnumHelper.asEnum<SubCategory>(subchk.Key).Value, subchk.Name) |> ignore

                for chc in subchk.GetChcs() do
                    let rule = new Rule()
                    rule.Repo <- chc.RuleRepo
                    rule.Key <- chc.RuleKey
                    rule.ConfigKey <- chc.RuleKey + "@" + chc.RuleRepo
                    rule.EnableSetDeafaults <- false

                    for prop in chc.GetProps() do
                        if prop.Key.Equals("remediationFactor") then
                            rule.Category <- (EnumHelper.asEnum<Category>(chk.Key)).Value
                            rule.Subcategory <- (EnumHelper.asEnum<SubCategory>(subchk.Key)).Value
                            try
                                rule.RemediationFactorVal <- Int32.Parse(prop.Val)
                            with
                            | ex -> ()
                            try
                                rule.RemediationFactorTxt <- (EnumHelper.asEnum<RemediationUnit>(prop.Txt)).Value
                            with
                            | ex -> ()

                        if prop.Key.Equals("remediationFunction") then
                            try
                                rule.RemediationFunction <- (EnumHelper.asEnum<RemediationFunction>(prop.Txt)).Value
                            with
                            | ex -> ()

                        if prop.Key.Equals("offset") then
                            try
                                rule.RemediationOffsetVal <- Int32.Parse(prop.Val)
                            with
                            | ex -> ()
                            try
                                rule.RemediationOffsetTxt <- (EnumHelper.asEnum<RemediationUnit>(prop.Txt)).Value
                            with
                            | ex -> ()

                    rule.EnableSetDeafaults <- true
                    profile.CreateRule(rule) |> ignore


    

    





