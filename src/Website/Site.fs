namespace Website

open IntelliFactory.Html
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets
open Actions

module Site =
    let NewPolicyPage = 
        Layout.WithTemplate "New policy" NewPolicy <| fun ctx ->
            [Text "Policy data"]

    let PolicyListPage =
        Layout.WithTemplate "Policy list" PolicyList <| fun ctx ->
            [Text "Policy list"]

    let HomePage =
        Layout.WithTemplate "Home" Home <| fun ctx ->
            [Text "Home"]

    let LoginPage (redirectAction:Option<Action>) : Content<Action> =
        let redirectLink =
            match redirectAction with
            | Some action -> action
            | None        -> Action.Home

        Layout.WithTemplate "Login" (Login None) <| fun context -> 
            [ 
                Div [ new LoginControl(redirectLink |> Layout.LinkTo context) ] 
            ]

    let RegisterPage  =
        Layout.WithTemplate "Register" Register <| fun context -> 
            [ 
                Div [ new RegisterControl() ]
            ]   

    let Application =
        let authorized sitelet =    
            let filter : Sitelet.Filter<Action> = {
                VerifyUser = fun _ -> true
                LoginRedirect = Some >> Login
            }
            Sitelet.Protect filter <| sitelet

        let contentPages =
            [
                ("/", Home, HomePage)
                ("/new", NewPolicy, NewPolicyPage)
                ("/list", PolicyList, PolicyListPage)
            ] 
            |> List.map (
                fun (path, action, handler) ->
                    let content = Sitelet.Content path action handler
                    if IsProtected action
                        then content |> authorized
                        else content)
            |> Sitelet.Sum

        let loginPages =
            Sitelet.Infer <| fun action ->
                match action with
                | Action.Logout ->
                    Users.Logout()
                    Content.Redirect Home
                | Login action ->
                    LoginPage action
                | Action.Register ->
                    RegisterPage
                | _ -> Content.NotFound
        [
            contentPages
            loginPages
        ] |> Sitelet.Sum

type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Application
        member this.Actions = []

[<assembly: WebsiteAttribute(typeof<Website>)>]
do ()
