namespace SqaleManager

open System
open System.IO
open System.Reflection
open System.Runtime.Serialization.Formatters.Binary
open System.Security
open System.Web
open System.Xml
open System.Text
open System.Xml.Linq
open SonarRestService
open System.ComponentModel 

type SqaleManager() =
    let content = new StringBuilder()
    let importLog = new System.Collections.Generic.List<ImportLogEntry>()
    let SqaleDefaultModelDefinitionPath = "defaultmodel.xml"

    let EncodeStringAsXml(str : string) = SecurityElement.Escape(str).Replace("‘", "&#8216;").Replace("’", "&#8217;").Replace("–", "&#8211;").Replace("—", "&#8212;").Replace("„", "&#8222;").Replace("‟", "&#8223;")

    member x.GetImportLog() = importLog
        
    member x.GetDefaultSqaleModel() = 
        let assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)
        let asm = Assembly.GetExecutingAssembly()
        use stream = asm.GetManifestResourceStream("defaultmodel.xml")
        let xmldoc = XDocument.Load(stream).ToString()
        let model = new SqaleModel()
        model.LoadSqaleModelFromString(xmldoc)
        model

    member x.CreateModelFromRules(rules : System.Collections.Generic.IEnumerable<Rule>) = 
        let model = x.GetDefaultSqaleModel()

        for rule in rules do
            model.GetProfile().AddRule(rule)

        model

    member x.ParseSqaleModelFromXmlFile(file : string) =
        let model = new SqaleModel()
        model.LoadSqaleModelFromFile(file)
        model

    member x.GetRepositoriesInModel(model : SqaleModel) =
        let mutable repos : string list = []

        for rule in model.GetProfile().Rules do
            try
                Array.find (fun elem -> elem.Equals(rule.repo)) (List.toArray repos) |> ignore
            with
            | :? System.Collections.Generic.KeyNotFoundException -> repos <- repos @ [rule.repo]

        repos

    member x.WriteProfileToFile(model : SqaleModel, repo : string, fileName : string) =
        let mutable rules : Rule list = []

        for ruleinprofile in model.GetProfile().Rules do
            if ruleinprofile.repo.Equals(repo) then
                rules <- rules @ [ruleinprofile]

        x.CreateQualityProfile(fileName, rules)

    member x.CreateQualityProfile(file : string, rules : Rule list) =

        let addLine (line:string) =                  
            use wr = new StreamWriter(file, true)
            wr.WriteLine(line)

        let writeXmlRule(rule : Rule) =
            addLine(sprintf """    <rule key="%s">""" rule.key)
            addLine(sprintf """        <name><![CDATA[%s]]></name>""" rule.name)
            addLine(sprintf """        <configKey><![CDATA[%s]]></configKey>""" rule.configKey)
            addLine(sprintf """        <category name="%s" />""" (EnumHelper.getEnumDescription(rule.category)))
            addLine(sprintf """        <description><![CDATA[  %s  ]]></description>""" rule.description)
            addLine(sprintf """    </rule>""")

        addLine(sprintf """<?xml version="1.0" encoding="ASCII"?>""")
        addLine(sprintf """<rules>""")

        for rule in rules do
            writeXmlRule rule

        addLine(sprintf """</rules>""")       

    member x.AddAProfileFromFileToSqaleModel(repo : string, model : SqaleModel, fileToRead : string) =
        let addLine (line:string, fileToWrite:string) =                  
            use wr = new StreamWriter(fileToWrite, true)
            wr.WriteLine(line)

        try
            let profile = RulesXmlOldType.Parse(File.ReadAllText(fileToRead))

            for rule in profile.GetRules() do
                let createdRule = new Rule()
                createdRule.enableSetDeafaults <- false
                createdRule.repo <- repo
                try
                    createdRule.configKey <- rule.Configkey
                with
                | ex -> ()
                try
                    createdRule.category <- (EnumHelper.asEnum<Category>(rule.Category.Value)).Value                   
                with
                | ex -> ()
                createdRule.description <- rule.Description
                createdRule.name <- rule.Name
                createdRule.enableSetDeafaults <- true
                createdRule.key <- rule.Key
                model.CreateRuleInProfile(createdRule) |> ignore
        with
        | ex ->
            let profile = RulesXmlNewType.Parse(File.ReadAllText(fileToRead))

            for rule in profile.GetRules() do
                let createdRule = new Rule()
                createdRule.enableSetDeafaults <- false
                createdRule.repo <- repo
                createdRule.configKey <- rule.ConfigKey.Replace("![CDATA[", "").Replace("]]", "").Trim()
                try
                    createdRule.category <- (EnumHelper.asEnum<Category>(rule.Category.Name)).Value
                with
                | ex -> ()
                createdRule.description <- rule.Description.Replace("![CDATA[", "").Replace("]]", "").Trim()
                createdRule.name <- rule.Name.Replace("![CDATA[", "").Replace("]]", "").Trim()
                createdRule.key <- rule.Key
                createdRule.enableSetDeafaults <- true
                model.CreateRuleInProfile(createdRule) |> ignore

    member x.WriteCharacteristicsFromScaleModelToFile(model : SqaleModel, fileToWrite : string) =
        content.Clear() |> ignore

        let addLine (line:string, fileToWrite:string) =                  
            content.AppendLine(line) |> ignore

        if File.Exists(fileToWrite) then
            File.Delete(fileToWrite)

        addLine("""<?xml version="1.0"?>""", fileToWrite)
        addLine("""<sqale>""", fileToWrite)
        for char in model.GetCharacteristics() do
            addLine(sprintf """    <chc>""", fileToWrite)
            addLine(sprintf """    <key>%s</key>""" (char.key.ToString()), fileToWrite)
            addLine(sprintf """    <name>%s</name>""" char.name, fileToWrite)
            for subchar in char.GetSubChars do
                addLine(sprintf """        <chc>""", fileToWrite)
                addLine(sprintf """            <key>%s</key>""" (subchar.key.ToString()), fileToWrite)
                addLine(sprintf """            <name>%s</name>""" subchar.name, fileToWrite)
                addLine(sprintf """        </chc>""", fileToWrite)

            addLine(sprintf """    </chc>""", fileToWrite)

        addLine("""</sqale>""", fileToWrite)          
        addLine("""""", fileToWrite)

        File.WriteAllText(fileToWrite, content.ToString())
    
    member x.WriteSqaleModelToFile(model : SqaleModel, fileToWrite : string) =

        content.Clear() |> ignore

        let addLine (line:string, fileToWrite:string) =  
            content.AppendLine(line) |> ignore

        if File.Exists(fileToWrite) then
            File.Delete(fileToWrite)

        let writePropToFile(key : string, value : string, txt : string, file : string) = 
            addLine(sprintf """                <prop>""", file)
            addLine(sprintf """                    <key>%s</key>""" key, file)
            if not(String.IsNullOrEmpty(value)) then
                addLine(sprintf """                    <val>%s</val>""" value, file)
            if not(String.IsNullOrEmpty(txt)) then
                addLine(sprintf """                    <txt>%s</txt>""" txt, file)

            addLine(sprintf """                </prop>""", file)

        let writeRulesChcToFile (charName : Category, subcharName : SubCategory, file : string) = 
            for rule in model.GetProfile().Rules do
                if rule.category.Equals(charName) && rule.subcategory.Equals(subcharName) then
                    addLine(sprintf """            <chc>""", fileToWrite)
                    addLine(sprintf """                <rule-repo>%s</rule-repo>""" rule.repo, file)
                    addLine(sprintf """                <rule-key>%s</rule-key>""" rule.key, file)
                    writePropToFile("remediationFunction", "", EnumHelper.getEnumDescription(rule.remediationFunction), file)
                    writePropToFile("remediationFactor", rule.remediationFactorVal.Replace(',', '.'), EnumHelper.getEnumDescription(rule.remediationFactorTxt), file)

                    if not(rule.remediationFunction.Equals(RemediationFunction.CONSTANT_ISSUE)) then
                        if String.IsNullOrEmpty(rule.remediationOffsetVal) then                 
                            writePropToFile("offset", "0.0", "d", file)
                        else
                            writePropToFile("offset", rule.remediationOffsetVal.Replace(',', '.'), EnumHelper.getEnumDescription(rule.remediationOffsetTxt), file)

                    addLine(sprintf """            </chc>""", fileToWrite)
           
        addLine("""<?xml version="1.0"?>""", fileToWrite)
        addLine("""<sqale>""", fileToWrite)
        for char in model.GetCharacteristics() do
            addLine(sprintf """    <chc>""", fileToWrite)
            addLine(sprintf """    <key>%s</key>""" (char.key.ToString()), fileToWrite)
            addLine(sprintf """    <name>%s</name>""" char.name, fileToWrite)
            for subchar in char.GetSubChars do
                addLine(sprintf """        <chc>""", fileToWrite)
                addLine(sprintf """            <key>%s</key>""" (subchar.key.ToString()), fileToWrite)
                addLine(sprintf """            <name>%s</name>""" subchar.name, fileToWrite)
                writeRulesChcToFile(char.key, subchar.key, fileToWrite)
                addLine(sprintf """        </chc>""", fileToWrite)
                

            addLine(sprintf """    </chc>""", fileToWrite)

        addLine("""</sqale>""", fileToWrite)          
        addLine("""""", fileToWrite)      
                
        File.WriteAllText(fileToWrite, content.ToString())               

    member x.SaveSqaleModelToDsk(model : SqaleModel, fileToWrite : string) =
        let WriteToBytes obj = 
            let formatter = new BinaryFormatter()
            use writeStream = new StreamWriter(fileToWrite, true)
            formatter.Serialize(writeStream.BaseStream, obj)
            writeStream.Flush
        WriteToBytes model

    member x.AddProfileDefinition(model : SqaleModel, file : string) = 
        let profile = ProfileDefinition.Parse(File.ReadAllText(file))
        model.language <- profile.Language
        model.profileName <- profile.Name
        for rule in profile.Rules.GetRules() do
            if not(model.GetProfile().IsRulePresent(rule.Key)) then
                let ruletoUpdate = new Rule()
                ruletoUpdate.severity <- (Enum.Parse(typeof<Severity>, rule.Priority) :?> Severity)
                ruletoUpdate.repo <- rule.RepositoryKey
                ruletoUpdate.key <- rule.Key
                model.GetProfile().AddRule(ruletoUpdate)
            else
                let ruletoUpdate = model.GetProfile().GetRule(rule.Key)
                ruletoUpdate.severity <- (Enum.Parse(typeof<Severity>, rule.Priority) :?> Severity)
                ruletoUpdate.repo <- rule.RepositoryKey

    member x.CombineWithDefaultProfileDefinition(model : SqaleModel, file : string) = 
        let profile = ProfileDefinition.Parse(File.ReadAllText(file))
        model.language <- profile.Language
        model.profileName <- profile.Name
        for rule in profile.Rules.GetRules() do
            if model.GetProfile().IsRulePresent(rule.Key) then
                let ruletoUpdate = model.GetProfile().GetRule(rule.Key)
                ruletoUpdate.severity <- (Enum.Parse(typeof<Severity>, rule.Priority) :?> Severity)
                ruletoUpdate.repo <- rule.RepositoryKey                
            
    member x.SaveSqaleModelAsXmlProject(model : SqaleModel, fileToWrite : string) =

        content.Clear() |> ignore
                
        let addLine (line:string, fileToWrite:string) =                  
            content.AppendLine(line) |> ignore

        if File.Exists(fileToWrite) then
            File.Delete(fileToWrite)

        addLine(sprintf """<?xml version="1.0" encoding="ASCII"?>""", fileToWrite)
        addLine(sprintf """<sqaleManager xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="cxx-model-project.xsd">""", fileToWrite)
        //for char in model.GetCharacteristics() do
        //    addLine(sprintf """    <characteristic>""", fileToWrite)
        //    addLine(sprintf """        <key>%s</key>""" char.key, fileToWrite)
        //    addLine(sprintf """        <name>%s</name>""" char.name, fileToWrite)            
        //    for subchar in char.GetSubChars do
        //        addLine(sprintf """        <subcaracteristic>""", fileToWrite)
        //        addLine(sprintf """            <key>%s</key>""" subchar.key, fileToWrite)
        //        addLine(sprintf """            <name>%s</name>""" subchar.name, fileToWrite)                
         //       addLine(sprintf """        </subcaracteristic>""", fileToWrite)
         //   addLine(sprintf """    </characteristic>""", fileToWrite)
        
        addLine(sprintf """    <rules>""", fileToWrite)
        for rule in model.GetProfile().Rules do
            addLine(sprintf """    <rule key="%s">""" rule.key, fileToWrite)           
            addLine(sprintf """        <name>%s</name>""" (EncodeStringAsXml(rule.name)), fileToWrite)
            if String.IsNullOrEmpty(EnumHelper.getEnumDescription(rule.subcategory)) then
                addLine(sprintf """        <requirement>undefined</requirement>""", fileToWrite)
            else
                addLine(sprintf """        <requirement>%s</requirement>""" (EnumHelper.getEnumDescription(rule.subcategory)), fileToWrite)
            if String.IsNullOrEmpty(rule.remediationFactorVal) then
                addLine(sprintf """        <remediationFactorVal>0.0</remediationFactorVal>""", fileToWrite)
            else
                addLine(sprintf """        <remediationFactorVal>%s</remediationFactorVal>""" (rule.remediationFactorVal.Replace(',', '.')), fileToWrite)

            if String.IsNullOrEmpty(EnumHelper.getEnumDescription(rule.remediationFactorTxt)) then
                addLine(sprintf """        <remediationFactorUnit>undefined</remediationFactorUnit>""", fileToWrite)
            else 
                addLine(sprintf """        <remediationFactorUnit>%s</remediationFactorUnit>""" (EnumHelper.getEnumDescription(rule.remediationFactorTxt)), fileToWrite)

            if String.IsNullOrEmpty(EnumHelper.getEnumDescription(rule.remediationFunction)) then
                addLine(sprintf """        <remediationFunction>undefined</remediationFunction>""", fileToWrite)
            else
                addLine(sprintf """        <remediationFunction>%s</remediationFunction>""" (EnumHelper.getEnumDescription(rule.remediationFunction)), fileToWrite)

            if String.IsNullOrEmpty(rule.remediationOffsetVal) then
                addLine(sprintf """        <remediationOffsetVal>0.0</remediationOffsetVal>""", fileToWrite)
            else
                addLine(sprintf """        <remediationOffsetVal>%s</remediationOffsetVal>""" (rule.remediationOffsetVal.Replace(',', '.')), fileToWrite)

            if String.IsNullOrEmpty(EnumHelper.getEnumDescription(rule.remediationOffsetTxt)) then
                addLine(sprintf """        <remediationOffsetUnit>undefined</remediationOffsetUnit>""", fileToWrite)
            else
                addLine(sprintf """        <remediationOffsetUnit>%s</remediationOffsetUnit>""" (EnumHelper.getEnumDescription(rule.remediationOffsetTxt)), fileToWrite)

            if String.IsNullOrEmpty(rule.severity.ToString()) then
                addLine(sprintf """        <severity>undefined</severity>""", fileToWrite)
            else
                addLine(sprintf """        <severity>%s</severity>""" (EnumHelper.getEnumDescription(rule.severity)), fileToWrite)

            addLine(sprintf """        <repo>%s</repo>""" rule.repo, fileToWrite)
            addLine(sprintf """        <description>%s</description>""" (EncodeStringAsXml(rule.description).Trim()), fileToWrite)            
            addLine(sprintf """    </rule>""", fileToWrite)
        addLine(sprintf """    </rules>""", fileToWrite)
        addLine(sprintf """</sqaleManager>""", fileToWrite)

        File.WriteAllText(fileToWrite, content.ToString())

    member x.GetCategoryFromSubcategoryKey(model : SqaleModel, requirement : SubCategory) = 
        let chars = model.GetCharacteristics()
        let mutable key = Category.UNDEFINED
                         
        for char in chars do
            if char.IsSubCharPresent(requirement) then
                key <- char.key
        key

    member x.ImportSqaleProjectFromFile(fileToRead : string) =

        let model = x.GetDefaultSqaleModel()

        let dskmodel = CxxProjectDefinition.Parse(File.ReadAllText(fileToRead))

        importLog.Clear()
        for item in dskmodel.Rules.GetRules() do
            let entryLog = new ImportLogEntry()
            let info = item.XElement :> IXmlLineInfo
            entryLog.message <- item.XElement.Value
            if info.HasLineInfo() then
                entryLog.line <- info.LineNumber
            try
                let rule = new Rule()
                rule.enableSetDeafaults <- false
                rule.key <- item.Key
                rule.name <- item.Name
                rule.repo <- item.Repo
                rule.configKey <- item.Name + "@" + item.Repo
                rule.description <- item.Description
                rule.category <- x.GetCategoryFromSubcategoryKey(model, EnumHelper.asEnum<SubCategory>(item.Requirement).Value)
                rule.subcategory <- (EnumHelper.asEnum<SubCategory>(item.Requirement)).Value
                rule.remediationFactorVal <- item.RemediationFactorVal.ToString()
                rule.remediationFactorTxt <- (EnumHelper.asEnum<RemediationUnit>(item.RemediationFactorUnit)).Value
                rule.remediationFunction <- (EnumHelper.asEnum<RemediationFunction>(item.RemediationFunction)).Value
                rule.remediationOffsetTxt <- (EnumHelper.asEnum<RemediationUnit>(item.RemediationOffsetUnit)).Value
                rule.remediationOffsetVal <- item.RemediationOffsetVal.ToString()
                rule.severity <- (EnumHelper.asEnum<Severity>(item.Severity)).Value
                rule.enableSetDeafaults <- true
                model.CreateRuleInProfile(rule) |> ignore
            with
             | ex ->
                entryLog.exceptionMessage <- ex.Message
                importLog.Add(entryLog)
        model                 

    member x.AddProfileDefinitionFromServerToModel(model : SqaleModel, language : string, profile : string, conectionConf : ExtensionTypes.ConnectionConfiguration) =
        let service = SonarRestService(new JsonSonarConnector()) 
        let profile = (service :> ISonarRestService).GetEnabledRulesInProfile(conectionConf , language, profile)
        let rules = (service :> ISonarRestService).GetRules(conectionConf , language)

        for rule in profile.[0].Rules do
            let createdRule = new Rule()
            createdRule.repo <- rule.Repo            
            createdRule.key <- rule.Key
            createdRule.severity <- (Enum.Parse(typeof<Severity>, rule.Severity) :?> Severity)

            for ruledef in rules do
                if ruledef.Key.EndsWith(rule.Key, true, Globalization.CultureInfo.InvariantCulture) then
                    createdRule.description <- ruledef.Description
                    createdRule.name <- ruledef.Name
                    createdRule.configKey <- ruledef.ConfigKey                    

            model.CreateRuleInProfile(createdRule) |> ignore

        ()

    member x.MergeSqaleDataModels(sourceModel : SqaleModel, externalModel : SqaleModel) = 
        for rule in externalModel.GetProfile().Rules do
            if not(rule.category.Equals("undefined")) then
                try
                    let ruleinModel = sourceModel.GetProfile().Rules |> List.find (fun elem -> elem.key.Equals(rule.key))
                    ruleinModel.category <- rule.category
                    ruleinModel.subcategory <- rule.subcategory
                    ruleinModel.remediationFactorTxt <- rule.remediationFactorTxt
                    ruleinModel.remediationFactorVal <- rule.remediationFactorVal
                    ruleinModel.remediationFunction <- rule.remediationFunction
                with
                | ex -> ()

    member x.LoadSqaleModelFromDsk(fileToRead : string) =
        let ReadFromBytes(file : string)  = 
            let formatter = new BinaryFormatter()
            use readStream = new StreamReader(file)
            let obj = formatter.Deserialize(readStream.BaseStream)
            unbox obj
        let model:SqaleModel = ReadFromBytes fileToRead
        model

