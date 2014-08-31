namespace SqaleManager

open FSharp.Data
open System.Xml.Linq
open System.IO
open System.Reflection
open System.ComponentModel

type ImportLogEntry() = 
    member val line = -1 with get, set
    member val message = "" with get, set
    member val exceptionMessage = "" with get, set

type SubCharacteristics(key : SubCategory, name : string) = 
    member val key = key with get
    member val name = name with get

type Characteristic(key : Category, name : string) =
    let mutable subchars : SubCharacteristics list = []
    member val key = key with get
    member val name = name with get
   
    member x.CreateSubChar(key : SubCategory, name : string) =
        if not(x.IsSubCharPresent(key)) then
            let newChar = new SubCharacteristics(key, name)
            subchars <- subchars @ [newChar]

    member x.IsSubCharPresent(key : SubCategory) =
        let CompareKey(subelem : SubCharacteristics) =
             subelem.key.Equals(key)

        (List.toArray subchars) |> Array.exists (fun elem -> CompareKey(elem))

    member this.GetSubChars = subchars

module GlobalsVars =
    let mutable characteristics : Characteristic list = []

    let GetChar(key : Category) =                    
        (List.toArray characteristics) |> Array.find (fun elem -> elem.key.Equals(key))

    let IsCharPresent(key : Category) =                    
        (List.toArray characteristics) |> Array.exists (fun elem -> elem.key.Equals(key))

    let CreateAChar(key : Category, name : string) =
        if not(IsCharPresent(key)) then
            let newChar = new Characteristic(key, name)
            characteristics <- characteristics @ [newChar]
            newChar
        else
            GetChar(key)

    let defaultSqaleModel =
        let assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)
        let asm = Assembly.GetExecutingAssembly()
        use stream = asm.GetManifestResourceStream("defaultmodel.xml")
        let xmldoc = XDocument.Load(stream).ToString()
        let sqale = SqaleModelType.Parse(xmldoc)
        
        for chk in sqale.GetChcs() do
            let char = CreateAChar(EnumHelper.asEnum<Category>(chk.Key).Value, chk.Name)            

            for subchk in chk.GetChcs() do
                char.CreateSubChar(EnumHelper.asEnum<SubCategory>(subchk.Key).Value, subchk.Name)

        characteristics


type Rule() =
    let characteristics = GlobalsVars.defaultSqaleModel
    let mutable categ = Category.UNDEFINED 
    let mutable allowedsub = new  System.Collections.ObjectModel.ObservableCollection<SubCategory>()

    let event = Event<_, _>()
    interface INotifyPropertyChanged with
        member this.add_PropertyChanged(e) = event.Publish.AddHandler(e)
        member this.remove_PropertyChanged(e) = event.Publish.RemoveHandler(e)

    
    member val key = "" with get, set
    member val name = "" with get, set
    member val description = "" with get, set 
    member val configKey = "" with get, set
    member this.SubcategoryValues
        with get () = allowedsub
        and set (value) = allowedsub <- value 
    
    member val enableSetDeafaults = true with get, set   
    member this.category 
        with get () = categ
        and set (value) =
            categ <- value
            for char in characteristics do
                if char.key.Equals(value) then
                    this.SubcategoryValues.Clear()
                    for subchar in char.GetSubChars do
                        this.SubcategoryValues.Add(subchar.key)

            if this.enableSetDeafaults then 
                // set defaults
                if this.SubcategoryValues.Count <> 0 then
                    this.subcategory <- this.SubcategoryValues.[0]
                    event.Trigger(this, new PropertyChangedEventArgs("subcategory"))

                if this.remediationFunction.Equals(RemediationFunction.UNDEFINED) then
                    this.remediationFunction <- RemediationFunction.LINEAR
                    event.Trigger(this, new PropertyChangedEventArgs("remediationFunction"))

                if this.remediationFactorTxt.Equals(RemediationUnit.UNDEFINED) then
                   this.remediationFactorTxt <- RemediationUnit.MN
                   event.Trigger(this, new PropertyChangedEventArgs("remediationFactorTxt")) 

                if this.remediationOffsetTxt.Equals(RemediationUnit.UNDEFINED) then
                   this.remediationOffsetTxt <- RemediationUnit.MN
                   event.Trigger(this, new PropertyChangedEventArgs("remediationOffsetTxt")) 

                if this.remediationFactorVal.Equals("0") || this.remediationFactorVal.Equals("0.0") || this.remediationFactorVal.Equals("0,0") then
                   this.remediationFactorVal <- "5.0"
                   event.Trigger(this, new PropertyChangedEventArgs("remediationFactorVal"))



    member val subcategory = SubCategory.UNDEFINED with get, set
    member val subcategoryVal = SubCategory.UNDEFINED with get, set
    member val remediationFactorTxt = RemediationUnit.UNDEFINED with get, set
    member val remediationFunction = RemediationFunction.UNDEFINED with get, set
    member val remediationOffsetTxt = RemediationUnit.UNDEFINED with get, set
    member val severity = Severity.UNDEFINED with get, set
    member val remediationOffsetVal = "" with get, set
    member val remediationFactorVal = "" with get, set
    member val repo = "" with get, set

    member x.MergeRule(rule:Rule) = 
        x.key <- rule.key
        x.name <- rule.name
        x.description <- rule.description
        x.configKey <- rule.configKey
        x.category <- rule.category
        x.subcategory <- rule.subcategory
        x.remediationFactorTxt <- rule.remediationFactorTxt
        x.remediationFunction <- rule.remediationFunction
        x.remediationOffsetTxt <- rule.remediationOffsetTxt
        x.severity <- rule.severity
        x.remediationOffsetVal <- rule.remediationOffsetVal
        x.remediationFactorVal <- rule.remediationFactorVal
        x.repo <- rule.repo
     
type Profile() = 
    let mutable rules : Rule list = []

    member x.Rules = rules

    member x.AddRule (rule : Rule) =
        rules <- rules @ [rule]
    
    member x.IsRulePresent(key : string) =              
        (List.toArray rules) |> Array.exists (fun elem -> elem.key.Equals(key)) 

    member x.GetRule(key : string) =              
        (List.toArray rules) |> Array.find (fun elem -> elem.key.Equals(key)) 

    member x.CreateRule(rule : Rule) =
        if not(x.IsRulePresent(rule.key)) then
            rules <- rules @ [rule]
        
