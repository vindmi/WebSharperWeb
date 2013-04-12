namespace Website

open IntelliFactory.Html
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets
open Actions

module Site =
    let PolicyViewPage = 
        Layout.WithTemplate "Policy view" PolicyView <| fun ctx ->
            [
                Div [Text "Policy data"]
                A [HRef <| ctx.Link Logout] -< [Text "logout"]
            ]

    let PolicyListViewPage =
        Layout.WithTemplate "Policy list view" PolicyListView <| fun ctx ->
            [
                Div [Text "Policy list"]
                A [HRef <| ctx.Link Logout] -< [Text "logout"]
            ]

    let LoginPage : Content<Action> =
        PageContent <| fun context -> 
        {
            Page.Default with
                Title = Some "Welcome"
                Body = [ Div [Text "login" ] ] //[ Div [ new LoginControl("/") ] ]
        }

    let Application =
        let authorized sitelet =    
            let filter : Sitelet.Filter<Action> = {
                VerifyUser = fun _ -> true//System.Web.HttpContext.Current.User.Identity.IsAuthenticated
                LoginRedirect = fun _ -> Login
            }
            Sitelet.Protect filter <| sitelet

        let salesPages =
            Sitelet.Sum [
                Sitelet.Content "/" PolicyView PolicyViewPage
                Sitelet.Content "list" PolicyListView PolicyListViewPage
            ] |> authorized

        let logoutAction =
            Sitelet.Infer <| fun action ->
                match action with
                | Action.Logout ->
                    // Logout user and redirect to home
                    Users.logout
                    Content.Redirect Login
                | _ -> Content.NotFound
        [
            salesPages
            Sitelet.Content "/login" Login LoginPage
            logoutAction
        ] |> Sitelet.Sum

type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Application
        member this.Actions = [PolicyView; PolicyListView; Login; Logout]

[<assembly: WebsiteAttribute(typeof<Website>)>]
do ()
