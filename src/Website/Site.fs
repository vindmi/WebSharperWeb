namespace Website

open IntelliFactory.Html
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets
open Actions

module Site =
    let private RandomizeUrl url =
        url + "?d=" +
            System.Uri.EscapeUriString (System.DateTime.Now.ToString())

    let PolicyViewPage = 
        Layout.WithTemplate "Policy view" PolicyView <| fun ctx ->
            [
                Div [Text "Policy data"]
                A [HRef <| RandomizeUrl (ctx.Link Logout)] -< [Text "logout"]
            ]

    let PolicyListViewPage =
        Layout.WithTemplate "Policy list view" PolicyListView <| fun ctx ->
            [
                Div [Text "Policy list"]
                A [HRef <| ctx.Link Logout] -< [Text "logout"]
            ]

    let LoginPage (redirectAction:Option<Action>) : Content<Action> =
        let redirectLink =
            match redirectAction with
            | Some action -> action
            | None        -> Action.PolicyView

        PageContent <| fun context -> 
        {
            Page.Default with
                Title = Some "Welcome"
                Body = [ Div [ new LoginControl(redirectLink |> context.Link) ] ]
        }

    let Application =
        let authorized sitelet =    
            let filter : Sitelet.Filter<Action> = {
                VerifyUser = fun _ -> true
                LoginRedirect = Some >> Login
            }
            Sitelet.Protect filter <| sitelet

        let salesPages =
            Sitelet.Sum [
                Sitelet.Content "/" PolicyView PolicyViewPage
                Sitelet.Content "/list" PolicyListView PolicyListViewPage
            ] 
            |> authorized

        let loginPages =
            Sitelet.Infer <| fun action ->
                match action with
                | Action.Logout ->
                    WebMatrix.WebData.WebSecurity.Logout()
                    Content.Redirect PolicyView
                | Login action ->
                    LoginPage action
                | _ -> Content.NotFound
        [
            salesPages
            loginPages
        ] |> Sitelet.Sum

type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Application
        member this.Actions = []

[<assembly: WebsiteAttribute(typeof<Website>)>]
do ()
