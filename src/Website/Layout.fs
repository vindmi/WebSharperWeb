module Layout
    open IntelliFactory.Html
    open IntelliFactory.WebSharper
    open IntelliFactory.WebSharper.Sitelets
    open System.Web
    open Actions

    type Page =
        {
            Title : string
            Body : list<Content.HtmlElement>
        }

    let MainTemplate =
        let path = HttpContext.Current.Server.MapPath("~/Main.html")
        Content.Template<Page>(path)
            .With("title", fun x -> x.Title)
            .With("body", fun x -> x.Body)
            .With("menu", fun x -> 
                [
                    LI [Text "Policy view"]
                    LI [Text "Policy list"]
                ])

    let WithTemplate title body : Content<Action> =
        Content.WithTemplate MainTemplate <| fun context ->
            {
                Title = title
                Body = body context
            }