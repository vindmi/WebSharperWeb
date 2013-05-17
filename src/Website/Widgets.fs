namespace Website

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Formlet
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.Sitelets
open IntelliFactory.WebSharper.Web
open Users

module Widgets =
    [<Inline "window.location.assign($url)">]
    let private redirect (url: string) = ()

    [<JavaScript>]
    let private input label requiredMessage =
        Controls.Input ""
        |> Enhance.WithTextLabel label
        |> fun inp ->
            match requiredMessage with
                | Some string -> 
                    Validator.IsNotEmpty requiredMessage.Value inp |> Enhance.WithValidationIcon
                | None -> inp

    module LoginWidget =
        [<JavaScript>]
        let private warningPanel label =
            Formlet.Do {
                let! _ =
                    Formlet.OfElement <| fun _ ->
                        Div [Attr.Class "warningPanel"] -< [Text label]
                return! Formlet.Never ()
            }

        [<JavaScript>]
        let private withLoadingPane (a: Async<'T>) (f: 'T -> Formlet<'U>) : Formlet<'U> =
            let loadingPane =
                Formlet.BuildFormlet <| fun _ ->
                    let elem = 
                        Div [Attr.Class "loadingPane"]
                    let state = new Event<Result<'T>>()
                    async {
                        let! x = a
                        do state.Trigger (Result.Success x)
                        return ()
                    }
                    |> Async.Start
                    elem, ignore, state.Publish
            Formlet.Replace loadingPane f

        [<Rpc>]
        let private login (loginInfo: LoginData) =
            Users.Login loginInfo.Name loginInfo.Password None
            |> async.Return

        [<Rpc>]
        let private loginSimple (loginInfo: LoginData) =
            Users.Login loginInfo.Name loginInfo.Password None

        [<JavaScript>]
        let Create (redirectUrl: string) : Formlet<unit> =
            let uName = input "Username" (Some "Enter Username")
            let pw = input "Password" (Some "Enter Password")
            let loginF =
                Formlet.Yield (fun n pw -> { LoginData.Name = n; Password = pw })
                <*> uName <*> pw

            Formlet.Do {
                let! uInfo = 
                    loginF
                    |> Enhance.WithCustomSubmitAndResetButtons
                        {Enhance.FormButtonConfiguration.Default with Label = Some "Login"}
                        {Enhance.FormButtonConfiguration.Default with Label = Some "Reset"}

                let loginAndReturn =
                    if loginSimple uInfo then
                        redirect redirectUrl
                        Formlet.Return ()
                    else
                        Formlet.Never()
                return!
                    loginAndReturn
//                    withLoadingPane (login uInfo) (fun loggedIn ->
//                        if loggedIn then
//                            redirect redirectUrl
//                            Formlet.Return ()
//                        else
//                            warningPanel "Login failed")
            }
            |> Enhance.WithFormContainer

    module RegisterWidget =
        [<Rpc>]
        let private register clientData =
            Users.Register clientData

        [<Rpc>]
        let private authenticate userName =
            Users.Authenticate userName

        [<JavaScript>]
        let Create : Formlet<unit> =
            let firstName = input "Firstname" (Some "Enter Firstname")
            let lastName = input "Lastname" (Some "Enter Lastname")
            let idCode = input "Id code" (Some "Enter Id code")
            let uName = input "Username" (Some "Enter Username")
            let pw = input "Password" (Some "Enter Password")
            let pw2 =  input "Confirm Password" (Some "Confirm Password")
            
            let registerF =
                Formlet.Yield (fun un pw fn ln id -> 
                {
                    Name = un; 
                    Password = pw;
                    firstName = fn;
                    lastName = ln;
                    idCode = id;
                    birthDate = System.DateTime.Now
                })
                <*> uName 
                <*> pw 
                <*> firstName 
                <*> lastName 
                <*> idCode

            Formlet.Do {
                let! uInfo = 
                    registerF
                    |> Enhance.WithCustomSubmitButton
                        { Enhance.FormButtonConfiguration.Default with Label = Some "Register" }
                let registerAndReturn =
                    if register uInfo then
                        authenticate uInfo.Name
                        redirect "/"
                        Formlet.Return ()
                    else
                        Formlet.Never()
                return!
                    registerAndReturn
            }
            |> Enhance.WithFormContainer

    module PolicyWidget =
        open Premium
        open Policies
        open Actions

        [<Rpc>]
        let private CalculatePremium data =
            Premium.Calculate data

        [<Rpc>]
        let private BuyPolicy draft price =
            Policies.Issue draft (Users.CurrentUser().Value) price

        [<JavaScript>]
        let Create (redirectUrl) : Formlet<unit> =
            let constructionType = 
                Controls.Select 0 [ ("Stone / Brick", 0); ("Wood", 1) ]
                    |> Enhance.WithTextLabel "Construction type"
            let constructionYear =  
                input "Construction year" None//(Some "Enter construction year")
                    |> Validator.IsInt "Enter number"
                    |> Enhance.WithValidationIcon
            let area = 
                input "Area (m2)" None
                    |> Validator.IsInt "Enter number"
                    |> Enhance.WithValidationIcon
            let objectType = 
                Controls.Select 0 [ ("Flat", 0); ("House", 1); ("Garage", 2) ] 
                    |> Enhance.WithTextLabel "Object type"
            let isFireAlarm = 
                Controls.RadioButtonGroup None [ ("Yes", true); ("No", false) ] 
                    |> Enhance.WithTextLabel "Is there fire alarm?"
            let isBurglaryAlarm = 
                Controls.RadioButtonGroup None [ ("Yes", true); ("No", false) ] 
                    |> Enhance.WithTextLabel "Is there burglary alarm?"

            let policyForm =
                Formlet.Yield (fun objType constrType year area fire burgl -> 
                { 
                    objectType = objType
                    constructionType = constrType
                    constructionYear = int year
                    area = int area
                    isFireAlarm = fire
                    isBurglaryAlarm = burgl
                })
                <*> objectType
                <*> constructionType
                <*> constructionYear 
                <*> area 
                <*> isFireAlarm
                <*> isBurglaryAlarm

            let buyFormlet (draft:PolicyData) (price:int) : Formlet<unit> = 
                Formlet.Do {
                    let! prInp =
                        Controls.ReadOnlyInput (price.ToString())
                        |> Enhance.WithTextLabel "Price"
                        |> Enhance.WithCustomSubmitButton
                            { Enhance.FormButtonConfiguration.Default with Label = Some "Buy" }
                        |> Enhance.WithFormContainer

                    let ret =
                        BuyPolicy draft price |> ignore
                        redirect redirectUrl
                        Formlet.Return()

                    return! ret
                }
                |> Enhance.WithFormContainer

            Formlet.Do {
                let! policyData = 
                    policyForm |> Enhance.WithCustomSubmitButton
                        { Enhance.FormButtonConfiguration.Default with Label = Some "Calculate price" }
                return! buyFormlet policyData (CalculatePremium policyData)
            }
            |> Enhance.WithFormContainer

    module PolicyListWidget =
        open DataAccess
        open IntelliFactory.Html

        let Render (dataSource:Policy array) =
            let toNode e =
                e :> Html.INode<Control>

            let row (policy:Policy) =
                let policyPeriod =
                    policy.valid_from.ToShortDateString() 
                    + "-" 
                    + policy.valid_tille.ToShortDateString()
                TR [ 
                    TD [Text policy.number]
                    TD [Text policyPeriod]
                    TD [Text (string policy.premium)]
                ]

            let rows = Array.map (row >> toNode) dataSource |> Seq.toList
            let tableClass = Class "table" |> toNode
            let header = 
                THead [ 
                    TR [
                        TH [Text "Number"]
                        TH [Text "Period"]
                        TH [Text "Premium"] 
                    ] 
                ] |> toNode

            Table (tableClass::(header::rows))

type LoginControl(redirectUrl: string) =
    inherit IntelliFactory.WebSharper.Web.Control()

    new () = new LoginControl("?")
    [<JavaScript>]
    override this.Body = Widgets.LoginWidget.Create redirectUrl :> _

type RegisterControl() =
    inherit IntelliFactory.WebSharper.Web.Control()

    [<JavaScript>]
    override this.Body = Widgets.RegisterWidget.Create :> _

type PolicyControl(redirectUrl: string) =
    inherit IntelliFactory.WebSharper.Web.Control()

    [<JavaScript>]
    override this.Body = Widgets.PolicyWidget.Create redirectUrl :> _
