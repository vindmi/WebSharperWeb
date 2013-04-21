namespace Website

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Formlet
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.Sitelets
open IntelliFactory.WebSharper.Web
open Users

module Widgets =
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
    
        [<Inline "window.location.assign($url)">]
        let private redirect (url: string) = ()

        [<Rpc>]
        let private login (loginInfo: LoginData) =
            Users.Login loginInfo.Name loginInfo.Password None
            |> async.Return

        [<JavaScript>]
        let Create (redirectUrl: string) : Formlet<unit> =
            let uName =
                Controls.Input ""
                |> Validator.IsNotEmpty "Enter Username"
                |> Enhance.WithValidationIcon
                |> Enhance.WithTextLabel "Username"
            let pw =
                Controls.Password ""
                |> Validator.IsNotEmpty "Enter Password"
                |> Enhance.WithValidationIcon
                |> Enhance.WithTextLabel "Password"
            let loginF =
                Formlet.Yield (fun n pw -> {Name=n; Password=pw})
                <*> uName <*> pw

            Formlet.Do {
                let! uInfo = 
                    loginF
                    |> Enhance.WithCustomSubmitAndResetButtons
                        {Enhance.FormButtonConfiguration.Default with Label = Some "Login"}
                        {Enhance.FormButtonConfiguration.Default with Label = Some "Reset"}
                return!
                    withLoadingPane (login uInfo) (fun loggedIn ->
                        if loggedIn then
                            redirect redirectUrl
                            Formlet.Return ()
                        else
                            warningPanel "Login failed")
            }
            |> Enhance.WithFormContainer

    module RegisterWidget =
        ()

/// Exposes the login form so that it can be used in sitelets.
type LoginControl(redirectUrl: string) =
    inherit IntelliFactory.WebSharper.Web.Control ()

    new () = new LoginControl("?")
    [<JavaScript>]
    override this.Body = Widgets.LoginWidget.Create redirectUrl :> _
