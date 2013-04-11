module Layout
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
        } 

    let menu currentAction ctx : Content.HtmlElement list =
        let createMenuItem = fun (text, action) ->     
            let cssClass = if action = currentAction then "active" else ""
            LI [Class cssClass] 
                -< [A [HRef (ctx.Link action)] 
                    -< [Text text]]
        
        let items =
            [
                ("Policy view", PolicyView)
                ("Policy list", PolicyListView)
            ] |> List.map(fun item -> createMenuItem item)

        [UL [Class "nav nav-list"] -< items]

    let WithTemplate title action body : Content<Action> =
        let path = HttpContext.Current.Server.MapPath("~/Main.html")
        let initTemplate =
            Content.Template<Page>(path)
                .With("title", fun x -> x.Title)
                .With("body", fun x -> x.Body)
                .With("menu", fun x -> x.Menu)

        Content.WithTemplate initTemplate <| fun context ->
            {
                Title = title
                Body = body context
                Menu = menu action context
            }