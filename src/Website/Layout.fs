namespace Website

module Layout =
    open IntelliFactory.Html
    open IntelliFactory.WebSharper
    open IntelliFactory.WebSharper.Sitelets
    open System.Web
    open Actions

    type Page =
        {
            Title : string
            Body : Content.HtmlElement list
            Menu : Content.HtmlElement list
            Authentication: Content.HtmlElement list
        }

    let RandomizeUrl url =
        url + "?d=" +
            System.Uri.EscapeUriString (System.DateTime.Now.ToString())

    let LinkTo ctx action =
        ctx.Link action |> RandomizeUrl

    let menu currentAction ctx : Content.HtmlElement list =
        let createMenuItem = fun (text, action) ->     
            let cssClass = if action = currentAction then "active" else ""
            LI [Class cssClass] 
                -< [A [HRef (ctx.Link action)] 
                    -< [Text text]]

        let items =
            [
                ("Home", Home)
                ("Policy view", NewPolicy)
                ("Policy list", PolicyList)
            ]
            |> List.filter(fun (_, action) -> not (IsProtected action) || Users.IsAuthenticated())
            |> List.map(fun item -> createMenuItem item) 

        [UL [Class "nav nav-tabs"] -< items]

    let authLink ctx : Content.HtmlElement list =
        let link = 
            match Users.IsAuthenticated() with 
                | true -> (Logout, "logout") 
                | _ -> (Login None, "login")

        [A [HRef <| LinkTo ctx (fst link)] -< [Text (snd link)]]

    let WithTemplate title action body : Content<Action> =
        let path = HttpContext.Current.Server.MapPath("~/Main.html")
        let initTemplate =
            Content.Template<Page>(path)
                .With("title", fun x -> x.Title)
                .With("body", fun x -> x.Body)
                .With("menu", fun x -> x.Menu)
                .With("auth", fun x -> x.Authentication)

        Content.WithTemplate initTemplate <| fun context ->
            {
                Title = title
                Body = body context
                Menu = menu action context
                Authentication = authLink context
            }