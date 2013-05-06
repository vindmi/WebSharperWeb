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
                Formlet.Yield (fun fn ln id un pw -> 
                {
                    Name = un; 
                    Password = pw;
                    firstName = fn;
                    lastName = ln;
                    idCode = id;
                    birthDate = System.DateTime.Now
                })
                <*> uName <*> pw <*> firstName <*> lastName <*> idCode

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
        type PolicyData = {
            constructionType : int
            constructionYear : string
            area : string
            objectType : int
            isFireAlarm : int
            isBurglaryAlarm : int
        }

        let private policyHolderSummary =
            match Users.CurrentUser() with
            | Some(c) -> c.LastName + " " + c.FirstName + ", " + c.Code
            | _ -> ""

        [<JavaScript>]
        let Create : Formlet<unit> =
            //let policyHolder = [ Span [ Text policyHolderSummary ] ]
            let constructionType = Controls.Select 0 [ ("Stone / Brick", 0); ("Wood", 1) ] |> Enhance.WithTextLabel "Construction type"
            let constructionYear =  input "Construction year" (Some "Enter construction year")
            let area = input "Area (m2)" (Some "Enter area")
            let objectType = Controls.Select 0 [ ("Flat", 0); ("House", 1); ("Garage", 2) ] |> Enhance.WithTextLabel "Object type"
            let isFireAlarm = Controls.RadioButtonGroup None [ ("Yes", 0); ("No", 1) ] |> Enhance.WithTextLabel "Is there fire alarm?"
            let isBurglaryAlarm = Controls.RadioButtonGroup None [ ("Yes", 0); ("No", 1) ] |> Enhance.WithTextLabel "Is there burglary alarm?"

            let policyForm =
                Formlet.Yield (fun constrType year area objType fire burgl -> 
                {
                    constructionType = constrType
                    constructionYear = year
                    area = area
                    objectType = objType
                    isFireAlarm = fire
                    isBurglaryAlarm = burgl
                })
                <*> constructionType 
                <*> constructionYear 
                <*> area 
                <*> objectType 
                <*> isFireAlarm
                <*> isBurglaryAlarm

            Formlet.Do {
                let! policyData = 
                    policyForm |> Enhance.WithCustomSubmitButton
                        { Enhance.FormButtonConfiguration.Default with Label = Some "Calculate" }
                let calculateAndReturn =
                    Formlet.Never()
                return!
                    calculateAndReturn
            }
            |> Enhance.WithFormContainer

type LoginControl(redirectUrl: string) =
    inherit IntelliFactory.WebSharper.Web.Control()

    new () = new LoginControl("?")
    [<JavaScript>]
    override this.Body = Widgets.LoginWidget.Create redirectUrl :> _

type RegisterControl() =
    inherit IntelliFactory.WebSharper.Web.Control()

    [<JavaScript>]
    override this.Body = Widgets.RegisterWidget.Create :> _

type PolicyControl() =
    inherit IntelliFactory.WebSharper.Web.Control()

    [<JavaScript>]
    override this.Body = Widgets.PolicyWidget.Create :> _
