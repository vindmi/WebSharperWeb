module Layout
//    open IntelliFactory.Html
//    open IntelliFactory.WebSharper
//    open IntelliFactory.WebSharper.Sitelets
//    open System.Web
//
//    type Page =
//        {
//            Title : string
//            Body : list<Content.HtmlElement>
//        }
//
//    let MainTemplate =
//        let path = HttpContext.Current.Server.MapPath("~/Main.html")
//        Content.Template<Page>(path)
//            .With("title", fun x -> x.Title)
//            .With("body", fun x -> x.Body)
//
//    let WithTemplate title body : Content<Action> =
//        Content.WithTemplate MainTemplate <| fun context ->
//            {
//                Title = title
//                Body = body context
//            }